using CG1.Contexts;
using CG1.Tools;
using SharpGL.WPF;
using System.Windows.Input;
using System.Windows.Media;
using CG1.Core.Primitives;
using SharpGL;

namespace CG1.Core;

public class PrimitivesApp
{
    private readonly PrimitivesGroupsContext _context;
    private readonly PrimitivesEditor _primitivesEditor;
    private Mode _currentMode;

    public PrimitivesApp(PrimitivesGroupsContext context, PrimitivesEditor primitivesEditor)
    {
        _context = context;
        _primitivesEditor = primitivesEditor;
    }

    public void Render(OpenGLControl gl, OpenGLRoutedEventArgs args)
    {
        var openGL = args.OpenGL;
        openGL.ClearColor(1f, 1f, 1f, 1f);
        openGL.Clear(OpenGL.GL_COLOR_BUFFER_BIT);

        _context.SelectedGroup?.Highlight(openGL);

        foreach (var primitivesGroup in _context.Groups)
        {
            primitivesGroup.Draw(openGL);
        }

        openGL.Flush();
    }

    public void OnMouseClick(OpenGLControl gl, MouseButtonEventArgs args)
    {
        var position = args.GetPosition(gl);
        position.Y = gl.ActualHeight - position.Y;

        if (_currentMode == Mode.Painting)
        {
            if (_context.Groups.Count == 0)
            {
                _context.AddGroup();
            }
            if (_context.SelectedGroup == null)
            {
                _context.SelectGroup(_context.Groups.Count - 1);
            }
            _context.AddPrimitive(
                new Point(position.X, position.Y, 10, Color.FromArgb(255, 255, 0, 0))
            );
        }
        else if (_currentMode == Mode.Changing)
        {
            _context.SelectPrimitiveAtCursor(position);
            if (_context.SelectedPrimitive != null)
            {
                _primitivesEditor.StartEditing(_context.SelectedPrimitive);
            }
        }

        args.Handled = true;
    }
    public void OnMouseDoubleClick(OpenGLControl gl, MouseButtonEventArgs args)
    {
        var position = args.GetPosition(gl);
        position.Y = gl.ActualHeight - position.Y;

        if (_currentMode == Mode.Changing)
        {
            _context.SelectGroupAtCursor(position);
            if (_context.SelectedGroup != null)
            {
                _primitivesEditor.StartEditing(_context.SelectedGroup);
            }
        }

        args.Handled = true;
    }

    public void OnEnterKeyDown()
    {
        if (_currentMode == Mode.Painting)
        {
            _context.AddGroup();
        }
        else if (_currentMode == Mode.Changing)
        {
            //принятие изменений
        }
    }

    public void OnSelectionKeyDown(Key key)
    {
        //Проверки что не пустой список и выбрано хотя бы что-то
        if (key == Key.Left)
        {
            _context.SelectPreviousGroup();
        }
        else if (key == Key.Right)
        {
            _context.SelectNextGroup();
        }
        else if (_currentMode == Mode.Changing)
        {
            if (key == Key.Up)
            {
                _context.SelectPreviousPrimitive();
            }
            else if (key == Key.Down)
            {
                _context.SelectNextPrimitive();
            }
        }
    }

    public void OnMoveKeyDown(double x, double y)
    {
        _primitivesEditor.Move(x, y);
    }

    public void OnChangeColorKeyDown(float a = 0, float r = 0, float g = 0, float b = 0)
    {
        _primitivesEditor.EditColor(a, r, g, b);
    }

    public void OnSizeKeyDown(float size)
    {
        _primitivesEditor.ChangeSize(size);
    }

    public void OnCancelActionKeyDown()
    {
        if (_currentMode == Mode.Painting)
        {
            if (_context.SelectedGroup == null) return;
            if (_context.SelectedGroup.Count > 0)
            {
                _context.RemoveLastPrimitive();
            }
            else _context.RemoveLastGroup();
        }
        else if (_currentMode == Mode.Changing)
        {
            _primitivesEditor.CancelChanges();
        }
    }

    public void OnDeleteKeyDown()
    {
        
    }

    public void OnReturnKeyDown()
    {
        //Добавить проверку выбрано ли что-то
        if (_currentMode == Mode.Changing)
        {
            _primitivesEditor.ReturnChanges();
            _primitivesEditor.StopEditing();
        }
        
        _context.Unselect();
    }

    public void SetMode(Mode mode)
    {
        if (mode == Mode.Painting)
        {
            _currentMode = mode;
        }
        else if (mode == Mode.Changing)
        {
            if (_context.SelectedGroup is not { Count: > 0 }) return;
            _currentMode = mode;
        }
    }
}