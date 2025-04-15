// This implementation is based on the C implementation available at:
// https://ai.stanford.edu/~rubner/emd/default.htm

using System;

namespace EmdFlat
{
    /// <summary>
    /// Provides methods for computing the Earth Mover's Distance.
    /// </summary>
    public sealed unsafe class Emd
    {
        private static readonly double INFINITY = 1e40;
        private static readonly double EPSILON = 1e-12;

        private int maxSigSize1;
        private int maxSigSize2;
        private int maxIterations;

        /* GLOBAL VARIABLE DECLARATION */

        /* SIGNATURES SIZES */
        private int _n1;
        private int _n2;

        /* THE COST MATRIX */
        private double[][] _C;

        /* THE BASIC VARIABLES VECTOR */
        private node2_t[] managed_X;

        /* VARIABLES TO HANDLE _X EFFICIENTLY */
        private node2_t* _EndX;
        private node2_t* _EnterX;
        private bool[][] _IsX;
        private node2_t*[] _RowsX;
        private node2_t*[] _ColsX;
        private double _maxW;
        private double _maxC;

        private node1_t[] managed_U;
        private node1_t[] managed_V;

        private double[] managed_S;
        private double[] managed_D;

        private node2_t*[] managed_Loop;

        private bool[] managed_IsUsed;

        private node1_t[] managed_Ur;
        private node1_t[] managed_Vr;

        private double[][] Delta;

        /// <summary>
        /// Initializes a new instance of <see cref="Emd"/>.
        /// </summary>
        /// <param name="maxSigSize1">
        /// The maximum size of the first signature.
        /// </param>
        /// <param name="maxSigSize2">
        /// The maximum size of the second signature.
        /// </param>
        /// <param name="maxIterations">
        /// The maximum number of iterations.
        /// If this limit is exceeded, an <see cref="EmdException"/> is thrown.
        /// </param>
        public Emd(int maxSigSize1 = 100, int maxSigSize2 = 100, int maxIterations = 500)
        {
            this.maxSigSize1 = maxSigSize1;
            this.maxSigSize2 = maxSigSize2;
            this.maxIterations = maxIterations;

            var p1 = maxSigSize1 + 1;
            var p2 = maxSigSize2 + 1;
            var max = Math.Max(p1, p2);

            _C = CreateJaggedArray<double>(p1, p2);
            managed_X = new node2_t[max * 2];
            _IsX = CreateJaggedArray<bool>(p1, p2);
            _RowsX = new node2_t*[max];
            _ColsX = new node2_t*[max];
            managed_U = new node1_t[max];
            managed_V = new node1_t[max];
            managed_S = new double[max];
            managed_D = new double[max];
            managed_Loop = new node2_t*[2 * max];
            managed_IsUsed = new bool[2 * max];
            managed_Ur = new node1_t[max];
            managed_Vr = new node1_t[max];
            Delta = CreateJaggedArray<double>(p1, p2);
        }

        /// <summary>
        /// Computes the Earth Mover's Distance between two signatures using the given distance function.
        /// </summary>
        /// <typeparam name="feature_t">
        /// The type of the features in the signatures.
        /// </typeparam>
        /// <param name="Signature1">
        /// Pointers to the two signatures that their distance we want to compute.
        /// </param>
        /// <param name="Signature2">
        /// Pointers to the two signatures that their distance we want to compute.
        /// </param>
        /// <param name="Dist">
        /// Pointer to the ground distance function.
        /// i.e. the function that computes the distance between two features.
        /// </param>
        /// <param name="Flow">
        /// Pointer to a vector of flow_t (defined in emd.h) where the resulting flow will be stored.
        /// Flow must have n1+n2-1 elements, where n1 and n2 are the sizes of the two signatures respectively.
        /// If NULL, the flow is not returned.
        /// </param>
        /// <param name="FlowSize">
        /// In case Flow is not NULL, FlowSize should point to a integer where the number of flow elements (always less or equal to n1+n2-1) will be written.
        /// </param>
        /// <returns></returns>
        /// <exception cref="EmdException"></exception>
        public double emd<feature_t>(
            signature_t<feature_t> Signature1, signature_t<feature_t> Signature2,
            Func<feature_t, feature_t, double> Dist,
            flow_t* Flow, int* FlowSize)
        {
            fixed (node2_t* _X = managed_X)
            fixed (node1_t* U = managed_U)
            fixed (node1_t* V = managed_V)
            {
                int itr;
                double totalCost;
                double w;
                node2_t* XP;
                flow_t* FlowP = null;

                w = init(Signature1, Signature2, Dist);

                if (_n1 > 1 && _n2 > 1)  /* IF _n1 = 1 OR _n2 = 1 THEN WE ARE DONE */
                {
                    for (itr = 1; itr < maxIterations; itr++)
                    {
                        /* FIND BASIC VARIABLES */
                        findBasicVariables(U, V);

                        /* CHECK FOR OPTIMALITY */
                        if (isOptimal(U, V))
                            break;

                        /* IMPROVE SOLUTION */
                        newSol();
                    }

                    if (itr == maxIterations)
                        throw new EmdException($"Maximum number of iterations has been reached ({maxIterations}).");
                }

                /* COMPUTE THE TOTAL FLOW */
                totalCost = 0;
                if (Flow != null)
                    FlowP = Flow;
                for (XP = _X; XP < _EndX; XP++)
                {
                    if (XP == _EnterX)  /* _EnterX IS THE EMPTY SLOT */
                        continue;
                    if (XP->i == Signature1.n || XP->j == Signature2.n)  /* DUMMY FEATURE */
                        continue;

                    if (XP->val == 0)  /* ZERO FLOW */
                        continue;

                    totalCost += (double)XP->val * _C[XP->i][XP->j];
                    if (Flow != null)
                    {
                        FlowP->from = XP->i;
                        FlowP->to = XP->j;
                        FlowP->amount = XP->val;
                        FlowP++;
                    }
                }
                if (Flow != null)
                    *FlowSize = (int)(FlowP - Flow);

                /* RETURN THE NORMALIZED COST == EMD */
                return totalCost / w;
            }
        }

        private double init<feature_t>(
            signature_t<feature_t> Signature1, signature_t<feature_t> Signature2,
            Func<feature_t, feature_t, double> Dist)
        {
            fixed (node2_t* _X = managed_X)
            fixed (double* S = managed_S)
            fixed (double* D = managed_D)
            {
                int i, j;
                double sSum, dSum, diff;

                _n1 = Signature1.n;
                _n2 = Signature2.n;

                if (_n1 > maxSigSize1 || _n2 > maxSigSize2)
                {
                    throw new EmdException("Signature size is too large.");
                }

                /* COMPUTE THE DISTANCE MATRIX */
                _maxC = 0;
                for (i = 0; i < _n1; i++)
                    for (j = 0; j < _n2; j++)
                    {
                        _C[i][j] = Dist(Signature1.Features[i], Signature2.Features[j]);
                        if (_C[i][j] > _maxC)
                            _maxC = _C[i][j];
                    }

                /* SUM UP THE SUPPLY AND DEMAND */
                sSum = 0.0;
                for (i = 0; i < _n1; i++)
                {
                    S[i] = Signature1.Weights[i];
                    sSum += Signature1.Weights[i];
                    _RowsX[i] = null;
                }
                dSum = 0.0;
                for (j = 0; j < _n2; j++)
                {
                    D[j] = Signature2.Weights[j];
                    dSum += Signature2.Weights[j];
                    _ColsX[j] = null;
                }

                /* IF SUPPLY DIFFERENT THAN THE DEMAND, ADD A ZERO-COST DUMMY CLUSTER */
                diff = sSum - dSum;
                if (Math.Abs(diff) >= EPSILON * sSum)
                {
                    if (diff < 0.0)
                    {
                        for (j = 0; j < _n2; j++)
                            _C[_n1][j] = 0;
                        S[_n1] = -diff;
                        _RowsX[_n1] = null;
                        _n1++;
                    }
                    else
                    {
                        for (i = 0; i < _n1; i++)
                            _C[i][_n2] = 0;
                        D[_n2] = diff;
                        _ColsX[_n2] = null;
                        _n2++;
                    }
                }

                /* INITIALIZE THE BASIC VARIABLE STRUCTURES */
                for (i = 0; i < _n1; i++)
                    for (j = 0; j < _n2; j++)
                        _IsX[i][j] = false;
                _EndX = _X;

                _maxW = sSum > dSum ? sSum : dSum;

                /* FIND INITIAL SOLUTION */
                russel(S, D);

                _EnterX = _EndX++;  /* AN EMPTY SLOT (ONLY _n1+_n2-1 BASIC VARIABLES) */

                return sSum > dSum ? dSum : sSum;
            }
        }

        private void findBasicVariables(node1_t* U, node1_t* V)
        {
            int i, j;
            bool found;
            int UfoundNum, VfoundNum;
            node1_t u0Head, u1Head;
            node1_t* CurU, PrevU;
            node1_t v0Head, v1Head;
            node1_t* CurV, PrevV;

            /* INITIALIZE THE ROWS LIST (U) AND THE COLUMNS LIST (V) */
            u0Head.Next = CurU = U;
            for (i = 0; i < _n1; i++)
            {
                CurU->i = i;
                CurU->Next = CurU + 1;
                CurU++;
            }
            (--CurU)->Next = null;
            u1Head.Next = null;

            CurV = V + 1;
            v0Head.Next = _n2 > 1 ? V + 1 : null;
            for (j = 1; j < _n2; j++)
            {
                CurV->i = j;
                CurV->Next = CurV + 1;
                CurV++;
            }
            (--CurV)->Next = null;
            v1Head.Next = null;

            /* THERE ARE _n1+_n2 VARIABLES BUT ONLY _n1+_n2-1 INDEPENDENT EQUATIONS,
               SO SET V[0]=0 */
            V[0].i = 0;
            V[0].val = 0;
            v1Head.Next = V;
            v1Head.Next->Next = null;

            /* LOOP UNTIL ALL VARIABLES ARE FOUND */
            UfoundNum = VfoundNum = 0;
            while (UfoundNum < _n1 || VfoundNum < _n2)
            {
                found = false;
                if (VfoundNum < _n2)
                {
                    /* LOOP OVER ALL MARKED COLUMNS */
                    PrevV = &v1Head;
                    for (CurV = v1Head.Next; CurV != null; CurV = CurV->Next)
                    {
                        j = CurV->i;
                        /* FIND THE VARIABLES IN COLUMN j */
                        PrevU = &u0Head;
                        for (CurU = u0Head.Next; CurU != null; CurU = CurU->Next)
                        {
                            i = CurU->i;
                            if (_IsX[i][j])
                            {
                                /* COMPUTE U[i] */
                                CurU->val = _C[i][j] - CurV->val;
                                /* ...AND ADD IT TO THE MARKED LIST */
                                PrevU->Next = CurU->Next;
                                CurU->Next = u1Head.Next != null ? u1Head.Next : null;
                                u1Head.Next = CurU;
                                CurU = PrevU;
                            }
                            else
                                PrevU = CurU;
                        }
                        PrevV->Next = CurV->Next;
                        VfoundNum++;
                        found = true;
                    }
                }
                if (UfoundNum < _n1)
                {
                    /* LOOP OVER ALL MARKED ROWS */
                    PrevU = &u1Head;
                    for (CurU = u1Head.Next; CurU != null; CurU = CurU->Next)
                    {
                        i = CurU->i;
                        /* FIND THE VARIABLES IN ROWS i */
                        PrevV = &v0Head;
                        for (CurV = v0Head.Next; CurV != null; CurV = CurV->Next)
                        {
                            j = CurV->i;
                            if (_IsX[i][j])
                            {
                                /* COMPUTE V[j] */
                                CurV->val = _C[i][j] - CurU->val;
                                /* ...AND ADD IT TO THE MARKED LIST */
                                PrevV->Next = CurV->Next;
                                CurV->Next = v1Head.Next != null ? v1Head.Next : null;
                                v1Head.Next = CurV;
                                CurV = PrevV;
                            }
                            else
                                PrevV = CurV;
                        }
                        PrevU->Next = CurU->Next;
                        UfoundNum++;
                        found = true;
                    }
                }
                if (!found)
                {
                    throw new EmdException("Unexpected error in findBasicVariables.");
                }
            }
        }

        private bool isOptimal(node1_t* U, node1_t* V)
        {
            double delta, deltaMin;
            int i, j, minI = 0, minJ = 0;

            /* FIND THE MINIMAL Cij-Ui-Vj OVER ALL i,j */
            deltaMin = INFINITY;
            for (i = 0; i < _n1; i++)
                for (j = 0; j < _n2; j++)
                    if (!_IsX[i][j])
                    {
                        delta = _C[i][j] - U[i].val - V[j].val;
                        if (deltaMin > delta)
                        {
                            deltaMin = delta;
                            minI = i;
                            minJ = j;
                        }
                    }

            if (deltaMin == INFINITY)
            {
                throw new EmdException("Unexpected error in isOptimal.");
            }

            _EnterX->i = minI;
            _EnterX->j = minJ;

            /* IF NO NEGATIVE deltaMin, WE FOUND THE OPTIMAL SOLUTION */
            return deltaMin >= -EPSILON * _maxC;

            /*
               return deltaMin >= -EPSILON;
             */
        }

        private void newSol()
        {
            fixed (node2_t** Loop = managed_Loop)
            {
                int i, j, k;
                double xMin;
                int steps;
                node2_t* CurX, LeaveX = null;

                /* ENTER THE NEW BASIC VARIABLE */
                i = _EnterX->i;
                j = _EnterX->j;
                _IsX[i][j] = true;
                _EnterX->NextC = _RowsX[i];
                _EnterX->NextR = _ColsX[j];
                _EnterX->val = 0;
                _RowsX[i] = _EnterX;
                _ColsX[j] = _EnterX;

                /* FIND A CHAIN REACTION */
                steps = findLoop(Loop);

                /* FIND THE LARGEST VALUE IN THE LOOP */
                xMin = INFINITY;
                for (k = 1; k < steps; k += 2)
                {
                    if (Loop[k]->val < xMin)
                    {
                        LeaveX = Loop[k];
                        xMin = Loop[k]->val;
                    }
                }

                /* UPDATE THE LOOP */
                for (k = 0; k < steps; k += 2)
                {
                    Loop[k]->val += xMin;
                    Loop[k + 1]->val -= xMin;
                }

                /* REMOVE THE LEAVING BASIC VARIABLE */
                i = LeaveX->i;
                j = LeaveX->j;
                _IsX[i][j] = false;
                if (_RowsX[i] == LeaveX)
                    _RowsX[i] = LeaveX->NextC;
                else
                    for (CurX = _RowsX[i]; CurX != null; CurX = CurX->NextC)
                        if (CurX->NextC == LeaveX)
                        {
                            CurX->NextC = CurX->NextC->NextC;
                            break;
                        }
                if (_ColsX[j] == LeaveX)
                    _ColsX[j] = LeaveX->NextR;
                else
                    for (CurX = _ColsX[j]; CurX != null; CurX = CurX->NextR)
                        if (CurX->NextR == LeaveX)
                        {
                            CurX->NextR = CurX->NextR->NextR;
                            break;
                        }

                /* SET _EnterX TO BE THE NEW EMPTY SLOT */
                _EnterX = LeaveX;
            }
        }

        private int findLoop(node2_t** Loop)
        {
            fixed (node2_t* _X = managed_X)
            fixed (bool* IsUsed = managed_IsUsed)
            {
                int i, steps;
                node2_t** CurX;
                node2_t* NewX;

                for (i = 0; i < _n1 + _n2; i++)
                    IsUsed[i] = false;

                CurX = Loop;
                NewX = *CurX = _EnterX;
                IsUsed[_EnterX - _X] = true;
                steps = 1;

                do
                {
                    if (steps % 2 == 1)
                    {
                        /* FIND AN UNUSED X IN THE ROW */
                        NewX = _RowsX[NewX->i];
                        while (NewX != null && IsUsed[NewX - _X])
                            NewX = NewX->NextC;
                    }
                    else
                    {
                        /* FIND AN UNUSED X IN THE COLUMN, OR THE ENTERING X */
                        NewX = _ColsX[NewX->j];
                        while (NewX != null && IsUsed[NewX - _X] && NewX != _EnterX)
                            NewX = NewX->NextR;
                        if (NewX == _EnterX)
                            break;
                    }

                    if (NewX != null)  /* FOUND THE NEXT X */
                    {
                        /* ADD X TO THE LOOP */
                        *++CurX = NewX;
                        IsUsed[NewX - _X] = true;
                        steps++;
                    }
                    else  /* DIDN'T FIND THE NEXT X */
                    {
                        /* BACKTRACK */
                        do
                        {
                            NewX = *CurX;
                            do
                            {
                                if (steps % 2 == 1)
                                    NewX = NewX->NextR;
                                else
                                    NewX = NewX->NextC;
                            } while (NewX != null && IsUsed[NewX - _X]);

                            if (NewX == null)
                            {
                                IsUsed[*CurX - _X] = false;
                                CurX--;
                                steps--;
                            }
                        } while (NewX == null && CurX >= Loop);

                        IsUsed[*CurX - _X] = false;
                        *CurX = NewX;
                        IsUsed[NewX - _X] = true;
                    }
                } while (CurX >= Loop);

                if (CurX == Loop)
                {
                    throw new EmdException("Unexpected error in findLoop.");
                }

                return steps;
            }
        }

        private void russel(double* S, double* D)
        {
            fixed (node1_t* Ur = managed_Ur)
            fixed (node1_t* Vr = managed_Vr)
            {
                int i, j, minI = 0, minJ = 0;
                bool found;
                double deltaMin, oldVal, diff;
                node1_t uHead;
                node1_t* CurU, PrevU;
                node1_t vHead;
                node1_t* CurV, PrevV;
                node1_t* PrevUMinI = null, PrevVMinJ = null, Remember;

                /* INITIALIZE THE ROWS LIST (Ur), AND THE COLUMNS LIST (Vr) */
                uHead.Next = CurU = Ur;
                for (i = 0; i < _n1; i++)
                {
                    CurU->i = i;
                    CurU->val = -INFINITY;
                    CurU->Next = CurU + 1;
                    CurU++;
                }
                (--CurU)->Next = null;

                vHead.Next = CurV = Vr;
                for (j = 0; j < _n2; j++)
                {
                    CurV->i = j;
                    CurV->val = -INFINITY;
                    CurV->Next = CurV + 1;
                    CurV++;
                }
                (--CurV)->Next = null;

                /* FIND THE MAXIMUM ROW AND COLUMN VALUES (Ur[i] AND Vr[j]) */
                for (i = 0; i < _n1; i++)
                    for (j = 0; j < _n2; j++)
                    {
                        double v;
                        v = _C[i][j];
                        if (Ur[i].val <= v)
                            Ur[i].val = v;
                        if (Vr[j].val <= v)
                            Vr[j].val = v;
                    }

                /* COMPUTE THE Delta MATRIX */
                for (i = 0; i < _n1; i++)
                    for (j = 0; j < _n2; j++)
                        Delta[i][j] = _C[i][j] - Ur[i].val - Vr[j].val;

                /* FIND THE BASIC VARIABLES */
                do
                {
                    /* FIND THE SMALLEST Delta[i][j] */
                    found = false;
                    deltaMin = INFINITY;
                    PrevU = &uHead;
                    for (CurU = uHead.Next; CurU != null; CurU = CurU->Next)
                    {
                        i = CurU->i;
                        PrevV = &vHead;
                        for (CurV = vHead.Next; CurV != null; CurV = CurV->Next)
                        {
                            j = CurV->i;
                            if (deltaMin > Delta[i][j])
                            {
                                deltaMin = Delta[i][j];
                                minI = i;
                                minJ = j;
                                PrevUMinI = PrevU;
                                PrevVMinJ = PrevV;
                                found = true;
                            }
                            PrevV = CurV;
                        }
                        PrevU = CurU;
                    }

                    if (!found)
                        break;

                    /* ADD X[minI][minJ] TO THE BASIS, AND ADJUST SUPPLIES AND COST */
                    Remember = PrevUMinI->Next;
                    addBasicVariable(minI, minJ, S, D, PrevUMinI, PrevVMinJ, &uHead);

                    /* UPDATE THE NECESSARY Delta[][] */
                    if (Remember == PrevUMinI->Next)  /* LINE minI WAS DELETED */
                    {
                        for (CurV = vHead.Next; CurV != null; CurV = CurV->Next)
                        {
                            j = CurV->i;
                            if (CurV->val == _C[minI][j])  /* COLUMN j NEEDS UPDATING */
                            {
                                /* FIND THE NEW MAXIMUM VALUE IN THE COLUMN */
                                oldVal = CurV->val;
                                CurV->val = -INFINITY;
                                for (CurU = uHead.Next; CurU != null; CurU = CurU->Next)
                                {
                                    i = CurU->i;
                                    if (CurV->val <= _C[i][j])
                                        CurV->val = _C[i][j];
                                }

                                /* IF NEEDED, ADJUST THE RELEVANT Delta[*][j] */
                                diff = oldVal - CurV->val;
                                if (Math.Abs(diff) < EPSILON * _maxC)
                                    for (CurU = uHead.Next; CurU != null; CurU = CurU->Next)
                                        Delta[CurU->i][j] += diff;
                            }
                        }
                    }
                    else  /* COLUMN minJ WAS DELETED */
                    {
                        for (CurU = uHead.Next; CurU != null; CurU = CurU->Next)
                        {
                            i = CurU->i;
                            if (CurU->val == _C[i][minJ])  /* ROW i NEEDS UPDATING */
                            {
                                /* FIND THE NEW MAXIMUM VALUE IN THE ROW */
                                oldVal = CurU->val;
                                CurU->val = -INFINITY;
                                for (CurV = vHead.Next; CurV != null; CurV = CurV->Next)
                                {
                                    j = CurV->i;
                                    if (CurU->val <= _C[i][j])
                                        CurU->val = _C[i][j];
                                }

                                /* If NEEDED, ADJUST THE RELEVANT Delta[i][*] */
                                diff = oldVal - CurU->val;
                                if (Math.Abs(diff) < EPSILON * _maxC)
                                    for (CurV = vHead.Next; CurV != null; CurV = CurV->Next)
                                        Delta[i][CurV->i] += diff;
                            }
                        }
                    }
                } while (uHead.Next != null || vHead.Next != null);
            }
        }

        private void addBasicVariable(
            int minI, int minJ, double* S, double* D,
            node1_t* PrevUMinI, node1_t* PrevVMinJ,
            node1_t* UHead)
        {
            double T;

            if (Math.Abs(S[minI] - D[minJ]) <= EPSILON * _maxW)  /* DEGENERATE CASE */
            {
                T = S[minI];
                S[minI] = 0;
                D[minJ] -= T;
            }
            else if (S[minI] < D[minJ])  /* SUPPLY EXHAUSTED */
            {
                T = S[minI];
                S[minI] = 0;
                D[minJ] -= T;
            }
            else  /* DEMAND EXHAUSTED */
            {
                T = D[minJ];
                D[minJ] = 0;
                S[minI] -= T;
            }

            /* X(minI,minJ) IS A BASIC VARIABLE */
            _IsX[minI][minJ] = true;

            _EndX->val = T;
            _EndX->i = minI;
            _EndX->j = minJ;
            _EndX->NextC = _RowsX[minI];
            _EndX->NextR = _ColsX[minJ];
            _RowsX[minI] = _EndX;
            _ColsX[minJ] = _EndX;
            _EndX++;

            /* DELETE SUPPLY ROW ONLY IF THE EMPTY, AND IF NOT LAST ROW */
            if (S[minI] == 0 && UHead->Next->Next != null)
                PrevUMinI->Next = PrevUMinI->Next->Next;  /* REMOVE ROW FROM LIST */
            else
                PrevVMinJ->Next = PrevVMinJ->Next->Next;  /* REMOVE COLUMN FROM LIST */
        }

        private static T[][] CreateJaggedArray<T>(int rows, int cols)
        {
            var array = new T[rows][];
            for (var i = 0; i < rows; i++)
            {
                array[i] = new T[cols];
            }
            return array;
        }
    }
}
