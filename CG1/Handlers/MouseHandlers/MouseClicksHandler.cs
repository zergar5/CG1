using System.Collections.Generic;
using System.Windows.Media;
using CG1.Core;
using CG1.Core.Primitives;

namespace CG1.Handlers.MouseHandlers;

public class MouseClicksHandler
{
    private readonly List<PrimitivesGroup> _primitivesGroups;

    public MouseClicksHandler(List<PrimitivesGroup> primitivesGroups)
    {
        _primitivesGroups = primitivesGroups;
    }

    public int AddPrimitive(System.Windows.Point position, int currentGroupIndex)
    {
        var currentPrimitiveIndex = 0;
        if (_primitivesGroups.Count - 1 >= currentGroupIndex) 
            currentPrimitiveIndex = _primitivesGroups[currentGroupIndex].Count + 1;
        else
        {
            _primitivesGroups.Add(new PrimitivesGroup());
        }

        _primitivesGroups[currentGroupIndex].Add(new Point(position.X, position.Y, 10, Color.FromArgb(255, 255, 0, 0)));

        return currentPrimitiveIndex;
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