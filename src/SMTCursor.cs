using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using VVVV.Utils.VMath;

namespace VVVV.Nodes
{
    internal class SMTCursor
    {
        private Vector2D _xy;
        private Vector2D _Startxy;
        private int _ID;
        private int _type;
        private long _SlideID;
        private bool _pleasedeleteme;

        public Vector2D xy
        {
            get
            {
                return this._xy;
            }
            set
            {
                this._xy = value;
            }
        }

        public Vector3D xyz
        {
            get
            {
                return new Vector3D(this.xy);
            }
        }

        public int ID
        {
            get
            {
                return this._ID;
            }
        }

        public bool pleasedeleteme
        {
            get
            {
                return this._pleasedeleteme;
            }
            set
            {
                this._pleasedeleteme = value;
            }
        }

        public int type
        {
            get
            {
                return this._type;
            }
        }

        public Vector2D deltaxy
        {
            get
            {
                return this._xy - this._Startxy;
            }
        }

        public long SlideID
        {
            get
            {
                return this._SlideID;
            }
            set
            {
                this._SlideID = value;
            }
        }

        public SMTCursor(Vector2D xy, int ID, long SlideID, int type)
        {
            this._xy = xy;
            this._Startxy = xy;
            this._ID = ID;
            this._type = type;
            this._SlideID = SlideID;
            this._pleasedeleteme = false;
        }

        public bool isID(int inID)
        {
            return inID == this._ID;
        }

        public bool isID(double inID)
        {
            return Convert.ToInt32(inID) == this._ID;
        }
    }
}