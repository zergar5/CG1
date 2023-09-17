using System;
using System.Windows;
using SharpGL;

namespace CG1.Core.Primitives;

//public class Triangle : IPrimitive
//{
//    private Point[] _points;

//    public Triangle()
//    {
//        _points = new Point[3];
//    }

//    public Triangle(Point point1, Point point2, Point point3)
//    {
//        _points = new[] { point1, point2, point3 };
//    }

//    public Triangle(Point[] points)
//    {
//        if(points.Length == 3) { _points = points; }
//    }

//    public void Draw(OpenGL gl)
//    {
//        throw new System.NotImplementedException();
//    }

//    public void Move(double x, double y)
//    {
//        foreach (var t in _points)
//        {
//            t.Move(x, y);
//        }
//    }

//    public void Rotate(OpenGL gl, double angle)
//    {
//        throw new System.NotImplementedException();
//    }

//    public bool Contains(System.Windows.Point point)
//    {
//        throw new NotImplementedException();
//    }
//}