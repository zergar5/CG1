using CG1.Core;
using CG1.Core.Primitives;
using System.Collections.Generic;
using System.Windows.Media;

namespace CG1.Handlers.MouseHandlers;

public class MouseClicksHandler
{
    private readonly List<PrimitivesGroup> _primitivesGroups;

    public MouseClicksHandler(List<PrimitivesGroup> primitivesGroups)
    {
        _primitivesGroups = primitivesGroups;
    }

    public int AddGroup()
    {
        _primitivesGroups.Add(new PrimitivesGroup());

        return _primitivesGroups.Count - 1;
    }

    public int AddPrimitive(System.Windows.Point position, int currentGroupIndex)
    {
        _primitivesGroups[currentGroupIndex].Add(new Point(position.X, position.Y, 10, Color.FromArgb(255, 255, 0, 0)));

        return _primitivesGroups[currentGroupIndex].Count;
    }

    public (int, int) FindPrimitive(System.Windows.Point point)
    {
        var currentGroup = _primitivesGroups.FindIndex(p => p.Contains(point));
        if (currentGroup == -1) return (-1, -1);
        var currentPrimitive = _primitivesGroups[currentGroup].FindIndex(point);

        return (currentGroup, currentPrimitive);
    }

    public int FindPrimitivesGroup(System.Windows.Point point)
    {
        return _primitivesGroups.FindIndex(p => p.Contains(point));
    }
}