using CanvasMan.Interfaces;
using CanvasMan.Tools;
using CanvasMan.Tools.Abstract;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasMan.Managers {
	public class ToolManager {
		private readonly Dictionary<string, Tool> tools; // Stores all tools by name
		public Action? RefreshCanvasCallback { get; set; }
		public Tool ActiveTool { get; private set; }     // The currently active tool

		// Constructor to initialize the ToolManager
		public ToolManager() {
			tools = new Dictionary<string, Tool>();
			ActiveTool = null;
		}

		// Add a new tool to the manager
		public void AddTool(Tool tool) {
			if (!tools.ContainsKey(tool.Name)) {
				tools.Add(tool.Name, tool);
				tool.RefreshCanvasCallback = () => RefreshCanvasCallback?.Invoke();
			}
		}

		// Remove a tool by name
		public void RemoveTool(string toolName) {
			if (tools.ContainsKey(toolName)) {
				if (ActiveTool?.Name == toolName) {
					ActiveTool.Deactivate(); // Deactivate if it’s the active tool
					ActiveTool = null;
				}
				tools.Remove(toolName);
			}
		}

		// Activate a tool by its name
		public void ActivateTool(string toolName) {
			if (tools.ContainsKey(toolName)) {
				// Deactivate the currently active tool, if any
				ActiveTool?.Deactivate();

				// Set the new active tool and activate it
				ActiveTool = tools[toolName];
				ActiveTool.Activate();
			}
		}

		// Delegate mouse events to the active tool
		public void HandleMouseDown(MouseEventArgs e) {
			ActiveTool?.OnMouseDown(e);
		}

		public void HandleMouseMove(MouseEventArgs e) {
			ActiveTool?.OnMouseMove(e);
		}

		public void HandleMouseUp(MouseEventArgs e) {
			ActiveTool?.OnMouseUp(e);
		}

		// Handle key press events
		public void HandleKeyDown(KeyEventArgs e) {
			if (ActiveTool is IKeyInteractiveTool keyInteractiveTool) {
				keyInteractiveTool.OnKeyDown(e);
			}
		}

		// Handle key release events
		public void HandleKeyUp(KeyEventArgs e) {
			if (ActiveTool is IKeyInteractiveTool keyInteractiveTool) {
				keyInteractiveTool.OnKeyUp(e);
			}
		}

		internal void CommitTool() {
			if (ActiveTool is ICommitableTool commitableTool) {
				commitableTool.CommitTool();
			}
		}
		internal void ClearToolState() {
			if (ActiveTool is IBasicStateControlTool basicStateControlTool) {
				basicStateControlTool.ClearToolState();
			}
		}

		internal void SetBrushSize(int newBrushSize) {
			if (ActiveTool is BrushTool bTool) {
				bTool.BrushSize = newBrushSize;
			}
		}
	}
}