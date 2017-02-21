using VVVV.Utils.VMath;
using System;

namespace VVVV.Nodes.MultiTouchStack {
	public class Cursor : INotifyRelativesOfDeath
	{
		public int Index;
		public Vector2D Position;
		public Vector2D Movement;

		public Double LifeTime {
			get
			{
				var lifespan = DateTime.Now - this.FBirthTime;
				return lifespan.TotalSeconds;
			}
		}

		public Slide AssignedSlide;
		public HitEvent AssignedHitEvent;

		DateTime FBirthTime = DateTime.Now;

		public void NotifyRelativesOfDeath()
		{
			if (this.AssignedSlide != null)
			{
				this.AssignedSlide.AttachedCursors.RemoveAll(cursor => cursor == this);
			}
			if (this.AssignedHitEvent != null)
			{
				this.AssignedHitEvent.AttachedCursors.RemoveAll(cursor => cursor == this);
			}
		}
	}
}