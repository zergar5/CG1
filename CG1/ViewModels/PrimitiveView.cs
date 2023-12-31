﻿using CG1.Models.Primitives;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace CG1.ViewModels;

public class PrimitiveView : INotifyPropertyChanged
{
    public int Index
    {
        get => _index;
        set
        {
            _index = value;
            OnPropertyChanged(nameof(_index));
        }
    }

    private int _index;

    public Color Color
    {
        get => _color;
        set
        {
            _color = value;
            OnPropertyChanged(nameof(_color));
        }
    }

    private Color _color;

    private IPrimitive _primitive;
    public PrimitiveView(IPrimitive primitive, int index)
    {
        _color = primitive.GetColor();
        _index = index;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}