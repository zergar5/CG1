using System.Diagnostics.Eventing.Reader;
using System.Windows.Media;
using CG1.Core;
using CG1.Core.Primitives;

namespace CG1.Tools;

public class PrimitivesEditor
{
    public IPrimitive EditingPrimitive => _editingPrimitive;
    public PrimitivesGroup EditingGroup => _editingGroup;

    private IPrimitive? _editingPrimitive;
    private PrimitivesGroup? _editingGroup;

    public PrimitivesEditor()
    {
        _editingGroup = new PrimitivesGroup();
    }

    public void StartEditing(IPrimitive primitive)
    {
        _editingPrimitive = primitive.Clone();
    }

    public void StartEditing(PrimitivesGroup primitivesGroup)
    {
        _editingGroup = primitivesGroup.Copy(primitivesGroup);
    }

    public void StopEditing()
    {
        _editingPrimitive = null;
        _editingGroup = null;
    }

    public void CancelChanges()
    {
        if (_editingGroup != null) EditingGroup.;
        else if (_editingPrimitive != null) EditingPrimitive.Move(x, y);
    }

    public void ReturnChanges()
    {
        ClearEditing();
        if (_editingGroup != null) EditingGroup.;
        else if (_editingPrimitive != null) EditingPrimitive.Move(x, y);
    }

    public void Move(double x, double y)
    {
        if(_editingGroup != null) EditingGroup.Move(x, y);
        else if(_editingPrimitive != null) EditingPrimitive.Move(x, y);
    }

    public void EditColor(float a, float r, float g, float b)
    {
        if (_editingGroup != null) EditingGroup.ChangeColor(a, r, g, b);
        else if (_editingPrimitive != null) EditingPrimitive.ChangeColor(a, r, g, b);
    }

    public void ChangeSize(float size)
    {
        if (_editingGroup != null) EditingGroup.ChangeSize(size);
        else if (_editingPrimitive != null) EditingPrimitive.ChangeSize(size);
    }

    private void ClearEditing()
    {
        _editingPrimitive = null;
        _editingGroup = null;
    }
}