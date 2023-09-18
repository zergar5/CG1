using CG1.Core.Primitives;
using CG1.Core;

namespace CG1.Handlers.KeyboardHandlers;

public class SizeKeysHandler
{
    public PrimitivesGroup TemporaryGroup { private get; set; }
    public IPrimitive TemporaryPrimitive { private get; set; }

    public void ChangePrimitiveSize(float size)
    {
        TemporaryPrimitive.ChangeSize(size);
    }

    public void ChangeGroupSize(float size)
    {
        TemporaryGroup.ChangeSize(size);
    }
}