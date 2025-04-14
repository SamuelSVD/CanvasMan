using System.Windows.Forms;

namespace CanvasMan.UI.Panels {
	public class DoubleBufferedPanel : Panel {
		public DoubleBufferedPanel() {
			// Enable double buffering to reduce flicker
			DoubleBuffered = true;
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
			UpdateStyles();
		}
		public void DoMouseDown(MouseEventArgs e) {
			OnMouseDown(e);
		}
		public void DoMouseMove(MouseEventArgs e) {
			OnMouseMove(e);
		}
		public void DoMouseUp(MouseEventArgs e) {
			OnMouseUp(e);
		}
	}
}
