using CG1.Contexts;
using SharpGL;
using SharpGL.WPF;
using System.Windows.Input;
using System.Windows.Media;

namespace CG1.Models;

public class PrimitivesApp
{
    private readonly PrimitivesGroupsContext _context;
    private readonly PrimitivesEditor _primitivesEditor;
    public Mode CurrentMode { get; private set; }

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
                if (CurrentMode == Mode.Changing && _primitivesEditor.EditingGroup != null)
                {
                    _primitivesEditor.EditingGroup.Highlight(openGL);
                    _primitivesEditor.EditingGroup.Draw(openGL);
                }
            }
            else
            {
                _context.SelectedPrimitive?.Highlight(openGL);
                if (CurrentMode == Mode.Changing && _primitivesEditor.EditingPrimitive != null)
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

        if (CurrentMode == Mode.Painting)
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
                new Primitives.Point((float)position.X, (float)position.Y, 6, Colors.Red)
            );
        }
        else if (CurrentMode == Mode.Changing)
        {
            _context.SelectPrimitiveAtCursor(position);
            if (_context.SelectedPrimitive != null)
            {
                _primitivesEditor.StartEditing(_context.SelectedPrimitive);
            }
        }
    }
    public void OnMouseDoubleClick(OpenGLControl gl, MouseButtonEventArgs args)
    {
        var position = args.GetPosition(gl);
        position.Y = gl.ActualHeight - position.Y;

        if (CurrentMode == Mode.Changing)
        {
            _context.SelectGroupAtCursor(position);
            if (_context.SelectedGroup != null)
            {
                _primitivesEditor.StartEditing(_context.SelectedGroup);
            }
        }
    }

    public void OnEnterKeyDown()
    {
        if (CurrentMode == Mode.Painting)
        {
            _context.AddGroup();
        }
        else if (CurrentMode == Mode.Changing)
        {
            if (_primitivesEditor.EditingGroup != null && _context.SelectedGroup != null)
            {
                _context.Groups[_context.SelectedGroupIndex] = _primitivesEditor.EditingGroup;
                _context.SelectedGroup.MakeNonTransparent();
                _primitivesEditor.StartEditing(_context.SelectedGroup);
            }
            else if (_primitivesEditor.EditingPrimitive != null && _context.SelectedPrimitive != null)
            {
                _context.Groups[_context.SelectedGroupIndex][_context.SelectedPrimitiveIndex] =
                    _primitivesEditor.EditingPrimitive;
                _context.SelectedPrimitive.MakeNonTransparent();
                _primitivesEditor.StartEditing(_context.SelectedPrimitive);
            }
        }
    }

    public void OnSelectionKeyDown(Key key)
    {
        if (key == Key.Left)
        {
            _context.SelectPreviousGroup();
            if (CurrentMode != Mode.Changing || _context.SelectedGroup == null) return;
            _primitivesEditor.StartEditing(_context.SelectedGroup);
        }
        else if (key == Key.Right)
        {
            _context.SelectNextGroup();
            if (CurrentMode != Mode.Changing || _context.SelectedGroup == null) return;
            _primitivesEditor.StartEditing(_context.SelectedGroup);
        }
        else if (CurrentMode == Mode.Changing)
        {
            if (key == Key.Up)
            {
                _context.SelectPreviousPrimitive();
                if (_context.SelectedPrimitive == null) return;

                _primitivesEditor.StartEditing(_context.SelectedPrimitive);
            }
            else if (key == Key.Down)
            {
                _context.SelectNextPrimitive();
                if (_context.SelectedPrimitive == null) return;
                _primitivesEditor.StartEditing(_context.SelectedPrimitive);
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
        if (CurrentMode == Mode.Painting)
        {
            if (_context.SelectedGroup == null) return;
            if (_context.SelectedGroup.Count > 0)
            {
                _context.RemoveLastPrimitive();
            }
            else _context.RemoveLastGroup();
        }
        else if (CurrentMode == Mode.Changing)
        {
            _primitivesEditor.CancelChanges();
            _context.CancelChanges();
            if (_context.SelectedGroup == null) return;
            if (_context.SelectedPrimitive != null)
            {
                _primitivesEditor.StartEditing(_context.SelectedPrimitive);
            }
            else _primitivesEditor.StartEditing(_context.SelectedGroup);
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

        if (CurrentMode == Mode.Changing)
        {
            _primitivesEditor.StopEditing();
        }

        _context.Unselect();
    }

    public void OnReturnKeyDown()
    {
        if (CurrentMode == Mode.Changing)
        {
            _primitivesEditor.StopEditing();
        }

        _context.Unselect();
    }

    public void OnGroupSelected(int index)
    {
        if (_context.SelectedGroupIndex == index) return;
        _context.SelectGroup(index);
    }

    public void OnColorPickerColorChanged(Color color)
    {
        if (CurrentMode == Mode.Changing)
        {
            _primitivesEditor.SetColor(color);
        }
    }

    public void SetMode(Mode mode)
    {
        if (mode == Mode.Painting)
        {
            _primitivesEditor.StopEditing();

            if (_context.SelectedGroup != null) _context.SelectGroup(_context.SelectedGroupIndex);
            else if (_context.Groups.Count > 0) _context.SelectGroup(_context.Groups.Count - 1);
            else _context.Unselect();
            CurrentMode = mode;
        }
        else if (mode == Mode.Changing)
        {
            if (_context.SelectedGroup is not { Count: > 0 }) return;
            _primitivesEditor.StartEditing(_context.SelectedGroup);
            CurrentMode = mode;
        }
    }

}