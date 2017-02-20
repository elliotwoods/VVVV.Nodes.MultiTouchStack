using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VVVV.Utils.VMath;

namespace VVVV.Nodes
{
    internal class SMTIntersect
    {
        private long _SlideID;
        private Vector2D _ScreenXY;
        private Vector2D _SlideXY;

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

        public Vector2D SlideXY
        {
            get
            {
                return this._SlideXY;
            }
            set
            {
                this._SlideXY = value;
            }
        }

        public Vector2D ScreenXY
        {
            get
            {
                return this._ScreenXY;
            }
            set
            {
                this._ScreenXY = value;
            }
        }

        public SMTIntersect(long inSlideID, Vector2D inScreenXY, Vector2D inSlideXY)
        {
            this._SlideID = inSlideID;
            this._ScreenXY = inScreenXY;
            this._SlideXY = inSlideXY;
        }
    }
}
