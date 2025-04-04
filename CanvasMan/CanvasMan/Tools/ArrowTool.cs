﻿using CanvasMan.Managers;
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
		private bool isHoveringOverArrow; // Tracks if we're hovering over the arrow

		private void UpdateCursor(Point mousePosition) {
			if (IsNearPoint(mousePosition, startPoint) || IsNearPoint(mousePosition, endPoint)) {
				// Hovering over an endpoint
				Cursor.Current = Cursors.Cross; // Or any cursor to indicate endpoint dragging
				isHoveringOverArrow = false;
			} else if (IsNearLine(mousePosition, startPoint, endPoint)) {
				// Hovering over the arrow line
				Cursor.Current = Cursors.SizeAll; // Standard "move" cursor
				isHoveringOverArrow = true;
			} else {
				// Not hovering over the arrow
				Cursor.Current = Cursors.Default;
				isHoveringOverArrow = false;
			}
		}
		public override void OnMouseMove(MouseEventArgs e, Graphics graphics) {
			UpdateCursor(e.Location);

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

		public override void ClearToolState() {
			isDefiningTool = false;
			isDraggingTool = false;
			isToolDefined = false;
			isDraggingStart = false;
			isDraggingEnd = false;
			isToolChanged = false;
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
			graphics.DrawImage(originalCanvasBitmap, 0, 0);
			DrawArrow(graphics);
		}
	}
}