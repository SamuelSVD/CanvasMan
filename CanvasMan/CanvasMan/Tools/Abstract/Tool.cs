using CanvasMan.Managers;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasMan.Tools.Abstract {
	public abstract class Tool {
		// Common properties for all tools
		public string Name { get; }
		public bool IsActive { get; private set; }

		// Callback delegate to trigger saving state.
		public Action? SaveStateCallback { get; set; }
		public Action? RefreshCanvasCallback { get; set; }

		protected ColourManager ColourManager;
		protected CanvasManager CanvasManager;
		// Constructor to initialize the tool's name
		protected Tool(ColourManager colourManager, CanvasManager canvasManager, string name) {
			Name = name;
			IsActive = false;
			ColourManager = colourManager;
			CanvasManager = canvasManager;
		}

		// Methods to activate or deactivate the tool
		public virtual void Activate() {
			IsActive = true;
			OnActivate();
		}

		public virtual void Deactivate() {
			IsActive = false;
			OnDeactivate();
		}

		public abstract void OnActivate();
		public abstract void OnDeactivate();
		// Abstract methods for handling mouse events
		// These must be overridden in derived tool classes
		public abstract void OnMouseDown(MouseEventArgs e);
		public abstract void OnMouseMove(MouseEventArgs e);
		public abstract void OnMouseUp(MouseEventArgs e);
	}
}