using CanvasMan.Interfaces;
using CanvasMan.Managers;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasMan.Tools {
	public class SelectionTool : Tool, IKeyInteractiveTool {
		private Rectangle selectionRectangle; // Defines the selected area
		private Bitmap selectedRegion;        // Stores the image data of the selected region
		private bool isDefiningSelection;     // Tracks if the selection is being defined
		private bool isDraggingSelection;              // Tracks if the selection is being dragged
		private Point dragStartPoint;         // Stores the starting point of a drag operation
		private Bitmap canvasBitmap;          // Reference to the canvas bitmap
		private Bitmap originalCanvasBitmap;
		private Point lastDragPoint;

		// Constructor to initialize the Select Tool
		public SelectionTool(ColourManager colourManager, string name = "Select") : base(name, colourManager) {
			selectionRectangle = Rectangle.Empty;
			isDraggingSelection = false;
			isDefiningSelection = false;
		}

		// Set the canvas bitmap (required for handling selections)
		public void SetCanvasBitmap(Bitmap canvas) {
			canvasBitmap = canvas;
		}

		// Handle mouse down for starting a new selection or dragging
		public override void OnMouseDown(MouseEventArgs e, Graphics graphics) {
			Logger.Log("SelectionTool - OnMouseDown");
			if (selectionRectangle.IsEmpty) {
				isDraggingSelection = false;
				isDefiningSelection = true;
				selectionRectangle = new Rectangle(e.Location, Size.Empty);
				selectedRegion = null;
				originalCanvasBitmap = (Bitmap)canvasBitmap.Clone();
			} else if (selectionRectangle.Contains(e.Location)) {
				// Start dragging the selected area
				isDraggingSelection = true;
				dragStartPoint = e.Location;
				lastDragPoint = e.Location;
			} else {
				// Commit the dragged location and clear the selection
				isDraggingSelection = false;
				isDefiningSelection = true;
				selectionRectangle = new Rectangle(e.Location, Size.Empty);
				selectedRegion = null;
				originalCanvasBitmap = (Bitmap)canvasBitmap.Clone();
			}
		}

		// Handle mouse movement for resizing the selection or dragging
		public override void OnMouseMove(MouseEventArgs e, Graphics graphics) {
			if (isDraggingSelection && selectedRegion != null) {
				// Calculate the offset for dragging
				int dx = e.Location.X - dragStartPoint.X;
				int dy = e.Location.Y - dragStartPoint.Y;
				lastDragPoint = e.Location;

				// Clear the canvas and redraw the original image
				graphics.Clear(Color.White);
				graphics.DrawImage(originalCanvasBitmap, 0, 0);

				// Draw the selected region at the new position
				graphics.DrawImage(selectedRegion, selectionRectangle.X + dx, selectionRectangle.Y + dy);
			} else if (isDefiningSelection && selectionRectangle != Rectangle.Empty) {
				// Update the size of the selection rectangle
				selectionRectangle.Width = e.X - selectionRectangle.X;
				selectionRectangle.Height = e.Y - selectionRectangle.Y;

				graphics.Clear(Color.White);
				graphics.DrawImage(originalCanvasBitmap, 0, 0);

				// Draw the selection rectangle (dashed outline)
				using (var pen = new Pen(Color.Blue, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash }) {
					graphics.DrawRectangle(pen, selectionRectangle);
				}
			}
		}

		// Handle mouse up for completing a selection or ending dragging
		public override void OnMouseUp(MouseEventArgs e, Graphics graphics) {
			Logger.Log("SelectionTool - OnMouseUp");
			if (isDefiningSelection) {
				// Stop dragging
				isDefiningSelection = false;
				if (!selectionRectangle.IsEmpty) {
					// Capture the selected region from the canvas bitmap
					selectedRegion = canvasBitmap.Clone(selectionRectangle, canvasBitmap.PixelFormat);
				}
			} else if (isDraggingSelection) {
				selectionRectangle.X += dragStartPoint.X - lastDragPoint.X;
				selectionRectangle.Y += dragStartPoint.Y - lastDragPoint.Y;
			}
		}

		// Copy and move the selected region (e.g., for CTRL + Arrow keys functionality)
		public void CopyAndMoveSelection(Graphics graphics, int offsetX, int offsetY) {
			if (!selectionRectangle.IsEmpty && selectedRegion != null) {
				// Update the position of the selection rectangle
				selectionRectangle.X += offsetX;
				selectionRectangle.Y += offsetY;

				// Draw the copied selection at the new position
				graphics.DrawImage(selectedRegion, selectionRectangle);
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