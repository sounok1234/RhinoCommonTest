using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;


namespace SplitCurves.Lib
{
	public static class Curves
	{
		public static List<Curve> DivideCurve(Curve boundary, List<Plane> planes)
		{
			var breps = Brep.CreatePlanarBreps(boundary, 0.01);

			var planeSurfaces = new List<Brep>();
			foreach (Plane plane in planes)
			{
				PlaneSurface surface = PlaneSurface.CreateThroughBox(plane, breps[0].GetBoundingBox(false));
				planeSurfaces.Add(surface.ToBrep());
			}
			var splits = breps[0].Split(planeSurfaces, 0.01);
			var edges = new List<Curve>();
			foreach (Brep split in splits)
            {
				try
                {
					var edge = split.Edges;
					var loop = Curve.JoinCurves(edge, 0.01);
					edges.Add(loop[0]);
                }
				catch
                {
					
                }
            }
			return edges;
		}

	}
}
