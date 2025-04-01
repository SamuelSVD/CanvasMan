using System;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasMan.Panels {
	public class ViewPortPanel : DoubleBufferedPanel {
		private Bitmap canvasImage;          // The canvas image to display
		private Point dragStartPoint;       // Tracks the starting point for dragging
		private bool isDragging;            // Flag for tracking if dragging is active
		private float zoomFactor = 1.0f;    // Current zoom level
		private PointF canvasOffset;        // Offset for panning the canvas
		private DoubleBufferedPanel canvasPanel;

		public ViewPortPanel(Bitmap canvasImage, DoubleBufferedPanel canvasPanel) {
			this.canvasImage = canvasImage;
			this.canvasPanel = canvasPanel;

			// Enable double buffering
			DoubleBuffered = true;

			// Initial offsets (center the canvas)
			canvasOffset = new PointF(0, 0);

			// Event subscriptions
			MouseDown += ViewPortPanel_MouseDown;
			MouseMove += ViewPortPanel_MouseMove;
			MouseUp += ViewPortPanel_MouseUp;
			MouseWheel += ViewPortPanel_MouseWheel;
			AutoScroll = true;
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);

			// Clear the background
			e.Graphics.Clear(Color.Gray);

			// Apply zoom and pan transformations
			e.Graphics.TranslateTransform(canvasOffset.X, canvasOffset.Y);
			e.Graphics.ScaleTransform(zoomFactor, zoomFactor);

			// Draw the canvas image
			if (canvasImage != null) {
				e.Graphics.DrawImage(canvasImage, 0, 0);
			}
		}

		private void ViewPortPanel_MouseDown(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Middle) {
				isDragging = true;
				dragStartPoint = e.Location; // Save the mouse position for dragging
				Cursor = Cursors.Hand;      // Change the cursor to indicate dragging
			} else {
				// Forward other clicks to canvasPanel
				Point transformedPoint = TransformMousePoint(e.Location);
				var newArgs = new MouseEventArgs(e.Button, e.Clicks, transformedPoint.X, transformedPoint.Y, e.Delta);
				canvasPanel.DoMouseDown(newArgs);
			}
		}

		private void ViewPortPanel_MouseMove(object sender, MouseEventArgs e) {
			if (isDragging) {
				// Calculate the offset based on mouse movement
				canvasOffset.X += e.X - dragStartPoint.X;
				canvasOffset.Y += e.Y - dragStartPoint.Y;

				dragStartPoint = e.Location; // Update the drag start point

				Invalidate(); // Redraw the panel
			} else {
				// Forward other clicks to canvasPanel
				Point transformedPoint = TransformMousePoint(e.Location);
				var newArgs = new MouseEventArgs(e.Button, e.Clicks, transformedPoint.X, transformedPoint.Y, e.Delta);
				canvasPanel.DoMouseMove(newArgs);
			}
		}

		private void ViewPortPanel_MouseUp(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Middle && isDragging) {
				isDragging = false;
				Cursor = Cursors.Default; // Restore the default cursor
			} else {
				// Forward other clicks to canvasPanel
				Point transformedPoint = TransformMousePoint(e.Location);
				var newArgs = new MouseEventArgs(e.Button, e.Clicks, transformedPoint.X, transformedPoint.Y, e.Delta);
				canvasPanel.DoMouseUp(newArgs);
			}
		}

		private void ViewPortPanel_MouseWheel(object sender, MouseEventArgs e) {
			if (ModifierKeys == Keys.Control) // Check if CTRL is held
			{
				float zoomDelta = e.Delta > 0 ? 0.1f : -0.1f;
				zoomFactor = Math.Max(0.1f, zoomFactor + zoomDelta); // Clamp the zoom level

				// Adjust canvas offset to keep the zoom centered on the mouse position
				var mousePos = e.Location;
				canvasOffset.X -= mousePos.X * zoomDelta / zoomFactor;
				canvasOffset.Y -= mousePos.Y * zoomDelta / zoomFactor;

				Invalidate(); // Redraw the panel
			}
		}
		private Point TransformMousePoint(Point mouseLocation) {
			// Adjust for canvasOffset and zoomFactor
			float x = (mouseLocation.X - canvasOffset.X) / zoomFactor;
			float y = (mouseLocation.Y - canvasOffset.Y) / zoomFactor;
			return new Point((int)x, (int)y);
		}
	}
}