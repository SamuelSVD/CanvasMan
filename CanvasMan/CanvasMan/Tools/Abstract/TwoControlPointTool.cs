using CanvasMan.Controls;
using CanvasMan.Managers;

namespace CanvasMan.Tools.Abstract {
	public abstract class TwoControlPointTool : CanvasBitmapConsumerTool {
		protected ControlPoint startPoint = new ControlPoint();
		protected ControlPoint endPoint = new ControlPoint();
		protected List<ControlPoint> controlPoints = new List<ControlPoint>();
		protected ControlLine controlLine;
		protected ControlManager controlManager = new ControlManager();
		public TwoControlPointTool(ColourManager colourManager, CanvasManager canvasManager, string name) : base(colourManager, canvasManager, name) {
			controlLine = new ControlLine(startPoint, endPoint);
			controlPoints.Add(startPoint);
			controlPoints.Add(endPoint);
			controlManager.Controls.Add(startPoint);
			controlManager.Controls.Add(endPoint);
			controlManager.Controls.Add(controlLine);
		}
		public override void OnMouseDown(MouseEventArgs e) {
			if (isDefiningTool || controlManager.IsActive()) {
				// If we were dragging the selection, this means someone released
				// the mouse press outside of the app
				OnMouseUp(e);
			} else if (isToolDefined) {
				// Check if the user clicked near an endpoint or on the arrow line
				if (controlManager.OnMouseDown(e.Location, 5)) {
					return;
				}
				CommitTool();
				ClearToolState();

				// Initial press, start defining the 
				StartToolDefinition(e);
				//				}
			} else if (!isDefiningTool && !controlLine.IsActive) {
				// Initial press, start defining the 
				StartToolDefinition(e);
				SaveCanvasBitmapState();
			}
		}
		private void UpdateCursor(Point mousePosition) {
			if (controlManager.IsHovered(mousePosition, 5)) {
				isToolChanged = true;
				Cursor.Current = Cursors.SizeAll;
				return;
			}
			Cursor.Current = Cursors.Default;
		}
		public override void OnMouseMove(MouseEventArgs e) {
			UpdateCursor(e.Location);

			if (controlLine.OnMouseMove(e.Location)) {
				isToolChanged = true;
			}

			if (controlManager.OnMouseMove(e.Location)) {
				isToolChanged = true;
			}
			if (isDefiningTool) {
				endPoint.Location = e.Location;
				isToolChanged = true;
			}

			if (isDefiningTool || controlLine.IsActive || controlPoints.Exists(t => t.IsActive)) {
				DrawCurrentState();
			}
		}

		public override void OnMouseUp(MouseEventArgs e) {
			// Stop dragging operations
			if (isDefiningTool) {
				EndToolDefinition(e);
			}
			if (controlLine.OnMouseUp(e.Location)) {
				isToolChanged = true;
			}
			foreach (ControlPoint point in controlPoints) {
				if (point.OnMouseUp(e.Location)) {
					isToolChanged = true;
					break;
				}
			}
		}
		public override void StartToolDefinition(MouseEventArgs e) {
			isDefiningTool = true;
			startPoint.Location = e.Location;
		}
		public override void EndToolDefinition(MouseEventArgs e) {
			endPoint.Location = new Point(e.Location.X, e.Location.Y);
			isDefiningTool = false;
			controlLine.IsActive = false;
			isToolDefined = true;
			isToolChanged = true;
		}
		public override void ClearToolState() {
			isDefiningTool = false;
			controlLine.IsActive = false;
			isToolDefined = false;
			isToolChanged = false;
		}
	}
}
