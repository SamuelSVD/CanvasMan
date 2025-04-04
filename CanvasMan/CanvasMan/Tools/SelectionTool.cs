using CanvasMan.Interfaces;
using CanvasMan.Managers;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasMan.Tools {
	public class SelectionTool : CanvasBitmapConsumerTool, IKeyInteractiveTool {
		private Bitmap ? selectedRegion;        // Stores the image data of the selected region
		private Rectangle selectionRectangle; // Defines the selected area
		private Point initialSelectionRectanglePosition;

		// Constructor to initialize the Select Tool
		public SelectionTool(ColourManager colourManager, string name = "Select") : base(colourManager, name) {
			selectionRectangle = Rectangle.Empty;
		}

		// Handle mouse down for starting a new selection or dragging
		public override void OnMouseDown(MouseEventArgs e, Graphics graphics) {
			Logger.Log("SelectionTool - OnMouseDown");
			if (isDefiningTool || isDraggingTool) {
				// If we were dragging the selection, this means someone released
				// the mouse press outside of the app
				OnMouseUp(e, graphics);
			} else if (isToolDefined) {
				if (selectionRectangle.Contains(e.Location)) {
					// Start dragging the selected area
					isDraggingTool = true;
					initialDragPoint = e.Location;
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

		// Handle mouse movement for resizing the selection or dragging
		public override void OnMouseMove(MouseEventArgs e, Graphics graphics) {
			if (isDraggingTool && isToolDefined) {
				// Calculate the offset for dragging
				int dx = e.Location.X - initialDragPoint.X;
				int dy = e.Location.Y - initialDragPoint.Y;
				selectionRectangle.X = initialSelectionRectanglePosition.X + dx;
				selectionRectangle.Y = initialSelectionRectanglePosition.Y + dy;
				// Redraw the current state
				DrawCurrentState(graphics);
			} else if (isDefiningTool && selectionRectangle != Rectangle.Empty) {
				// Update the size of the defenition rectangle
				int dw = e.X - initialDragPoint.X;
				int dh = e.Y - initialDragPoint.Y;
				int x;
				int y;
				// Resize the rectangle if negative width or height
				if (dw < 0) {
					x = initialDragPoint.X + dw;
					dw = -dw;
				} else {
					x = initialDragPoint.X;
				}
				if (dh < 0) {
					y = initialDragPoint.Y + dh;
					dh = -dh;
				} else {
					y = initialDragPoint.Y;
				}
				selectionRectangle = new Rectangle(x, y, dw, dh);
				// Redraw the current state
				DrawCurrentState(graphics);
			}
		}
		public override void DrawCurrentState(Graphics graphics) {
			graphics.Clear(Color.White);
			graphics.DrawImage(originalCanvasBitmap, 0, 0);
			if (isToolDefined) graphics.DrawImage(selectedRegion, selectionRectangle.X, selectionRectangle.Y);
			if (isDefiningTool) {
				using (var pen = new Pen(Color.Red, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash }) {
					graphics.DrawRectangle(pen, selectionRectangle.X, selectionRectangle.Y, selectionRectangle.Width + 1, selectionRectangle.Height + 1);
				}
			}
			if (isDraggingTool) {
				using (var pen = new Pen(Color.Blue, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash }) {
					graphics.DrawRectangle(pen, selectionRectangle.X, selectionRectangle.Y, selectionRectangle.Width + 1, selectionRectangle.Height + 1);
				}
			}
		}

		// Handle mouse up for completing a selection or ending dragging
		public override void OnMouseUp(MouseEventArgs e, Graphics graphics) {
			Logger.Log("SelectionTool - OnMouseUp");
			if (isDefiningTool && !isDraggingTool) {
				EndToolDefinition(e, graphics);
			} else if (isDraggingTool) {
				initialSelectionRectanglePosition.X = selectionRectangle.X;
				initialSelectionRectanglePosition.Y = selectionRectangle.Y;
				isDraggingTool = false;
			}
		}

		// Copy and move the selected region (e.g., for CTRL + Arrow keys functionality)
		public void CopyAndMoveSelection(Graphics graphics, int offsetX, int offsetY) {
			if (isToolDefined) {
				CommitTool(graphics);
				// Update the position of the selection rectangle
				selectionRectangle.X += offsetX;
				selectionRectangle.Y += offsetY;

				// Draw the copied selection at the new position
				isDraggingTool = true;
				DrawCurrentState(graphics);
				isDraggingTool = false;
			}
		}

		// Copy and move the selected region (e.g., for CTRL + Arrow keys functionality)
		public void MoveSelection(Graphics graphics, int offsetX, int offsetY) {
			if (isToolDefined) {
				// Update the position of the selection rectangle
				selectionRectangle.X += offsetX;
				selectionRectangle.Y += offsetY;

				// Draw the copied selection at the new position
				isDraggingTool = true;
				DrawCurrentState(graphics);
				isDraggingTool = false;
			}
		}

		// Implement OnKeyDown for handling key presses
		public void OnKeyDown(KeyEventArgs e, Graphics graphics) {
			if (e.Control) // Check if CTRL is held
			{
				int offsetX = 0, offsetY = 0;

				// Determine the direction based on the arrow key pressed
				switch (e.KeyCode) {
					case Keys.Up:
						offsetY = -10; // Move up
						break;
					case Keys.Down:
						offsetY = 10; // Move down
						break;
					case Keys.Left:
						offsetX = -10; // Move left
						break;
					case Keys.Right:
						offsetX = 10; // Move right
						break;
				}

				// Perform movement logic
				CopyAndMoveSelection(graphics, offsetX, offsetY);
			}
		}

		// Implement OnKeyUp for handling key releases
		public void OnKeyUp(KeyEventArgs e, Graphics graphics) {
			// Optional: Add any logic needed when a key is released
		}

		public override void ClearToolState() {
			isDefiningTool = false;
			isDraggingTool = false;
			isToolDefined = false;
			selectedRegion = null;
		}

		public override void StartToolDefinition(MouseEventArgs e) {
			isDefiningTool = true;
			initialDragPoint = e.Location;
			selectionRectangle = new Rectangle(e.Location, Size.Empty);
		}

		public override void EndToolDefinition(MouseEventArgs e, Graphics graphics) {
			isDefiningTool = false;
			if (!selectionRectangle.IsEmpty) {
				if (selectionRectangle.X < 0) {
					int x = -selectionRectangle.X;
					int w = selectionRectangle.Width - x;
					if (w < 0) {
						selectionRectangle.Width = 0;
					} else {
						selectionRectangle.X = 0;
						selectionRectangle.Width = w;
					}
				}
				if (selectionRectangle.Y < 0) {
					int y = -selectionRectangle.Y;
					int h = selectionRectangle.Height - y;
					if (h < 0) {
						selectionRectangle.Height = 0;
					} else {
						selectionRectangle.Y = 0;
						selectionRectangle.Height = h;
					}
				}
				// Capture the selected region from the canvas bitmap
				if (selectionRectangle.Width > 0 && selectionRectangle.Height > 0) {
					selectedRegion = originalCanvasBitmap?.Clone(selectionRectangle, canvasBitmap.PixelFormat);
					isToolDefined = true;
					initialSelectionRectanglePosition = new Point(selectionRectangle.X, selectionRectangle.Y);
				} else {
					// Commit the dragged location and clear the selection
					isDraggingTool = false;
					DrawCurrentState(graphics);
					SaveCanvasBitmapState();
				}
			}
		}
	}
}