using CanvasMan.Managers;
using CanvasMan.Tools.Abstract;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasMan.Tools {
	public class BrushTool : Tool {
		public int BrushSize { get; set; } // The size (width) of the brush

        private Point? lastPoint = null; // Store the last drawn mouse position as a nullable Point.
		private bool isDragging = false;
		// Constructor to initialize the brush tool
		public BrushTool(ColourManager colourManager, CanvasManager canvasManager, string name = "Brush", int size = 5) : base(colourManager, canvasManager, name) {
			BrushSize = size;
		}

		// Handle the mouse down event (not necessary for continuous strokes)
		public override void OnMouseDown(MouseEventArgs e) {
			lastPoint = e.Location;

			// Optional: Create a small dot where the mouse is clicked
			if (e.Button == MouseButtons.Left) {
				using (var brush = new SolidBrush(ColourManager.CurrentColor)) {
					CanvasManager.CanvasGraphics.FillEllipse(brush, e.X - BrushSize / 2, e.Y - BrushSize / 2, BrushSize, BrushSize);
					if (BrushSize == 1) {
						CanvasManager.CanvasGraphics.FillRectangle(brush, e.X, e.Y, 1, 1);
					}
				}
			}
			if (e.Button == MouseButtons.Right) {
				using (var brush = new SolidBrush(ColourManager.SecondaryColor)) {
					CanvasManager.CanvasGraphics.FillEllipse(brush, e.X - BrushSize / 2, e.Y - BrushSize / 2, BrushSize, BrushSize);
					if (BrushSize == 1) {
						CanvasManager.CanvasGraphics.FillRectangle(brush, e.X, e.Y, 1, 1);
					}
				}
			}
			isDragging = true;
		}

		// Handle the mouse move event (for continuous strokes)
		public override void OnMouseMove(MouseEventArgs e) {
			if (lastPoint is not null) {
				if (e.Button == MouseButtons.Left && lastPoint is not null) {
					using (var brush = new SolidBrush(ColourManager.CurrentColor)) {
						CanvasManager.CanvasGraphics.FillEllipse(brush, e.X - BrushSize / 2, e.Y - BrushSize / 2, BrushSize, BrushSize);
					}
					using (var pen = new Pen(ColourManager.CurrentColor, BrushSize)) {
						CanvasManager.CanvasGraphics.DrawLine(pen, lastPoint!.Value, e.Location);
					}
				}
				if (e.Button == MouseButtons.Right && lastPoint is not null) {
					using (var brush = new SolidBrush(ColourManager.SecondaryColor)) {
						CanvasManager.CanvasGraphics.FillEllipse(brush, e.X - BrushSize / 2, e.Y - BrushSize / 2, BrushSize, BrushSize);
					}
					using (var pen = new Pen(ColourManager.SecondaryColor, BrushSize)) {
						CanvasManager.CanvasGraphics.DrawLine(pen, lastPoint!.Value, e.Location);
					}
				}
				lastPoint = e.Location;
			}
		}

		// Handle the mouse up event (no specific behavior needed here for the brush)
		public override void OnMouseUp(MouseEventArgs e) {
			// BrushTool doesn't need to do anything special on mouse release
			isDragging = false;
			SaveStateCallback?.Invoke();
		}

		public override void OnActivate() {}

		public override void OnDeactivate() {
			if (isDragging) {
				SaveStateCallback?.Invoke();
			}
			isDragging = false;
		}
	}
}