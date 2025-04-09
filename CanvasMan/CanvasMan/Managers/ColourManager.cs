using System;

namespace CanvasMan.Managers {
	public class ColourManager {
		// Event to notify subscribers when the current color changes.
		public event Action? ColorChanged;

		private Color currentColor = Color.Black;
		public Color CurrentColor {
			get => currentColor;
			set {
				if (currentColor != value) {
					currentColor = value;
					// Notify any subscribers (like your tools) about the color change.
					ColorChanged?.Invoke();
				}
			}
		}

		private Color secondaryColor = Color.White;
		public Color SecondaryColor {
			get => secondaryColor;
			set {
				if (secondaryColor != value) {
					secondaryColor = value;
					// Notify any subscribers (like your tools) about the color change.
					ColorChanged?.Invoke();
				}
			}
		}
	}
}
