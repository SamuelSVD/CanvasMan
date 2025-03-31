using CanvasMan.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasMan.Tools {
	public abstract class CanvasBitmapConsumerTool: Tool {
		protected Bitmap ? canvasBitmap; // Reference to the canvas bitmap
		protected Bitmap ? originalCanvasBitmap; // Reference to the canvas bitmap

		public CanvasBitmapConsumerTool(ColourManager colourManager, string name) : base(colourManager, name) {
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
	}
}
