using SharpGL;
using System;
using System.Windows.Media;

namespace CG1.Core.Primitives;

public interface IPrimitive
{
    public void Draw(OpenGL gl);
    public void Draw(OpenGL gl, float size, Color color);
    public void Highlight(OpenGL gl);
    public void Move(double x, double y);
    public void Rotate(double angle);
    public void SetColor(byte a, byte r, byte g, byte b);
    public Color GetColor();
    public void ChangeColor(short a, short r, short g, short b);
    public void MakeTransparent();
    public void MakeNonTransparent();
    public void SetSize(float size);
    public float GetSize();
    public void ChangeSize(float size);
    public void CancelChanges();
    public bool Contains(System.Windows.Point point);
    public IPrimitive Clone();
}