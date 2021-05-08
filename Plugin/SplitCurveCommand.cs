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

			//Point3d point;
			//Rhino.Input.RhinoGet.GetPoint("Enter the number of divisions for the closed curve", false, out point);
			//doc.Objects.AddPoint(point);
			//doc.Views.Redraw();
			int _int = 2;
			Rhino.Input.RhinoGet.GetInteger("Enter the number of cutters/lines needed. You will be prompted to draw that many lines across the curve to divide", false, ref _int);
			var curves = new List<Line>();
			for (int i = 0; i < _int; i++)
			{
				int layer_index;
				Layer layer = doc.Layers.FindName("Cutter");
				if (layer == null)
				{
					layer_index = doc.Layers.Add("Cutter", System.Drawing.Color.Blue);
					RhinoDoc.ActiveDoc.Layers.SetCurrentLayerIndex(layer_index, true);
				} else
                {
					layer_index = layer.Index;
					RhinoDoc.ActiveDoc.Layers.SetCurrentLayerIndex(layer_index, true);
				}
				
				Line line;
				Rhino.Input.RhinoGet.GetLine(out line);
				curves.Add(line);
				doc.Objects.AddLine(line);
				doc.Objects.UnselectAll();
			}
			doc.Views.Redraw();

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
				int layer_index;
				Layer layer = doc.Layers.FindName("SplitCurves");
				if (layer == null)
				{
					layer_index = doc.Layers.Add("SplitCurves", System.Drawing.Color.Red);
					RhinoDoc.ActiveDoc.Layers.SetCurrentLayerIndex(layer_index, true);
				}
				else
				{
					layer_index = layer.Index;
					RhinoDoc.ActiveDoc.Layers.SetCurrentLayerIndex(layer_index, true);
				}

				doc.Objects.AddCurve(split);
			}

			doc.Views.Redraw();

			return Result.Success;
		}
	}
}
