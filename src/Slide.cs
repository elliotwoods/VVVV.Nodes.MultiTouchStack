using VVVV.Utils.VMath;
using System;
using System.Collections.Generic;

namespace VVVV.Nodes.MultiTouchStack {
	public class Slide : INotifyRelativesOfDeath
	{
		public int Index;
		public Matrix4x4 Transform;
		public List<String> Tags;
		public Double MinimumScale;
		public Double MaximumScale;
		public IHitTestFunction DragHitTestFunction;
		public List<HitEvent> HitEvents;
		public List<Cursor> AttachedCursors;

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
			switch(this.AttachedCursors.Count)
			{
				case 1:
					{
						this.Transform = this.Transform * VMath.Translate(new Vector3D(this.AttachedCursors[0].Movement));
						break;
					}
				case 2:
					{
						var cursor0Now = this.AttachedCursors[0].Position;
						var cursor1Now = this.AttachedCursors[1].Position;
						var cursor0Previous = this.AttachedCursors[0].Position - this.AttachedCursors[0].Movement;
						var cursor1Previous = this.AttachedCursors[1].Position - this.AttachedCursors[1].Movement;

						double num1 = (double)cursor0Previous.x;
						double num2 = (double)cursor0Previous.y;
						double num3 = (double)cursor1Previous.x;
						double num4 = (double)cursor1Previous.y;
						double num5 = (double)cursor0Now.x;
						double num6 = (double)cursor0Now.y;
						double num7 = (double)cursor1Now.x;
						double num8 = (double)cursor1Now.y;
						double num9 = num3 - num1;
						double num10 = num4 - num2;
						double num11 = num7 - num5;
						double num12 = num8 - num6;
						Math.Sqrt((num11 * num11 + num12 * num12) / (num9 * num9 + num10 * num10));
						double num13 = (num9 * num11 + num10 * num12) / (num9 * num9 + num10 * num10);
						double num14 = (num10 * num11 - num9 * num12) / (num9 * num9 + num10 * num10);
						double num15 = num5 - (num13 * num1 + num14 * num2);
						double num16 = num6 - (-num14 * num1 + num13 * num2);

						var newTransform = this.Transform * new Matrix4x4(num13, -num14, 0.0, 0.0, num14, num13, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, num15, num16, 0.0, 1.0);

						//check if newTransform doesn't break the rules before applying it
						{
							var scale = Math.Sqrt(newTransform[0, 0] * newTransform[0, 0] + newTransform[0, 1] * newTransform[0, 1])
								+ Math.Sqrt(newTransform[1, 0] * newTransform[1, 0] + newTransform[1, 1] * newTransform[1, 1]);
							scale /= 2.0f;

							if(scale >= this.MinimumScale && scale <= this.MaximumScale)
							{
								this.Transform = newTransform;
							} else
							{
								
							}
						}
						break;
					}
				default:
					break;
			}
		}

		public void NotifyRelativesOfDeath()
		{
			//when we are disconnected from the World
			// we wait for our attached cursors to die
			// before we do.
		}
	}
}