using VVVV.Utils.VMath;

//Note that all HitTestFnunction's should be fully contained
//	as they may be called at any time (e.g. after the node
//	which created them has been disposed).

namespace VVVV.Nodes.MultiTouchStack {
	public interface IHitTestFunction {
		bool TestHit(Vector2D localCoordinates);
	}
}