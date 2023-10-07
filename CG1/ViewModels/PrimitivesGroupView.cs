using CG1.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CG1.ViewModels;

public class PrimitivesGroupView : INotifyPropertyChanged
{
    public int Index
    {
        get => _index;
        set
        {
            _index = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Number));
        }
    }
    private int _index;

    public int Number => Index + 1;

    public int PrimitivesCount
    {
        get => _primitivesCount;
        set
        {
            _primitivesCount = value;
            OnPropertyChanged();
        }
    }
    private int _primitivesCount;

    public PrimitivesGroupView(PrimitivesGroup group, int index)
    {
        _primitivesCount = group.Count;
        _index = index;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}