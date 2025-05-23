﻿using CanvasMan.Managers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasMan.UI.Panels {
	public class ViewPortPanel : DoubleBufferedPanel {
		private Point dragStartPoint;       // Tracks the starting point for dragging
		private bool isDragging;            // Flag for tracking if dragging is active
		private float zoomFactor = 1.0f;    // Current zoom level
		private PointF canvasOffset;        // Offset for panning the canvas
		private DoubleBufferedPanel canvasPanel;
		private CanvasManager canvasManager;
		public ViewPortPanel(CanvasManager canvasManager, DoubleBufferedPanel canvasPanel) {
			this.canvasManager = canvasManager;
			this.canvasPanel = canvasPanel;

			// Enable double buffering
			DoubleBuffered = true;


			// Event subscriptions
			MouseDown += ViewPortPanel_MouseDown;
			MouseMove += ViewPortPanel_MouseMove;
			MouseUp += ViewPortPanel_MouseUp;
			MouseWheel += ViewPortPanel_MouseWheel;
			AutoScroll = true;

			// Initial offsets (center the canvas)
			canvasOffset = new PointF(0, 0);
		}
		public void InitializeCanvas() {
			int centerX = (ClientSize.Width - canvasManager.CanvasImage.Width) / 2;
			int centerY = (ClientSize.Height - canvasManager.CanvasImage.Height) / 2;
			canvasOffset = new PointF(centerX, centerY);
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);

			// Clear the background
			e.Graphics.Clear(Color.Gray);

			// e.Graphics.DrawLine(new Pen(Color.Red), 0, 0, Width, Height);
			// Apply zoom and pan transformations
			e.Graphics.TranslateTransform(canvasOffset.X, canvasOffset.Y);
			e.Graphics.ScaleTransform(zoomFactor, zoomFactor);

			// Disable smoothing to maintain sharpness
			e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

			// Draw the canvas image
			if (canvasManager.CanvasImage != null) {
				e.Graphics.DrawImage(canvasManager.CanvasImage, 0, 0);
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

				CorrectCanvasOffset();

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

		//private void ViewPortPanel_MouseWheel(object sender, MouseEventArgs e) {
		//	if (ModifierKeys == Keys.Control) // Check if CTRL is held
		//	{
		//		float zoomDelta = e.Delta > 0 ? 0.1f : -0.1f;
		//		zoomFactor = Math.Max(0.1f, zoomFactor + zoomDelta); // Clamp the zoom level

		//		// Adjust canvas offset to keep the zoom centered on the mouse position
		//		var mousePos = e.Location;
		//		canvasOffset.X -= mousePos.X * zoomDelta / zoomFactor;
		//		canvasOffset.Y -= mousePos.Y * zoomDelta / zoomFactor;

		//		Invalidate(); // Redraw the panel
		//	}
		//}
		private void ViewPortPanel_MouseWheel(object sender, MouseEventArgs e) {
			if (ModifierKeys == Keys.Control) // Ensure CTRL is held for zooming
			{
				float zoomDelta = e.Delta > 0 ? 0.2f : -0.2f;
				AdjustCanvasOffsetOnZoom(e.Location, zoomDelta);
			}
		}

		private Point TransformMousePoint(Point mouseLocation) {
			// Adjust for canvasOffset and zoomFactor
			float x = (mouseLocation.X - canvasOffset.X) / zoomFactor;
			float y = (mouseLocation.Y - canvasOffset.Y) / zoomFactor;
			return new Point((int)x, (int)y);
		}
		private PointF GetMousePositionRelativeToCanvas(Point mouseLocation) {
			// Convert mouse coordinates to canvas coordinates
			float relativeX = (mouseLocation.X - canvasOffset.X) / zoomFactor;
			float relativeY = (mouseLocation.Y - canvasOffset.Y) / zoomFactor;

			return new PointF(relativeX, relativeY);
		}
		private void AdjustCanvasOffsetOnZoom(Point mouseLocation, float zoomDelta) {
			// Get the mouse position before zooming
			PointF relativePosition = GetMousePositionRelativeToCanvas(mouseLocation);

			// Apply zoom adjustment
			zoomFactor = Math.Max(0.1f, zoomFactor + zoomDelta); // Prevent zoom factor from going too low

			// Calculate new offset to maintain cursor position
			canvasOffset.X = mouseLocation.X - relativePosition.X * zoomFactor;
			canvasOffset.Y = mouseLocation.Y - relativePosition.Y * zoomFactor;

			Invalidate(); // Redraw viewport
		}
		private void CorrectCanvasOffset() {
			/*if (canvasOffset.X > canvasPanel.Width) {
				canvasOffset.X = canvasPanel.Width;
			}
			if (canvasOffset.X < -canvasPanel.Width) {
				canvasOffset.X = 0;
			}
			if (canvasOffset.Y > canvasPanel.Height) {
				canvasOffset.Y = canvasPanel.Height;
			}
			if (canvasOffset.X < -canvasPanel.Height) {
				canvasOffset.X = -canvasPanel.Height;
			}*/
		}
	}
}