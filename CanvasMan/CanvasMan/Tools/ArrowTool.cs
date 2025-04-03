using CanvasMan.Managers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasMan.Tools {
	public class ArrowTool : CanvasBitmapConsumerTool {
		private Point startPoint = Point.Empty;
		private Point endPoint = Point.Empty;
		private Point initialStartPoint = Point.Empty;
		private Point initialEndPoint = Point.Empty;
		private bool isDraggingStart;
		private bool isDraggingEnd;
		private const int grabRadius = 8; // Area around endpoints to detect dragging

		public ArrowTool(ColourManager colourManager, string name = "Arrow") : base(colourManager, name) {
			startPoint = Point.Empty;
			endPoint = Point.Empty;
		}

		public override void OnMouseDown(MouseEventArgs e, Graphics graphics) {
			if (isDefiningTool || isDraggingTool) {
				// If we were dragging the selection, this means someone released
				// the mouse press outside of the app
				OnMouseUp(e, graphics);
			} else if (isToolDefined) {
				// Check if the user clicked near an endpoint or on the arrow line
				if (IsNearPoint(e.Location, startPoint)) {
					isDraggingStart = true;
				} else if (IsNearPoint(e.Location, endPoint)) {
					isDraggingEnd = true;
				} else if (IsNearLine(e.Location, startPoint, endPoint)) {
					isDraggingTool = true;
					initialDragPoint = e.Location;
					initialStartPoint = startPoint;
					initialEndPoint = endPoint;
				} else {
					// Commit the dragged location and clear the selection
					DrawCurrentState(graphics);
					SaveCanvasBitmapState();
					ClearToolState();
					SaveStateCallback?.Invoke();

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
			} else if (isDraggingStart) {
				// Move only the start point
				startPoint = e.Location;
			} else if (isDraggingEnd) {
				// Move only the end point
				endPoint = e.Location;
			} else if (isDefiningTool) {
				endPoint = e.Location;
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
			} else if (isDraggingStart) {
				startPoint = e.Location;
			} else if (isDraggingEnd) {
				endPoint = e.Location;
			}
			isDraggingTool = false;
			isDraggingStart = false;
			isDraggingEnd = false;
		}

		private void DrawArrow(Graphics graphics) {
			using (Pen pen = new Pen(ColourManager.CurrentColor, 3)) {
				graphics.DrawLine(pen, startPoint, endPoint);

				// Calculate the arrowhead
				DrawArrowHead(graphics, pen, startPoint, endPoint);
			}
		}

		private void DrawArrowHead(Graphics graphics, Pen pen, Point start, Point end) {
			const int arrowSize = 10;
			var direction = new PointF(end.X - start.X, end.Y - start.Y);
			var length = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
			if (length != 0) {
				var unitDir = new PointF((float)(direction.X / length), (float)(direction.Y / length));

				var arrowPoint1 = new PointF(
					end.X - unitDir.X * arrowSize + unitDir.Y * arrowSize,
					end.Y - unitDir.Y * arrowSize - unitDir.X * arrowSize);

				var arrowPoint2 = new PointF(
					end.X - unitDir.X * arrowSize - unitDir.Y * arrowSize,
					end.Y - unitDir.Y * arrowSize + unitDir.X * arrowSize);

				graphics.DrawLine(pen, end, Point.Round(arrowPoint1));
				graphics.DrawLine(pen, end, Point.Round(arrowPoint2));
			}
		}

		private bool IsNearPoint(Point click, Point target) {
			return Math.Abs(click.X - target.X) <= grabRadius &&
				   Math.Abs(click.Y - target.Y) <= grabRadius;
		}

		private bool IsNearLine(Point click, Point start, Point end) {
			// Check if the point is close to the line
			float distance = DistanceFromLine(click, start, end);
			return distance < grabRadius;
		}

		private float DistanceFromLine(Point p, Point a, Point b) {
			float num = Math.Abs((b.Y - a.Y) * p.X - (b.X - a.X) * p.Y + b.X * a.Y - b.Y * a.X);
			float denom = (float)Math.Sqrt(Math.Pow(b.Y - a.Y, 2) + Math.Pow(b.X - a.X, 2));
			return num / denom;
		}

		protected override void ClearToolState() {
			isDefiningTool = false;
			isDraggingTool = false;
			isToolDefined = false;
			isDraggingStart = false;
			isDraggingEnd = false;
			startPoint = Point.Empty;
			endPoint = Point.Empty;
		}

		protected override void StartToolDefinition(MouseEventArgs e) {
			isDefiningTool = true;
			startPoint = e.Location;
		}

		protected override void EndToolDefinition(MouseEventArgs e, Graphics graphics) {
			endPoint = e.Location;
			isDefiningTool = false;
			isDraggingTool = false;
			isDraggingStart = false;
			isDraggingEnd = false;
			isToolDefined = true;
		}

		protected override void DrawCurrentState(Graphics graphics) {
			graphics.DrawImage(originalCanvasBitmap, 0, 0);
			DrawArrow(graphics);
		}
	}
}