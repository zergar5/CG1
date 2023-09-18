using System;
using System.Windows.Forms;
using System.Windows.Media;
using SharpGL;

namespace CG1.Core.Primitives;

public class Point : IPrimitive
{
    public double X { get; }
    public double Y { get; }

    private float _size = 10;
    private Color _color = Color.FromArgb(255, 255, 0, 0);
    private double _tX;
    private double _tY;

    public Point() { }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }

    public Point(double x, double y, float size) : this(x, y)
    {
        _size = size;
    }

    public Point(double x, double y, Color color) : this(x, y)
    {
        _color = color;
    }

    public Point(double x, double y, float size, Color color) : this(x, y, size)
    {
        _color = color;
    }

    public void Draw(OpenGL gl)
    {
        gl.Color(_color.R, _color.G, _color.B, _color.A);
        gl.PointSize(_size);
        gl.Begin(OpenGL.GL_POINTS);
        gl.Vertex(X + _tX, Y + _tY, 0d);
        gl.End();
    }

    public void Draw(OpenGL gl, float size, Color color)
    {
        gl.Color(color.R, color.G, color.B, color.A);
        gl.PointSize(size);
        gl.Begin(OpenGL.GL_POINTS);
        gl.Vertex(X + _tX, Y + _tY, 0d);
        gl.End();
    }

    public void Highlight(OpenGL gl)
    {
        Draw(gl, _size + 4, Color.FromArgb(_color.A, 0, 0, 0));
    }

    public void Move(double x, double y)
    {
        _tX += x;
        _tY += y;
    }

    public void Rotate(OpenGL gl, double angle)
    {
        gl.Rotate(angle, 1d, 1d, 0d);
    }

    public void SetColor(byte a, byte r, byte g, byte b)
    {
        _color = Color.FromArgb(a, r, g, b);
    }

    public Color GetColor()
    {
        return _color;
    }

    public void ChangeColor(short a, short r, short g, short b)
    {
        _color.A = (byte)(_color.A + a);
        _color.R = (byte)(_color.R + r);
        _color.G = (byte)(_color.G + g);
        _color.B = (byte)(_color.B + b);
    }

    public void MakeTransparent()
    {
        _color.A = 122;
    }

    public void MakeNonTransparent()
    {
        _color.A = 255;
    }

    public void SetSize(float size)
    {
        _size = size;
    }

    public float GetSize()
    {
        return _size;
    }

    public void ChangeSize(float size)
    {
        _size += size;
    }

    public void Reset()
    {
        _tX = 0;
        _tY = 0;
        _size = 10;
        _color = Color.FromArgb(255, 255, 0, 0);
    }

    public bool Contains(System.Windows.Point point)
    {
        var x = X + _tX;
        var y = Y + _tY;
        return x - _size <= point.X && y - _size <= point.Y && x + _size >= point.X && y + _size >= point.Y;
    }

    public IPrimitive Clone()
    {
        return new Point(X + _tX, Y + _tY, _size, _color);
    }
}