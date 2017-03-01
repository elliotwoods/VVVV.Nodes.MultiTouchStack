using VVVV.Utils.VMath;

namespace VVVV.Nodes.MultiTouchStack {
	public class Settings {
		public bool TapBringsToFront = true;
		public Vector2D CanvasSize = new Vector2D(2.0, 2.0);
		public double MinimumCanvasScale = 0.01;
		public double MaximumCanvasScale = 100.0;
	}
}