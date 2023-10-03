using CG1.Models.Primitives;

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
    }

    public void StartEditing(PrimitivesGroup primitivesGroup)
    {
        StopEditing();
        _editingGroup = primitivesGroup.Clone();
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

    public void ChangeSize(float size)
    {
        if (_editingGroup != null) EditingGroup.ChangeSize(size);
        else if (_editingPrimitive != null) EditingPrimitive.ChangeSize(size);
    }

    public void AcceptChanges(IPrimitive primitive)
    {
        primitive = _editingPrimitive;
        StopEditing();
        StartEditing(primitive);
    }

    public void AcceptChanges(PrimitivesGroup primitivesGroup)
    {
        primitivesGroup = _editingGroup;
        StopEditing();
        StartEditing(primitivesGroup);
    }

    public void CancelChanges()
    {
        if (_editingGroup != null) EditingGroup.CancelChanges();
        else if (_editingPrimitive != null) EditingPrimitive.CancelChanges();
    }

    public void StopEditing()
    {
        _editingPrimitive = null;
        _editingGroup = null;
    }
}