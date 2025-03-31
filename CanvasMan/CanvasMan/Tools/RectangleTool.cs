using CanvasMan.Managers;
using CanvasMan.Tools;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasMan {
	public class RectangleTool : CanvasBitmapConsumerTool {
		// Fields to track the start and current points during drawing
		private Point? startPoint;
		private Point? currentPoint;

		public Color RectangleColor { get; set; }  // Color of the rectangle
		public int BorderWidth { get; set; }       // Width of the rectangle border

		private bool isDrawing;                    // Tracks whether drawing is ongoing

		public RectangleTool(ColourManager colourManager, string name, int borderWidth = 1) : base(colourManager, name) {
			BorderWidth = borderWidth;
			isDrawing = false;
		}

		public override void OnMouseDown(MouseEventArgs e, Graphics graphics) {
			if (e.Button == MouseButtons.Left) {
				// Store the start point and mark as drawing
				startPoint = e.Location;
				currentPoint = e.Location;
				isDrawing = true;
				// Save the initial state of the canvas
				SaveCanvasBitmapState();
			}
		}

		public override void OnMouseMove(MouseEventArgs e, Graphics graphics) {
			if (isDrawing && e.Button == MouseButtons.Left) {
				// Update the current point
				currentPoint = e.Location;

				// Reset the canvas to the original state to avoid overlapping artifacts
				graphics.DrawImage(originalCanvasBitmap, 0, 0);

				// Perform the rectangle drawing
				DrawRectangle(graphics, startPoint.Value, currentPoint.Value);
			}
		}

		public override void OnMouseUp(MouseEventArgs e, Graphics graphics) {
			if (isDrawing) {
				// Mark as no longer drawing and finalize the rectangle
				isDrawing = false;
				currentPoint = e.Location;

				// Draw the final rectangle
				DrawRectangle(graphics, startPoint.Value, currentPoint.Value);
				startPoint = null;
				currentPoint = null;
			}
		}

		private void DrawRectangle(Graphics graphics, Point start, Point end) {
			int x = Math.Min(start.X, end.X);
			int y = Math.Min(start.Y, end.Y);
			int width = Math.Abs(end.X - start.X);
			int height = Math.Abs(end.Y - start.Y);

			using (var pen = new Pen(RectangleColor, BorderWidth)) {
				graphics.DrawRectangle(pen, x, y, width, height);
			}
		}
	}
}