using CanvasMan.Interfaces;
using CanvasMan.Utils;

namespace CanvasMan.Controls {
	public class ControlPoint : ControlBase {
		public Point Location = Point.Empty;
		private Point initialLocation = Point.Empty;
		private Point initialDragPoint = Point.Empty;
		public bool IsActive { get; set; } = false;
		Action<MovementDelta>? OnMovedCallback { get; set; }
		public override void Draw(Graphics graphics) {
			throw new NotImplementedException();
		}

		public override bool IsHovered(Point mouseLocation, double hoverRadius) {
			return ControlUtils.IsNearPoint(mouseLocation, Location, hoverRadius);
		}

		public override bool OnMouseDown(Point mouseLocation, double grabRadius) {
			if (ControlUtils.IsNearPoint(mouseLocation, Location, grabRadius)) {
				IsActive = true;
				initialLocation = Location;
				initialDragPoint = mouseLocation;
			}
			return IsActive;
		}

		public override bool OnMouseMove(Point mouseLocation) {
			if (IsActive) {
				int dx = mouseLocation.X - initialDragPoint.X;
				int dy = mouseLocation.Y - initialDragPoint.Y;
				Location = new Point(initialLocation.X + dx, initialLocation.Y + dy);
			}
			return IsActive;
		}

		public override bool OnMouseUp(Point mouseLocation) {
			if (IsActive) {
				IsActive = false;
			}
			return IsActive;
		}
	}
}
