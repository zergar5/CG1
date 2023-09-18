using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CG1.Core.Primitives;
using SharpGL;
using Point = System.Windows.Point;

namespace CG1.Core;

public class PrimitivesGroup
{
    private readonly List<IPrimitive> _primitives;
    public int Count => _primitives.Count;

    public PrimitivesGroup()
    {
        _primitives = new List<IPrimitive>();
    }

    public PrimitivesGroup(List<IPrimitive> primitives)
    {
        _primitives = primitives;
    }

    public IPrimitive this[int index]
    {
        get => _primitives[index];
        set => _primitives[index] = value;
    }

    public void Draw(OpenGL openGl)
    {
        foreach (var primitive in _primitives)
        {
            primitive.Draw(openGl);
        }
    }

    public void Highlight(OpenGL openGl)
    {
        foreach (var primitive in _primitives)
        {
            primitive.Highlight(openGl);
        }
    }

    public void Move(double x, double y)
    {
        foreach (var primitive in _primitives)
        {
            primitive.Move(x, y);
        }
    }

    public void ChangeColor(short a, short r, short g, short b)
    {
        foreach (var primitive in _primitives)
        {
            primitive.ChangeColor(a, r, g, b);
        }
    }

    public void MakeTransparent()
    {
        foreach (var primitive in _primitives)
        {
            primitive.MakeTransparent();
        }
    }

    public void MakeNonTransparent()
    {
        foreach (var primitive in _primitives)
        {
            primitive.MakeNonTransparent();
        }
    }

    public void Reset()
    {
        foreach (var primitive in _primitives)
        {
            primitive.Reset();
        }
    }

    public void Add(IPrimitive primitive)
    {
        _primitives.Add(primitive);
    }

    public void RemoveAt(int index)
    {
        _primitives.RemoveAt(index);
    }

    public int FindIndex(Point point)
    {
        return _primitives.FindIndex(p => p.Contains(point));
    }

    public bool Contains(Point point)
    {
        return _primitives.Any(p => p.Contains(point));
    }

    public PrimitivesGroup Clone()
    {
        return new PrimitivesGroup(_primitives.Select(x => x.Clone()).ToList());
    }

    public IEnumerator<IPrimitive> GetEnumerator() => ((IEnumerable<IPrimitive>)_primitives).GetEnumerator();
}