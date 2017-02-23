using System;
using System.Collections.Generic;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.MultiTouchStack
{
	//when a HitEvent is removed, we wait for the Cursor to die before we do
	public class HitEvent : IDisposable
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

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					var hitTestFunction = this.HitTestFunction as IDisposable;
					if(hitTestFunction != null)
					{
						hitTestFunction.Dispose();
					}
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}
		#endregion
	}
}