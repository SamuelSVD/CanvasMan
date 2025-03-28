using System;

namespace CanvasMan.Managers {
	public class ColourManager {
		// Event to notify subscribers when the current color changes.
		public event Action<Color> ? ColorChanged;

		private Color currentColor = Color.Black;
		public Color CurrentColor {
			get => currentColor;
			set {
				if (currentColor != value) {
					currentColor = value;
					// Notify any subscribers (like your tools) about the color change.
					ColorChanged?.Invoke(currentColor);
				}
			}
		}
	}
}
