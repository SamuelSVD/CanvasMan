using CanvasMan.Interfaces;
using CanvasMan.Utils;

namespace CanvasMan.Controls {
	public class ControlLine : ControlBase {
		public ControlPoint EndpointA;
		public ControlPoint EndpointB;
		private Point initialEndpointA = Point.Empty;
		private Point initialEndpointB = Point.Empty;

		public ControlLine(ControlPoint endpointA, ControlPoint endpointB) {
			this.EndpointA = endpointA;
			this.EndpointB = endpointB;
		}
		public override void Draw(Graphics graphics) {
			throw new NotImplementedException();
		}

		public override bool IsHovered(Point mouseLocation, double hoverRadius) {
			return ControlUtils.IsNearLine(mouseLocation, EndpointA.Location, EndpointB.Location, hoverRadius);
		}

		public override bool OnMouseDown(Point mouseLocation, double grabRadius) {
			if (ControlUtils.IsNearLine(mouseLocation, EndpointA.Location, EndpointB.Location, grabRadius)) {
				IsActive = true;
				initialEndpointA = EndpointA.Location;
				initialEndpointB = EndpointB.Location;
				initialDragPoint = mouseLocation;
			}
			return IsActive;
		}

		public override bool OnMouseMove(Point mouseLocation) {
			if (IsActive) {
				int dx = mouseLocation.X - initialDragPoint.X;
				int dy = mouseLocation.Y - initialDragPoint.Y;
				EndpointA.SetLocation(new Point(initialEndpointA.X + dx, initialEndpointA.Y + dy));
				EndpointB.SetLocation(new Point(initialEndpointB.X + dx, initialEndpointB.Y + dy));
			}
			return IsActive;
		}

		public override bool OnMouseUp(Point mouseLocation) {
			if (IsActive) {
				IsActive = false;
			}
			return IsActive;
		}

		protected override void OnOffset(float x, float y) {
			throw new NotImplementedException();
		}
	}
}
