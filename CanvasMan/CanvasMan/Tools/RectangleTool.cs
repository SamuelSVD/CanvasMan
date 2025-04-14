using CanvasMan.Managers;
using CanvasMan.Tools.Abstract;
using CanvasMan.Utils;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace CanvasMan {
	public class RectangleTool : RectControlPointTool {
		public int BorderWidth { get; set; }       // Width of the rectangle border
		public RectangleTool(ColourManager colourManager, CanvasManager canvasManager, string name, int borderWidth = 1) : base(colourManager, canvasManager, name) {
			BorderWidth = borderWidth;
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

		public override void EndToolDefinition(MouseEventArgs e) {
			endPoint.SetLocation(e.Location);
			isDefiningTool = false;
			isDraggingTool = false;
			isToolDefined = true;
			isToolChanged = true;
		}

		public override void DrawCurrentState() {
			// Reset the canvas to the original state to avoid overlapping artifacts
			CanvasManager.CanvasGraphics.DrawImage(originalCanvasBitmap, 0, 0);

			// Perform the rectangle drawing
			DrawRectangle(CanvasManager.CanvasGraphics, startPoint.Location, endPoint.Location);
		}
		public override void StartToolDefinition(MouseEventArgs e) {
			isDefiningTool = true;
			startPoint.SetLocation(e.Location);
		}
	}
}