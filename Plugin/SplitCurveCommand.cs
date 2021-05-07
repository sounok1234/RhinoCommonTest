using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;


namespace SplitCurves.Plugin
{
	public class SplitCurveCommand : Command
	{
		public SplitCurveCommand()
		{
			// Rhino only creates one instance of each command class defined in a
			// plug-in, so it is safe to store a refence in a static property.
			Instance = this;
			
		}

		///<summary>The only instance of this command.</summary>
		public static SplitCurveCommand Instance { get; private set; }

		///<returns>The command name as it appears on the Rhino command line.</returns>
		public override string EnglishName
		{
			get { return "SplitCurve"; }
		}

		protected override Result RunCommand(RhinoDoc doc, RunMode mode)
		{
			var go = new Rhino.Input.Custom.GetObject();
			go.SetCommandPrompt("Select closed curve to divide");
			go.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;
			go.GetMultiple(1, 1);
			var closedCurve = go.Object(0).Curve();
			doc.Objects.UnselectAll();
			/*
			var go2 = new Rhino.Input.Custom.GetObject();
			go2.SetCommandPrompt("Select curves to divide closed curve");
			go2.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;
			go2.GetMultiple(1, 1000000);
			var curves = new List<Curve>();
			for (int i = 0; i < go2.ObjectCount; i++)
			{
				curves.Add(go2.Object(i).Curve());
			}
			*/

			//Point3d point;
			//Rhino.Input.RhinoGet.GetPoint("Enter the number of divisions for the closed curve", false, out point);
			//doc.Objects.AddPoint(point);
			//doc.Views.Redraw();
			Line line;
			Rhino.Input.RhinoGet.GetLine(out line); 
			doc.Objects.UnselectAll();
			Line line2;
			Rhino.Input.RhinoGet.GetLine(out line2);
			var curves = new List<Line>();
			curves.Add(line);
			curves.Add(line2);

			const double intersection_tolerance = 0.001;
			const double overlap_tolerance = 0.0;

			var parameters = new List<double>();
			foreach (Line curve in curves)
			{
				var events = Rhino.Geometry.Intersect.Intersection.CurveLine(closedCurve, curve, intersection_tolerance, overlap_tolerance);
				if (events != null)
				{
					for (int i = 0; i < events.Count; i++)
					{
						var ccx_event = events[i];
						if (curve.DistanceTo(ccx_event.PointA, true) < 0.01)
                        {
							parameters.Add(ccx_event.ParameterA);
						}
						
					}
				}
			}
			parameters.Sort();
			var splitCurves = closedCurve.Split(parameters);

			foreach (Curve split in splitCurves)
			{
				doc.Objects.AddCurve(split);
			}
			//doc.Objects.Delete(go);
			doc.Views.Redraw();

			return Result.Success;
		}
	}
}
