using CanvasMan.Interfaces;
using CanvasMan.Utils;

namespace CanvasMan.Controls {
	public class ControlPoint : ControlBase {
		public Point Location {
			get {
				return ControlUtils.Point(LocationF);
			}
		}
		public virtual PointF LocationF {
			get;
			protected set;
		} = Point.Empty;
		private PointF lastDragPoint = Point.Empty;
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
				lastDragPoint = mouseLocation;
			}
			return IsActive;
		}

		public override bool OnMouseMove(Point mouseLocation) {
			if (IsActive) {
				int dx = (int) (mouseLocation.X - lastDragPoint.X);
				int dy = (int) (mouseLocation.Y - lastDragPoint.Y);
				Offset(dx, dy);
				lastDragPoint = mouseLocation;
			}
			return IsActive;
		}

		public override bool OnMouseUp(Point mouseLocation) {
			if (IsActive) {
				IsActive = false;
			}
			return IsActive;
		}
		public void SetLocation(Point location) {
			SetLocation(ControlUtils.PointF(location));
		}
		public void SetLocation(PointF location) {
			LocationF = location;
		}
		protected override void OnOffset(float x, float y) {
			LocationF = new PointF(Location.X + x, Location.Y + y);
		}
	}
}
