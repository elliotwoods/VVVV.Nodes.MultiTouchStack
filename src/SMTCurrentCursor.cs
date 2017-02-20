using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.Utils.VMath;

namespace VVVV.Nodes
{
    internal class SMTCurrentCursor
    {
        private int _ID;
        private double _x;
        private double _y;

        public int ID
        {
            get
            {
                return this._ID;
            }
        }

        public Vector2D xy
        {
            get
            {
                return new Vector2D(this._x, this._y);
            }
        }

        public SMTCurrentCursor(int ID, double x, double y)
        {
            this._ID = ID;
            this._x = x;
            this._y = y;
        }
    }
}
