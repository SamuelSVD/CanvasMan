using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Compression;

namespace CanvasMan.Managers {
	public class StateManager {
		private readonly Stack<byte[]> undoStack;
		private readonly Stack<byte[]> redoStack;

		public StateManager() {
			undoStack = new Stack<byte[]>();
			redoStack = new Stack<byte[]>();
		}

		/// <summary>
		/// Saves the current state into the undo stack and clears the redo stack.
		/// </summary>
		public void SaveState(Bitmap currentState) {
			// Clone the bitmap to store a copy of the current state.
			undoStack.Push(CompressBitmapLossless((Bitmap)currentState.Clone()));
			// Clear redo since a new operation invalidates the redo history.
			redoStack.Clear();
		}

		/// <summary>
		/// Undo: returns the previous state from the undo stack.
		/// The current state is pushed to the redo stack.
		/// </summary>
		public Bitmap Undo(Bitmap currentState) {
			if (undoStack.Count > 1) {
				redoStack.Push(undoStack.Pop());
				return DecompressBitmapLossless(undoStack.Peek());
			}
			return currentState;
		}

		/// <summary>
		/// Redo: returns the next state from the redo stack.
		/// The current state is moved back into the undo stack.
		/// </summary>
		public Bitmap Redo(Bitmap currentState) {
			if (redoStack.Count > 0) {
				undoStack.Push(redoStack.Pop());
				return DecompressBitmapLossless(undoStack.Peek());
			}
			return currentState;
		}

		// Optional helper properties
		public int UndoCount => undoStack.Count;
		public int RedoCount => redoStack.Count;
		/// <summary>
		/// Compresses the bitmap as PNG and then applies GZip compression.
		/// </summary>
		private byte[] CompressBitmapLossless(Bitmap bmp) {
			using (MemoryStream pngStream = new MemoryStream()) {
				// Save bitmap as PNG into the stream.
				bmp.Save(pngStream, ImageFormat.Png);
				byte[] pngData = pngStream.ToArray();

				// Now compress the PNG data with GZip.
				using (MemoryStream gzipStream = new MemoryStream()) {
					using (GZipStream compressor = new GZipStream(gzipStream, CompressionLevel.Optimal, leaveOpen: true)) {
						compressor.Write(pngData, 0, pngData.Length);
					}
					return gzipStream.ToArray();
				}
			}
		}

		/// <summary>
		/// Decompresses the PNG data from the GZip-compressed byte array and creates a Bitmap.
		/// </summary>
		private Bitmap DecompressBitmapLossless(byte[] data) {
			using (MemoryStream gzipStream = new MemoryStream(data)) {
				using (GZipStream decompressor = new GZipStream(gzipStream, CompressionMode.Decompress))
				using (MemoryStream pngStream = new MemoryStream()) {
					decompressor.CopyTo(pngStream);
					pngStream.Position = 0;
					return new Bitmap(pngStream);
				}
			}
		}

	}
}