using System;
using System.IO;
using System.Text;
using OpenTK.Graphics.ES20;
using OpenTK.Mathematics;

namespace BFML._3D;

public class Shader : IDisposable
{
    private readonly int _handle;

    public Shader(string vertexPath, string fragmentPath)
    {
        int vertexShader;
        int fragmentShader;
        string vertexShaderSource;
        string fragmentShaderSource;

        using (StreamReader reader = new StreamReader(vertexPath, Encoding.UTF8))
        {
            vertexShaderSource = reader.ReadToEnd();
        }

        using (StreamReader reader = new StreamReader(fragmentPath, Encoding.UTF8))
        {
            fragmentShaderSource = reader.ReadToEnd();
        }

        vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);

        fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);

        GL.CompileShader(vertexShader);

        GL.GetShader(_handle, ShaderParameter.CompileStatus, out int succes);
        if (succes == 0)
        {
            string infoLog = GL.GetShaderInfoLog(vertexShader);
            Console.WriteLine(infoLog);
        }

        GL.CompileShader(fragmentShader);

        GL.GetShader(_handle, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(fragmentShader);
            Console.WriteLine(infoLog);
        }

        _handle = GL.CreateProgram();

        GL.AttachShader(_handle, vertexShader);
        GL.AttachShader(_handle, fragmentShader);

        GL.LinkProgram(_handle);

        GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out int succe);
        if (succe == 0)
        {
            string infoLog = GL.GetProgramInfoLog(_handle);
            Console.WriteLine(infoLog);
        }

        GL.DetachShader(_handle, vertexShader);
        GL.DetachShader(_handle, fragmentShader);
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);
    }

    public void Use()
    {
        GL.UseProgram(_handle);
    }

    public void SetUniformFloat(string name, ref float value)
    {
        int location = GL.GetUniformLocation(_handle, name);
        GL.Uniform1(location, value);
    }

    public void SetUniformMatrix4(string name, ref Matrix4 value)
    {
        int location = GL.GetUniformLocation(_handle, name);
        GL.UniformMatrix4(location, true, ref value);
    }

    #region Disposing

    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        GL.DeleteProgram(_handle);
        _disposed = true;
    }

    ~Shader()
    {
        GL.DeleteProgram(_handle);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}