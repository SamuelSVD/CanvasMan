using CanvasMan.Interfaces;
using CanvasMan.Managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasMan.Tools {
	public abstract class CanvasBitmapConsumerTool: Tool, ICommitableTool, IBasicStateControlTool {
		private const int grabRadius = 8; // Area around endpoints to detect dragging
		protected Bitmap ? canvasBitmap; // Reference to the canvas bitmap
		protected Bitmap ? originalCanvasBitmap; // Reference to the canvas bitmap
		protected bool isDefiningTool = false;
		protected bool isDraggingTool = false;
		protected bool isToolDefined = false;
		protected bool isToolChanged = false;
		protected Point initialDragPoint = Point.Empty;         // Stores the starting point of a drag operation

		public CanvasBitmapConsumerTool(ColourManager colourManager, string name) : base(colourManager, name) {
			colourManager.ColorChanged += ColourManager_ColorChanged;
		}

		private void ColourManager_ColorChanged(Color obj) {
			if (IsActive && canvasBitmap is not null) {
				DrawCurrentState(Graphics.FromImage(canvasBitmap));
				RefreshCanvasCallback?.Invoke();
			}
		}

		// Set the canvas bitmap (required for handling selections)
		public void SetCanvasBitmap(Bitmap canvas) {
			canvasBitmap = canvas;
		}
		public void SaveCanvasBitmapState() {
			originalCanvasBitmap = (Bitmap)canvasBitmap.Clone();
		}
		public void ClearCanvasBitmapState() {
			originalCanvasBitmap = null;
		}
		public abstract void ClearToolState();
		public abstract void StartToolDefinition(MouseEventArgs e);
		public abstract void EndToolDefinition(MouseEventArgs e, Graphics graphics);
		public abstract void DrawCurrentState(Graphics graphics);

		public override void OnActivate(Graphics graphics) {
			ClearToolState();
			SaveCanvasBitmapState();
		}
		public override void OnDeactivate(Graphics graphics) {
			if (isToolDefined) CommitTool(graphics);
			else ClearToolState();
		}
		public void CommitTool(Graphics graphics) {
			DrawCurrentState(graphics);
			SaveCanvasBitmapState();
			if (isToolChanged) {
				SaveStateCallback?.Invoke();
				isToolChanged = false;
			}
		}

		protected bool IsNearPoint(Point click, Point target) {
			return Math.Abs(click.X - target.X) <= grabRadius &&
				   Math.Abs(click.Y - target.Y) <= grabRadius;
		}

		protected bool IsNearLine(Point click, Point start, Point end) {
			// Check if the point is close to the line
			float distance = DistanceFromLine(click, start, end);
			return distance < grabRadius;
		}

		protected float DistanceFromLine(Point p, Point a, Point b) {
			float num = Math.Abs((b.Y - a.Y) * p.X - (b.X - a.X) * p.Y + b.X * a.Y - b.Y * a.X);
			float denom = (float)Math.Sqrt(Math.Pow(b.Y - a.Y, 2) + Math.Pow(b.X - a.X, 2));
			return num / denom;
		}
	}
}
