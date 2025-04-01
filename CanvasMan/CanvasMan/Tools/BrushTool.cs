using CanvasMan.Managers;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasMan.Tools {
	public class BrushTool : Tool {
		public int BrushSize { get; set; } // The size (width) of the brush

        private Point? lastPoint = null; // Store the last drawn mouse position as a nullable Point.

		// Constructor to initialize the brush tool
		public BrushTool(ColourManager colourManager, string name = "Brush", int size = 5) : base(colourManager, name) {
			BrushSize = size;
		}

		// Handle the mouse down event (not necessary for continuous strokes)
		public override void OnMouseDown(MouseEventArgs e, Graphics graphics) {
			lastPoint = e.Location;

			// Optional: Create a small dot where the mouse is clicked
			if (e.Button == MouseButtons.Left) {
				using (var brush = new SolidBrush(ColourManager.CurrentColor)) {
					graphics.FillEllipse(brush, e.X - BrushSize / 2, e.Y - BrushSize / 2, BrushSize, BrushSize);
				}
			}
		}

		// Handle the mouse move event (for continuous strokes)
		public override void OnMouseMove(MouseEventArgs e, Graphics graphics) {
			if (e.Button == MouseButtons.Left) {
				using (var brush = new SolidBrush(ColourManager.CurrentColor)) {
					graphics.FillEllipse(brush, e.X - BrushSize / 2, e.Y - BrushSize / 2, BrushSize, BrushSize);
				}
				using (var pen = new Pen(ColourManager.CurrentColor, BrushSize))
                {
                    graphics.DrawLine(pen, lastPoint!.Value, e.Location);
                }
				lastPoint = e.Location;
			}
		}

		// Handle the mouse up event (no specific behavior needed here for the brush)
		public override void OnMouseUp(MouseEventArgs e, Graphics graphics) {
			// BrushTool doesn't need to do anything special on mouse release
			SaveStateCallback?.Invoke();
		}
	}
}