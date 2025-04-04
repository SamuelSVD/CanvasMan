namespace CanvasMan.Interfaces {
	// Optional: Interface for tools that respond to key events
	public interface IBasicStateControlTool {
		public abstract void ClearToolState();
		public abstract void StartToolDefinition(MouseEventArgs e);
		public abstract void EndToolDefinition(MouseEventArgs e, Graphics graphics);
		public abstract void DrawCurrentState(Graphics graphics);
	}
}
