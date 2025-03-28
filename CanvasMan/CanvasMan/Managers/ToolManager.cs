using CanvasMan.Interfaces;
using CanvasMan.Tools;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasMan.Managers {
	public class ToolManager {
		private readonly Dictionary<string, Tool> tools; // Stores all tools by name
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
		public void HandleMouseDown(MouseEventArgs e, Graphics graphics) {
			ActiveTool?.OnMouseDown(e, graphics);
		}

		public void HandleMouseMove(MouseEventArgs e, Graphics graphics) {
			ActiveTool?.OnMouseMove(e, graphics);
		}

		public void HandleMouseUp(MouseEventArgs e, Graphics graphics) {
			ActiveTool?.OnMouseUp(e, graphics);
		}

		// Handle key press events
		public void HandleKeyDown(KeyEventArgs e, Graphics graphics) {
			if (ActiveTool is IKeyInteractiveTool keyInteractiveTool) {
				keyInteractiveTool.OnKeyDown(e, graphics);
			}
		}

		// Handle key release events
		public void HandleKeyUp(KeyEventArgs e, Graphics graphics) {
			if (ActiveTool is IKeyInteractiveTool keyInteractiveTool) {
				keyInteractiveTool.OnKeyUp(e, graphics);
			}
		}
	}
}