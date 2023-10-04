using CG1.Contexts;
using CG1.Models;
using SharpGL;
using SharpGL.WPF;
using System.Windows;
using System.Windows.Input;
using CG1.ViewModels;

namespace CG1;

public partial class MainWindow : Window
{
    private OpenGL _gl;
    public PrimitivesApp PrimitivesApp { get; }
    public PrimitivesGroupsContextView ContextView { get; }

    public MainWindow()
    {
        InitializeComponent();
        ContextView = new PrimitivesGroupsContextView(GroupsTable, this);
        PrimitivesApp = new PrimitivesApp(new PrimitivesGroupsContext(ContextView),
            new PrimitivesEditor());

        DataContext = ContextView;
    }

    private void OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
    {
        PrimitivesApp.Render((OpenGLControl)sender, args);
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
            PrimitivesApp.OnMouseClick(gl, args);
        }
        else if (args.ClickCount == 2)
        {
            PrimitivesApp.OnMouseDoubleClick(gl, args);
        }
    }

    private void GlWindowOnKeyDown(object sender, KeyEventArgs args)
    {
        if (args.Key == Key.Enter)
        {
            PrimitivesApp.OnEnterKeyDown();
        }
        else if (args.Key is Key.Left or Key.Right or Key.Up or Key.Down)
        {
            PrimitivesApp.OnSelectionKeyDown(args.Key);
        }
        else if (args.Key == Key.W)
        {
            PrimitivesApp.OnMoveKeyDown(0f, 5f);
        }
        else if (args.Key == Key.A)
        {
            PrimitivesApp.OnMoveKeyDown(-5f, 0f);
        }
        else if (args.Key == Key.S)
        {
            PrimitivesApp.OnMoveKeyDown(0f, -5f);
        }
        else if (args.Key == Key.D)
        {
            PrimitivesApp.OnMoveKeyDown(5f, 0f);
        }
        else if (args.Key == Key.P)
        {
            PrimitivesApp.SetMode(Mode.Painting);
        }
        else if (args.Key == Key.C)
        {
            PrimitivesApp.SetMode(Mode.Changing);
        }
        else if (args.Key == Key.R)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                PrimitivesApp.OnChangeColorKeyDown(r: -15);
            }
            else
            {
                PrimitivesApp.OnChangeColorKeyDown(r: 15);
            }
        }
        else if (args.Key == Key.G)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                PrimitivesApp.OnChangeColorKeyDown(g: -15);
            }
            else
            {
                PrimitivesApp.OnChangeColorKeyDown(g: 15);
            }
        }
        else if (args.Key == Key.B)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                PrimitivesApp.OnChangeColorKeyDown(b: -15);
            }
            else
            {
                PrimitivesApp.OnChangeColorKeyDown(b: 15);
            }
        }
        else if (args.Key == Key.OemPlus)
        {
            PrimitivesApp.OnSizeKeyDown(1f);
        }
        else if (args.Key == Key.OemMinus)
        {
            PrimitivesApp.OnSizeKeyDown(-1f);
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
                PrimitivesApp.OnCancelActionKeyDown();
            }
        }
        else if (args.Key == Key.Delete)
        {
            PrimitivesApp.OnDeleteKeyDown();
        }
        else if (args.Key == Key.Escape)
        {
            PrimitivesApp.OnReturnKeyDown();
        }
    }

    private void SetOrthoProjection(OpenGL gl)
    {
        gl.MatrixMode(OpenGL.GL_PROJECTION);
        gl.LoadIdentity();
        gl.Ortho2D(0d, GlWindow.ActualWidth, 0d, GlWindow.ActualHeight);
        gl.MatrixMode(OpenGL.GL_MODELVIEW);
        gl.LoadIdentity();
    }

    private void AddGroupButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnEnterKeyDown();
    }

    private void DeleteLastButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnCancelActionKeyDown();
    }

    private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnDeleteKeyDown();
    }

    private void PreviousGroupButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnSelectionKeyDown(Key.Left);
    }

    private void NextGroupButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnSelectionKeyDown(Key.Right);
    }

    private void PreviousPrimitiveButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnSelectionKeyDown(Key.Up);
    }

    private void NextPrimitiveButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnSelectionKeyDown(Key.Down);
    }

    private void AcceptChangesButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnEnterKeyDown();
    }

    private void CancelChangesButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnCancelActionKeyDown();
    }

    private void UpButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnMoveKeyDown(0f, 5f);
    }

    private void LeftButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnMoveKeyDown(-5f, 0f);
    }

    private void DownButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnMoveKeyDown(0f, -5f);
    }

    private void RightButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnMoveKeyDown(5f, 0f);
    }

    private void RPlusButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnChangeColorKeyDown(r: 15);
    }

    private void RMinusButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnChangeColorKeyDown(r: -15);
    }

    private void GPlusButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnChangeColorKeyDown(g: 15);
    }

    private void GMinusButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnChangeColorKeyDown(g: -15);
    }

    private void BPlusButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnChangeColorKeyDown(b: 15);
    }

    private void BMinusButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnChangeColorKeyDown(b: -15);
    }

    private void SizePlusButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnSizeKeyDown(1f);
    }

    private void SizeMinusButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnSizeKeyDown(-1f);
    }

    private void PaintingButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.SetMode(Mode.Painting);
    }

    private void ChangingButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.SetMode(Mode.Changing);
    }

    private void EscapeButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrimitivesApp.OnReturnKeyDown();
    }

    private void ToggleButton_Checked(object sender, RoutedEventArgs e)
    {

    }

    private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
    {

    }

    private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {

    }
}