using System.Collections.Generic;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.MultiTouchStack
{
	public class World
	{
		public List<Slide> Slides; // ordered by z-order
		public Dictionary<int, Cursor> Cursors;
		public Matrix4x4 CanvasTransform;

		public List<Slide> SlidesToRemoveThisFrame;

		public List<Cursor> CursorsAddedThisFrame;
		public List<Cursor> CursorsLostThisFrame;
		public List<HitEvent> HitEventsFiredThisFrame;

		public World()
		{
			this.Slides = new List<Slide>();
			this.Cursors = new Dictionary<int, Cursor>();
			this.CanvasTransform = new Matrix4x4();

			this.SlidesToRemoveThisFrame = new List<Slide>();

			this.CursorsAddedThisFrame = new List<Cursor>();
			this.CursorsLostThisFrame = new List<Cursor>();
			this.HitEventsFiredThisFrame = new List<HitEvent>();
		}

		public int NextAvailableIndex
		{
			get
			{
				int nextIndex = 0;
				foreach(var slide in this.Slides)
				{
					if(slide.Index >= nextIndex)
					{
						nextIndex = slide.Index + 1;
					}
				}
				return nextIndex;
			}
		}

		public List<Cursor> CursorsOnCanvas
		{
			get
			{
				var cursorsOnCanvas = new List<Cursor>();
				foreach(var cursor in this.Cursors.Values)
				{
					if(cursor.AssignedSlide == null)
					{
						cursorsOnCanvas.Add(cursor);
					}
				}
				return cursorsOnCanvas;
			}
		}

	}
}