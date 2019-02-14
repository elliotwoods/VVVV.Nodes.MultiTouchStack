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
		public Behaviors.IBehavior Behaviour;
		public Constraints.IConstraint Constraint;
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

		/// <summary>
		/// Perform any multi-touch actions
		/// </summary>
		public void Update()
		{
			if (this.AttachedCursors.Count > 0)
			{
				// Find the behavior for the multitouch action
				var behaviorChain = this.Behaviour;
				if (behaviorChain == null)
				{
					//default to full multitouch
					behaviorChain = Behaviors.FullMultitouchNode.PrincipalBehavior;
				}

				// Setup the arguments to perform the action
				var performArguments = new Behaviors.PerformArguments
				{
					Cursors = this.AttachedCursors
				};

				// If we have constraints, add the settings to the performArguments
				if (this.Constraint != null)
				{
					var checkConstraintArguments = new Constraints.CheckConstraintArguments
					{
						Slide = this
					};

					performArguments.ValidateFunction = (Behaviors.ValidateFunctionArguments validateFunctionArguments) =>
					{
						return this.Constraint.CheckConstraint(validateFunctionArguments, checkConstraintArguments);
					};
				}

				// Perform the behavior chain (inside the chain it checks constraints if any)
				{
					Matrix4x4 newTransform;
					if (behaviorChain.PerformAndValidate(performArguments, this.Transform, out newTransform))
					{
						this.Transform = newTransform;
					}
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