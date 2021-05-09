using System.Collections.Generic;
using System.Drawing;
using System;

using Rhino.Geometry;
using Rhino;

using Xunit;

using SplitCurves.Lib;

namespace SplitCurves.Testing
{
    [Collection("Rhino Collection")]
    public class CurveSplitTest
    {
        [Fact]
        public void NewTest()
        {
            // Arrange
            Rectangle3d rect = new Rectangle3d(Plane.WorldXY, 5000, 1000);
            Curve Boundary = rect.ToNurbsCurve();

            List<Plane> splitPlanes = new List<Plane>();

            for (int i = 1; i < 5; i++)
            {
                Point3d planeOrigin = new Point3d(i * 500, 0, 0);
                Plane splitPlane = new Plane(planeOrigin, Vector3d.XAxis);
                splitPlanes.Add(splitPlane);
            }

            // Act
            List<Curve> splitCurves = Curves.DivideCurve(Boundary, splitPlanes);

            // Assert
            Assert.NotEmpty(splitPlanes); 

        }

    }
}