using System;

namespace EmdFlat
{
    internal unsafe struct node2_t
    {
        public int i, j;
        public double val;
        public node2_t* NextC;  /* NEXT COLUMN */
        public node2_t* NextR;  /* NEXT ROW */
    }
}
