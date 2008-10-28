// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Windows.Controls.DataVisualization
{
#if SILVERLIGHT
    /// <summary>
    /// A collection of functions for manipulating geometry objects.
    /// </summary>
    internal static class GeometryFunctions
    {
        /////// <summary>
        /////// Clones a PathSegment object.
        /////// </summary>
        /////// <param name="segment">The path segment object to clone.</param>
        /////// <returns>The cloned path segment object.</returns>
        ////private static PathSegment Clone(this PathSegment segment)
        ////{
        ////    // Attempt to clone line segment
        ////    LineSegment lineSegment = segment as LineSegment;
        ////    if (lineSegment != null)
        ////    {
        ////        LineSegment outputLineSegment =
        ////            new LineSegment
        ////            {
        ////                Point = lineSegment.Point
        ////            };
        ////        return outputLineSegment;
        ////    }

        ////    // Attempt to clone arc segment
        ////    ArcSegment arcSegment = segment as ArcSegment;
        ////    if (arcSegment != null)
        ////    {
        ////        ArcSegment outputArcSegment =
        ////            new ArcSegment
        ////            {
        ////                Point = arcSegment.Point,
        ////                IsLargeArc = arcSegment.IsLargeArc,
        ////                RotationAngle = arcSegment.RotationAngle,
        ////                Size = arcSegment.Size,
        ////                SweepDirection = arcSegment.SweepDirection
        ////            };
        ////        return outputArcSegment;
        ////    }

        ////    // Attempt to clone bezier segment
        ////    BezierSegment bezierSegment = segment as BezierSegment;
        ////    if (bezierSegment != null)
        ////    {
        ////        BezierSegment outputBezierSegment =
        ////            new BezierSegment
        ////            {
        ////                Point1 = bezierSegment.Point1,
        ////                Point2 = bezierSegment.Point2,
        ////                Point3 = bezierSegment.Point3
        ////            };
        ////        return outputBezierSegment;
        ////    }

        ////    // Attempt to clone poly bezier segment
        ////    PolyBezierSegment polyBezierSegment = segment as PolyBezierSegment;
        ////    if (polyBezierSegment != null)
        ////    {
        ////        PolyBezierSegment outputPolyBezierSegment = new PolyBezierSegment();
        ////        foreach (Point point in polyBezierSegment.Points)
        ////        {
        ////            outputPolyBezierSegment.Points.Add(point);
        ////        }
        ////        return outputPolyBezierSegment;
        ////    }

        ////    // Attempt to clone poly line segment
        ////    PolyLineSegment polyLineSegment = segment as PolyLineSegment;
        ////    if (polyLineSegment != null)
        ////    {
        ////        PolyLineSegment outputPolyLineSegment = new PolyLineSegment();
        ////        foreach (Point point in polyLineSegment.Points)
        ////        {
        ////            outputPolyLineSegment.Points.Add(point);
        ////        }
        ////        return outputPolyLineSegment;
        ////    }

        ////    // Attempt to clone quadratic bezier segment
        ////    QuadraticBezierSegment quadraticBezierSegment = segment as QuadraticBezierSegment;
        ////    if (quadraticBezierSegment != null)
        ////    {
        ////        QuadraticBezierSegment outputQuadraticBezierSegment =
        ////            new QuadraticBezierSegment
        ////            {
        ////                Point1 = quadraticBezierSegment.Point1,
        ////                Point2 = quadraticBezierSegment.Point2
        ////            };
        ////        return outputQuadraticBezierSegment;
        ////    }

        ////    throw new NotImplementedException();
        ////}

        /////// <summary>
        /////// Clones a Geometry object.
        /////// </summary>
        /////// <param name="geometry">The geometry object to clone.</param>
        /////// <returns>The cloned geometry object.</returns>
        ////public static Geometry Clone(this Geometry geometry)
        ////{
        ////    // Attempt to clone rectangle geometry
        ////    RectangleGeometry rectangleGeometry = geometry as RectangleGeometry;
        ////    if (rectangleGeometry != null)
        ////    {
        ////        RectangleGeometry outputRectangleGeometry =
        ////            new RectangleGeometry
        ////            {
        ////                Rect = rectangleGeometry.Rect,
        ////                RadiusX = rectangleGeometry.RadiusX,
        ////                RadiusY = rectangleGeometry.RadiusY
        ////            };
        ////        return outputRectangleGeometry;
        ////    }

        ////    // Attempt to clone ellipse geometry
        ////    EllipseGeometry ellipseGeometry = geometry as EllipseGeometry;
        ////    if (ellipseGeometry != null)
        ////    {
        ////        EllipseGeometry outputEllipseGeometry =
        ////            new EllipseGeometry
        ////            {
        ////                Center = ellipseGeometry.Center,
        ////                RadiusX = ellipseGeometry.RadiusX,
        ////                RadiusY = ellipseGeometry.RadiusY,
        ////                Transform = ellipseGeometry.Transform
        ////            };
        ////        return outputEllipseGeometry;
        ////    }

        ////    // Attempt to clone geometry group
        ////    GeometryGroup groupGeometry = geometry as GeometryGroup;
        ////    if (groupGeometry != null)
        ////    {
        ////        GeometryGroup outputGeometryGroup =
        ////            new GeometryGroup
        ////            {
        ////                FillRule = groupGeometry.FillRule,
        ////                Transform = groupGeometry.Transform
        ////            };
        ////        foreach (Geometry child in groupGeometry.Children)
        ////        {
        ////            outputGeometryGroup.Children.Add(child.Clone());
        ////        }

        ////        return outputGeometryGroup;
        ////    }

        ////    // Attempt to clone line geometry
        ////    LineGeometry lineGeometry = geometry as LineGeometry;
        ////    if (lineGeometry != null)
        ////    {
        ////        LineGeometry outputLineGeometry =
        ////            new LineGeometry
        ////            {
        ////                StartPoint = lineGeometry.StartPoint,
        ////                EndPoint = lineGeometry.EndPoint
        ////            };

        ////        return outputLineGeometry;
        ////    }

        ////    // Attempt to clone path geometry
        ////    PathGeometry pathGeometry = geometry as PathGeometry;
        ////    if (pathGeometry != null)
        ////    {
        ////        PathGeometry outputPathGeometry =
        ////            new PathGeometry
        ////            {
        ////                FillRule = pathGeometry.FillRule,
        ////                Transform = pathGeometry.Transform
        ////            };

        ////        foreach (PathFigure pathFigure in pathGeometry.Figures)
        ////        {
        ////            PathFigure outputPathFigure =
        ////            new PathFigure
        ////            {
        ////                IsClosed = pathFigure.IsClosed,
        ////                IsFilled = pathFigure.IsFilled,
        ////                StartPoint = pathFigure.StartPoint
        ////            };

        ////            foreach (PathSegment pathSegment in pathFigure.Segments)
        ////            {
        ////                outputPathFigure.Segments.Add(pathSegment.Clone());
        ////            }

        ////            outputPathGeometry.Figures.Add(outputPathFigure);
        ////        }

        ////        return outputPathGeometry;
        ////    }

        ////    throw new NotImplementedException();
        ////}
    }
#endif
}
