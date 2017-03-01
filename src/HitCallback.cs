using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVVV.Nodes.MultiTouchStack
{
	public class HitCallback
	{
		[Flags]
		public enum CursorActionType
		{
			Down = 1 << 1,
			ReleaseInside = 1 << 2,
			ReleaseOutside = 1 << 3,

			Release = ReleaseInside | ReleaseOutside,
			Any = Down | Release
		}

		public HitEvent Event;
		public Cursor Cursor;
		public CursorActionType ActionType;
	}
}
