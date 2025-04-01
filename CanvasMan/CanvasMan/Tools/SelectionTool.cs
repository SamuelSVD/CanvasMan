using CanvasMan.Interfaces;
using CanvasMan.Managers;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasMan.Tools {
	public class SelectionTool : CanvasBitmapConsumerTool, IKeyInteractiveTool {
		private bool isDefiningSelection;     // Tracks if the selection is being defined
		private bool isDraggingSelection;              // Tracks if the selection is being dragged
		private Bitmap ? selectedRegion;        // Stores the image data of the selected region
		private Rectangle selectionRectangle; // Defines the selected area
		private Point initialDragPoint;         // Stores the starting point of a drag operation
		private Point latestDragPoint;
		private Point initialSelectionRectanglePosition;

		// Constructor to initialize the Select Tool
		public SelectionTool(ColourManager colourManager, string name = "Select") : base(colourManager, name) {
			selectionRectangle = Rectangle.Empty;
			isDefiningSelection = false;
			isDraggingSelection = false;
		}

		// Handle mouse down for starting a new selection or dragging
		public override void OnMouseDown(MouseEventArgs e, Graphics graphics) {
			Logger.Log("SelectionTool - OnMouseDown");
			if (isDefiningSelection || isDraggingSelection) {
				// If we were dragging the selection, this means someone released
				// the mouse press outside of the app
				OnMouseUp(e, graphics);
			} else if (selectedRegion is not null) {
				if (selectionRectangle.Contains(e.Location)) {
					// Start dragging the selected area
					isDraggingSelection = true;
					initialDragPoint = e.Location;
					latestDragPoint = e.Location;
				} else {
					// Commit the dragged location and clear the selection
					isDraggingSelection = false;
					DrawCurrentState(graphics);
					SaveCanvasBitmapState();
					selectedRegion = null;

					SaveStateCallback?.Invoke();

					// Initial press, start defining the 
					isDefiningSelection = true;
					initialDragPoint = e.Location;
					selectionRectangle = new Rectangle(e.Location, Size.Empty);
				}
			} else if (!isDefiningSelection && !isDraggingSelection) {
				// Initial press, start defining the 
				isDefiningSelection = true;
				initialDragPoint = e.Location;
				selectionRectangle = new Rectangle(e.Location, Size.Empty);
				SaveCanvasBitmapState();
			}
		}

		// Handle mouse movement for resizing the selection or dragging
		public override void OnMouseMove(MouseEventArgs e, Graphics graphics) {
			if (isDraggingSelection && selectedRegion != null) {
				// Calculate the offset for dragging
				int dx = e.Location.X - initialDragPoint.X;
				int dy = e.Location.Y - initialDragPoint.Y;
				latestDragPoint = e.Location;
				selectionRectangle.X = initialSelectionRectanglePosition.X + dx;
				selectionRectangle.Y = initialSelectionRectanglePosition.Y + dy;

				// Redraw the current state
				DrawCurrentState(graphics);
			} else if (isDefiningSelection && selectionRectangle != Rectangle.Empty) {
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
		private void DrawCurrentState(Graphics graphics) {
			graphics.Clear(Color.White);
			graphics.DrawImage(originalCanvasBitmap, 0, 0);
			if (selectedRegion is not null) graphics.DrawImage(selectedRegion, selectionRectangle.X, selectionRectangle.Y);
			if (isDefiningSelection) {
				using (var pen = new Pen(Color.Red, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash }) {
					graphics.DrawRectangle(pen, selectionRectangle.X, selectionRectangle.Y, selectionRectangle.Width + 1, selectionRectangle.Height + 1);
				}
			}
			if (isDraggingSelection) {
				using (var pen = new Pen(Color.Blue, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash }) {
					graphics.DrawRectangle(pen, selectionRectangle.X, selectionRectangle.Y, selectionRectangle.Width + 1, selectionRectangle.Height + 1);
				}
			}
		}

		// Handle mouse up for completing a selection or ending dragging
		public override void OnMouseUp(MouseEventArgs e, Graphics graphics) {
			Logger.Log("SelectionTool - OnMouseUp");
			if (isDefiningSelection && !isDraggingSelection) {
				// Stop dragging
				isDefiningSelection = false;
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
						initialSelectionRectanglePosition = new Point(selectionRectangle.X, selectionRectangle.Y);
					} else {
						// Commit the dragged location and clear the selection
						isDraggingSelection = false;
						DrawCurrentState(graphics);
						SaveCanvasBitmapState();
					}
				}
			} else if (isDraggingSelection) {
				initialSelectionRectanglePosition.X = selectionRectangle.X;
				initialSelectionRectanglePosition.Y = selectionRectangle.Y;
				isDraggingSelection = false;
			}
		}

		// Copy and move the selected region (e.g., for CTRL + Arrow keys functionality)
		public void CopyAndMoveSelection(Graphics graphics, int offsetX, int offsetY) {
			if (!selectionRectangle.IsEmpty && selectedRegion != null) {
				DrawCurrentState(graphics);
				SaveCanvasBitmapState();
				SaveStateCallback?.Invoke();
				// Update the position of the selection rectangle
				selectionRectangle.X += offsetX;
				selectionRectangle.Y += offsetY;

				// Draw the copied selection at the new position
				isDraggingSelection = true;
				DrawCurrentState(graphics);
				isDraggingSelection = false;
			}
		}

		// Copy and move the selected region (e.g., for CTRL + Arrow keys functionality)
		public void MoveSelection(Graphics graphics, int offsetX, int offsetY) {
			if (!selectionRectangle.IsEmpty && selectedRegion != null) {
				// Update the position of the selection rectangle
				selectionRectangle.X += offsetX;
				selectionRectangle.Y += offsetY;

				// Draw the copied selection at the new position
				isDraggingSelection = true;
				DrawCurrentState(graphics);
				isDraggingSelection = false;
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
	}
}