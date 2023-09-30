using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Markup;
using CG1.Core;
using CG1.Core.Primitives;

namespace CG1.Contexts;

public class PrimitivesGroupsContext
{
    public List<PrimitivesGroup> Groups { get; private set; }
    public PrimitivesGroup? SelectedGroup => SelectedGroupIndex < 0 ? null : _primitivesGroups[SelectedGroupIndex];
    public int SelectedGroupIndex { get; private set; }

    private readonly List<PrimitivesGroup> _primitivesGroups;

    public PrimitivesGroupsContext()
    {
        _primitivesGroups = new List<PrimitivesGroup>();
        SelectedGroupIndex = -1;
    }

    public void AddGroup()
    {
        _primitivesGroups.Add(new PrimitivesGroup());
        SelectedGroupIndex = _primitivesGroups.Count - 1;
    }

    public void AddPrimitive(IPrimitive primitive)
    {
        SelectedGroup.Add(primitive);
    }

    public void RemoveLastGroup()
    {
        if (_primitivesGroups.Count > 0)
        {
            _primitivesGroups.RemoveAt(_primitivesGroups.Count - 1);
        }
    }

    public void RemoveLastPrimitive()
    {
        
    }

    public void RemoveGroupAt(int index)
    {

    }

    public void RemovePrimitiveAt(int index)
    {

    }

    public void SelectGroup(int index)
    {

    }

    public void SelectNextGroup()
    {
        if (SelectedGroupIndex < _primitivesGroups.Count - 1)
        {
            SelectedGroupIndex++;
        }
    }

    public void SelectPreviousGroup()
    {
        if (SelectedGroupIndex > 0)
        {
            SelectedGroupIndex--;
        }
    }

    public void SelectPrimitive(int index)
    {

    }

    public void Unselect()
    {

    }

    public void SelectPrimitiveAtCursor(System.Windows.Point position)
    {

    }

    public void SelectGroupAtCursor(System.Windows.Point position)
    {
        
    }

    
}