namespace CanvasMan.Interfaces {
	// Optional: Interface for tools that respond to key events
	public interface IKeyInteractiveTool {
		// Used for capturing key down of non control functions, or mix of keys
		void OnPreviewKeyDown(PreviewKeyDownEventArgs e);
		bool OnProcessCommandKey(ref Message msg, Keys keyData);
		void OnKeyDown(KeyEventArgs e);
		void OnKeyUp(KeyEventArgs e);
	}
}
