using CG1.Models.Primitives;
using System.Windows.Media;

namespace CG1.Models;

public class PrimitivesEditor
{
    public IPrimitive? EditingPrimitive => _editingPrimitive;
    public PrimitivesGroup? EditingGroup => _editingGroup;

    private IPrimitive? _editingPrimitive;
    private PrimitivesGroup? _editingGroup;

    public void StartEditing(IPrimitive primitive)
    {
        StopEditing();
        _editingPrimitive = primitive.Clone();
        primitive.MakeTransparent();
    }

    public void StartEditing(PrimitivesGroup primitivesGroup)
    {
        StopEditing();
        _editingGroup = primitivesGroup.Clone();
        primitivesGroup.MakeTransparent();
    }

    public void Move(float x, float y)
    {
        if (_editingGroup != null) EditingGroup.Move(x, y);
        else if (_editingPrimitive != null) EditingPrimitive.Move(x, y);
    }

    public void EditColor(short a, short r, short g, short b)
    {
        if (_editingGroup != null) EditingGroup.ChangeColor(a, r, g, b);
        else if (_editingPrimitive != null) EditingPrimitive.ChangeColor(a, r, g, b);
    }

    public void SetColor(Color color)
    {
        if (_editingGroup != null) EditingGroup.SetColor(color);
        else if (_editingPrimitive != null) EditingPrimitive.SetColor(color);
    }

    public void ChangeSize(float size)
    {
        if (_editingGroup != null) EditingGroup.ChangeSize(size);
        else if (_editingPrimitive != null) EditingPrimitive.ChangeSize(size);
    }

    public void CancelChanges()
    {
        StopEditing();
    }

    public void StopEditing()
    {
        _editingPrimitive = null;
        _editingGroup = null;
    }
}