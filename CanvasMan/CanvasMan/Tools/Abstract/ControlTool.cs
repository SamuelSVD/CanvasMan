using CanvasMan.Controls;
using CanvasMan.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CanvasMan.Tools.Abstract {
	public abstract class ControlTool : CanvasBitmapConsumerTool {
		protected ControlManager ControlManager = new ControlManager();
		public ControlTool(ColourManager colourManager, CanvasManager canvasManager, string name) : base(colourManager, canvasManager, name) {
		}
		public override void ClearToolState() {
			isDefiningTool = false;
			isToolDefined = false;
			isToolChanged = false;
			ControlManager.DeactivateAll();
		}
		public override void OnMouseDown(MouseEventArgs e) {
			if (isDefiningTool || ControlManager.IsActive()) {
				// If we were dragging the selection, this means someone released
				// the mouse press outside of the app
				OnMouseUp(e);
			} else if (isToolDefined) {
				// Check if the user clicked near an endpoint or on the arrow line
				if (ControlManager.OnMouseDown(e.Location, 5)) {
					return;
				}
				CommitTool();
				ClearToolState();

				// Initial press, start defining the 
				StartToolDefinition(e);
			} else if (!isDefiningTool && !ControlManager.IsActive()) {
				// Initial press, start defining the 
				StartToolDefinition(e);
				SaveCanvasBitmapState();
			}
		}
		private void UpdateCursor(Point mousePosition) {
			if (ControlManager.IsHovered(mousePosition, 5)) {
				isToolChanged = true;
				Cursor.Current = Cursors.SizeAll;
				return;
			}
			Cursor.Current = Cursors.Default;
		}
		public override void OnMouseMove(MouseEventArgs e) {
			if(isToolDefined) UpdateCursor(e.Location);

			if (ControlManager.OnMouseMove(e.Location)) {
				isToolChanged = true;
			}
			if (isDefiningTool) {
				OnMouseMoveDefiningTool(e);
				isToolChanged = true;
			}

			if (isDefiningTool || ControlManager.IsActive()) {
				OnMouseMoveToolDefined(e);
				DrawCurrentState();
			}
		}
		public override void OnMouseUp(MouseEventArgs e) {
			// Stop dragging operations
			if (isDefiningTool) {
				EndToolDefinition(e);
			}
			foreach (ControlBase point in ControlManager.Controls) {
				if (point.OnMouseUp(e.Location)) {
					isToolChanged = true;
					break;
				}
			}
		}
		public abstract override void StartToolDefinition(MouseEventArgs e);
		public abstract override void EndToolDefinition(MouseEventArgs e);
		public abstract override void DrawCurrentState();
		public abstract void OnMouseMoveDefiningTool(MouseEventArgs e);
		public abstract void OnMouseMoveToolDefined(MouseEventArgs e);
	}
}
