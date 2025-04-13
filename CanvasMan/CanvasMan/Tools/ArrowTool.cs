using CanvasMan.Controls;
using CanvasMan.Managers;
using CanvasMan.Tools.Abstract;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasMan.Tools {
	public class ArrowTool : TwoControlPointTool {

		public ArrowTool(ColourManager colourManager, CanvasManager canvasManager, string name = "Arrow") : base(colourManager, canvasManager, name) {
		}

		private void DrawArrow(Graphics graphics) {
			using (Pen pen = new Pen(ColourManager.CurrentColor, 3)) {
				graphics.DrawLine(pen, startPoint.Location, endPoint.Location);

				// Calculate the arrowhead
				DrawArrowHead(graphics, pen, startPoint.Location, endPoint.Location);
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

		public override void DrawCurrentState() {
			CanvasManager.CanvasGraphics.DrawImage(originalCanvasBitmap, 0, 0);
			DrawArrow(CanvasManager.CanvasGraphics);
		}
	}
}