using System.Windows.Forms;

namespace CanvasMan {
	public class DoubleBufferedPanel : Panel {
		public DoubleBufferedPanel() {
			// Enable double buffering to reduce flicker
			this.DoubleBuffered = true;
			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
			this.UpdateStyles();
		}
	}
}
