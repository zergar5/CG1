using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace CG1.ViewModels;

public class PrimitivesGroupsContextView : INotifyPropertyChanged
{
    public ObservableCollection<PrimitivesGroupView> Views
    {
        get => _views;
        set
        {
            _views = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<PrimitivesGroupView> _views;

    public int SelectedGroupIndex
    {
        get => _selectedGroupIndex;
        set
        {
            _selectedGroupIndex = value;
            if (value < 0)
            {
                _groupsTable.UnselectAll();
            }
            else
            {
                SelectedInTableGroup = _views[value];
                _groupsTable.SelectedIndex = value;
            }
            OnPropertyChanged();
        }
    }
    private int _selectedGroupIndex = -1;

    public PrimitivesGroupView SelectedInTableGroup
    {
        get => _selectedInTableGroup;
        set
        {
            _selectedInTableGroup = value;
            OnPropertyChanged();
        }
    }
    private PrimitivesGroupView _selectedInTableGroup;

    private readonly ListView _groupsTable;
    private readonly MainWindow _window;

    public PrimitivesGroupsContextView(ListView groupsTable, MainWindow window)
    {
        _groupsTable = groupsTable;
        _window = window;

        _views = new ObservableCollection<PrimitivesGroupView>();
        _views.CollectionChanged += (_, _)
            => OnPropertyChanged(nameof(Views));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}