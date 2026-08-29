using System;
using System.IO;

using Silk.NET;
using Silk.NET.OpenGL;

namespace Voxelia.Engine.Rendering
{
    public class Shader
    {
        public uint Id { get; private set; }

        public Shader(string vsPath, string fsPath)
        {
            using GL gl = Program.Gl;

            string vertexPath = Path.Combine(AppContext.BaseDirectory, vsPath);
            string fragmentPath = Path.Combine(AppContext.BaseDirectory, fsPath);

            if (!File.Exists(vertexPath))
            {
                Console.WriteLine("Shader error: Vertex shader at \"" + vertexPath + "\" does not exist");
                return;
            }
            if (!File.Exists(fragmentPath))
            {
                Console.WriteLine("Shader error: Fragment shader at \"" + fragmentPath + "\" does not exist");
                return;
            }

            string vsCode = File.ReadAllText(vertexPath);

            uint vsId = gl.CreateShader(ShaderType.VertexShader);
            gl.ShaderSource(vsId, vsCode);
            gl.CompileShader(vsId);
            gl.GetShader(vsId, ShaderParameterName.CompileStatus, out int vsStatus);
            if (vsStatus != (int)GLEnum.True)
            {
                Console.WriteLine("Shader compilation error in vertex shader: " + gl.GetShaderInfoLog(vsId));
                return;
            }
            
            string fsCode = File.ReadAllText(fragmentPath);

            uint fsId = gl.CreateShader(ShaderType.FragmentShader);
            gl.ShaderSource(fsId, fsCode);
            gl.CompileShader(fsId);
            gl.GetShader(fsId, ShaderParameterName.CompileStatus, out int fsStatus);
            if (fsStatus != (int)GLEnum.True)
            {
                Console.WriteLine("Shader compilation error in fragment shader: " + gl.GetShaderInfoLog(fsId));
                return;
            }

            Id = gl.CreateProgram();
            gl.AttachShader(Id, vsId);
            gl.AttachShader(Id, fsId);
            gl.LinkProgram(Id);
            gl.GetProgram(Id, ProgramPropertyARB.LinkStatus, out int lStatus);
            if (lStatus != (int)GLEnum.True)
            {
                Console.WriteLine("Shader program error: " + gl.GetProgramInfoLog(Id));
                return;
            }

            gl.DetachShader(Id, vsId);
            gl.DetachShader(Id, fsId);
            gl.DeleteShader(vsId);
            gl.DeleteShader(fsId);
        }

        public void Use()
        {
            Program.Gl.UseProgram(Id);
        }

        public int GetUniformLoc(string uniform)
        {
            int result = Program.Gl.GetUniformLocation(Id, uniform);

            if (result == -1)
            {
                Console.WriteLine("Shader loc not found in shader Id " + Id);
            }

            return result;
        }
    }
}