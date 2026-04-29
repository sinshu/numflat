using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumFlat
{
    public struct IndexValuePair<T>
    {
        private int index;
        private T value;

        internal IndexValuePair(int index, T value)
        {
            this.index = index;
            this.value = value;
        }

        public override string ToString()
        {
            return "(" + index + ", " + value + ")";
        }

        public int Index => index;

        public T Value => value;
    }
}
