using Hexa.NET.OpenGL;
using System.Collections.Generic;
using System.Numerics;

namespace MidnightEngine.Engine.Graphics
{
    public class ShaderManager
    {
        private GL gl;
        private Dictionary<string, Shader> shaders;

        public ShaderManager(GL gl)
        {
            this.gl = gl;
            this.shaders = new Dictionary<string, Shader>();
        }

        public Shader LoadShader(string name, string vertexPath, string fragmentPath)
        {
            if (shaders.ContainsKey(name))
            {
                return shaders[name];
            }

            var shader = new Shader(gl, vertexPath, fragmentPath);
            shaders[name] = shader;
            return shader;
        }

        public Shader GetShader(string name)
        {
            if (shaders.TryGetValue(name, out var shader))
            {
                return shader;
            }

            throw new KeyNotFoundException($"Shader '{name}' not found.");
        }

        public void UnloadShader(string name)
        {
            if (shaders.TryGetValue(name, out var shader))
            {
                shader.Dispose();
                shaders.Remove(name);
            }
        }

        public void UnloadAllShaders()
        {
            foreach (var shader in shaders.Values)
            {
                shader.Dispose();
            }
            shaders.Clear();
        }
    }

    public class Shader : IDisposable
    {
        private GL gl;
        private uint programId;
        private bool disposed = false;

        public uint ProgramId => programId;

        public Shader(GL gl, string vertexPath, string fragmentPath)
        {
            this.gl = gl;

            // Shader kódok betöltése
            string vertexCode = File.ReadAllText(vertexPath);
            string fragmentCode = File.ReadAllText(fragmentPath);

            // Shader program létrehozása
            programId = CreateProgram(vertexCode, fragmentCode);
        }

        public Shader(GL gl, string vertexCode, string fragmentCode, bool fromSource = true)
        {
            this.gl = gl;
            programId = CreateProgram(vertexCode, fragmentCode);
        }

        private unsafe uint CreateProgram(string vertexCode, string fragmentCode)
        {
            // Vertex shader fordítása
            uint vertexShader = CompileShader(GLShaderType.VertexShader, vertexCode);

            // Fragment shader fordítása
            uint fragmentShader = CompileShader(GLShaderType.FragmentShader, fragmentCode);

            // Program létrehozása és linkelése
            uint program = gl.CreateProgram();
            gl.AttachShader(program, vertexShader);
            gl.AttachShader(program, fragmentShader);
            gl.LinkProgram(program);

            // Linkelés ellenőrzése
            int success;
            gl.GetProgramiv(program, GLProgramPropertyARB.LinkStatus, &success);
            if (success == 0)
            {
                string infoLog = gl.GetProgramInfoLog(program);
                throw new Exception($"Shader linking failed: {infoLog}");
            }

            // Shader-ek törlése a memóriából
            gl.DeleteShader(vertexShader);
            gl.DeleteShader(fragmentShader);

            return program;
        }

        private unsafe uint CompileShader(GLShaderType type, string source)
        {
            uint shader = gl.CreateShader(type);
            gl.ShaderSource(shader, source);
            gl.CompileShader(shader);

            // Fordítás ellenőrzése
            int success;
            gl.GetShaderiv(shader, GLShaderParameterName.CompileStatus, &success);
            if (success == 0)
            {
                string infoLog = gl.GetShaderInfoLog(shader);
                throw new Exception($"Shader compilation failed: {infoLog}");
            }

            return shader;
        }

        public void Use()
        {
            gl.UseProgram(programId);
        }

        // Uniform beállítási metódusok
        public void SetBool(string name, bool value)
        {
            int location = gl.GetUniformLocation(programId, name);
            gl.Uniform1i(location, value ? 1 : 0);
        }

        public void SetInt(string name, int value)
        {
            int location = gl.GetUniformLocation(programId, name);
            gl.Uniform1i(location, value);
        }

        public void SetFloat(string name, float value)
        {
            int location = gl.GetUniformLocation(programId, name);
            gl.Uniform1f(location, value);
        }

        public void SetVector2(string name, Vector2 value)
        {
            int location = gl.GetUniformLocation(programId, name);
            gl.Uniform2f(location, value.X, value.Y);
        }

        public void SetVector3(string name, Vector3 value)
        {
            int location = gl.GetUniformLocation(programId, name);
            gl.Uniform3f(location, value.X, value.Y, value.Z);
        }

        public void SetVector4(string name, Vector4 value)
        {
            int location = gl.GetUniformLocation(programId, name);
            gl.Uniform4f(location, value.X, value.Y, value.Z, value.W);
        }

        public void SetMatrix4(string name, Matrix4x4 value)
        {
            int location = gl.GetUniformLocation(programId, name);
            unsafe
            {
                // A Matrix4x4 már teljesen memória területen van, nincs szükség fix-re
                gl.UniformMatrix4fv(location, 1, false, (float*)&value);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    gl.DeleteProgram(programId);
                }
                disposed = true;
            }
        }

        ~Shader()
        {
            Dispose(false);
        }
    }
}