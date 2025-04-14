using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasMan.Controls {
	public class ReferenceControlPoint : ControlPoint {
		private ControlPoint referenceX;
		private ControlPoint referenceY;
		public override PointF LocationF {
			get {
				return new PointF(referenceX.Location.X, referenceY.Location.Y);
			}
		}
		public ReferenceControlPoint(ControlPoint referenceX, ControlPoint referenceY) {
			this.referenceX = referenceX;
			this.referenceY = referenceY;
		}
	}
}
