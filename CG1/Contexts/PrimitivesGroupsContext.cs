using CG1.Models;
using CG1.Models.Primitives;
using System.Collections.Generic;
using System.Linq;
using CG1.ViewModels;
using System.Windows.Forms;

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
            if (SelectedPrimitiveIndex < 0 || SelectedPrimitiveIndex >= SelectedGroup.Count) return null;
            return SelectedGroup[SelectedPrimitiveIndex];
        }
    }

    public int SelectedGroupIndex { get; private set; } = -1;
    public int SelectedPrimitiveIndex { get; private set; } = -1;

    private readonly List<PrimitivesGroup> _primitivesGroups = new();
    private readonly PrimitivesGroupsContextView _contextView;

    public PrimitivesGroupsContext(PrimitivesGroupsContextView contextView)
    {
        _contextView = contextView;
    }

    public void AddGroup()
    {
        _primitivesGroups.Add(new PrimitivesGroup());
        SelectedGroupIndex = _primitivesGroups.Count - 1;
        _contextView.Views.Add(new PrimitivesGroupView(SelectedGroup, SelectedGroupIndex));
        _contextView.SelectedGroupIndex = SelectedGroupIndex;
        SelectedPrimitiveIndex = -1;
    }

    public void RemoveGroupAt(int index)
    {
        if (index < 0 || index > _primitivesGroups.Count) return;
        _contextView.Views.RemoveAt(index);
        foreach (var group in _contextView.Views.Skip(index))
        {
            group.Index--;
        }
        _primitivesGroups.RemoveAt(index);
        Unselect();
    }

    public void RemoveLastGroup()
    {
        if (_primitivesGroups.Count <= 0) return;
        _primitivesGroups.RemoveAt(_primitivesGroups.Count - 1);
        _contextView.Views.RemoveAt(_primitivesGroups.Count - 1);
        Unselect();
    }

    public void SelectGroup(int index)
    {
        if (index < 0 || index > _primitivesGroups.Count) return;
        MakeNonTransparent();
        SelectedGroupIndex = index;
        _contextView.SelectedGroupIndex = index;
        SelectedPrimitiveIndex = SelectedGroup.Count;
    }

    public void SelectGroupAtCursor(System.Windows.Point position)
    {
        MakeNonTransparent();
        SelectGroup(_primitivesGroups.FindIndex(p => p.Contains(position)));
    }

    public void SelectNextGroup()
    {
        if (SelectedGroupIndex == -1)
        {
            if (_primitivesGroups.Count > 0)
            {
                SelectGroup(_primitivesGroups.Count - 1);
                SelectedPrimitiveIndex = SelectedGroup.Count;
                return;
            }
        }
        MakeNonTransparent();
        if (SelectedGroupIndex >= _primitivesGroups.Count - 1) return;
        SelectedGroupIndex++;
        _contextView.SelectedGroupIndex++;
        SelectedPrimitiveIndex = SelectedGroup.Count;
    }

    public void SelectPreviousGroup()
    {
        if (SelectedGroupIndex == -1)
        {
            if (_primitivesGroups.Count > 0)
            {
                SelectGroup(0);
                SelectedPrimitiveIndex = SelectedGroup.Count;
                return;
            }
        }
        MakeNonTransparent();
        if (SelectedGroupIndex <= 0) return;
        SelectedGroupIndex--;
        _contextView.SelectedGroupIndex--;
        SelectedPrimitiveIndex = SelectedGroup.Count;
    }

    public void AddPrimitive(IPrimitive primitive)
    {
        SelectedGroup.Add(primitive);
        SelectedPrimitiveIndex = SelectedGroup.Count;
        _contextView.SelectedInTableGroup.PrimitivesCount++;
    }

    public void RemoveLastPrimitive()
    {
        if (SelectedGroupIndex < 0) return;
        if (SelectedGroup.Count == 0) return;
        SelectedGroup.RemoveAt(SelectedGroup.Count - 1);
        _contextView.SelectedInTableGroup.PrimitivesCount--;
        Unselect();
    }

    public void RemovePrimitiveAt(int index)
    {
        if (SelectedGroupIndex < 0) return;
        if (index < 0 || index > SelectedGroup.Count) return;
        SelectedGroup.RemoveAt(index);
        _contextView.SelectedInTableGroup.PrimitivesCount--;
        Unselect();
    }

    public void SelectPrimitive(int index)
    {
        if (SelectedGroupIndex < 0) return;
        if (index < 0 || index > SelectedGroup.Count) return;
        MakeNonTransparent();
        SelectedPrimitiveIndex = index;
    }

    public void SelectPrimitiveAtCursor(System.Windows.Point position)
    {
        SelectGroupAtCursor(position);
        if (SelectedGroupIndex >= 0) SelectPrimitive(SelectedGroup.FindIndex(position));
    }

    public void SelectNextPrimitive()
    {
        if (SelectedGroupIndex < 0) return;
        if (SelectedPrimitiveIndex == SelectedGroup.Count || SelectedPrimitiveIndex == -1)
        {
            SelectPrimitive(SelectedGroup.Count - 1);
            return;
        }
        MakeNonTransparent();
        if (SelectedPrimitiveIndex >= SelectedGroup.Count - 1) return;
        SelectedPrimitiveIndex++;
    }

    public void SelectPreviousPrimitive()
    {
        if (SelectedGroupIndex < 0) return;
        if (SelectedPrimitiveIndex == SelectedGroup.Count || SelectedPrimitiveIndex == -1)
        {
            SelectPrimitive(0);
            return;
        }
        MakeNonTransparent();
        if (SelectedPrimitiveIndex <= 0) return;
        SelectedPrimitiveIndex--;
    }

    public void Unselect()
    {
        MakeNonTransparent();
        SelectedGroupIndex = -1;
        SelectedPrimitiveIndex = -1;
        _contextView.SelectedGroupIndex = -1;
    }

    public void CancelChanges()
    {
        MakeNonTransparent();
        if (SelectedGroupIndex == -1) return;
        if (SelectedPrimitiveIndex == SelectedGroup.Count)
        {
            SelectedGroup.CancelChanges();
        }
        else if (SelectedPrimitiveIndex != -1)
        {
            SelectedPrimitive.CancelChanges();
        }
    }

    private void MakeNonTransparent()
    {
        if (SelectedGroupIndex == -1) return;
        if (SelectedPrimitiveIndex == SelectedGroup.Count)
        {
            SelectedGroup.MakeNonTransparent();
        }
        else if (SelectedPrimitiveIndex != -1)
        {
            SelectedPrimitive.MakeNonTransparent();
        }
    }
}