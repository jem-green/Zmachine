using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMachineLibrary
{
    public class Cursor
    {
        int _left;
        int _top;

        public Cursor()
        {
        }
        public Cursor(int left, int top)
        {
            _left = left;
            _top = top;
        }

        public int Left
        {
            get
            {
                return (_left);
            }
            set
            {
                _left = value;
            }
        }
        public int Top
        {
            get
            {
                return (_top);
            }
            set
            {
                _top = value;
            }
        }
    }
}
