﻿using CanvasMan.Controls;
using CanvasMan.Managers;
using CanvasMan.Tools.Abstract;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasMan.Tools {
	public class LineTool : TwoControlPointTool {
		public int BrushSize = 5;
		public LineTool(ColourManager colourManager, CanvasManager canvasManager, string name = "Line") : base(colourManager, canvasManager, name) {
		}
		public override void DrawCurrentState() {
			CanvasManager.CanvasGraphics.DrawImage(originalCanvasBitmap, 0, 0);
			DrawLine(CanvasManager.CanvasGraphics);
		}
		private void DrawLine(Graphics graphics) {
			using (var brush = new SolidBrush(ColourManager.CurrentColor)) {
				graphics.FillEllipse(brush, startPoint.Location.X - BrushSize / 2, startPoint.Location.Y - BrushSize / 2, BrushSize, BrushSize);
				graphics.FillEllipse(brush, endPoint.Location.X - BrushSize / 2, endPoint.Location.Y - BrushSize / 2, BrushSize, BrushSize);
			}
			using (Pen pen = new Pen(ColourManager.CurrentColor, 3)) {
				graphics.DrawLine(pen, startPoint.Location, endPoint.Location);
			}
		}
	}
}