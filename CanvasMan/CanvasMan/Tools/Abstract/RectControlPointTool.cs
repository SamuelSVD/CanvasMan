using CanvasMan.Controls;
using CanvasMan.Managers;
using CanvasMan.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasMan.Tools.Abstract {
	public abstract class RectControlPointTool : ControlTool {
		protected ControlPoint startPoint = new ControlPoint();
		protected ControlPoint endPoint = new ControlPoint();
		protected ReferenceControlPoint topRightPoint;
		protected ReferenceControlPoint bottomLeftPoint;
		protected ControlLine topLine;
		protected ControlLine leftLine;
		protected ControlLine rightLine;
		protected ControlLine bottomLine;

		protected bool isDraggingTool = false;

		// Fields to track the start and current points during drawing
		protected Point currentPoint = Point.Empty;
		protected Point initialStartPoint = Point.Empty;
		protected Point initialEndPoint = Point.Empty;

		public RectControlPointTool(ColourManager colourManager, CanvasManager canvasManager, string name) : base(colourManager, canvasManager, name) {
			topRightPoint = new ReferenceControlPoint(endPoint, startPoint);
			bottomLeftPoint = new ReferenceControlPoint(startPoint, endPoint);
			topLine = new ControlLine(startPoint, topRightPoint);
			leftLine = new ControlLine(startPoint, bottomLeftPoint);
			rightLine = new ControlLine(endPoint, topRightPoint);
			bottomLine = new ControlLine(endPoint, bottomLeftPoint);
			ControlManager.Controls.Add(startPoint);
			ControlManager.Controls.Add(endPoint);
			ControlManager.Controls.Add(topRightPoint);
			ControlManager.Controls.Add(bottomLeftPoint);
			ControlManager.Controls.Add(topLine);
			ControlManager.Controls.Add(leftLine);
			ControlManager.Controls.Add(rightLine);
			ControlManager.Controls.Add(bottomLine);
		}
		public override void ClearToolState() {
			isDefiningTool = false;
			isDraggingTool = false;
			isToolDefined = false;
			isToolChanged = false;
			ControlManager.DeactivateAll();
		}
		public override void OnMouseMoveDefiningTool(MouseEventArgs e) {
			endPoint.SetLocation(e.Location);
			isToolChanged = true;
		}
		public override void OnMouseMoveToolDefined(MouseEventArgs e) {
			if (isDraggingTool) {
				// Move the entire arrow by offsetting both points
				int dx = e.Location.X - initialDragPoint.X;
				int dy = e.Location.Y - initialDragPoint.Y;
				startPoint.Offset(dx, dy);
				endPoint.Offset(dx, dy);
				isToolChanged = true;
			} else if (startPoint.IsActive) {
				// Move only the start point
				startPoint.SetLocation(e.Location);
				isToolChanged = true;
			} else if (endPoint.IsActive) {
				// Move only the end point
				endPoint.SetLocation(e.Location);
				isToolChanged = true;
			}
		}
		public abstract override void StartToolDefinition(MouseEventArgs e);
		public abstract override void EndToolDefinition(MouseEventArgs e);
		public abstract override void DrawCurrentState();
	}
}
