using CG1.Core;
using CG1.Core.Primitives;
using SharpGL;
using System.Collections.Generic;

namespace CG1.Handlers.KeyboardHandlers;

public class MoveKeysHandler
{
    public PrimitivesGroup TemporaryGroup { get; private set; }
    public IPrimitive TemporaryPrimitive { get; private set; }
    public int TemporaryGroupIndex { get; private set; } = -1;
    public int TemporaryPrimitiveIndex { get; private set; } = -1;

    private readonly OpenGL _gl;

    public MoveKeysHandler(OpenGL gl)
    {
        _gl = gl;
    }

    public void MovePrimitive(IPrimitive primitive, int selectedPrimitiveIndex, double tX, double tY)
    {
        if (TemporaryPrimitiveIndex != selectedPrimitiveIndex)
        {
            TemporaryPrimitive = primitive.Clone();
            var color = TemporaryPrimitive.GetColor();
            TemporaryPrimitive.SetColor((byte)(color.A / 2), color.R, color.G, color.B);
            TemporaryPrimitiveIndex = selectedPrimitiveIndex;
        }
        primitive.Move(tX, tY);
    }

    public void MovePrimitivesGroup(PrimitivesGroup primitivesGroup, int selectedGroupIndex, double tX, double tY)
    {
        if (TemporaryGroupIndex != selectedGroupIndex)
        {
            TemporaryGroup = primitivesGroup.Clone();
            TemporaryGroupIndex = selectedGroupIndex;
        }
        primitivesGroup.Move(tX, tY);
    }

    public void SetTemporaryGroup
}