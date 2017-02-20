using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VVVV.PluginInterfaces.V1;
using VVVV.Utils.VMath;

namespace VVVV.Nodes
{
    public class stack_mt : IPlugin, IDisposable
    {
        private List<SMTCurrentCursor> _CurrentCursors = new List<SMTCurrentCursor>();
        private List<SMTHitTarget> _CurrentTargets = new List<SMTHitTarget>();
        private List<SMTIntersect> _CurrentIntersects = new List<SMTIntersect>();
        private List<SMTCursor> _CurrentPassthroughs = new List<SMTCursor>();
        private List<int> _CurrentTrashSlides = new List<int>();
        private List<SMTSlide> _SlideList = new List<SMTSlide>();
        private List<SMTCursor> _CursorList = new List<SMTCursor>();
        private long UniqueSlideID;
        public int ButtonAction;
        public Vector2D ButtonPosition;
        public double ButtonScale;
        public double ButtonDuration;
        public int ConstraintID;
        private IPluginHost FHost;
        private bool FDisposed;
        private IValueConfig PinInButtonSize;
        private IValueConfig PinInButtonPosition;
        private IValueConfig PinInButtonDuration;
        private IEnumConfig PinInButtonAction;
        private IEnumConfig PinInConstraint;
        private IValueIn PinInFingerXY;
        private IValueIn PinInFingerID;
        private ITransformIn PinInAddTransform;
        private IValueIn PinInAddIndex;
        private IValueIn PinInAddAspectRatio;
        private IValueIn PinInDoAdd;
        private IValueIn PinInAddOwner;
        private IValueIn PinInRemoveAllFromOwner;
        private IValueIn PinInMoveID;
        private IValueIn PinInMoveDuration;
        private IValueIn PinInMoveDo;
        private ITransformIn PinInMoveTransform;
        private IValueIn PinInHide;
        private IValueIn PinInShow;
        private IValueIn PinInReset;
        private ITransformOut PinOutTransform;
        private ITransformOut PinOutButtonTransform;
        private ITransformOut PinOutShadowTransform;
        private IValueOut PinOutIndex;
        private IValueOut PinOutAspectRatio;
        private IValueOut PinOutCount;
        private IValueOut PinOutSlideCursor;
        private IValueOut PinOutCursorSlideID;
        private IValueOut PinOutOver;
        private IValueOut PinOutHit;
        private IValueOut PinOutPassThru;
        private ITransformOut PinOutTrashTransform;
        private IValueOut PinOutTrashOwner;
        private IValueOut PinOutCloseProgress;
        private IValueOut PinOutTrashID;
        private static IPluginInfo FPluginInfo;

        public static IPluginInfo PluginInfo
        {
            get
            {
                if (stack_mt.FPluginInfo == null)
                {
                    stack_mt.FPluginInfo = (IPluginInfo)new VVVV.PluginInterfaces.V1.PluginInfo();
                    stack_mt.FPluginInfo.Name = ("Stack");
                    stack_mt.FPluginInfo.Category = ("MultiTouch");
                    stack_mt.FPluginInfo.Version = ("");
                    stack_mt.FPluginInfo.Author = ("sugokuGENKI");
                    stack_mt.FPluginInfo.Help = ("Gives positions for items in a multitouch stack");
                    stack_mt.FPluginInfo.Tags = ("");
                    stack_mt.FPluginInfo.Credits = ("");
                    stack_mt.FPluginInfo.Bugs = ("");
                    stack_mt.FPluginInfo.Warnings = ("");
                    MethodBase method = new StackTrace(true).GetFrame(0).GetMethod();
                    stack_mt.FPluginInfo.Namespace = (method.DeclaringType.Namespace);
                    stack_mt.FPluginInfo.Class = (method.DeclaringType.Name);
                }
                return stack_mt.FPluginInfo;
            }
        }

        public bool AutoEvaluate
        {
            get
            {
                return false;
            }
        }

        ~stack_mt()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.FDisposed)
            {
                int num = disposing ? 1 : 0;
                this.FHost.Log((TLogType)0, "PluginTemplate is being deleted");
            }
            this.FDisposed = true;
        }

        public void SetPluginHost(IPluginHost Host)
        {
            this.FHost = Host;
            this.FHost.CreateValueConfig("Button size", 1, (string[])null, (TSliceMode)0, (TPinVisibility)3, out this.PinInButtonSize);
            this.PinInButtonSize.SetSubType(0.0, double.MaxValue, 0.001, 0.1, false, false, false);
            this.FHost.CreateValueConfig("Button position", 2, (string[])null, (TSliceMode)0, (TPinVisibility)3, out this.PinInButtonPosition);
            this.PinInButtonPosition.SetSubType2D(double.MinValue, double.MaxValue, 0.001, 0.5, -0.5, false, false, false);
            this.FHost.CreateValueConfig("Button duration", 1, new string[1]
      {
        "s"
      }, (TSliceMode)0, (TPinVisibility)3, out this.PinInButtonDuration);
            this.PinInButtonDuration.SetSubType(0.0, double.MaxValue, 0.001, 0.2, false, false, false);
            this.FHost.UpdateEnum("Stack MT button action", "bang", new string[2]
      {
        "bang",
        "close"
      });
            this.FHost.CreateEnumConfig("Button action", (TSliceMode)0, (TPinVisibility)1, out this.PinInButtonAction);
            this.PinInButtonAction.SetSubType("Stack MT button action");
            this.FHost.UpdateEnum("Stack MT constraints", "none", new string[3]
      {
        "none",
        "square",
        "circle"
      });
            this.FHost.CreateEnumConfig("Constraints", (TSliceMode)0, (TPinVisibility)1, out this.PinInConstraint);
            this.PinInConstraint.SetSubType("Stack MT constraints");
            this.FHost.CreateValueInput("Finger ID", 1, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinInFingerID);
            this.PinInFingerID.SetSubType(double.MinValue, double.MaxValue, 0.001, 0.0, false, false, true);
            this.FHost.CreateValueInput("Finger", 2, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinInFingerXY);
            this.PinInFingerXY.SetSubType2D(double.MinValue, double.MaxValue, 0.001, 0.0, 0.0, false, false, false);
            this.FHost.CreateTransformInput("Add Transform", (TSliceMode)1, (TPinVisibility)3, out this.PinInAddTransform);
            this.FHost.CreateValueInput("Add index", 1, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinInAddIndex);
            this.PinInAddIndex.SetSubType(0.0, double.MaxValue, 1.0, 0.0, false, false, true);
            this.FHost.CreateValueInput("Add owner", 1, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinInAddOwner);
            this.PinInAddOwner.SetSubType(double.MinValue, double.MaxValue, 1.0, 0.0, false, false, true);
            this.FHost.CreateValueInput("Add aspect ratio", 1, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinInAddAspectRatio);
            this.PinInAddAspectRatio.SetSubType(double.MinValue, double.MaxValue, 0.001, 0.0, false, false, false);
            this.FHost.CreateValueInput("Add", 1, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinInDoAdd);
            this.PinInDoAdd.SetSubType(0.0, 1.0, 1.0, 0.0, true, false, false);
            this.FHost.CreateValueInput("Move ID", 1, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinInMoveID);
            this.PinInMoveID.SetSubType(double.MinValue, double.MaxValue, 1.0, 0.0, false, false, true);
            this.FHost.CreateTransformInput("Move Transform", (TSliceMode)1, (TPinVisibility)3, out this.PinInMoveTransform);
            this.FHost.CreateValueInput("Move Duration", 1, new string[1]
      {
        "s"
      }, (TSliceMode)1, (TPinVisibility)3, out this.PinInMoveDuration);
            this.PinInMoveDuration.SetSubType(0.0, double.MaxValue, 0.001, 0.2, false, false, false);
            this.FHost.CreateValueInput("Move", 1, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinInMoveDo);
            this.PinInMoveDo.SetSubType(0.0, 1.0, 1.0, 0.0, true, false, false);
            this.FHost.CreateValueInput("Hide ID", 1, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinInHide);
            this.PinInHide.SetSubType(double.MinValue, double.MaxValue, 1.0, -1.0, false, false, true);
            this.FHost.CreateValueInput("Show ID", 1, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinInShow);
            this.PinInShow.SetSubType(double.MinValue, double.MaxValue, 1.0, -1.0, false, false, true);
            this.FHost.CreateValueInput("Remove all from owner", 1, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinInRemoveAllFromOwner);
            this.PinInRemoveAllFromOwner.SetSubType(double.MinValue, double.MaxValue, 1.0, -1.0, false, false, true);
            this.FHost.CreateValueInput("Reset", 1, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinInReset);
            this.PinInReset.SetSubType(0.0, 1.0, 1.0, 0.0, true, false, false);
            ((IPluginIO)this.PinInFingerID).Order = (0);
            ((IPluginIO)this.PinInFingerXY).Order = (1);
            ((IPluginIO)this.PinInAddAspectRatio).Order = (2);
            ((IPluginIO)this.PinInAddIndex).Order = (3);
            ((IPluginIO)this.PinInAddOwner).Order = (4);
            ((IPluginIO)this.PinInAddTransform).Order = (5);
            ((IPluginIO)this.PinInDoAdd).Order = (6);
            ((IPluginIO)this.PinInMoveDo).Order = (7);
            ((IPluginIO)this.PinInMoveTransform).Order = (8);
            ((IPluginIO)this.PinInMoveDuration).Order = (9);
            ((IPluginIO)this.PinInMoveID).Order = (10);
            ((IPluginIO)this.PinInShow).Order = (11);
            ((IPluginIO)this.PinInHide).Order = (12);
            ((IPluginIO)this.PinInRemoveAllFromOwner).Order = (13);
            ((IPluginIO)this.PinInReset).Order = (14);
            this.FHost.CreateTransformOutput("Slide Transform", (TSliceMode)1, (TPinVisibility)3, out this.PinOutTransform);
            this.FHost.CreateTransformOutput("Shadow Transform", (TSliceMode)1, (TPinVisibility)3, out this.PinOutShadowTransform);
            this.FHost.CreateTransformOutput("Button Transform", (TSliceMode)1, (TPinVisibility)3, out this.PinOutButtonTransform);
            this.FHost.CreateValueOutput("Index", 1, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinOutIndex);
            this.PinOutIndex.SetSubType(0.0, double.MaxValue, 1.0, 0.0, false, false, true);
            this.FHost.CreateValueOutput("Aspect Ratio", 1, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinOutAspectRatio);
            this.PinOutAspectRatio.SetSubType(0.0, double.MaxValue, 0.001, 0.0, false, false, false);
            this.FHost.CreateValueOutput("Slide Cursor", 2, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinOutSlideCursor);
            this.PinOutSlideCursor.SetSubType2D(double.MinValue, double.MaxValue, 0.001, 0.0, 0.0, false, false, false);
            this.FHost.CreateValueOutput("Cursor Slide ID", 1, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinOutCursorSlideID);
            this.PinOutCursorSlideID.SetSubType(double.MinValue, double.MaxValue, 1.0, 0.0, false, false, true);
            this.FHost.CreateValueOutput("Cursor passthrough", 3, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinOutPassThru);
            this.PinOutPassThru.SetSubType2D(double.MinValue, double.MaxValue, 0.001, 0.0, 0.0, false, false, false);
            this.FHost.CreateValueOutput("Selected", 1, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinOutOver);
            this.PinOutOver.SetSubType(0.0, 1.0, 1.0, 0.0, false, true, true);
            this.FHost.CreateValueOutput("Hit", 1, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinOutHit);
            this.PinOutHit.SetSubType(0.0, 1.0, 1.0, 0.0, true, false, false);
            this.FHost.CreateValueOutput("Close progress", 1, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinOutCloseProgress);
            this.PinOutCloseProgress.SetSubType(0.0, 1.0, 0.001, 0.0, false, false, false);
            this.FHost.CreateTransformOutput("Trash Transform", (TSliceMode)1, (TPinVisibility)3, out this.PinOutTrashTransform);
            this.FHost.CreateValueOutput("Trash ID", 1, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinOutTrashID);
            this.PinOutTrashID.SetSubType(0.0, double.MaxValue, 1.0, 0.0, false, false, true);
            this.FHost.CreateValueOutput("Trash owner ID", 1, (string[])null, (TSliceMode)1, (TPinVisibility)3, out this.PinOutTrashOwner);
            this.PinOutTrashOwner.SetSubType(0.0, double.MaxValue, 1.0, 0.0, false, false, true);
            this.FHost.CreateValueOutput("Count", 1, (string[])null, (TSliceMode)0, (TPinVisibility)3, out this.PinOutCount);
            this.PinOutCount.SetSubType(0.0, double.MaxValue, 1.0, 0.0, false, false, true);
            ((IPluginIO)this.PinOutTransform).Order = (0);
            ((IPluginIO)this.PinOutAspectRatio).Order = (1);
            ((IPluginIO)this.PinOutShadowTransform).Order = (2);
            ((IPluginIO)this.PinOutIndex).Order = (3);
            ((IPluginIO)this.PinOutButtonTransform).Order = (4);
            ((IPluginIO)this.PinOutCloseProgress).Order = (5);
            ((IPluginIO)this.PinOutTrashID).Order = (6);
            ((IPluginIO)this.PinOutTrashOwner).Order = (7);
            ((IPluginIO)this.PinOutTrashTransform).Order = (8);
            ((IPluginIO)this.PinOutOver).Order = (9);
            ((IPluginIO)this.PinOutHit).Order = (10);
            ((IPluginIO)this.PinOutCursorSlideID).Order = (11);
            ((IPluginIO)this.PinOutSlideCursor).Order = (12);
            ((IPluginIO)this.PinOutPassThru).Order = (13);
            ((IPluginIO)this.PinOutCount).Order = (14);
        }

        public void Configurate(IPluginConfig Input)
        {
            string str1 = "";
            string str2 = "";
            double num1;
            double num2;
            this.PinInButtonPosition.GetValue2D(0, out num1, out num2);
            this.PinInButtonSize.GetValue(0, out this.ButtonScale);
            this.PinInButtonDuration.GetValue(0, out this.ButtonDuration);
            this.ButtonPosition = new Vector2D(num1, num2);
            this.PinInButtonAction.GetString(0, out str2);
            switch (str2)
            {
                case "bang":
                    this.ButtonAction = 0;
                    break;
                case "close":
                    this.ButtonAction = 1;
                    break;
            }
            if (this.PinInConstraint == null)
                return;
            this.PinInConstraint.GetString(0, out str1);
            switch (str1)
            {
                case "none":
                    this.ConstraintID = 0;
                    break;
                case "square":
                    this.ConstraintID = 1;
                    break;
                case "circle":
                    this.ConstraintID = 2;
                    break;
            }
        }

        public void Evaluate(int SpreadMax)
        {
            double num1 = 0.0;
            double num2 = 0.0;
            if (((IPluginIn)this.PinInDoAdd).SliceCount > 0)
                this.PinInDoAdd.GetValue(0, out num1);
            if (((IPluginIn)this.PinInReset).SliceCount > 0)
                this.PinInReset.GetValue(0, out num2);
            if (num1 > 0.0)
                this.fnAddSlide();
            if (num2 > 0.0)
                this.fnReset();
            this.fnRemoveAllFromOwner();
            if (((IPluginIn)this.PinInShow).SliceCount > 0 || ((IPluginIn)this.PinInHide).SliceCount > 0)
                this.fnShowAndHide();
            this.fnMaybeMove();
            this.fnAddCurrentCursors();
            this.fnFindCursors();
            this.fnPressAndRelease();
            this.fnFindPassThrus();
            this.fnOutput();
        }

        private void fnAddSlide()
        {
            int num1 = Math.Max(((IPluginIn)this.PinInAddIndex).SliceCount, ((IPluginIn)this.PinInAddTransform).SliceCount);
            for (int index = 0; index < num1; ++index)
            {
                Matrix4x4 Transform;
                this.PinInAddTransform.GetMatrix(index, out Transform);
                double num2;
                this.PinInAddIndex.GetValue(index, out num2);
                double num3;
                this.PinInAddOwner.GetValue(index, out num3);
                double AspectRatio;
                this.PinInAddAspectRatio.GetValue(index, out AspectRatio);
                this._SlideList.Add(new SMTSlide(Transform, Convert.ToInt32(num2), Convert.ToInt32(num3), this.UniqueSlideID++, AspectRatio, this));
            }
        }

        private void fnReset()
        {
            this._SlideList.Clear();
        }

        private void fnRemoveAllFromOwner()
        {
            this._CurrentTrashSlides.Clear();
            int sliceCount = ((IPluginIn)this.PinInRemoveAllFromOwner).SliceCount;
            for (int index1 = 0; index1 < sliceCount; ++index1)
            {
                double num1;
                this.PinInRemoveAllFromOwner.GetValue(index1, out num1);
                int num2 = Convert.ToInt32(num1);
                for (int index2 = 0; index2 < this._SlideList.Count; ++index2)
                {
                    if (this._SlideList[index2].owner == num2)
                    {
                        this._CurrentTrashSlides.Add(index2);
                        this._SlideList[index2].isClosed = true;
                    }
                }
            }
        }

        private void fnShowAndHide()
        {
            List<SMTSlide> list = new List<SMTSlide>();
            int sliceCount1 = ((IPluginIn)this.PinInShow).SliceCount;
            int sliceCount2 = ((IPluginIn)this.PinInHide).SliceCount;
            double num;
            int intID;
            for (int index = 0; index < sliceCount1; ++index)
            {
                this.PinInShow.GetValue(index, out num);
                intID = Convert.ToInt32(num);
                foreach (SMTSlide smtSlide in this._SlideList.FindAll((Predicate<SMTSlide>)(thisslide => thisslide.ImageID == intID)))
                    smtSlide.isHidden = false;
            }
            for (int index = 0; index < sliceCount2; ++index)
            {
                this.PinInHide.GetValue(index, out num);
                intID = Convert.ToInt32(num);
                foreach (SMTSlide smtSlide in this._SlideList.FindAll((Predicate<SMTSlide>)(thisslide => thisslide.ImageID == intID)))
                    smtSlide.isHidden = true;
            }
        }

        private void fnMaybeMove()
        {
            if (!((IPluginIn)this.PinInMoveDo).PinIsChanged)
                return;
            int num1 = Math.Max(((IPluginIn)this.PinInMoveDo).SliceCount, Math.Max(Math.Max(((IPluginIn)this.PinInMoveDuration).SliceCount, ((IPluginIn)this.PinInMoveID).SliceCount), ((IPluginIn)this.PinInMoveTransform).SliceCount));
            for (int index = 0; index < num1; ++index)
            {
                double num2;
                this.PinInMoveDo.GetValue(index, out num2);
                if (num2 > 0.5)
                {
                    double num3;
                    this.PinInMoveID.GetValue(index, out num3);
                    double MoveDuration;
                    this.PinInMoveDuration.GetValue(index, out MoveDuration);
                    Matrix4x4 MoveEndMatrix;
                    this.PinInMoveTransform.GetMatrix(index, out MoveEndMatrix);
                    int num4 = Convert.ToInt32(num3);
                    foreach (SMTSlide smtSlide in this._SlideList)
                    {
                        if (smtSlide.ImageID == num4)
                            smtSlide.doMove(MoveEndMatrix, MoveDuration);
                    }
                }
            }
        }

        private void fnAddCurrentCursors()
        {
            this._CurrentCursors.Clear();
            for (int index = 0; index < ((IPluginIn)this.PinInFingerID).SliceCount; ++index)
            {
                double num;
                this.PinInFingerID.GetValue(index, out num);
                double x;
                double y;
                this.PinInFingerXY.GetValue2D(index, out x, out y);
                this._CurrentCursors.Add(new SMTCurrentCursor(Convert.ToInt32(num), x, y));
            }
        }

        private void fnFindCursors()
        {
            for (int iCursor = 0; iCursor < this._CursorList.Count; ++iCursor)
            {
                SMTCurrentCursor smtCurrentCursor = this._CurrentCursors.Find((Predicate<SMTCurrentCursor>)(thiscursor => this._CursorList[iCursor].isID(thiscursor.ID)));
                if (smtCurrentCursor != null)
                {
                    this._CursorList[iCursor].pleasedeleteme = false;
                    this._CursorList[iCursor].xy = smtCurrentCursor.xy;
                }
                else
                    this._CursorList[iCursor].pleasedeleteme = true;
            }
            this._SlideList.Sort();
            this._CurrentTargets.Clear();
            for (int index = 0; index < this._SlideList.Count; ++index)
            {
                if (!this._SlideList[index].isMoving && !this._SlideList[index].isHidden)
                {
                    int backindex = this._SlideList.Count - index;
                    this._CurrentTargets.Add(new SMTHitTarget(this._SlideList[index].getMatrix(backindex), this._SlideList[index].UniqueID, 0U));
                    this._CurrentTargets.Add(new SMTHitTarget(this._SlideList[index].getCloseButtonMatrix(backindex), this._SlideList[index].UniqueID, 1U));
                }
            }
            this._CurrentTargets.Sort();
            for (int iCursor = 0; iCursor < this._CurrentCursors.Count; ++iCursor)
            {
                if (!this._CursorList.Exists((Predicate<SMTCursor>)(thiscursor => thiscursor.isID(this._CurrentCursors[iCursor].ID))))
                {
                    SMTHitTarget smtHitTarget = this._CurrentTargets.Find((Predicate<SMTHitTarget>)(thistarget => thistarget.checkHit(this._CurrentCursors[iCursor].xy)));
                    if (smtHitTarget != null)
                    {
                        if ((int)smtHitTarget.type == 0)
                            this._CursorList.Add(new SMTCursor(this._CurrentCursors[iCursor].xy, this._CurrentCursors[iCursor].ID, smtHitTarget.UniqueID, 0));
                        else
                            this._CursorList.Add(new SMTCursor(this._CurrentCursors[iCursor].xy, this._CurrentCursors[iCursor].ID, smtHitTarget.UniqueID, 1));
                    }
                    else
                        this._CursorList.Add(new SMTCursor(this._CurrentCursors[iCursor].xy, this._CurrentCursors[iCursor].ID, -1L, -1));
                }
            }
            this._CursorList.RemoveAll((Predicate<SMTCursor>)(thiscursor => thiscursor.pleasedeleteme));
        }

        private void fnPressAndRelease()
        {
            this._CurrentIntersects.Clear();
            for (int iSlide = 0; iSlide < this._SlideList.Count; ++iSlide)
            {
                List<SMTCursor> all = this._CursorList.FindAll((Predicate<SMTCursor>)(thiscursor =>
                {
                    if (thiscursor.SlideID == this._SlideList[iSlide].UniqueID)
                        return thiscursor.type == 0;
                    return false;
                }));
                if (this._SlideList[iSlide].nCursors != all.Count)
                {
                    this._SlideList[iSlide].detachCursors();
                    foreach (SMTCursor smtCursor in all)
                    {
                        this._SlideList[iSlide].attachCursor(new SMTAttachedCursor(smtCursor.xy, smtCursor.ID));
                        List<SMTIntersect> list = this._CurrentIntersects;
                        long uniqueId = this._SlideList[iSlide].UniqueID;
                        Vector2D xy1 = smtCursor.xy;
                        Vector3D vector3D = (!(this._SlideList[iSlide].getMatrix(0))) * smtCursor.xyz;
                        // ISSUE: explicit reference operation
                        Vector2D xy2 = ((Vector3D)@vector3D).xy;
                        SMTIntersect smtIntersect = new SMTIntersect(uniqueId, xy1, xy2);
                        list.Add(smtIntersect);
                    }
                }
                else
                    this._CurrentIntersects.AddRange((IEnumerable<SMTIntersect>)this._SlideList[iSlide].updateCursors(all));
                if (this._CursorList.FindAll((Predicate<SMTCursor>)(thiscursor =>
                {
                    if (thiscursor.SlideID == this._SlideList[iSlide].UniqueID)
                        return thiscursor.type == 1;
                    return false;
                })).Count > 0)
                {
                    this._SlideList[iSlide].isBeingPressed = true;
                    if (this._SlideList[iSlide].isClosed)
                        this._CurrentTrashSlides.Add(iSlide);
                }
                else
                    this._SlideList[iSlide].isBeingPressed = false;
            }
        }

        private void fnFindPassThrus()
        {
            this._CurrentPassthroughs = this._CursorList.FindAll((Predicate<SMTCursor>)(thiscursor => thiscursor.type == -1));
        }

        private void fnOutput()
        {
            this._SlideList.Sort();
            int count1 = this._CurrentTrashSlides.Count;
            ((IPluginOut)this.PinOutTrashTransform).SliceCount = (count1);
            ((IPluginOut)this.PinOutTrashOwner).SliceCount = (count1);
            ((IPluginOut)this.PinOutTrashID).SliceCount = (count1);
            for (int index = 0; index < count1; ++index)
            {
                this.PinOutTrashTransform.SetMatrix(index, this._SlideList[this._CurrentTrashSlides[index]].getMatrix(0));
                this.PinOutTrashOwner.SetValue(index, (double)this._SlideList[this._CurrentTrashSlides[index]].owner);
                this.PinOutTrashID.SetValue(index, (double)this._SlideList[this._CurrentTrashSlides[index]].ImageID);
            }
            this._SlideList.RemoveAll((Predicate<SMTSlide>)(thisslide => thisslide.isClosed));
            int count2 = this._SlideList.FindAll((Predicate<SMTSlide>)(thisslide => thisslide.isHidden)).Count;
            int num = this._SlideList.Count - count2;
            ((IPluginOut)this.PinOutTransform).SliceCount = (num);
            ((IPluginOut)this.PinOutButtonTransform).SliceCount = (num);
            ((IPluginOut)this.PinOutShadowTransform).SliceCount = (num);
            ((IPluginOut)this.PinOutIndex).SliceCount = (num);
            ((IPluginOut)this.PinOutOver).SliceCount = (num);
            ((IPluginOut)this.PinOutCloseProgress).SliceCount = (num);
            ((IPluginOut)this.PinOutAspectRatio).SliceCount = (num);
            ((IPluginOut)this.PinOutHit).SliceCount = (num);
            this.PinOutCount.SetValue(0, Convert.ToDouble(num));
            for (int index = 0; index < num + count2; ++index)
            {
                if (!this._SlideList[index].isHidden)
                {
                    int backindex = num - index - 1;
                    this.PinOutTransform.SetMatrix(backindex, this._SlideList[index].getMatrix(backindex));
                    this.PinOutButtonTransform.SetMatrix(backindex, this._SlideList[index].getCloseButtonMatrix(backindex));
                    this.PinOutShadowTransform.SetMatrix(backindex, this._SlideList[index].getShadowMatrix(backindex));
                    this.PinOutIndex.SetValue(backindex, (double)this._SlideList[index].ImageID);
                    this.PinOutOver.SetValue(backindex, this._SlideList[index].hasCursors() ? 1.0 : 0.0);
                    this.PinOutCloseProgress.SetValue(backindex, this._SlideList[index].closeProgress);
                    this.PinOutAspectRatio.SetValue(backindex, this._SlideList[index].AspectRatio);
                    this.PinOutHit.SetValue(backindex, this._SlideList[index].isHit ? 1.0 : 0.0);
                }
            }
            int count3 = this._CurrentIntersects.Count;
            ((IPluginOut)this.PinOutSlideCursor).SliceCount = (count3);
            ((IPluginOut)this.PinOutCursorSlideID).SliceCount = (count3);
            for (int index = 0; index < count3; ++index)
            {
                Vector2D vector2D = 2.0 * this._CurrentIntersects[index].SlideXY;
                this.PinOutCursorSlideID.SetValue(index, (double)this._CurrentIntersects[index].SlideID);
                this.PinOutSlideCursor.SetValue2D(index, (double)vector2D.x, (double)vector2D.y);
            }
            int count4 = this._CurrentPassthroughs.Count;
            ((IPluginOut)this.PinOutPassThru).SliceCount = (count4);
            for (int index = 0; index < count4; ++index)
            {
                Vector2D xy = this._CurrentPassthroughs[index].xy;
                this.PinOutPassThru.SetValue3D(index, (double)xy.x, (double)xy.y, (double)this._CurrentPassthroughs[index].ID);
            }
        }

        private void fnOutputDebug()
        {
            this._SlideList.Sort();
            ((IPluginOut)this.PinOutSlideCursor).SliceCount = (this._CursorList.Count);
            for (int index = 0; index < this._CursorList.Count; ++index)
            {
                Vector2D xy = this._CursorList[index].xy;
                this.PinOutIndex.SetValue(index, (double)this._CursorList[index].ID);
                this.PinOutSlideCursor.SetValue2D(index, (double)xy.x, (double)xy.y);
            }
        }
    }
}
