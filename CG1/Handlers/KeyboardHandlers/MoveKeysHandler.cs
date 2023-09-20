using CG1.Core;
using CG1.Core.Primitives;
using SharpGL;

namespace CG1.Handlers.KeyboardHandlers;

public class MoveKeysHandler
{
    public PrimitivesGroup TemporaryGroup { private get; set; }
    public IPrimitive TemporaryPrimitive { private get; set; }
    private readonly OpenGL _gl;

    public MoveKeysHandler(OpenGL gl)
    {
        _gl = gl;
    }

    public void MovePrimitive(double tX, double tY)
    {
        TemporaryPrimitive.Move(tX, tY);
    }

    public void MovePrimitivesGroup(double tX, double tY)
    {
        TemporaryGroup.Move(tX, tY);
    }

    public void RotatePrimitive(double angle)
    {
        TemporaryPrimitive.Rotate(angle);
    }

    public void RotatePrimitivesGroup(double angle)
    {
        TemporaryGroup.Rotate(angle);
    }
}