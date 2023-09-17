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

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MoveKeysHandler _moveKeysHandler;
    private readonly MouseClicksHandler _mouseClicksHandler;

    private readonly List<PrimitivesGroup> _primitivesGroups = new ();
    private PrimitivesGroup SelectedGroup => _primitivesGroups[_selectedGroupIndex];
    private IPrimitive SelectedPrimitive => SelectedGroup[_selectedPrimitiveIndex];

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
    private double _centreX;
    private double _centreY;

    public MainWindow()
    {
        InitializeComponent();
        _moveKeysHandler = new MoveKeysHandler(glWindow.OpenGL);
        _mouseClicksHandler = new MouseClicksHandler(_primitivesGroups);
    }

    private void OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
    {
        var gl = args.OpenGL;

        gl.ClearColor(1f, 1f, 1f, 1f);
        gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT);

        if (_primitivesGroups.Count > 0)
        {
            //gl.Enable(OpenGL.GL_POINT_SMOOTH);
            //gl.Hint(OpenGL.GL_POINT_SMOOTH_HINT, OpenGL.GL_NICEST);

            if (_primitivesGroups.Count > _selectedGroupIndex && SelectedGroup.Count > 0)
            {
                if (_currentMode == Mode.Painting)
                {
                    SelectedGroup.Highlight(gl);
                }
                else if(_currentMode == Mode.Changing)
                {
                    if (_selectedPrimitiveIndex != SelectedGroup.Count)
                    {
                        SelectedPrimitive.Highlight(gl);
                        _temporaryPrimitive?.Draw(gl);
                    }
                    else
                    {
                        SelectedGroup.Highlight(gl);
                        _temporaryGroup?.Draw(gl);
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
        //if (_width != 0 && _height != 0)
        //{
        //    _scaleX = glWindow.ActualWidth / _width;
        //    _scaleY = glWindow.ActualHeight / _height;
        //}

        _actualWidth = glWindow.ActualWidth;
        _actualHeight = glWindow.ActualHeight;

        _centreX = glWindow.ActualWidth / 2;
        _centreY = glWindow.ActualHeight / 2;

        var gl = args.OpenGL;

        

        gl.MatrixMode(OpenGL.GL_PROJECTION);
        gl.LoadIdentity();
        gl.Ortho(0d, glWindow.ActualWidth, 0d, glWindow.ActualHeight, -1d, 1d);
        gl.MatrixMode(OpenGL.GL_MODELVIEW);
        gl.LoadIdentity();

        //gl.Scale(_scaleX, _scaleY, 1);

        OpenGLDraw(sender, args);
    }

    private void GlWindowOnMouseClick(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed) return;
        var position = e.GetPosition(glWindow);
        position.Y = _actualHeight - position.Y;

        if (_currentMode == Mode.Painting && e.ClickCount == 1)
        {
            _selectedPrimitiveIndex = _mouseClicksHandler.AddPrimitive(position, _selectedGroupIndex);
        }
        else if (_currentMode == Mode.Changing)
        {
            if (e.ClickCount == 1)
            {
                var (selectedGroup, selectedPrimitive) = _mouseClicksHandler.FindPrimitive(position);
                if (selectedGroup == -1) return;
                _selectedGroupIndex = selectedGroup;
                _selectedPrimitiveIndex = selectedPrimitive;

                _temporaryGroup = null;
                _temporaryPrimitive = CreateTemporaryPrimitive();
            }
            else if (e.ClickCount == 2)
            {
                _selectedGroupIndex = _mouseClicksHandler.FindPrimitivesGroup(position);
                _selectedPrimitiveIndex = _primitivesGroups[_selectedGroupIndex].Count;
                
                _temporaryGroup = CreateTemporaryGroup();
                _temporaryPrimitive = null;
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
                    SelectedGroup[_selectedPrimitiveIndex] = _temporaryPrimitive;
                    _temporaryPrimitive = CreateTemporaryPrimitive();
                }
                if (_temporaryGroup != null)
                {
                    _primitivesGroups[_selectedGroupIndex] = _temporaryGroup;
                    _temporaryGroup = CreateTemporaryGroup();
                }
            }
        }
        else if (e.Key == Key.Left)
        {
            if (_selectedGroupIndex < 0) return;
            _selectedGroupIndex--;
            if (_currentMode != Mode.Changing) return;
            _temporaryGroup = SelectedGroup.Clone();
            _temporaryPrimitive = null;
        }
        else if (e.Key == Key.Right)
        {
            if (_selectedGroupIndex >= _primitivesGroups.Count) return;
            _selectedGroupIndex++;
            if (_currentMode != Mode.Changing) return;
            _temporaryGroup = SelectedGroup.Clone();
            _temporaryPrimitive = null;
        }
        else if (e.Key == Key.W)
        {
            if (_currentMode != Mode.Changing) return;
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
            _currentMode = Mode.Painting;
            _temporaryGroup = null;
            _temporaryPrimitive = null;
            _selectedPrimitiveIndex = _primitivesGroups[_selectedGroupIndex].Count;
        }
        else if (e.Key == Key.C)
        {
            _currentMode = Mode.Changing;
            _temporaryGroup = CreateTemporaryGroup();
            _temporaryPrimitive = null;
            _selectedPrimitiveIndex = _primitivesGroups[_selectedGroupIndex].Count;
        }
        else if (e.Key == Key.R)
        {
            if (_selectedPrimitiveIndex == SelectedGroup.Count)
            {
                _moveKeysHandler.MovePrimitivesGroup(5d, 0);
            }
            else
            {
                _moveKeysHandler.MovePrimitive(5d, 0);
            }
        }
        else if (e.Key == Key.G) {}
        else if (e.Key == Key.B) {}
        else if (e.Key == Key.Q) { }
        else if (e.Key == Key.E) { }
        else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.Z) {}
    }

    private PrimitivesGroup CreateTemporaryGroup()
    {
        var temporaryGroup = SelectedGroup.Clone();

        foreach (var primitive in temporaryGroup)
        {
            var color = primitive.GetColor();
            primitive.SetColor((byte)(color.A / 2), color.R, color.G, color.B);
        }

        _moveKeysHandler.TemporaryGroup = temporaryGroup;
        return temporaryGroup;
    }

    private IPrimitive CreateTemporaryPrimitive()
    {
        var temporaryPrimitive = SelectedPrimitive.Clone();

        var color = temporaryPrimitive.GetColor();
        temporaryPrimitive.SetColor((byte)(color.A / 2), color.R, color.G, color.B);

        _moveKeysHandler.TemporaryPrimitive = temporaryPrimitive;
        return temporaryPrimitive;
    }

    private PrimitivesGroup ClearTemporaryPrimitive()
    {
        var temporaryGroup = SelectedGroup.Clone();
        foreach (var primitive in temporaryGroup)
        {
            var color = primitive.GetColor();
            primitive.SetColor((byte)(color.A / 2), color.R, color.G, color.B);
        }
        _moveKeysHandler.TemporaryGroup = temporaryGroup;
        return temporaryGroup;
    }
}