using System;
using System.Collections.Generic;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.MultiTouchStack
{
	public class World : IDisposable
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

		public Matrix4x4 GetZOrderTransform(int indexInList)
		{
			return VMath.Translate(0.0, 0.0, 1.0 - (double)indexInList / (double)this.Slides.Count);
		}

		public Matrix4x4 GetSlideTransformWithZOrder(int indexInList)
		{
			return this.Slides[indexInList].Transform * this.GetZOrderTransform(indexInList);
		}

		public Matrix4x4 GetSlideTransformWithZOrder(Slide slide)
		{
			var indexInList = this.Slides.FindIndex(slideInList => ReferenceEquals(slide, slideInList));
			return slide.Transform * GetZOrderTransform(indexInList);
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					foreach(var slide in this.Slides)
					{
						slide.Dispose();
					}

					foreach(var cursor in this.Cursors)
					{
						cursor.Value.AssignedHitEvent = null;
						cursor.Value.AssignedSlide = null;
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