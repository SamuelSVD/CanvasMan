using CanvasMan.Interfaces;
using CanvasMan.Managers;
using CanvasMan.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasMan.Controls {
	public abstract class ControlBase : ISelectableItem {
		protected Point initialDragPoint { get; set; } = Point.Empty;
		public Boolean IsActive { get; set; } = false;
		public Action<MovementDelta>? OnMovedCallback { get; set; }
		public void Offset(float x, float y) {
			OnOffset(x, y);
			if (IsActive) {
				OnMovedCallback?.Invoke(new MovementDelta(x, y));
			}
		}
		public abstract void Draw(Graphics graphics);

		public abstract bool IsHovered(Point mouseLocation, double hoverRadius);

		public abstract bool OnMouseDown(Point mouseLocation, double hoverRadius);

		public abstract bool OnMouseMove(Point mouseLocation);

		public abstract bool OnMouseUp(Point mouseLocation);
		protected abstract void OnOffset(float x, float y);
	}
}
