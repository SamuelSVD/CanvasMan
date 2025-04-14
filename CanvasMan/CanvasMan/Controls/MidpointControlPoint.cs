using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasMan.Controls {
	internal class MidpointControlPoint : ControlPoint {
		private ControlPoint reference1;
		private ControlPoint reference2;
		public override PointF LocationF {
			get {
				return new PointF(
					(reference1.LocationF.X + reference2.LocationF.X) / 2, 
					(reference1.LocationF.Y + reference2.LocationF.Y) / 2
				);
			}
		}
		public MidpointControlPoint(ControlPoint reference1, ControlPoint reference2) {
			this.reference1 = reference1;
			this.reference2 = reference2;
		}
	}
}
