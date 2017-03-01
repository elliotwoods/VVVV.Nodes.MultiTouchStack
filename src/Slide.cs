using VVVV.Utils.VMath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VVVV.Nodes.MultiTouchStack {
	public class Slide : IDisposable
	{
		public WeakReference<World> World;
		public int Index;
		public Matrix4x4 Transform;
		public List<String> Tags;
		public Double MinimumScale;
		public Double MaximumScale;
		public IHitTestFunction DragHitTestFunction;
		public List<HitEvent> HitEvents;

		public List<Cursor> AttachedCursors;
		public List<HitCallback> HitCallbacksThisFrame;

		public Slide(World world)
		{
			this.World = new WeakReference<World>(world);
			this.AttachedCursors = new List<Cursor>();
			this.HitCallbacksThisFrame = new List<HitCallback>();
		}

		public Matrix4x4 TransformWithZOrder
		{
			get
			{
				World world;
				if (this.World.TryGetTarget(out world))
				{
					var indexInWorld = world.Slides.IndexOf(this);
					var zTransform = world.GetZOrderTransform(indexInWorld);
					return this.Transform * zTransform;
				} else
				{
					return this.Transform;
				}
				
			}
		}

		public bool DragHitTest(Vector2D localCoordinates)
		{
			if (this.DragHitTestFunction == null)
			{
				//use default drag test function
				return localCoordinates.x >= -0.5
					&& localCoordinates.x <= +0.5
					&& localCoordinates.y >= -0.5
					&& localCoordinates.y <= +0.5;
			}
			else
			{
				//use special drag test function where available
				return this.DragHitTestFunction.TestHit(localCoordinates);
			}
		}

		public void Update()
		{
			var movementOccured = Utils.MultitouchTransform(ref this.Transform, this.AttachedCursors, this.MinimumScale, this.MaximumScale);

			//if movement occurred, ensure we are within canvas limits
			if(movementOccured)
			{
				World world;
				if(this.World.TryGetTarget(out world))
				{
					var width = world.Settings.CanvasSize.x;
					var height = world.Settings.CanvasSize.y;

					this.Transform[3, 0] = VMath.Clamp(this.Transform[3, 0], -width, width);
					this.Transform[3, 1] = VMath.Clamp(this.Transform[3, 1], -height, height);
				}
	
			}
		}

		public Vector2D CanvasToSlide(Vector2D cursorInCanvas)
		{
			var cursorInSlide3 = VMath.Inverse(this.Transform) * cursorInCanvas;
			return new Vector2D(cursorInSlide3.x, cursorInSlide3.y);
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		~Slide()
		{
			this.Dispose();
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			if (!disposedValue)
			{
				var dragHitTestFunction = this.DragHitTestFunction as IDisposable;
				if (dragHitTestFunction != null)
				{
					dragHitTestFunction.Dispose();
				}
				foreach (var hitEvent in this.HitEvents)
				{
					hitEvent.Dispose();
				}

				this.AttachedCursors.Clear();
				this.HitEvents.Clear();

				disposedValue = true;
			}
		}
		#endregion
	}
}