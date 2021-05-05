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
			var go = new Rhino.Input.Custom.GetObject();
			//go.SetCommandPrompt("Select the planes"); 
			go.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;

			foreach (Plane plane in planes)
			{
				CurveIntersections events = Intersection.CurvePlane(boundary, plane, 0.01);
				
				if (events != null) 
				{
					var parameters = new List<double>();
					for (int i = 0; i < events.Count; i++)
					{
						var ccx_event = events[i];
						double para = ccx_event.ParameterA;
						parameters.Add(para);
					}
				}
			}
			return null;
		}

	}
}
