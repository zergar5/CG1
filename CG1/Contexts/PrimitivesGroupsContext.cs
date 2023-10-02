using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Markup;
using CG1.Core;
using CG1.Core.Primitives;

namespace CG1.Contexts;

public class PrimitivesGroupsContext
{
    public List<PrimitivesGroup> Groups => _primitivesGroups;
    public PrimitivesGroup? SelectedGroup => SelectedGroupIndex < 0 ? null : _primitivesGroups[SelectedGroupIndex];
    public IPrimitive? SelectedPrimitive
    {
        get
        {
            if (SelectedGroup == null) return null;
            return SelectedPrimitiveIndex < 0 ? null : SelectedGroup[SelectedPrimitiveIndex];
        }
    }

    public int SelectedGroupIndex { get; private set; } = -1;
    public int SelectedPrimitiveIndex { get; private set; } = -1;

    private readonly List<PrimitivesGroup> _primitivesGroups = new();

    public void AddGroup()
    {
        _primitivesGroups.Add(new PrimitivesGroup());
        SelectedGroupIndex = _primitivesGroups.Count - 1;
    }

    public void RemoveGroupAt(int index)
    {
        if (index < 0 || index > _primitivesGroups.Count) return;
        _primitivesGroups.RemoveAt(index);
        Unselect();
    }

    public void RemoveLastGroup()
    {
        if (_primitivesGroups.Count <= 0) return;
        _primitivesGroups.RemoveAt(_primitivesGroups.Count - 1);
        Unselect();
    }

    public void SelectGroup(int index)
    {
        if (index < 0 || index > _primitivesGroups.Count) return;
        SelectedGroupIndex = index;
        SelectedPrimitiveIndex = SelectedGroup.Count;
    }

    public void SelectGroupAtCursor(System.Windows.Point position)
    {
        SelectGroup(_primitivesGroups.FindIndex(p => p.Contains(position)));
    }

    public void SelectNextGroup()
    {
        if (SelectedGroupIndex == -1)
        {
            SelectGroup(_primitivesGroups.Count - 1);
            SelectedPrimitiveIndex = SelectedGroup.Count;
            return;
        }
        if (SelectedGroupIndex >= _primitivesGroups.Count - 1) return;
        SelectedGroupIndex++;
        SelectedPrimitiveIndex = SelectedGroup.Count;
    }

    public void SelectPreviousGroup()
    {
        if (SelectedGroupIndex == -1)
        {
            SelectGroup(0);
            SelectedPrimitiveIndex = SelectedGroup.Count;
            return;
        }
        if (SelectedGroupIndex <= 1) return;
        SelectedGroupIndex--;
        SelectedPrimitiveIndex = SelectedGroup.Count;
    }

    public void AddPrimitive(IPrimitive primitive)
    {
        SelectedGroup.Add(primitive);
    }

    public void RemoveLastPrimitive()
    {
        if (SelectedGroupIndex < 0) return;
        if (SelectedGroup.Count == 0) return;
        SelectedGroup.RemoveAt(SelectedGroup.Count - 1);
        Unselect();
    }

    public void RemovePrimitiveAt(int index)
    {
        if (SelectedGroupIndex < 0) return;
        if (index < 0 || index > SelectedGroup.Count) return;
        SelectedGroup.RemoveAt(index);
        Unselect();
    }

    public void SelectPrimitive(int index)
    {
        if(SelectedGroupIndex < 0) return;
        if(index < 0 || index > SelectedGroup.Count) return;
        SelectedPrimitiveIndex = index;
    }

    public void SelectPrimitiveAtCursor(System.Windows.Point position)
    {
        SelectGroupAtCursor(position);
        if (SelectedGroupIndex > 0) SelectPrimitive(SelectedGroup.FindIndex(position));

    }

    public void SelectNextPrimitive()
    {
        if (SelectedGroupIndex <= 0) return;
        if (SelectedPrimitiveIndex == SelectedGroup.Count || SelectedPrimitiveIndex == -1)
        {
            SelectPrimitive(SelectedGroup.Count - 1);
            return;
        }
        if (SelectedPrimitiveIndex >= SelectedGroup.Count - 1) return;
        SelectedGroupIndex++;
    }

    public void SelectPreviousPrimitive()
    {
        if (SelectedGroupIndex <= 0) return;
        if (SelectedPrimitiveIndex == SelectedGroup.Count || SelectedPrimitiveIndex == -1)
        {
            SelectPrimitive(0);
            return;
        }
        if (SelectedPrimitiveIndex <= 1) return;
        SelectedPrimitiveIndex--;
    }

    public void Unselect()
    {
        SelectedGroupIndex = -1;
        SelectedPrimitiveIndex = -1;
    }
}