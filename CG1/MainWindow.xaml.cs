using SharpGL;
using SharpGL.SceneGraph;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpGL.WPF;
using System.Numerics;
using CG1.Core;
using CG1.Core.Primitives;
using CG1.Handlers;
using CG1.Handlers.KeyboardHandlers;
using CG1.Handlers.MouseHandlers;
using Point = System.Windows.Point;

namespace CG1;

public partial class MainWindow : Window
{
    private OpenGL _gl;
    private readonly MouseClicksHandler _mouseClicksHandler;
    private readonly MoveKeysHandler _moveKeysHandler;
    private readonly ColorKeysHandler _colorKeysHandler;
    private readonly SizeKeysHandler _sizeKeysHandler;

    private readonly List<PrimitivesGroup> _primitivesGroups = new();

    private PrimitivesGroup? SelectedGroup => _primitivesGroups[_selectedGroupIndex];
    private IPrimitive? SelectedPrimitive => SelectedGroup[_selectedPrimitiveIndex];

    private PrimitivesGroup? _temporaryGroup;
    private IPrimitive? _temporaryPrimitive;

    private int _selectedGroupIndex = -1;
    private int _selectedPrimitiveIndex = -1;
    private Mode _currentMode = Mode.Painting;
    private double _actualHeight;

    public MainWindow()
    {
        InitializeComponent();
        _moveKeysHandler = new MoveKeysHandler(glWindow.OpenGL);
        _mouseClicksHandler = new MouseClicksHandler(_primitivesGroups);
        _colorKeysHandler = new ColorKeysHandler();
        _sizeKeysHandler = new SizeKeysHandler();
    }

    private void OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
    {
        _gl.ClearColor(1f, 1f, 1f, 1f);
        _gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT);

        if (_primitivesGroups.Count > 0)
        {
            if (_selectedGroupIndex != -1)
            {
                if (_selectedPrimitiveIndex == SelectedGroup.Count)
                {
                    SelectedGroup.Highlight(_gl);
                    if (_currentMode == Mode.Changing)
                    {
                        _temporaryGroup?.Highlight(_gl);
                        _temporaryGroup?.Draw(_gl);
                    }
                }
                else
                {
                    if (_selectedPrimitiveIndex != -1)
                    {
                        SelectedPrimitive?.Highlight(_gl);
                        _temporaryPrimitive?.Highlight(_gl);
                        _temporaryPrimitive?.Draw(_gl);
                    }
                }
            }

            foreach (var primitivesGroup in _primitivesGroups)
            {
                primitivesGroup.Draw(_gl);
            }
        }

        _gl.Flush();
    }

    private void OpenGLInitialized(object sender, OpenGLRoutedEventArgs args)
    {
        _gl = args.OpenGL;

        _gl.Disable(OpenGL.GL_DEPTH_TEST);

        _gl.Enable(OpenGL.GL_BLEND);
        _gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
    }

    private void OpenGLControlResized(object sender, OpenGLRoutedEventArgs args)
    {
        _actualHeight = glWindow.ActualHeight;

        SetOrthoProjection(_gl);

        OpenGLDraw(sender, args);
    }

    private void GlWindowOnMouseClick(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed) return;
        var position = e.GetPosition(glWindow);
        position.Y = _actualHeight - position.Y;

        if (_currentMode == Mode.Painting)
        {
            if (e.ClickCount != 1) return;
            AddPrimitive(position);
        }
        else if (_currentMode == Mode.Changing)
        {
            if (e.ClickCount == 1)
            {
                SelectPrimitive(position);
            }
            else if (e.ClickCount == 2)
            {
                SelectPrimitivesGroup(position);
            }
        }
    }

    private void GlWindowOnKeyDown(object sender, KeyEventArgs e)
    {
        //Надо вынести
        if (e.Key == Key.Enter)
        {
            if (_currentMode == Mode.Painting)
            {
                AddGroup();
            }
            else if (_currentMode == Mode.Changing)
            {
                AcceptChanges();
            }
        }
        else if (e.Key == Key.Left)
        {
            ClearTemporaryGroup();
            SelectPreviousGroup();
        }
        else if (e.Key == Key.Right)
        {
            ClearTemporaryGroup();
            SelectNextGroup();
        }
        else if (e.Key == Key.Up)
        {
            ClearTemporaryPrimitive();
            SelectNextPrimitive();
        }
        else if (e.Key == Key.Down)
        {
            ClearTemporaryPrimitive();
            SelectPreviousPrimitive();
        }
        else if (e.Key == Key.W)
        {
            MovePrimitiveOrGroup(0d, 5d);
        }
        else if (e.Key == Key.A)
        {
            MovePrimitiveOrGroup(-5d, 0d);
        }
        else if (e.Key == Key.S)
        {
            MovePrimitiveOrGroup(0d, -5d);
        }
        else if (e.Key == Key.D)
        {
            MovePrimitiveOrGroup(5d, 0d);
        }
        else if (e.Key == Key.P)
        {
            ChangeModeToPainting();
        }
        else if (e.Key == Key.C)
        {
            ChangeModeToChanging();
        }
        else if (e.Key == Key.R)
        {
            if (_currentMode != Mode.Changing) return;
            if (_selectedPrimitiveIndex == -1 || _selectedGroupIndex == -1) return;
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ChangeColorOfPrimitiveOrGroup(0, -15, 0, 0);
            }
            else
            {
                ChangeColorOfPrimitiveOrGroup(0, 15, 0, 0);
            }
        }
        else if (e.Key == Key.G)
        {
            if (_currentMode != Mode.Changing) return;
            if (_selectedPrimitiveIndex == -1 || _selectedGroupIndex == -1) return;
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ChangeColorOfPrimitiveOrGroup(0, 0, -15, 0);
            }
            else
            {
                ChangeColorOfPrimitiveOrGroup(0, 0, 15, 0);
            }
        }
        else if (e.Key == Key.B)
        {
            if (_currentMode != Mode.Changing) return;
            if (_selectedPrimitiveIndex == -1 || _selectedGroupIndex == -1) return;
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ChangeColorOfPrimitiveOrGroup(0, 0, 0, -15);
            }
            else
            {
                ChangeColorOfPrimitiveOrGroup(0, 0, 0, 15);
            }
        }
        else if (e.Key == Key.OemPlus)
        {
            ChangeSizeOfPrimitiveOrGroup(1f);
        }
        else if (e.Key == Key.OemMinus)
        {
            ChangeSizeOfPrimitiveOrGroup(-1f);
        }
        else if (e.Key == Key.N)
        {
            _gl.Disable(OpenGL.GL_POINT_SMOOTH);
        }
        else if (e.Key == Key.M)
        {
            _gl.Enable(OpenGL.GL_POINT_SMOOTH);
            _gl.Hint(OpenGL.GL_POINT_SMOOTH_HINT, OpenGL.GL_NICEST);
        }
        else if (e.Key == Key.Z)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (_currentMode == Mode.Painting)
                {
                    DeleteLastGroupOrPrimitive();
                }
                else if (_currentMode == Mode.Changing)
                {
                    CancelChanges();
                }
            }
        }
        else if (e.Key == Key.Delete)
        {
            DeleteSelectedGroupOrPrimitive();
        }
        else if (e.Key == Key.Escape)
        {
            Cancel();
        }
    }

    private void CreateTemporaryGroup()
    {
        if (_selectedGroupIndex == -1) return;
        var temporaryGroup = SelectedGroup.Clone();
        SelectedGroup.MakeTransparent();

        _moveKeysHandler.TemporaryGroup = temporaryGroup;
        _colorKeysHandler.TemporaryGroup = temporaryGroup;
        _sizeKeysHandler.TemporaryGroup = temporaryGroup;
        _temporaryGroup = temporaryGroup;
    }

    private void CreateTemporaryPrimitive()
    {
        if (_selectedPrimitiveIndex == -1) return;
        var temporaryPrimitive = SelectedPrimitive.Clone();
        SelectedPrimitive.MakeTransparent();

        _moveKeysHandler.TemporaryPrimitive = temporaryPrimitive;
        _colorKeysHandler.TemporaryPrimitive = temporaryPrimitive;
        _sizeKeysHandler.TemporaryPrimitive = temporaryPrimitive;
        _temporaryPrimitive = temporaryPrimitive;
    }

    private void AcceptGroupChanges()
    {
        if (_selectedGroupIndex == -1) return;
        SelectedGroup.MakeNonTransparent();
        _primitivesGroups[_selectedGroupIndex] = _temporaryGroup;
    }

    private void AcceptPrimitiveChanges()
    {
        if (_selectedPrimitiveIndex == -1) return;
        SelectedPrimitive.MakeNonTransparent();
        SelectedGroup[_selectedPrimitiveIndex] = _temporaryPrimitive;
    }

    private void ClearTemporaryGroup()
    {
        _temporaryGroup = null;
        if (_selectedGroupIndex == -1) return;
        SelectedGroup.MakeNonTransparent();
    }

    private void ClearTemporaryPrimitive()
    {
        _temporaryPrimitive = null;
        if (_selectedPrimitiveIndex == -1 || _selectedGroupIndex == -1) return;
        if (_selectedPrimitiveIndex == SelectedGroup?.Count) return;
        SelectedPrimitive?.MakeNonTransparent();
    }

    private void SetOrthoProjection(OpenGL gl)
    {
        gl.MatrixMode(OpenGL.GL_PROJECTION);
        gl.LoadIdentity();
        gl.Ortho2D(0d, glWindow.ActualWidth, 0d, _actualHeight);
        gl.MatrixMode(OpenGL.GL_MODELVIEW);
        gl.LoadIdentity();
    }

    private void AddGroup()
    {
        _selectedGroupIndex = _mouseClicksHandler.AddGroup();
        _selectedPrimitiveIndex = SelectedGroup.Count;
    }

    private void AddPrimitive(Point position)
    {
        if (_primitivesGroups.Count == 0)
        {
            _selectedGroupIndex = _mouseClicksHandler.AddGroup();
        }

        _selectedPrimitiveIndex = _mouseClicksHandler.AddPrimitive(position, _selectedGroupIndex);
    }

    private void SelectPrimitive(Point position)
    {
        var (selectedGroup, selectedPrimitive) = _mouseClicksHandler.FindPrimitive(position);
        if (selectedGroup == -1) return;

        ClearTemporaryPrimitive();
        ClearTemporaryGroup();

        _selectedGroupIndex = selectedGroup;
        _selectedPrimitiveIndex = selectedPrimitive;

        CreateTemporaryPrimitive();
    }

    private void SelectPrimitivesGroup(Point position)
    {
        var selectedGroupIndex = _mouseClicksHandler.FindPrimitivesGroup(position);
        if (selectedGroupIndex == -1) return;

        ClearTemporaryPrimitive();
        ClearTemporaryGroup();

        _selectedGroupIndex = selectedGroupIndex;
        _selectedPrimitiveIndex = _primitivesGroups[_selectedGroupIndex].Count;

        CreateTemporaryGroup();
    }

    private void AcceptChanges()
    {
        if (_temporaryPrimitive != null)
        {
            AcceptPrimitiveChanges();
            CreateTemporaryPrimitive();
        }
        else if (_temporaryGroup != null)
        {
            AcceptGroupChanges();
            CreateTemporaryGroup();
        }
    }

    private void SelectPreviousGroup()
    {
        if (_selectedGroupIndex > 0)
        {
            _selectedGroupIndex--;
            _selectedPrimitiveIndex = SelectedGroup.Count;
        }
        else return;

        if (_currentMode != Mode.Changing) return;
        ClearTemporaryPrimitive();
        ClearTemporaryGroup();
        CreateTemporaryGroup();
    }

    private void SelectNextGroup()
    {
        if (_selectedGroupIndex < _primitivesGroups.Count - 1)
        {
            _selectedGroupIndex++;
            _selectedPrimitiveIndex = SelectedGroup.Count;
        }
        else return;

        if (_currentMode != Mode.Changing) return;
        ClearTemporaryPrimitive();
        ClearTemporaryGroup();
        CreateTemporaryGroup();
    }

    private void SelectPreviousPrimitive()
    {
        if (_currentMode != Mode.Changing) return;
        if (_selectedGroupIndex != -1 && _selectedPrimitiveIndex > 0)
        {
            ClearTemporaryPrimitive();
            ClearTemporaryGroup();
            _selectedPrimitiveIndex--;
            CreateTemporaryPrimitive();
        }
    }

    private void SelectNextPrimitive()
    {
        if (_currentMode != Mode.Changing) return;
        if (_selectedGroupIndex != -1 && _selectedPrimitiveIndex != -1 &&
            _selectedPrimitiveIndex < SelectedGroup.Count - 1)
        {
            ClearTemporaryPrimitive();
            ClearTemporaryGroup();
            _selectedPrimitiveIndex++;
            CreateTemporaryPrimitive();
        }
    }

    private void MovePrimitiveOrGroup(double x, double y)
    {
        if (_currentMode != Mode.Changing) return;
        if (_selectedPrimitiveIndex == -1 || _selectedGroupIndex == -1) return;
        if (_selectedPrimitiveIndex == SelectedGroup.Count)
        {
            _moveKeysHandler.MovePrimitivesGroup(x, y);
        }
        else
        {
            _moveKeysHandler.MovePrimitive(x, y);
        }
    }

    private void ChangeModeToPainting()
    {
        if (_currentMode == Mode.Painting) return;
        _currentMode = Mode.Painting;
        ClearTemporaryPrimitive();
        ClearTemporaryGroup();
        AddGroupButton.Visibility = Visibility.Visible;
        DeleteLastButton.Visibility = Visibility.Visible;
        DeleteButton.Visibility = Visibility.Hidden;
        PreviousPrimitiveButton.Visibility = Visibility.Hidden;
        NextPrimitiveButton.Visibility = Visibility.Hidden;
        AcceptChangesButton.Visibility = Visibility.Hidden;
        CancelChangesButton.Visibility = Visibility.Hidden;
        UpButton.Visibility = Visibility.Hidden;
        LeftButton.Visibility = Visibility.Hidden;
        DownButton.Visibility = Visibility.Hidden;
        RightButton.Visibility = Visibility.Hidden;
        //RLabel.Visibility = Visibility.Hidden;
        //RPlusButton.Visibility = Visibility.Hidden;
        //RMinusButton.Visibility = Visibility.Hidden;
        //GLabel.Visibility = Visibility.Hidden;
        //GPlusButton.Visibility = Visibility.Hidden;
        //GMinusButton.Visibility = Visibility.Hidden;
        //BLabel.Visibility = Visibility.Hidden;
        //BPlusButton.Visibility = Visibility.Hidden;
        //BMinusButton.Visibility = Visibility.Hidden;
        SizeLabel.Visibility = Visibility.Hidden;
        SizePlusButton.Visibility = Visibility.Hidden;
        SizeMinusButton.Visibility = Visibility.Hidden;
        //PaintingButton.Visibility = Visibility.Hidden;
        //ChangingButton.Visibility = Visibility.Visible;
        if (_selectedGroupIndex == -1) return;
        _selectedPrimitiveIndex = _primitivesGroups[_selectedGroupIndex].Count;
    }

    private void ChangeModeToChanging()
    {
        if (_currentMode == Mode.Changing) return;
        _currentMode = Mode.Changing;
        ClearTemporaryGroup();
        CreateTemporaryGroup();
        AddGroupButton.Visibility = Visibility.Hidden;
        DeleteLastButton.Visibility = Visibility.Hidden;
        DeleteButton.Visibility = Visibility.Visible;
        PreviousPrimitiveButton.Visibility = Visibility.Visible;
        NextPrimitiveButton.Visibility = Visibility.Visible;
        AcceptChangesButton.Visibility = Visibility.Visible;
        CancelChangesButton.Visibility = Visibility.Visible;
        UpButton.Visibility = Visibility.Visible;
        LeftButton.Visibility = Visibility.Visible;
        DownButton.Visibility = Visibility.Visible;
        RightButton.Visibility = Visibility.Visible;
        //RLabel.Visibility = Visibility.Visible;
        //RPlusButton.Visibility = Visibility.Visible;
        //RMinusButton.Visibility = Visibility.Visible;
        //GLabel.Visibility = Visibility.Visible;
        //GPlusButton.Visibility = Visibility.Visible;
        //GMinusButton.Visibility = Visibility.Visible;
        //BLabel.Visibility = Visibility.Visible;
        //BPlusButton.Visibility = Visibility.Visible;
        //BMinusButton.Visibility = Visibility.Visible;
        SizeLabel.Visibility = Visibility.Visible;
        SizePlusButton.Visibility = Visibility.Visible;
        SizeMinusButton.Visibility = Visibility.Visible;
        //PaintingButton.Visibility = Visibility.Visible;
        //ChangingButton.Visibility = Visibility.Hidden;
        if (_selectedGroupIndex == -1) return;
        _selectedPrimitiveIndex = _primitivesGroups[_selectedGroupIndex].Count;
    }

    private void ChangeColorOfPrimitiveOrGroup(short a, short r, short g, short b)
    {
        if (_selectedPrimitiveIndex == SelectedGroup.Count)
        {
            _colorKeysHandler.ChangeGroupColor(a, r, g, b);
        }
        else
        {
            _colorKeysHandler.ChangePrimitiveColor(a, r, g, b);
        }
    }

    private void ChangeSizeOfPrimitiveOrGroup(float size)
    {
        if (_currentMode != Mode.Changing) return;
        if (_selectedPrimitiveIndex == -1 || _selectedGroupIndex == -1) return;
        if (_selectedPrimitiveIndex == SelectedGroup.Count)
        {
            _sizeKeysHandler.ChangeGroupSize(size);
        }
        else
        {
            _sizeKeysHandler.ChangePrimitiveSize(size);
        }
    }

    private void DeleteLastGroupOrPrimitive()
    {
        if (_primitivesGroups.Count == 0) return;
        if (_primitivesGroups[^1].Count > 0)
        {
            _primitivesGroups[^1].RemoveAt(_primitivesGroups[^1].Count - 1);
            _selectedPrimitiveIndex = _primitivesGroups[^1].Count;
        }
        else
        {
            _primitivesGroups.RemoveAt(_primitivesGroups.Count - 1);
            if (_selectedGroupIndex > 0)
            {
                _selectedGroupIndex--;
                _selectedPrimitiveIndex = SelectedGroup.Count;
            }
        }
    }

    private void DeleteSelectedGroupOrPrimitive()
    {
        if (_currentMode == Mode.Changing)
        {
            if (_selectedPrimitiveIndex == SelectedGroup.Count)
            {
                ClearTemporaryGroup();
                _primitivesGroups.RemoveAt(_selectedGroupIndex);

                _selectedGroupIndex = -1;
                _selectedPrimitiveIndex = -1;
            }
            else
            {
                ClearTemporaryPrimitive();
                SelectedGroup.RemoveAt(_selectedPrimitiveIndex);
                _selectedPrimitiveIndex = -1;
            }
        }
    }

    private void CancelChanges()
    {
        if (_selectedGroupIndex != -1 && _selectedPrimitiveIndex == SelectedGroup.Count)
        {
            SelectedGroup.CancelChanges();
            ClearTemporaryGroup();
            CreateTemporaryGroup();
        }
        else if (_selectedPrimitiveIndex != -1)
        {
            SelectedPrimitive.CancelChanges();
            ClearTemporaryPrimitive();
            CreateTemporaryPrimitive();
        }
    }

    private void Cancel()
    {
        if (_currentMode != Mode.Changing) return;
        if (_selectedPrimitiveIndex == SelectedGroup.Count)
        {
            ClearTemporaryGroup();
            CreateTemporaryGroup();
        }
        else
        {
            ClearTemporaryPrimitive();
            CreateTemporaryPrimitive();
        }
    }

    private void AddGroupButton_OnClick(object sender, RoutedEventArgs e)
    {
        AddGroup();
    }

    private void DeleteLastButton_OnClick(object sender, RoutedEventArgs e)
    {
        DeleteLastGroupOrPrimitive();
    }

    private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
    {
        DeleteSelectedGroupOrPrimitive();
    }

    private void PreviousGroupButton_OnClick(object sender, RoutedEventArgs e)
    {
        SelectPreviousGroup();
    }

    private void NextGroupButton_OnClick(object sender, RoutedEventArgs e)
    {
        SelectNextGroup();
    }

    private void PreviousPrimitiveButton_OnClick(object sender, RoutedEventArgs e)
    {
        SelectPreviousPrimitive();
    }

    private void NextPrimitiveButton_OnClick(object sender, RoutedEventArgs e)
    {
        SelectNextPrimitive();
    }

    private void AcceptChangesButton_OnClick(object sender, RoutedEventArgs e)
    {
        AcceptChanges();
    }

    private void CancelChangesButton_OnClick(object sender, RoutedEventArgs e)
    {
        CancelChanges();
    }

    private void UpButton_OnClick(object sender, RoutedEventArgs e)
    {
        MovePrimitiveOrGroup(0d, 5d);
    }

    private void LeftButton_OnClick(object sender, RoutedEventArgs e)
    {
        MovePrimitiveOrGroup(-5d, 0d);
    }

    private void DownButton_OnClick(object sender, RoutedEventArgs e)
    {
        MovePrimitiveOrGroup(0d, -5d);
    }

    private void RightButton_OnClick(object sender, RoutedEventArgs e)
    {
        MovePrimitiveOrGroup(5d, 0d);
    }

    private void RPlusButton_OnClick(object sender, RoutedEventArgs e)
    {
        ChangeColorOfPrimitiveOrGroup(0, 15, 0, 0);
    }

    private void RMinusButton_OnClick(object sender, RoutedEventArgs e)
    {
        ChangeColorOfPrimitiveOrGroup(0, -15, 0, 0);
    }

    private void GPlusButton_OnClick(object sender, RoutedEventArgs e)
    {
        ChangeColorOfPrimitiveOrGroup(0, 0, 15, 0);
    }

    private void GMinusButton_OnClick(object sender, RoutedEventArgs e)
    {
        ChangeColorOfPrimitiveOrGroup(0, 0, -15, 0);
    }

    private void BPlusButton_OnClick(object sender, RoutedEventArgs e)
    {
        ChangeColorOfPrimitiveOrGroup(0, 0, 0, 15);
    }

    private void BMinusButton_OnClick(object sender, RoutedEventArgs e)
    {
        ChangeColorOfPrimitiveOrGroup(0, 0, 0, -15);
    }

    private void SizePlusButton_OnClick(object sender, RoutedEventArgs e)
    {
        ChangeSizeOfPrimitiveOrGroup(1f);
    }

    private void SizeMinusButton_OnClick(object sender, RoutedEventArgs e)
    {
        ChangeSizeOfPrimitiveOrGroup(-1f);
    }

    private void PaintingButton_OnClick(object sender, RoutedEventArgs e)
    {
        ChangeModeToPainting();
    }

    private void ChangingButton_OnClick(object sender, RoutedEventArgs e)
    {
        ChangeModeToChanging();
    }
}