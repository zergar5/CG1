using System;
using System.Windows.Forms;
using System.Windows.Media;
using SharpGL;

namespace CG1.Core.Primitives;

public class Point : IPrimitive
{
    private readonly double _x;
    private readonly double _y;
    private double _tX;
    private double _tY;

    private float _size = 10;
    private float _tSize;

    private Color _color = Color.FromArgb(255, 255, 0, 0);

    private double _angle;

    public Point() { }

    public Point(double x, double y)
    {
        _x = x;
        _y = y;
    }

    public Point(double x, double y, float size) : this(x, y)
    {
        _size = size;
    }

    public Point(double x, double y, Color color) : this(x, y)
    {
        _color = color;
    }

    public Point(double x, double y, double angle) : this(x, y)
    {
        _angle = angle;
    }

    public Point(double x, double y, float size, Color color) : this(x, y, size)
    {
        _color = color;
    }

    public Point(double x, double y, float size, double angle) : this(x, y, size)
    {
        _angle = angle;
    }

    public Point(double x, double y, Color color, double angle) : this(x, y, color)
    {
        _angle = angle;
    }

    public Point(double x, double y, float size, Color color, double angle) : this(x, y, size, color)
    {
        _angle = angle;
    }

    public void Draw(OpenGL gl)
    {
        gl.PushMatrix();
        gl.Translate(_x, _y, 0d);
        //gl.Rotate(_angle, 0d, 0d, 1d);
        gl.Color(_color.R, _color.G, _color.B, _color.A);
        gl.PointSize(_size);
        gl.Begin(OpenGL.GL_POINTS);
        gl.Vertex(_tX, _tY, 0d);
        gl.End();
        gl.PopMatrix();
    }

    public void Draw(OpenGL gl, float size, Color color)
    {
        gl.PushMatrix();
        gl.Translate(_x, _y, 0d);
        //gl.Rotate(_angle, 0d, 0d, 1d);
        gl.Color(color.R, color.G, color.B, color.A);
        gl.PointSize(size);
        gl.Begin(OpenGL.GL_POINTS);
        gl.Vertex(_tX, _tY, 0d);
        gl.End();
        gl.PopMatrix();
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

    public void Rotate(double angle)
    {
        _angle += angle;
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
        _tX = 0d;
        _tY = 0d;
        _size = 10f;
        _color = Color.FromArgb(255, 255, 0, 0);
        _angle = 0d;
    }

    public bool Contains(System.Windows.Point point)
    {
        var x = _x + _tX;
        var y = _y + _tY;
        return x - _size <= point.X && y - _size <= point.Y && x + _size >= point.X && y + _size >= point.Y;
    }

    public IPrimitive Clone()
    {
        return new Point(_x + _tX, _y + _tY, _size, _color);
    }
}