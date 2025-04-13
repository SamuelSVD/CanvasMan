using CanvasMan.Interfaces;
using CanvasMan.Managers;
using CanvasMan.Tools.Abstract;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasMan.Tools {
	public class SelectionTool : CanvasBitmapConsumerTool, IKeyInteractiveTool {
		private Bitmap ? selectedRegion;        // Stores the image data of the selected region
		private Rectangle selectionRectangle; // Defines the selected area
		private Point initialSelectionRectanglePosition;
		private bool isDraggingTool = false;

		// Constructor to initialize the Select Tool
		public SelectionTool(ColourManager colourManager, CanvasManager canvasManager, string name = "Select") : base(colourManager, canvasManager, name) {
			selectionRectangle = Rectangle.Empty;
		}

		// Handle mouse down for starting a new selection or dragging
		public override void OnMouseDown(MouseEventArgs e) {
			Logger.Log("SelectionTool - OnMouseDown");
			if (isDefiningTool || isDraggingTool) {
				// If we were dragging the selection, this means someone released
				// the mouse press outside of the app
				OnMouseUp(e);
			} else if (isToolDefined) {
				if (selectionRectangle.Contains(e.Location)) {
					// Start dragging the selected area
					isDraggingTool = true;
					initialDragPoint = e.Location;
				} else {
					// Commit the dragged location and clear the selection
					CommitTool();
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
		public override void OnMouseMove(MouseEventArgs e) {
			if (isDraggingTool && isToolDefined) {
				// Calculate the offset for dragging
				int dx = e.Location.X - initialDragPoint.X;
				int dy = e.Location.Y - initialDragPoint.Y;
				selectionRectangle.X = initialSelectionRectanglePosition.X + dx;
				selectionRectangle.Y = initialSelectionRectanglePosition.Y + dy;
				// Redraw the current state
				DrawCurrentState();
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
				DrawCurrentState();
			}
		}
		public override void DrawCurrentState() {
			CanvasManager.CanvasGraphics.Clear(Color.White);
			CanvasManager.CanvasGraphics.DrawImage(originalCanvasBitmap, 0, 0);
			if (isToolDefined) CanvasManager.CanvasGraphics.DrawImage(selectedRegion, selectionRectangle.X, selectionRectangle.Y);
			if (isDefiningTool) {
				using (var pen = new Pen(Color.Red, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash }) {
					CanvasManager.CanvasGraphics.DrawRectangle(pen, selectionRectangle.X, selectionRectangle.Y, selectionRectangle.Width + 1, selectionRectangle.Height + 1);
				}
			}
			if (isDraggingTool) {
				using (var pen = new Pen(Color.Blue, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash }) {
					CanvasManager.CanvasGraphics.DrawRectangle(pen, selectionRectangle.X, selectionRectangle.Y, selectionRectangle.Width + 1, selectionRectangle.Height + 1);
				}
			}
		}

		// Handle mouse up for completing a selection or ending dragging
		public override void OnMouseUp(MouseEventArgs e) {
			Logger.Log("SelectionTool - OnMouseUp");
			if (isDefiningTool && !isDraggingTool) {
				EndToolDefinition(e);
			} else if (isDraggingTool) {
				initialSelectionRectanglePosition.X = selectionRectangle.X;
				initialSelectionRectanglePosition.Y = selectionRectangle.Y;
				isDraggingTool = false;
			}
		}

		// Copy and move the selected region (e.g., for CTRL + Arrow keys functionality)
		public void CopyAndMoveSelection(int offsetX, int offsetY) {
			if (isToolDefined) {
				CommitTool();
				// Update the position of the selection rectangle
				selectionRectangle.X += offsetX;
				selectionRectangle.Y += offsetY;

				// Draw the copied selection at the new position
				isDraggingTool = true;
				DrawCurrentState();
				isDraggingTool = false;
			}
		}

		// Copy and move the selected region (e.g., for CTRL + Arrow keys functionality)
		public void MoveSelection(int offsetX, int offsetY) {
			if (isToolDefined) {
				// Update the position of the selection rectangle
				selectionRectangle.X += offsetX;
				selectionRectangle.Y += offsetY;

				// Draw the copied selection at the new position
				isDraggingTool = true;
				DrawCurrentState();
				isDraggingTool = false;
			}
		}

		// Implement OnKeyDown for handling key presses
		public void OnKeyDown(KeyEventArgs e) {
			int offsetX = 0, offsetY = 0;

			switch (e.KeyCode) {
				case Keys.Up:
					offsetY = -1; // Move up
					break;
				case Keys.Down:
					offsetY = 1; // Move down
					break;
				case Keys.Left:
					offsetX = -1; // Move left
					break;
				case Keys.Right:
					offsetX = 1; // Move right
					break;
			}
			if (e.Control) {
				// Copy and move the selection
				CopyAndMoveSelection(offsetX, offsetY);
			} else {
				MoveSelection(offsetX, offsetY);
			}
		}
		// Implement OnKeyUp for handling key releases
		public void OnKeyUp(KeyEventArgs e) {
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

		public override void EndToolDefinition(MouseEventArgs e) {
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
					selectedRegion = originalCanvasBitmap?.Clone(selectionRectangle, CanvasManager.CanvasImage.PixelFormat);
					isToolDefined = true;
					initialSelectionRectanglePosition = new Point(selectionRectangle.X, selectionRectangle.Y);
				} else {
					// Commit the dragged location and clear the selection
					isDraggingTool = false;
					DrawCurrentState();
					SaveCanvasBitmapState();
				}
			}
		}

		public void OnPreviewKeyDown(PreviewKeyDownEventArgs e) {
			if (e.KeyCode == Keys.Up) {
				e.IsInputKey = true;
			}
			if (e.KeyCode == Keys.Down) {
				e.IsInputKey = true;
			}
			if (e.KeyCode == Keys.Left) {
				e.IsInputKey = true;
			}
			if (e.KeyCode == Keys.Right) {
				e.IsInputKey = true;
			}
		}

		public bool OnProcessCommandKey(ref Message msg, Keys keyData) {
			// Handle CTRL + Arrow keys for duplicating and moving selection
			int offsetX = 0, offsetY = 0;

			switch (keyData) {
				case Keys.Up:
					offsetY = -1; // Move up
					break;
				case Keys.Down:
					offsetY = 1; // Move down
					break;
				case Keys.Left:
					offsetX = -1; // Move left
					break;
				case Keys.Right:
					offsetX = 1; // Move right
					break;
			}
			MoveSelection(offsetX, offsetY);
			return true;
		}
	}
}