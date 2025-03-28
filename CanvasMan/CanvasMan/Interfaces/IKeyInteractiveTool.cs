namespace CanvasMan.Interfaces {
	// Optional: Interface for tools that respond to key events
	public interface IKeyInteractiveTool {
		void OnKeyDown(KeyEventArgs e, Graphics graphics);
		void OnKeyUp(KeyEventArgs e, Graphics graphics);
	}
}
