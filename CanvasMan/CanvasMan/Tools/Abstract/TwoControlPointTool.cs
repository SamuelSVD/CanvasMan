using CanvasMan.Controls;
using CanvasMan.Managers;

namespace CanvasMan.Tools.Abstract {
	public abstract class TwoControlPointTool : ControlTool {
		protected ControlPoint startPoint = new ControlPoint();
		protected ControlPoint endPoint = new ControlPoint();
		protected ControlLine controlLine;
		public TwoControlPointTool(ColourManager colourManager, CanvasManager canvasManager, string name) : base(colourManager, canvasManager, name) {
			controlLine = new ControlLine(startPoint, endPoint);
			ControlManager.Controls.Add(startPoint);
			ControlManager.Controls.Add(endPoint);
			ControlManager.Controls.Add(controlLine);
		}

		public override void StartToolDefinition(MouseEventArgs e) {
			isDefiningTool = true;
			startPoint.SetLocation(e.Location);
		}
		public override void EndToolDefinition(MouseEventArgs e) {
			endPoint.SetLocation(new Point(e.Location.X, e.Location.Y));
			isDefiningTool = false;
			controlLine.IsActive = false;
			isToolDefined = true;
			isToolChanged = true;
		}
		public override void OnMouseMoveDefiningTool(MouseEventArgs e) {
			endPoint.SetLocation(e.Location);
		}
		public override void OnMouseMoveToolDefined(MouseEventArgs e) {
		}
	}
}
