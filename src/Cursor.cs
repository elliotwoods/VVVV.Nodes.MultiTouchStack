using VVVV.Utils.VMath;
using System;

namespace VVVV.Nodes.MultiTouchStack {
	public class Cursor
	{
		public WeakReference<World> World;
		public int Index;
		public Vector2D Position;
		public Vector2D Movement;

		public Cursor(World world)
		{
			this.World = new WeakReference<World>(world);
		}

		public Double LifeTime {
			get
			{
				var lifespan = DateTime.Now - this.FBirthTime;
				return lifespan.TotalSeconds;
			}
		}

		// keep alive any objects we're attached to even after they're removed from the World until Cursor dies
		public Slide AssignedSlide;
		public HitEvent AssignedHitEvent;

		DateTime FBirthTime = DateTime.Now;
	}
}