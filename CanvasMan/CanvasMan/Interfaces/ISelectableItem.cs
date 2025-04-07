using CanvasMan.Utils;

namespace CanvasMan.Interfaces {
	public interface ISelectableItem {
		bool IsHovered(Point mouseLocation, double hoverRadius); // Check if hovered
		bool OnMouseDown(Point mouseLocation, double hoverRadius);               // Handle mouse down
		bool OnMouseMove(Point mouseLocation);               // Handle dragging
		bool OnMouseUp(Point mouseLocation);                 // Handle mouse up
		void Draw(Graphics graphics);                        // Render the item
	}
}
