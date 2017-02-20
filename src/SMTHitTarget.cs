using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VVVV.Utils.VMath;

namespace VVVV.Nodes
{
    internal class SMTHitTarget : IComparable<SMTHitTarget>
    {
        private Matrix4x4 _Transform;
        private long _UniqueID;
        private uint _type;

        public double height
        {
            get
            {
                return (double)this._Transform.m43;
            }
        }

        public long UniqueID
        {
            get
            {
                return this._UniqueID;
            }
        }

        public uint type
        {
            get
            {
                return this._type;
            }
        }

        public SMTHitTarget(Matrix4x4 Transform, long UniqueID, uint type)
        {
            this._Transform = Transform;
            this._UniqueID = UniqueID;
            this._type = type;
        }

        public bool checkHit(Vector2D ScreenXY)
        {
            Vector3D vector3D = (!this._Transform) * new Vector3D(ScreenXY);
            if (Math.Abs((double)vector3D.x) < 0.5)
                return Math.Abs((double)vector3D.y) < 0.5;
            return false;
        }

        public int CompareTo(SMTHitTarget other)
        {
            return this.height.CompareTo(other.height);
        }
    }
}