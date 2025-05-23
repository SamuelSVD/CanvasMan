﻿using CanvasMan.Managers;
using CanvasMan.Tools.Abstract;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace CanvasMan.Tools {
	public class FillTool : Tool {
		// Constructor to initialize the Fill Tool
		public FillTool(ColourManager colourManager, CanvasManager canvasManager, string name = "Fill") : base(colourManager, canvasManager, name) {
		}

		// Handle mouse down to start the fill operation
		public override void OnMouseDown(MouseEventArgs e) {
			// Perform the flood fill operation
			Point clickPoint = new Point(e.X, e.Y);
			if (clickPoint.X < 0) return;
			if (clickPoint.Y < 0) return;
			if (clickPoint.X > CanvasManager.CanvasImage.Width) return;
			if (clickPoint.Y > CanvasManager.CanvasImage.Height) return;
			if (e.Button == MouseButtons.Left) {
				Color targetColor = CanvasManager.CanvasImage.GetPixel(clickPoint.X, clickPoint.Y);
				if (targetColor != ColourManager.CurrentColor) {
					FloodFill(CanvasManager.CanvasImage, clickPoint, targetColor, ColourManager.CurrentColor);
					SaveStateCallback?.Invoke();
				}
			}
			if (e.Button == MouseButtons.Right) {
				Color targetColor = CanvasManager.CanvasImage.GetPixel(clickPoint.X, clickPoint.Y);
				if (targetColor != ColourManager.SecondaryColor) {
					FloodFill(CanvasManager.CanvasImage, clickPoint, targetColor, ColourManager.SecondaryColor);
					SaveStateCallback?.Invoke();
				}
			}
		}

		// These methods are not needed for the FillTool
		public override void OnMouseMove(MouseEventArgs e) { }
		public override void OnMouseUp(MouseEventArgs e) { }

		// Optimized Flood Fill using LockBits and a stack (assuming 24bpp RGB)
		private void FloodFill(Bitmap bitmap, Point start, Color targetColor, Color replacementColor) {
			if (targetColor.ToArgb() == replacementColor.ToArgb())
				return;

			int width = bitmap.Width;
			int height = bitmap.Height;
			// Lock bitmap bits
			BitmapData bmpData = bitmap.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadWrite,
				PixelFormat.Format24bppRgb);

			int stride = bmpData.Stride;
			int bytesPerPixel = 3; // For 24bpp

			int totalBytes = stride * height;
			byte[] pixelBuffer = new byte[totalBytes];

			System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelBuffer, 0, totalBytes);

			// Get target and replacement colors as bytes (in BGR order)
			byte targetB = targetColor.B, targetG = targetColor.G, targetR = targetColor.R;
			byte repB = replacementColor.B, repG = replacementColor.G, repR = replacementColor.R;

			// Use a stack to perform an iterative flood fill
			Stack<Point> pixelStack = new Stack<Point>();
			pixelStack.Push(start);

			while (pixelStack.Count > 0) {
				Point pt = pixelStack.Pop();
				int x = pt.X;
				int y = pt.Y;

				if (x < 0 || x >= width || y < 0 || y >= height)
					continue;

				int pos = y * stride + x * bytesPerPixel;
				// Check if this pixel matches the target color
				if (pixelBuffer[pos] == targetB &&
					pixelBuffer[pos + 1] == targetG &&
					pixelBuffer[pos + 2] == targetR) {
					// Set replacement color (BGR order)
					pixelBuffer[pos] = repB;
					pixelBuffer[pos + 1] = repG;
					pixelBuffer[pos + 2] = repR;

					// Push adjacent pixels onto the stack
					pixelStack.Push(new Point(x + 1, y));
					pixelStack.Push(new Point(x - 1, y));
					pixelStack.Push(new Point(x, y + 1));
					pixelStack.Push(new Point(x, y - 1));
				}
			}

			// Copy updated pixels back to bitmap
			System.Runtime.InteropServices.Marshal.Copy(pixelBuffer, 0, bmpData.Scan0, totalBytes);
			bitmap.UnlockBits(bmpData);
		}

		public override void OnActivate() { }

		public override void OnDeactivate() { }
	}
}