using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using CG1.Models.Primitives;
using SharpGL.SceneGraph;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CG1.ModelsViews;

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

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}