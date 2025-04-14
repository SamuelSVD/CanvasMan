using CanvasMan.Controls;
using CanvasMan.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasMan.Managers {
	public class ControlManager : ISelectableItem {
		public List<ControlBase> Controls;
		public ControlManager() {
			Controls = new List<ControlBase>();
		}
		public void DeactivateAll() {
			foreach (ControlBase control in Controls) {
				control.IsActive = false;
			}
		}
		public bool IsActive() {
			foreach (ControlBase control in Controls) {
				if (control.IsActive) {
					return true;
				}
			}
			return false;
		}

		public void Draw(Graphics graphics) {
			foreach (ControlBase control in Controls) {
				control.Draw(graphics);
			}
		}

		public bool IsHovered(Point mouseLocation, double hoverRadius) {
			foreach (ControlBase control in Controls) {
				if (control.IsHovered(mouseLocation, hoverRadius)) {
					return true;
				}
			}
			return false;
		}

		public bool OnMouseDown(Point mouseLocation, double hoverRadius) {
			foreach (ControlBase control in Controls) {
				if (control.OnMouseDown(mouseLocation, hoverRadius)) {
					return true;
				}
			}
			return false;
		}

		public bool OnMouseMove(Point mouseLocation) {
			foreach (ControlBase control in Controls) {
				if (control.OnMouseMove(mouseLocation)) {
					return true;
				}
			}
			return false;
		}

		public bool OnMouseUp(Point mouseLocation) {
			foreach (ControlBase control in Controls) {
				if (control.OnMouseUp(mouseLocation)) {
					return true;
				}
			}
			return false;
		}
	}
}
