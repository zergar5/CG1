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
namespace CG1;

public partial class MainWindow : Window
{
    private readonly MouseClicksHandler _mouseClicksHandler; 
    private readonly MoveKeysHandler _moveKeysHandler;
    private readonly ColorKeysHandler _colorKeysHandler;
    private readonly SizeKeysHandler _sizeKeysHandler;

    private readonly List<PrimitivesGroup> _primitivesGroups = new ();
    private PrimitivesGroup? SelectedGroup => _primitivesGroups[_selectedGroupIndex];
    private IPrimitive? SelectedPrimitive => SelectedGroup[_selectedPrimitiveIndex];

    private PrimitivesGroup? _temporaryGroup;
    private IPrimitive? _temporaryPrimitive;

    private int _selectedGroupIndex = 0;
    private int _selectedPrimitiveIndex = 0;
    private Mode _currentMode = Mode.Painting;
    private double _width;
    private double _height;
    private double _actualWidth;
    private double _actualHeight;
    private double _scaleX = 1;
    private double _scaleY = 1;

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
        var gl = args.OpenGL;

        gl.ClearColor(1f, 1f, 1f, 1f);
        gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT);

        if (_primitivesGroups.Count > 0)
        {
            if (_selectedGroupIndex != -1)
            {
                if (_currentMode == Mode.Painting)
                {
                    SelectedGroup.Highlight(gl);
                }
                else if(_currentMode == Mode.Changing)
                {
                    if (_selectedPrimitiveIndex == SelectedGroup.Count)
                    {
                        if (_selectedGroupIndex != -1)
                        {
                            _temporaryGroup?.Highlight(gl);
                            _temporaryGroup?.Draw(gl);
                            SelectedGroup?.Highlight(gl);
                        }
                    }
                    else
                    {
                        if (_selectedPrimitiveIndex != -1)
                        {
                            _temporaryPrimitive?.Highlight(gl);
                            _temporaryPrimitive?.Draw(gl);
                            SelectedPrimitive?.Highlight(gl);
                            
                        }
                    }
                }
            }

            foreach (var primitivesGroup in _primitivesGroups)
            {
                primitivesGroup.Draw(gl);
            }
        }

        gl.Flush();
    }

    private void OpenGLInitialized(object sender, OpenGLRoutedEventArgs args)
    {
        var gl = args.OpenGL;

        gl.Disable(OpenGL.GL_DEPTH_TEST);

        gl.Enable(OpenGL.GL_BLEND);
        gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
    }

    private void OpenGLControlResized(object sender, OpenGLRoutedEventArgs args)
    {
        _actualWidth = glWindow.ActualWidth;
        _actualHeight = glWindow.ActualHeight;

        var gl = args.OpenGL;

        gl.MatrixMode(OpenGL.GL_PROJECTION);
        gl.LoadIdentity();
        gl.Ortho(0d, _actualWidth, 0d, _actualHeight, -1d, 1d);
        gl.MatrixMode(OpenGL.GL_MODELVIEW);
        gl.LoadIdentity();

        OpenGLDraw(sender, args);
    }

    private void GlWindowOnMouseClick(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed) return;
        var position = e.GetPosition(glWindow);
        position.Y = _actualHeight - position.Y;

        if (_currentMode == Mode.Painting && e.ClickCount == 1)
        {
            if (_primitivesGroups.Count == 0)
            {
                _primitivesGroups.Add(new PrimitivesGroup());
                _selectedGroupIndex = 0;
            }

            _selectedPrimitiveIndex = _mouseClicksHandler.AddPrimitive(position, _selectedGroupIndex);
        }
        else if (_currentMode == Mode.Changing)
        {
            if (e.ClickCount == 1)
            {
                var (selectedGroup, selectedPrimitive) = _mouseClicksHandler.FindPrimitive(position);
                if (selectedGroup == -1) return;

                ClearTemporaryGroup();

                _selectedGroupIndex = selectedGroup;
                _selectedPrimitiveIndex = selectedPrimitive;

                CreateTemporaryPrimitive();
            }
            else if (e.ClickCount == 2)
            {
                var selectedGroupIndex = _mouseClicksHandler.FindPrimitivesGroup(position);
                if (selectedGroupIndex == -1) return;

                ClearTemporaryPrimitive();

                _selectedGroupIndex = selectedGroupIndex;
                _selectedPrimitiveIndex = _primitivesGroups[_selectedGroupIndex].Count;

                CreateTemporaryGroup();
            }
        }
    }

    private void GlWindowOnLoaded(object sender, RoutedEventArgs e)
    {
        _width = glWindow.ActualWidth;
        _height = glWindow.ActualHeight;
    }

    private void GlWindowOnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if(_currentMode == Mode.Painting) _selectedGroupIndex = _mouseClicksHandler.AddGroup();
            else if (_currentMode == Mode.Changing)
            {
                if (_temporaryPrimitive != null)
                {
                    AcceptPrimitiveChanges();
                    CreateTemporaryPrimitive();
                }
                if (_temporaryGroup != null)
                {
                    AcceptGroupChanges();
                    CreateTemporaryGroup();
                }
            }
        }
        else if (e.Key == Key.Left)
        {
            if (_selectedGroupIndex > 0)
            {
                _selectedGroupIndex--;
            }
            _selectedPrimitiveIndex = SelectedGroup.Count;

            if (_currentMode != Mode.Changing) return;
            ClearTemporaryPrimitive();
            CreateTemporaryGroup();
        }
        else if (e.Key == Key.Right)
        {
            if (_selectedGroupIndex < _primitivesGroups.Count - 1)
            {
                _selectedGroupIndex++;
            }
            _selectedPrimitiveIndex = SelectedGroup.Count;

            if (_currentMode != Mode.Changing) return;
            ClearTemporaryPrimitive();
            CreateTemporaryGroup();
            
        }
        else if (e.Key == Key.Up)
        {
            if (_currentMode != Mode.Changing) return;
            if (_selectedPrimitiveIndex >= SelectedGroup.Count - 1) return;
            ClearTemporaryPrimitive();
            _selectedPrimitiveIndex++;
            CreateTemporaryPrimitive();
        }
        else if (e.Key == Key.Down)
        {
            if (_currentMode != Mode.Changing) return;
            if (_selectedPrimitiveIndex <= 0) return;
            ClearTemporaryPrimitive();
            _selectedPrimitiveIndex--;
            CreateTemporaryPrimitive();
        }
        else if (e.Key == Key.W)
        {
            if (_currentMode != Mode.Changing) return;
            if (_selectedPrimitiveIndex == -1 || _selectedGroupIndex == -1) return;
            if (_selectedPrimitiveIndex == SelectedGroup.Count)
            {
                _moveKeysHandler.MovePrimitivesGroup(0d, 5d);
            }
            else
            {
                _moveKeysHandler.MovePrimitive(0d, 5d);
            }
        }
        else if (e.Key == Key.A)
        {
            if (_currentMode != Mode.Changing) return;
            if (_selectedPrimitiveIndex == -1 || _selectedGroupIndex == -1) return;
            if (_selectedPrimitiveIndex == SelectedGroup.Count)
            {
                _moveKeysHandler.MovePrimitivesGroup(-5d, 0d);
            }
            else
            {
                _moveKeysHandler.MovePrimitive(-5d, 0);
            }
        }
        else if (e.Key == Key.S)
        {
            if (_currentMode != Mode.Changing) return;
            if (_selectedPrimitiveIndex == -1 || _selectedGroupIndex == -1) return;
            if (_selectedPrimitiveIndex == SelectedGroup.Count)
            {
                _moveKeysHandler.MovePrimitivesGroup(0d, -5d);
            }
            else
            {
                _moveKeysHandler.MovePrimitive(0d, -5d);
            }
        }
        else if (e.Key == Key.D)
        {
            if (_currentMode != Mode.Changing) return;
            if (_selectedPrimitiveIndex == -1 || _selectedGroupIndex == -1) return;
            if (_selectedPrimitiveIndex == SelectedGroup.Count)
            {
                _moveKeysHandler.MovePrimitivesGroup(5d, 0);
            }
            else
            {
                _moveKeysHandler.MovePrimitive(5d, 0);
            }
        }
        else if (e.Key == Key.P)
        {
            if (_currentMode == Mode.Painting) return;
            _currentMode = Mode.Painting;
            ClearTemporaryPrimitive();
            ClearTemporaryGroup();
            _selectedPrimitiveIndex = _primitivesGroups[_selectedGroupIndex].Count;
        }
        else if (e.Key == Key.C)
        {
            if (_currentMode == Mode.Changing) return;
            _currentMode = Mode.Changing;
            ClearTemporaryGroup();
            CreateTemporaryGroup();
            _selectedPrimitiveIndex = _primitivesGroups[_selectedGroupIndex].Count;
        }
        else if (e.Key == Key.R)
        {
            if (_selectedPrimitiveIndex == -1 || _selectedGroupIndex == -1) return;
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (_selectedPrimitiveIndex == SelectedGroup.Count)
                {
                    _colorKeysHandler.ChangeGroupColor(0, -15, 0, 0);
                }
                else
                {
                    _colorKeysHandler.ChangePrimitiveColor(0,-15, 0, 0);
                }
            }
            else
            {
                if (_selectedPrimitiveIndex == SelectedGroup.Count)
                {
                    _colorKeysHandler.ChangeGroupColor(0, 15, 0, 0);
                }
                else
                {
                    _colorKeysHandler.ChangePrimitiveColor(0, 15, 0, 0);
                }
            }
        }
        else if (e.Key == Key.G)
        {
            if (_selectedPrimitiveIndex == -1 || _selectedGroupIndex == -1) return;
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (_selectedPrimitiveIndex == SelectedGroup.Count)
                {
                    _colorKeysHandler.ChangeGroupColor(0, 0, -15, 0);
                }
                else
                {
                    _colorKeysHandler.ChangePrimitiveColor(0, 0, -15, 0);
                }
            }
            else
            {
                if (_selectedPrimitiveIndex == SelectedGroup.Count)
                {
                    _colorKeysHandler.ChangeGroupColor(0, 0, 15, 0);
                }
                else
                {
                    _colorKeysHandler.ChangePrimitiveColor(0, 0, 15, 0);
                }
            }
        }
        else if (e.Key == Key.B)
        {
            if (_selectedPrimitiveIndex == -1 || _selectedGroupIndex == -1) return;
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (_selectedPrimitiveIndex == SelectedGroup.Count)
                {
                    _colorKeysHandler.ChangeGroupColor(0, 0, 0, -15);
                }
                else
                {
                    _colorKeysHandler.ChangePrimitiveColor(0, 0, 0, -15);
                }
            }
            else
            {
                if (_selectedPrimitiveIndex == SelectedGroup.Count)
                {
                    _colorKeysHandler.ChangeGroupColor(0, 0, 0, 15);
                }
                else
                {
                    _colorKeysHandler.ChangePrimitiveColor(0, 0, 0, 15);
                }
            }
        }
        else if (e.Key == Key.OemPlus)
        {
            if (_currentMode != Mode.Changing) return;
            if (_selectedPrimitiveIndex == -1 || _selectedGroupIndex == -1) return;
            if (_selectedPrimitiveIndex == SelectedGroup.Count)
            {
                _sizeKeysHandler.ChangeGroupSize(5f);
            }
            else
            {
                _sizeKeysHandler.ChangePrimitiveSize(5f);
            }
        }
        else if (e.Key == Key.OemMinus)
        {
            if (_currentMode != Mode.Changing) return;
            if (_selectedPrimitiveIndex == -1 || _selectedGroupIndex == -1) return;
            if (_selectedPrimitiveIndex == SelectedGroup.Count)
            {
                _sizeKeysHandler.ChangeGroupSize(-5f);
            }
            else
            {
                _sizeKeysHandler.ChangePrimitiveSize(-5f);
            }
        }
        else if (e.Key == Key.Q)
        {
            //if (_currentMode != Mode.Changing) return;
            //if (_selectedPrimitiveIndex == SelectedGroup.Count)
            //{
            //    _moveKeysHandler.RotatePrimitivesGroup(5d);
            //}
            //else
            //{
            //    _moveKeysHandler.RotatePrimitive(5d);
            //}
        }
        else if (e.Key == Key.E)
        {
            //if (_currentMode != Mode.Changing) return;
            //if (_selectedPrimitiveIndex == SelectedGroup.Count)
            //{
            //    _moveKeysHandler.RotatePrimitivesGroup(-5d);
            //}
            //else
            //{
            //    _moveKeysHandler.RotatePrimitive(-5d);
            //}
        }
        else if (e.Key == Key.N)
        {
            var gl = glWindow.OpenGL;
            gl.Disable(OpenGL.GL_POINT_SMOOTH);
        }
        else if (e.Key == Key.M)
        {
            var gl = glWindow.OpenGL;
            gl.Enable(OpenGL.GL_POINT_SMOOTH);
            gl.Hint(OpenGL.GL_POINT_SMOOTH_HINT, OpenGL.GL_NICEST);
        }
        else if (e.Key == Key.Z)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (_currentMode == Mode.Painting)
                {
                    if (_primitivesGroups.Count <= 0) return;
                    if (_primitivesGroups[^1].Count > 0)
                    {
                        _primitivesGroups[^1].RemoveAt(_primitivesGroups[^1].Count - 1);
                    }
                    else
                    {
                        _primitivesGroups.RemoveAt(_primitivesGroups.Count - 1);
                        if(_selectedGroupIndex > 0) _selectedGroupIndex--;
                    }
                }
                else if (_currentMode == Mode.Changing)
                {
                    if (_selectedPrimitiveIndex == SelectedGroup.Count)
                    {
                        SelectedGroup.Reset();
                        CreateTemporaryGroup();
                    }
                    else
                    {
                        SelectedPrimitive.Reset();
                        CreateTemporaryPrimitive();
                    }
                }
            }
        }
        else if (e.Key == Key.Delete)
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
        else if (e.Key == Key.Escape)
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
    }

    private void CreateTemporaryGroup()
    {
        var temporaryGroup = SelectedGroup.Clone();
        SelectedGroup.MakeTransparent();

        _moveKeysHandler.TemporaryGroup = temporaryGroup;
        _colorKeysHandler.TemporaryGroup = temporaryGroup;
        _sizeKeysHandler.TemporaryGroup = temporaryGroup;
        _temporaryGroup = temporaryGroup;
    }

    private void CreateTemporaryPrimitive()
    {
        var temporaryPrimitive = SelectedPrimitive.Clone();
        SelectedPrimitive.MakeTransparent();

        _moveKeysHandler.TemporaryPrimitive = temporaryPrimitive;
        _colorKeysHandler.TemporaryPrimitive = temporaryPrimitive;
        _sizeKeysHandler.TemporaryPrimitive = temporaryPrimitive;
        _temporaryPrimitive = temporaryPrimitive;
    }

    private void AcceptGroupChanges()
    {
        SelectedGroup.MakeNonTransparent();
        _primitivesGroups[_selectedGroupIndex] = _temporaryGroup;
    }

    private void AcceptPrimitiveChanges()
    {
        SelectedPrimitive.MakeNonTransparent();
        SelectedGroup[_selectedPrimitiveIndex] = _temporaryPrimitive;
    }

    private void ClearTemporaryGroup()
    {
        _temporaryGroup = null;
        SelectedGroup?.MakeNonTransparent();
    }

    private void ClearTemporaryPrimitive()
    {
        _temporaryPrimitive = null;
        if(_selectedPrimitiveIndex == SelectedGroup.Count) return;
        SelectedPrimitive?.MakeNonTransparent();
    }
}