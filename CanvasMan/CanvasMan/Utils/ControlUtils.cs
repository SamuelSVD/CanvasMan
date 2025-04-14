using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasMan.Utils {
	internal class ControlUtils {
		public static PointF PointF(Point p) {
			return new PointF(p.X, p.Y);
		}
		public static Point Point(PointF p) {
			return new Point((int)p.X, (int)p.Y);
		}
		public static bool IsNearPoint(Point click, Point target, double grabRadius) {
			return IsNearPoint(PointF(click), PointF(target), grabRadius);
		}
		public static bool IsNearPoint(PointF click, PointF target, double grabRadius) {
			return Math.Abs(click.X - target.X) <= grabRadius && Math.Abs(click.Y - target.Y) <= grabRadius;
		}

		public static bool IsNearLine(Point click, Point start, Point end, double grabRadius) {
			return IsNearLine(PointF(click), PointF(start), PointF(end), grabRadius);
		}
		public static bool IsNearLine(PointF click, PointF start, PointF end, double grabRadius) {
			// Check if the point is close to the line
			double lineMagnitude = DistanceFromPoint(start, end);
			if (DistanceFromPoint(click, start) >= lineMagnitude + grabRadius) return false;
			if (DistanceFromPoint(click, end) >= lineMagnitude + grabRadius) return false;
			double distance = DistanceFromLine(click, start, end);
			return distance < grabRadius;
		}
		public static double DistanceFromLine(Point p, Point a, Point b) {
			return DistanceFromLine(PointF(p), PointF(a), PointF(b));
		}
		public static double DistanceFromLine(PointF p, PointF a, PointF b) {
			double num = Math.Abs((b.Y - a.Y) * p.X - (b.X - a.X) * p.Y + b.X * a.Y - b.Y * a.X);
			double denom = Math.Sqrt(Math.Pow(b.Y - a.Y, 2) + Math.Pow(b.X - a.X, 2));
			return num / denom;
		}

		public static double DistanceFromPoint(Point click, Point target) {
			return DistanceFromPoint(PointF(click), PointF(target));
		}
		public static double DistanceFromPoint(PointF click, PointF target) {
			double magnitude = Math.Sqrt((click.X - target.X) * (click.X - target.X) + (click.Y - target.Y) * (click.Y - target.Y));
			return magnitude;
		}
	}
}
