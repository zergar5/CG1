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
            args.Handled = true;
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