using SharpGL;
using System.Windows.Media;

namespace CG1.Models.Primitives;

public interface IPrimitive
{
    public void Draw(OpenGL gl);
    public void Draw(OpenGL gl, float size, Color color);
    public void Highlight(OpenGL gl);
    public void Move(float x, float y);
    public void SetPosition(float x, float y);
    public void Rotate(float angle);
    public void SetColor(Color color);
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
    public IPrimitive Copy(IPrimitive primitive);
}