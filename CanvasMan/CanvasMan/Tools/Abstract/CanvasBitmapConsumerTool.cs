using CanvasMan.Interfaces;
using CanvasMan.Managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasMan.Tools.Abstract {
	public abstract class CanvasBitmapConsumerTool : Tool, ICommitableTool, IBasicStateControlTool {
		private const int grabRadius = 8; // Area around endpoints to detect dragging
		protected Bitmap? originalCanvasBitmap; // Reference to the canvas bitmap
		protected bool isDefiningTool = false;
		protected bool isToolDefined = false;
		protected bool isToolChanged = false;
		protected Point initialDragPoint = Point.Empty;         // Stores the starting point of a drag operation

		public CanvasBitmapConsumerTool(ColourManager colourManager, CanvasManager canvasManager, string name) : base(colourManager, canvasManager, name) {
			colourManager.ColorChanged += ColourManager_ColorChanged;
		}

		private void ColourManager_ColorChanged() {
			if (IsActive) {
				DrawCurrentState();
				RefreshCanvasCallback?.Invoke();
			}
		}
		public void SaveCanvasBitmapState() {
			originalCanvasBitmap = (Bitmap)CanvasManager.CanvasImage.Clone();
		}
		public void ClearCanvasBitmapState() {
			originalCanvasBitmap = null;
		}
		public abstract void ClearToolState();
		public abstract void StartToolDefinition(MouseEventArgs e);
		public abstract void EndToolDefinition(MouseEventArgs e);
		public abstract void DrawCurrentState();

		public override void OnActivate() {
			ClearToolState();
			SaveCanvasBitmapState();
		}
		public override void OnDeactivate() {
			if (isToolDefined) CommitTool();
			else ClearToolState();
		}
		public void CommitTool() {
			DrawCurrentState();
			SaveCanvasBitmapState();
			if (isToolChanged) {
				SaveStateCallback?.Invoke();
				isToolChanged = false;
			}
		}
	}
}
