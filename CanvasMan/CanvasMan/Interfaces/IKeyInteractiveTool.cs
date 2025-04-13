namespace CanvasMan.Interfaces {
	// Optional: Interface for tools that respond to key events
	public interface IKeyInteractiveTool {
		void OnPreviewKeyDown(PreviewKeyDownEventArgs e);
		void OnKeyDown(KeyEventArgs e);
		void OnKeyUp(KeyEventArgs e);
	}
}
