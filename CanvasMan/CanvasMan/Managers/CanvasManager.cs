using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasMan.Managers {
	public class CanvasManager {
		public Bitmap CanvasImage;          // The canvas image to display
		public Graphics CanvasGraphics;
		public Bitmap OverlayImage;        // The canvas overlay to display
		public Graphics OverlayGraphics;
		public CanvasManager(int width, int height) {
			// Set up the canvas (panel with background image)
			CanvasImage = new Bitmap(width, height);
			CanvasGraphics = Graphics.FromImage(CanvasImage);
			CanvasGraphics.Clear(Color.White); // Clear canvas to white
			// Set up the overlay
			OverlayImage = new Bitmap(width, height);
			OverlayGraphics = Graphics.FromImage(OverlayImage);
		}
	}
}
