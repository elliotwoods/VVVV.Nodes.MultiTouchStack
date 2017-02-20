using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VVVV.Utils.VMath;

namespace VVVV.Nodes
{
    internal class SMTSlide : IComparable<SMTSlide>
    {
        private int _ImageID;
        private long _UniqueID;
        private int _OwnerID;
        private DateTime _LastTouched;
        private DateTime _StartPressClose;
        private bool _isButtonBeingPressed;
        private bool _isClosed;
        private Matrix4x4 _StartMatrix;
        private List<SMTAttachedCursor> _AttachedCursors;
        private double _AspectRatio;
        private Matrix4x4 _MoveStartMatrix;
        private Matrix4x4 _MoveEndMatrix;
        private bool _MoveisBeingMoved;
        private DateTime _MoveStartTime;
        private double _MoveDuration;
        private bool _isHit;
        private bool _isHidden;
        private stack_mt _Host;

        public DateTime LastTouched
        {
            get
            {
                return this._LastTouched;
            }
            set
            {
                this._LastTouched = value;
            }
        }

        public int ImageID
        {
            get
            {
                return this._ImageID;
            }
            set
            {
                this._ImageID = value;
            }
        }

        public long UniqueID
        {
            get
            {
                return this._UniqueID;
            }
            set
            {
                this._UniqueID = value;
            }
        }

        public int nCursors
        {
            get
            {
                return this._AttachedCursors.Count;
            }
        }

        public bool isBeingPressed
        {
            get
            {
                return this._isButtonBeingPressed;
            }
            set
            {
                if (value)
                    this.doTouch();
                if (value && !this._isButtonBeingPressed)
                    this._StartPressClose = DateTime.Now;
                else if (value && (DateTime.Now - this._StartPressClose).TotalMilliseconds > this._Host.ButtonDuration * 1000.0)
                {
                    if (this._Host.ButtonAction == 0)
                        this._isHit = true;
                    else
                        this._isClosed = true;
                    this._StartPressClose = DateTime.Now;
                }
                this._isButtonBeingPressed = value;
            }
        }

        public bool isHit
        {
            get
            {
                if (!this._isHit)
                    return false;
                this._isHit = false;
                return true;
            }
        }

        public bool isClosed
        {
            get
            {
                return this._isClosed;
            }
            set
            {
                this._isClosed = value;
            }
        }

        public bool isHidden
        {
            get
            {
                return this._isHidden;
            }
            set
            {
                this._isHidden = value;
            }
        }

        public double closeProgress
        {
            get
            {
                if (!this._isButtonBeingPressed)
                    return 0.0;
                return (DateTime.Now - this._StartPressClose).TotalMilliseconds / (this._Host.ButtonDuration * 1000.0);
            }
        }

        public int owner
        {
            get
            {
                return this._OwnerID;
            }
        }

        public double AspectRatio
        {
            get
            {
                return this._AspectRatio;
            }
        }

        public bool isMoving
        {
            get
            {
                return this._MoveisBeingMoved;
            }
        }

        public SMTSlide(Matrix4x4 Transform, int ImageID, int OwnerID, long UniqueID, double AspectRatio, stack_mt Host)
        {
            this._StartMatrix = Transform;
            this._ImageID = ImageID;
            this._UniqueID = UniqueID;
            this._OwnerID = OwnerID;
            this._LastTouched = DateTime.Now;
            this._AttachedCursors = new List<SMTAttachedCursor>();
            this._AspectRatio = AspectRatio;
            this._MoveisBeingMoved = false;
            this._Host = Host;
        }

        public void doTouch()
        {
            this._LastTouched = DateTime.Now;
        }

        private double z(int backindex)
        {
            return -Convert.ToDouble(backindex) / 1024.0;
        }

        public Matrix4x4 getMatrix(int backindex)
        {
            if (this.isMoving)
                return this.getMoveMatrix(backindex);
            Matrix4x4 input;
            switch (this._AttachedCursors.Count)
            {
                case 0:
                    input = this._StartMatrix;
                    break;
                case 1:
                    input = this._StartMatrix * VMath.Translate(new Vector3D(this._AttachedCursors[0].deltaxy));
                    break;
                default:
                    double num1 = (double)this._AttachedCursors[0].StartXY.x;
                    double num2 = (double)this._AttachedCursors[0].StartXY.y;
                    double num3 = (double)this._AttachedCursors[1].StartXY.x;
                    double num4 = (double)this._AttachedCursors[1].StartXY.y;
                    double num5 = (double)this._AttachedCursors[0].xy.x;
                    double num6 = (double)this._AttachedCursors[0].xy.y;
                    double num7 = (double)this._AttachedCursors[1].xy.x;
                    double num8 = (double)this._AttachedCursors[1].xy.y;
                    double num9 = num3 - num1;
                    double num10 = num4 - num2;
                    double num11 = num7 - num5;
                    double num12 = num8 - num6;
                    Math.Sqrt((num11 * num11 + num12 * num12) / (num9 * num9 + num10 * num10));
                    double num13 = (num9 * num11 + num10 * num12) / (num9 * num9 + num10 * num10);
                    double num14 = (num10 * num11 - num9 * num12) / (num9 * num9 + num10 * num10);
                    double num15 = num5 - (num13 * num1 + num14 * num2);
                    double num16 = num6 - (-num14 * num1 + num13 * num2);
                    input = this._StartMatrix * new Matrix4x4(num13, -num14, 0.0, 0.0, num14, num13, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, num15, num16, 0.0, 1.0);
                    break;
            }
            input.m43 = this.z(backindex);
            return this.checkConstraints(input);
        }

        private Matrix4x4 checkConstraints(Matrix4x4 input)
        {
            double num1 = (double)input.m41;
            double num2 = (double)input.m42;
            double num3 = Math.Sqrt(Math.Pow(num1, 2.0) + Math.Pow(num2, 2.0));
            double num4 = Math.Atan2(num2, num1);
            switch (this._Host.ConstraintID)
            {
                case 1:
                    num1 = Math.Max(Math.Min(num1, 1.0), -1.0);
                    num2 = Math.Max(Math.Min(num2, 1.0), -1.0);
                    break;
                case 2:
                    if (num3 > 1.0)
                    {
                        double num5 = 1.0;
                        num1 = num5 * Math.Cos(num4);
                        num2 = num5 * Math.Sin(num4);
                        break;
                    }
                    break;
            }
            input.m41 = num1;
            input.m42 = num2;
            return input;
        }

        public Matrix4x4 getCloseButtonMatrix(int backindex)
    {
      Matrix4x4 matrix = this.getMatrix(backindex);
      Vector3D vector3D = matrix * new Vector3D(this._Host.ButtonPosition);
      double num1 = Math.Acos(matrix.m11 / Math.Sqrt(Math.Pow((double) matrix.m11, 2.0) + Math.Pow((double) matrix.m12, 2.0)));
      if (matrix.m12 < 0.0)
        num1 = 2.0 * Math.PI - num1;
      Matrix4x4 matrix4x4 = VMath.Scale(this._Host.ButtonScale, this._Host.ButtonScale, 1.0) * VMath.RotateZ(num1) * VMath.Translate(vector3D);
      return matrix4x4;
    }

        public Matrix4x4 getShadowMatrix(int backindex)
        {
            Matrix4x4 matrix = this.getMatrix(backindex);
            double num = Math.Acos(matrix.m11 / Math.Sqrt(Math.Pow((double)matrix.m11, 2.0) + Math.Pow((double)matrix.m12, 2.0)));
            if (matrix.m12 < 0.0)
                num = 2.0 * Math.PI - num;
            return VMath.RotateZ(num);
        }

        public int CompareTo(SMTSlide other)
        {
            if (other.LastTouched.Equals(this.LastTouched))
                return other.UniqueID.CompareTo(this.UniqueID);
            return other.LastTouched.CompareTo(this._LastTouched);
        }

        public void detachCursors()
        {
            this._StartMatrix = this.getMatrix(0);
            this._AttachedCursors.Clear();
        }

        public void attachCursor(SMTAttachedCursor addcursor)
        {
            this._AttachedCursors.Add(addcursor);
            this.doTouch();
        }

        public List<SMTIntersect> updateCursors(List<SMTCursor> CurrentCursors)
        {
            List<SMTIntersect> list1 = new List<SMTIntersect>();
            foreach (SMTCursor smtCursor in CurrentCursors)
            {
                for (int index = 0; index < this._AttachedCursors.Count; ++index)
                {
                    if (smtCursor.isID(this._AttachedCursors[index].ID))
                    {
                        this._AttachedCursors[index].xy = smtCursor.xy;
                        List<SMTIntersect> list2 = list1;
                        long inSlideID = this._UniqueID;
                        Vector2D xy1 = smtCursor.xy;
                        Vector3D vector3D = (!this.getMatrix(0)) * smtCursor.xyz;
                        // ISSUE: explicit reference operation
                        Vector2D xy2 = ((Vector3D)@vector3D).xy;
                        SMTIntersect smtIntersect = new SMTIntersect(inSlideID, xy1, xy2);
                        list2.Add(smtIntersect);
                    }
                }
            }
            return list1;
        }

        public bool hasCursors()
        {
            return this._AttachedCursors.Count > 0;
        }

        public void doMove(Matrix4x4 MoveEndMatrix, double MoveDuration)
        {
            this._MoveStartMatrix = this.getMatrix(0);
            this._MoveEndMatrix = MoveEndMatrix;
            this._MoveDuration = MoveDuration;
            this._MoveStartTime = DateTime.Now;
            this._MoveisBeingMoved = true;
        }

        private Matrix4x4 getMoveMatrix(int backindex)
        {
            double num = (DateTime.Now - this._MoveStartTime).TotalSeconds / this._MoveDuration;
            if (num >= 1.0 || this._MoveDuration == 0.0)
                this._MoveisBeingMoved = false;
            Matrix4x4 matrix4x4 = !this._MoveisBeingMoved ? this._MoveEndMatrix : this._MoveStartMatrix + (num * (this._MoveEndMatrix - this._MoveStartMatrix));
            matrix4x4.m43 = this.z(backindex);
            this._StartMatrix = matrix4x4;
            this._LastTouched = DateTime.Now;
            return matrix4x4;
        }
    }
}
