using TwoDe.Shaders;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;

namespace TwoDe.Window;

public class Window(int Width, int height, string title) : GameWindow(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (Width, height), Title = title })
{
    private readonly Stopwatch _timer = new();
    private Shader? _shader;
    int vertexBufferObject;
    int vertexArrayObject;
    
    private readonly float[] _vertices = [
      // positions        // colors
      0.5f, -0.5f, 0.0f,  1.0f, 0.0f, 0.0f,   // bottom right
     -0.5f, -0.5f, 0.0f,  0.0f, 1.0f, 0.0f,   // bottom left
      0.0f,  0.5f, 0.0f,  0.0f, 0.0f, 1.0f    // top 
    ];

    protected override void OnLoad()
    {
        base.OnLoad();

        _timer.Start();
                
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
        
        _shader = new Shader("Src/Shaders/shader.vert", "Src/Shaders/shader.frag");

        vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexBufferObject);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        _shader?.Use();

        if (_shader is null)
        {
            return;
        }

        double timeValue = _timer.Elapsed.TotalSeconds;
        float greenValue = (float)Math.Sin(timeValue) / 2.0f + 0.5f;
        int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "ourColor");
        GL.Uniform4(vertexColorLocation, 0.0f, greenValue, 0.0f, 1.0f);

        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.BindVertexArray(vertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        base.OnUpdateFrame(args);
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, Size.X, Size.Y);
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        _timer.Stop();
        _shader?.Dispose();
    }
}






