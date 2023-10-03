using System;
using System.Windows.Media;
using CG1.Extensions;
using SharpGL;

namespace CG1.Models.Primitives;

public class Point : IPrimitive
{
    private float X => _x + _tX;
    private float Y => _y + _tY;

    private float _x;
    private float _y;
    private float _tX;
    private float _tY;

    private float Size
    {
        get
        {
            var size = _size + _tSize;
            if (size < 0) size = 1;
            else if (size > 8) size = 8;
            return size;
        }
    }

    private float _size = 6;
    private float _tSize;

    private Color Color => _color.ChangeColor(_tA, _tR, _tG, _tB);

    private Color _color = Colors.Red;
    private short _tA;
    private short _tR;
    private short _tG;
    private short _tB;

    public Point() { }

    public Point(float x, float y)
    {
        _x = x;
        _y = y;
    }

    public Point(float x, float y, float size) : this(x, y)
    {
        _size = size;
    }

    public Point(float x, float y, Color color) : this(x, y)
    {
        _color = color;
    }

    public Point(float x, float y, float size, Color color) : this(x, y, size)
    {
        _color = color;
    }

    public void Draw(OpenGL gl, float size, Color color)
    {
        gl.PushMatrix();
        gl.Translate(X, Y, 0d);
        gl.Color(color.R, color.G, color.B, color.A);
        gl.PointSize(size);
        gl.Begin(OpenGL.GL_POINTS);
        gl.Vertex(0, 0, 0d);
        gl.End();
        gl.PopMatrix();
    }

    public void Draw(OpenGL gl)
    {
        Draw(gl, Size, Color);
    }

    public void Highlight(OpenGL gl)
    {
        var size = Size + 4;
        if (size > 8) size = 8f;
        Draw(gl, size, Colors.Black);
    }

    public void Move(float x, float y)
    {
        _tX += x;
        _tY += y;
    }

    public void SetPosition(float x, float y)
    {
        _x = X;
        _y = Y;
        _tX = _x - x;
        _tY = _y - y;
    }

    public void Rotate(float angle) { }

    public void SetColor(Color color)
    {
        _color = Color;
        _tA = (short)(_color.A - color.A);
        _tR = (short)(_color.R - color.R);
        _tG = (short)(_color.G - color.G);
        _tB = (short)(_color.B - color.B);
    }

    public Color GetColor()
    {
        return Color;
    }

    public void ChangeColor(short a, short r, short g, short b)
    {
        _tA += a;
        _tR += r;
        _tG += g;
        _tB += b;
    }

    public void MakeTransparent()
    {
        _color.ScA = 0.5f;
    }

    public void MakeNonTransparent()
    {
        _color.ScA = 1f;
    }

    public void SetSize(float size)
    {
        _size = Size;
        _tSize = _size - size;
    }

    public float GetSize()
    {
        return Size;
    }

    public void ChangeSize(float size)
    {
        _tSize += size;
    }

    public void CancelChanges()
    {
        _tX = 0f;
        _tY = 0f;
        _tSize = 0f;
        _tA = 0;
        _tR = 0;
        _tG = 0;
        _tB = 0;
    }

    public bool Contains(System.Windows.Point point)
    {
        var x = X;
        var y = Y;
        return x - Size <= point.X && y - Size <= point.Y && x + Size >= point.X && y + Size >= point.Y;
    }

    public IPrimitive Clone()
    {
        return new Point(X, Y, Size, Color);
    }

    public IPrimitive Copy(IPrimitive primitive)
    {
        if (typeof(Point) == primitive.GetType())
        {
            primitive.CancelChanges();
            primitive.SetPosition(X, Y);
            primitive.SetColor(Color);
            primitive.SetSize(Size);
        }
        else throw new InvalidCastException();
        
        return primitive;
    }
}