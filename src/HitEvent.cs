using System.Collections.Generic;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.MultiTouchStack
{
	//when a HitEvent is removed, we wait for the Cursor to die before we do
	public class HitEvent
	{
		public enum CursorActionType
		{
			Down,
			Release,
			ReleaseInside,
			ReleaseOutside
		}

		public IHitTestFunction HitTestFunction;
		public string EventName;
		public CursorActionType CursorAction;
		public List<Cursor> AttachedCursors;

		public HitEvent()
		{
			this.AttachedCursors = new List<Cursor>();
		}
	}
}