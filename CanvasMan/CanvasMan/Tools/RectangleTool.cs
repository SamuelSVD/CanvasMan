using CanvasMan.Managers;
using CanvasMan.Tools.Abstract;
using CanvasMan.Utils;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace CanvasMan {
	public class RectangleTool : CanvasBitmapConsumerTool {
		// Fields to track the start and current points during drawing
		private Point currentPoint = Point.Empty;
		private Point startPoint = Point.Empty;
		private Point endPoint = Point.Empty;
		private Point initialStartPoint = Point.Empty;
		private Point initialEndPoint = Point.Empty;
		private bool isDraggingStart = false;
		private bool isDraggingEnd = false;
		private bool isDraggingTool = false;
		public int BorderWidth { get; set; }       // Width of the rectangle border
		public RectangleTool(ColourManager colourManager, string name, int borderWidth = 1) : base(colourManager, name) {
			BorderWidth = borderWidth;
		}

		public override void OnMouseDown(MouseEventArgs e, Graphics graphics) {
			if (isDefiningTool || isDraggingTool) {
				// If we were dragging the selection, this means someone released
				// the mouse press outside of the app
				OnMouseUp(e, graphics);
			} else if (isToolDefined) {
				// Check if the user clicked near an endpoint or on the arrow line
				if (ControlUtils.IsNearPoint(e.Location, startPoint, 5)) {
					isDraggingStart = true;
				} else if (ControlUtils.IsNearPoint(e.Location, endPoint, 5)) {
					isDraggingEnd = true;
				} else if (
					ControlUtils.IsNearLine(e.Location, startPoint, new Point(startPoint.X, endPoint.Y), 5) ||
					ControlUtils.IsNearLine(e.Location, startPoint, new Point(endPoint.X, startPoint.Y), 5) ||
					ControlUtils.IsNearLine(e.Location, endPoint, new Point(startPoint.X, endPoint.Y), 5) ||
					ControlUtils.IsNearLine(e.Location, endPoint, new Point(endPoint.X, startPoint.Y), 5)
				) {
					isDraggingTool = true;
					initialDragPoint = e.Location;
					initialStartPoint = startPoint;
					initialEndPoint = endPoint;
				} else {
					// Commit the dragged location and clear the selection
					CommitTool(graphics);
					ClearToolState();

					// Initial press, start defining the 
					StartToolDefinition(e);
				}
			} else if (!isDefiningTool && !isDraggingTool) {
				// Initial press, start defining the 
				StartToolDefinition(e);
				SaveCanvasBitmapState();
			}
		}

		public override void OnMouseMove(MouseEventArgs e, Graphics graphics) {
			if (isDraggingTool) {
				// Move the entire arrow by offsetting both points
				int dx = e.Location.X - initialDragPoint.X;
				int dy = e.Location.Y - initialDragPoint.Y;
				startPoint = new Point(initialStartPoint.X + dx, initialStartPoint.Y + dy);
				endPoint = new Point(initialEndPoint.X + dx, initialEndPoint.Y + dy);
				isToolChanged = true;
			} else if (isDraggingStart) {
				// Move only the start point
				startPoint = e.Location;
				isToolChanged = true;
			} else if (isDraggingEnd) {
				// Move only the end point
				endPoint = e.Location;
				isToolChanged = true;
			} else if (isDefiningTool) {
				endPoint = e.Location;
				isToolChanged = true;
			}

			if (isDefiningTool || isDraggingTool || isDraggingStart || isDraggingEnd) {
				DrawCurrentState(graphics);
			}
		}

		public override void OnMouseUp(MouseEventArgs e, Graphics graphics) {
			// Stop dragging operations
			if (isDefiningTool) {
				EndToolDefinition(e, graphics);
			} else if (isDraggingTool) {
				int dx = e.Location.X - initialDragPoint.X;
				int dy = e.Location.Y - initialDragPoint.Y;
				startPoint = new Point(initialStartPoint.X + dx, initialStartPoint.Y + dy);
				endPoint = new Point(initialEndPoint.X + dx, initialEndPoint.Y + dy);
				isDraggingTool = false;
				isToolChanged = true;
			} else if (isDraggingStart) {
				startPoint = e.Location;
			} else if (isDraggingEnd) {
				endPoint = e.Location;
			}
			isDraggingTool = false;
			isDraggingStart = false;
			isDraggingEnd = false;
		}

		private void DrawRectangle(Graphics graphics, Point start, Point end) {
			int x = Math.Min(start.X, end.X);
			int y = Math.Min(start.Y, end.Y);
			int width = Math.Abs(end.X - start.X);
			int height = Math.Abs(end.Y - start.Y);

			using (var pen = new Pen(ColourManager.CurrentColor, BorderWidth)) {
				graphics.DrawRectangle(pen, x, y, width, height);
			}
		}

		public override void ClearToolState() {
			isDefiningTool = false;
			isDraggingTool = false;
			isToolDefined = false;
			isToolChanged = false;
			isDraggingStart = false;
			isDraggingEnd = false;
			startPoint = Point.Empty;
			endPoint = Point.Empty;
		}

		public override void StartToolDefinition(MouseEventArgs e) {
			isDefiningTool = true;
			startPoint = e.Location;
		}

		public override void EndToolDefinition(MouseEventArgs e, Graphics graphics) {
			endPoint = e.Location;
			isDefiningTool = false;
			isDraggingTool = false;
			isDraggingStart = false;
			isDraggingEnd = false;
			isToolDefined = true;
			isToolChanged = true;
		}

		public override void DrawCurrentState(Graphics graphics) {
			// Reset the canvas to the original state to avoid overlapping artifacts
			graphics.DrawImage(originalCanvasBitmap, 0, 0);

			// Perform the rectangle drawing
			DrawRectangle(graphics, startPoint, endPoint);
		}
	}
}