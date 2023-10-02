using SharpGL;
using System.Windows.Media;

namespace CG1.Core.Primitives;

public class Point : IPrimitive
{
    private double X => _x + _tX;
    private double Y => _y + _tY;
    private readonly double _x;
    private readonly double _y;
    private double _tX;
    private double _tY;

    private float Size => _size + _tSize;
    private float _size = 10;
    private float _tSize;

    private Color Color => Color.FromArgb(
        (byte)(_color.A + _tA), (byte)(_color.R + _tR),
        (byte)(_color.G + _tG), (byte)(_color.B + _tB));


    private Color _color = Color.FromArgb(255, 255, 0, 0);
    private short _tA;
    private short _tR;
    private short _tG;
    private short _tB;

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
        gl.Color(Color.R, Color.G, Color.B, Color.A);
        gl.PointSize(Size);
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
        Draw(gl, Size + 4, Color.FromArgb(Color.A, 0, 0, 0));
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
        _tA = 0;
        _tR = 0;
        _tG = 0;
        _tB = 0;
    }

    public Color GetColor()
    {
        return Color;
    }

    public void ChangeColor(float a, float r, float g, float b)
    {
        _tA += a;
        _tR += r;
        _tG += g;
        _tB += b;
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
        _tSize = 0;
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
        _tX = 0d;
        _tY = 0d;
        _tSize = 0;
        _tA = 0;
        _tR = 0;
        _tG = 0;
        _tB = 0;
        _angle = 0d;
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
        return new Point(X, Y, Size, Color);
    }
}