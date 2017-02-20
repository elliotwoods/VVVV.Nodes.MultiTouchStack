using VVVV.Utils.VMath;

namespace VVVV.Nodes
{
    internal class SMTAttachedCursor
    {
        private Vector2D _StartXY;
        private Vector2D _CurrentXY;
        private int _ID;

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
                return this._CurrentXY;
            }
            set
            {
                this._CurrentXY = value;
            }
        }

        public Vector2D StartXY
        {
            get
            {
                return this._StartXY;
            }
        }

        public Vector2D deltaxy
        {
            get
            {
                return this._CurrentXY - this._StartXY;
            }
        }

        public SMTAttachedCursor(Vector2D XY, int ID)
        {
            this._CurrentXY = XY;
            this._StartXY = XY;
            this._ID = ID;
        }
    }
}
