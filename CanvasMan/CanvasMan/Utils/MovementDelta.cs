
namespace CanvasMan.Utils {
	public class MovementDelta {
		public double DeltaX = 0;
		public double DeltaY = 0;
		public bool IsNonZero {
			get {
				return DeltaX != 0 || DeltaY != 0;
			}
		}
		public MovementDelta(double deltaX, double deltaY) {
			this.DeltaX = deltaX;
			this.DeltaY = deltaY;
		}
	}
}
