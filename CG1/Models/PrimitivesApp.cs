using CG1.Contexts;
using CG1.Models.Primitives;
using SharpGL;
using SharpGL.WPF;
using System.Windows.Input;
using System.Windows.Media;

namespace CG1.Models;

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

        if (_context.SelectedGroup != null)
        {
            if (_context.SelectedPrimitiveIndex == _context.SelectedGroup.Count)
            {
                _context.SelectedGroup.Highlight(openGL);
                if (_currentMode == Mode.Changing && _primitivesEditor.EditingGroup != null)
                {
                    _primitivesEditor.EditingGroup.Highlight(openGL);
                    _primitivesEditor.EditingGroup.Draw(openGL);
                }
            }
            else
            {
                _context.SelectedPrimitive?.Highlight(openGL);
                if (_currentMode == Mode.Changing && _primitivesEditor.EditingPrimitive != null)
                {
                    _primitivesEditor.EditingPrimitive.Highlight(openGL);
                    _primitivesEditor.EditingPrimitive.Draw(openGL);
                }
            }
        }
        
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
                new Point((float)position.X, (float)position.Y, 6, Colors.Red)
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
            if (_primitivesEditor.EditingGroup != null && _context.SelectedGroup != null)
            {
                _primitivesEditor.AcceptChanges(_context.SelectedGroup);
            }
            else if (_primitivesEditor.EditingPrimitive != null && _context.SelectedPrimitive != null)
            {
                _primitivesEditor.AcceptChanges(_context.SelectedPrimitive);
            }
            
        }
    }

    public void OnSelectionKeyDown(Key key)
    {
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

    public void OnMoveKeyDown(float x, float y)
    {
        _primitivesEditor.Move(x, y);
    }

    public void OnChangeColorKeyDown(short a = 0, short r = 0, short g = 0, short b = 0)
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
        if (_context.SelectedGroup == null) return;
        if (_context.SelectedPrimitiveIndex == _context.SelectedGroup.Count)
        {
            _context.RemoveGroupAt(_context.SelectedGroupIndex);
        }
        else
        {
            _context.RemovePrimitiveAt(_context.SelectedPrimitiveIndex);
        }

        if (_currentMode == Mode.Changing)
        {
            _primitivesEditor.StopEditing();
        }

        _context.Unselect();
    }

    public void OnReturnKeyDown()
    {
        if (_currentMode == Mode.Changing)
        {
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
            _primitivesEditor.StartEditing(_context.SelectedGroup);
            
            _currentMode = mode;
        }
    }
}