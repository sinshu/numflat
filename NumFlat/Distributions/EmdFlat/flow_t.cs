#pragma warning disable CS1591

using System;

namespace EmdFlat
{
    public struct flow_t
    {
        public int from;       /* Feature number in signature 1 */
        public int to;         /* Feature number in signature 2 */
        public double amount;  /* Amount of flow from "from" to "to" */
    }
}
