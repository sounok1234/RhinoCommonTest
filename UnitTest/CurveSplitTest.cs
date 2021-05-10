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
            List<Plane> planes = new List<Plane>();

            Rectangle3d rect = new Rectangle3d(Plane.WorldXY, 5000, 1000);
            Curve boundary = rect.ToNurbsCurve();

            // Act + Assert
            Exception ex = Assert.Throws<ArgumentException>("planes", () => Curves.DivideCurve(boundary, planes));
            Assert.Contains("atleast one", ex.Message);

        }

    }
}