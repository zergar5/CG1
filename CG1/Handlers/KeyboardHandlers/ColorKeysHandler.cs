using CG1.Core;
using CG1.Core.Primitives;

namespace CG1.Handlers.KeyboardHandlers;

public class ColorKeysHandler
{
    public PrimitivesGroup TemporaryGroup { private get; set; }
    public IPrimitive TemporaryPrimitive { private get; set; }

    public void ChangePrimitiveColor(short a, short r, short g, short b)
    {
        TemporaryPrimitive.ChangeColor(a, r, g, b);
    }

    public void ChangeGroupColor(short a, short r, short g, short b)
    {
        TemporaryGroup.ChangeColor(a, r, g, b);
    }
}