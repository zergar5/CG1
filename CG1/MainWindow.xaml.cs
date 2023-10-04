using CG1.Contexts;
using CG1.Models;
using SharpGL;
using SharpGL.WPF;
using System.Windows;
using System.Windows.Input;

namespace CG1;

public partial class MainWindow : Window
{
    private OpenGL _gl;
    private readonly PrimitivesApp _primitivesApp;

    public MainWindow()
    {
        InitializeComponent();
        _primitivesApp = new PrimitivesApp(new PrimitivesGroupsContext(), new PrimitivesEditor());
    }

    private void OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
    {
        _primitivesApp.Render((OpenGLControl)sender, args);
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
        SetOrthoProjection(_gl);

        OpenGLDraw(sender, args);
    }

    private void GlWindowOnMouseClick(object sender, MouseButtonEventArgs args)
    {
        if (args.LeftButton != MouseButtonState.Pressed) return;

        var gl = (OpenGLControl)sender;

        if (args.ClickCount == 1)
        {
            _primitivesApp.OnMouseClick(gl, args);
        }
        else if (args.ClickCount == 2)
        {
            _primitivesApp.OnMouseDoubleClick(gl, args);
        }
    }

    private void GlWindowOnKeyDown(object sender, KeyEventArgs args)
    {
        if (args.Key == Key.Enter)
        {
            _primitivesApp.OnEnterKeyDown();
        }
        else if (args.Key is Key.Left or Key.Right or Key.Up or Key.Down)
        {
            _primitivesApp.OnSelectionKeyDown(args.Key);
        }
        else if (args.Key == Key.W)
        {
            _primitivesApp.OnMoveKeyDown(0f, 5f);
        }
        else if (args.Key == Key.A)
        {
            _primitivesApp.OnMoveKeyDown(-5f, 0f);
        }
        else if (args.Key == Key.S)
        {
            _primitivesApp.OnMoveKeyDown(0f, -5f);
        }
        else if (args.Key == Key.D)
        {
            _primitivesApp.OnMoveKeyDown(5f, 0f);
        }
        else if (args.Key == Key.P)
        {
            _primitivesApp.SetMode(Mode.Painting);
        }
        else if (args.Key == Key.C)
        {
            _primitivesApp.SetMode(Mode.Changing);
        }
        else if (args.Key == Key.R)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                _primitivesApp.OnChangeColorKeyDown(r: -15);
            }
            else
            {
                _primitivesApp.OnChangeColorKeyDown(r: 15);
            }
        }
        else if (args.Key == Key.G)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                _primitivesApp.OnChangeColorKeyDown(g: -15);
            }
            else
            {
                _primitivesApp.OnChangeColorKeyDown(g: 15);
            }
        }
        else if (args.Key == Key.B)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                _primitivesApp.OnChangeColorKeyDown(b: -15);
            }
            else
            {
                _primitivesApp.OnChangeColorKeyDown(b: 15);
            }
        }
        else if (args.Key == Key.OemPlus)
        {
            _primitivesApp.OnSizeKeyDown(1f);
        }
        else if (args.Key == Key.OemMinus)
        {
            _primitivesApp.OnSizeKeyDown(-1f);
        }
        else if (args.Key == Key.N)
        {
            _gl.Disable(OpenGL.GL_POINT_SMOOTH);
        }
        else if (args.Key == Key.M)
        {
            _gl.Enable(OpenGL.GL_POINT_SMOOTH);
            _gl.Hint(OpenGL.GL_POINT_SMOOTH_HINT, OpenGL.GL_NICEST);
        }
        else if (args.Key == Key.Z)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                _primitivesApp.OnCancelActionKeyDown();
            }
        }
        else if (args.Key == Key.Delete)
        {
            _primitivesApp.OnDeleteKeyDown();
        }
        else if (args.Key == Key.Escape)
        {
            _primitivesApp.OnReturnKeyDown();
        }
    }

    private void SetOrthoProjection(OpenGL gl)
    {
        gl.MatrixMode(OpenGL.GL_PROJECTION);
        gl.LoadIdentity();
        gl.Ortho2D(0d, glWindow.ActualWidth, 0d, glWindow.ActualHeight);
        gl.MatrixMode(OpenGL.GL_MODELVIEW);
        gl.LoadIdentity();
    }

    private void AddGroupButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnEnterKeyDown();
    }

    private void DeleteLastButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnCancelActionKeyDown();
    }

    private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnDeleteKeyDown();
    }

    private void PreviousGroupButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnSelectionKeyDown(Key.Left);
    }

    private void NextGroupButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnSelectionKeyDown(Key.Right);
    }

    private void PreviousPrimitiveButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnSelectionKeyDown(Key.Up);
    }

    private void NextPrimitiveButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnSelectionKeyDown(Key.Down);
    }

    private void AcceptChangesButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnEnterKeyDown();
    }

    private void CancelChangesButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnCancelActionKeyDown();
    }

    private void UpButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnMoveKeyDown(0f, 5f);
    }

    private void LeftButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnMoveKeyDown(-5f, 0f);
    }

    private void DownButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnMoveKeyDown(0f, -5f);
    }

    private void RightButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnMoveKeyDown(5f, 0f);
    }

    private void RPlusButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnChangeColorKeyDown(r: 15);
    }

    private void RMinusButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnChangeColorKeyDown(r: -15);
    }

    private void GPlusButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnChangeColorKeyDown(g: 15);
    }

    private void GMinusButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnChangeColorKeyDown(g: -15);
    }

    private void BPlusButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnChangeColorKeyDown(b: 15);
    }

    private void BMinusButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnChangeColorKeyDown(b: -15);
    }

    private void SizePlusButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnSizeKeyDown(1f);
    }

    private void SizeMinusButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnSizeKeyDown(-1f);
    }

    private void PaintingButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.SetMode(Mode.Painting);
    }

    private void ChangingButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.SetMode(Mode.Changing);
    }

    private void EscapeButton_OnClick(object sender, RoutedEventArgs e)
    {
        _primitivesApp.OnReturnKeyDown();
    }
}