using CanvasMan.Managers;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasMan.Tools {
	public abstract class Tool {
		// Common properties for all tools
		public string Name { get; }
		public bool IsActive { get; private set; }

		// Callback delegate to trigger saving state.
		public Action ? SaveStateCallback { get; set; }
		public Action? RefreshCanvasCallback { get; set; }

		protected ColourManager ColourManager;
		// Constructor to initialize the tool's name
		protected Tool(ColourManager colourManager, string name) {
			Name = name;
			IsActive = false;
			ColourManager = colourManager;
		}

		// Methods to activate or deactivate the tool
		public virtual void Activate(Graphics graphics) {
			IsActive = true;
			OnActivate(graphics);
		}

		public virtual void Deactivate(Graphics graphics) {
			IsActive = false;
			OnDeactivate(graphics);
		}

		public abstract void OnActivate(Graphics graphics);
		public abstract void OnDeactivate(Graphics graphics);
		// Abstract methods for handling mouse events
		// These must be overridden in derived tool classes
		public abstract void OnMouseDown(MouseEventArgs e, Graphics graphics);
		public abstract void OnMouseMove(MouseEventArgs e, Graphics graphics);
		public abstract void OnMouseUp(MouseEventArgs e, Graphics graphics);
	}
}