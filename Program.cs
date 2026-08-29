using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using Silk.NET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using StbImageSharp;

namespace Voxelia
{
    class Program
    {   
        private static uint _texture;
        private static uint _tbo;
        private static uint _program;
        private static uint _ebo;
        private static uint _vao;  
        private static uint _vbo;
        public static GL Gl { get; private set; }   
        private static IWindow _window;
        
        public static void Main()
        {
            Console.WriteLine("Hello, World!");

            WindowOptions options = WindowOptions.Default with
            {
                Size = new Vector2D<int>(800, 600),
                Title = "Voxelia"
            };
            
            _window = Window.Create(options);

            _window.FocusChanged += FordFocus;
            _window.Load   += OnLoad;
            _window.Update += OnUpdate;
            _window.Render += OnRender;

            _window.Run();

        }

        private static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
        {
            if (key == Key.Escape)
                _window.Close();
        }

        private static void FordFocus(bool isFocused)
        {
            if (!isFocused)
            {
                _window.Close();
            }
        }

        private static void OnUpdate(double deltaTime)
        {
            
        }

        private static unsafe void OnRender(double deltaTime)
        {
            Gl.Clear(ClearBufferMask.ColorBufferBit);

            Gl.BindVertexArray(_vao);
            Gl.UseProgram(_program);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2D, _texture);
            Gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (void*) 0);
        }

        private static unsafe void OnLoad() 
        {
            IInputContext input = _window.CreateInput();

            for (int i = 0; i < input.Keyboards.Count; i++)
                input.Keyboards[i].KeyDown += KeyDown;
            
            Gl = _window.CreateOpenGL();
            Gl.ClearColor(Color.BlanchedAlmond);

            _vao = Gl.GenVertexArray();
            Gl.BindVertexArray(_vao);

            float[] vertices =
            {
                0.5f, 0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
               -0.5f, -0.5f, 0.0f,
               -0.5f, 0.5f, 0.0f
            };

            float[] TexCoords =
            {
                1.0f, -1.0f,
                1.0f, 0.0f,
                0.0f, 0.0f,
                0.0f, -1.0f
            };

            uint[] indices =
            {
                0u, 1u, 3u,
                1u, 2u, 3u
            };

            _vbo = Gl.GenBuffer();
            Gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
            Gl.BufferData<float>(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), vertices, BufferUsageARB.StaticDraw);

            const uint positionLox = 0;
            Gl.EnableVertexAttribArray(positionLox);
            Gl.VertexAttribPointer(positionLox, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*) 0);
            
            _tbo = Gl.GenBuffer();
            Gl.BindBuffer(BufferTargetARB.ArrayBuffer, _tbo);
            Gl.BufferData<float>(BufferTargetARB.ArrayBuffer, (nuint)(TexCoords.Length * sizeof(float)), TexCoords, BufferUsageARB.StaticDraw);

            const uint texcoordLox = 1;
            Gl.EnableVertexAttribArray(texcoordLox);
            Gl.VertexAttribPointer(texcoordLox, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), (void*) 0);

            _ebo = Gl.GenBuffer();
            Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
            Gl.BufferData<uint>(BufferTargetARB.ElementArrayBuffer, (nuint) (indices.Length * sizeof(uint)), indices, BufferUsageARB.StaticDraw);

            
        
            const string vertexCode = @"
            #version 330 core

            layout (location = 0) in vec3 aPosition;
            layout (location = 1) in vec2 aTexCoord;

            out vec2 frag_TexCoords;

            void main() 
            {
                frag_TexCoords = aTexCoord;
                gl_Position = vec4(aPosition, 1);
            }";

            const string fragmentCode = @"
            #version 330 core
            
            uniform sampler2D uTexture;

            out vec4 out_color;
            in vec2 frag_TexCoords;

            void main()
            {
                //out_color = vec4(1.0, 0.5, 0.2, 1.0);
                out_color = texture(uTexture, frag_TexCoords);
            }";

            uint vertexShader = Gl.CreateShader(ShaderType.VertexShader);
            Gl.ShaderSource(vertexShader, vertexCode);
            Gl.CompileShader(vertexShader);

            Gl.GetShader(vertexShader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int) GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + Gl.GetShaderInfoLog(vertexShader));

            uint fragmentShader = Gl.CreateShader(ShaderType.FragmentShader);
            Gl.ShaderSource(fragmentShader, fragmentCode);
            Gl.CompileShader(fragmentShader);

            Gl.GetShader(fragmentShader, ShaderParameterName.CompileStatus, out int fStatus);
            if (fStatus != (int) GLEnum.True)
                throw new Exception("Fragment shader failed to compile: " + Gl.GetShaderInfoLog(fragmentShader));

            _program = Gl.CreateProgram();
            Gl.AttachShader(_program, vertexShader);
            Gl.AttachShader(_program, fragmentShader);

            Gl.LinkProgram(_program);

            Gl.GetProgram(_program, ProgramPropertyARB.LinkStatus, out int lStatus);
            if (lStatus != (int) GLEnum.True)
                throw new Exception("Program failed to link: " + Gl.GetProgramInfoLog(_program));

            Gl.DetachShader(_program, vertexShader);
            Gl.DetachShader(_program, fragmentShader);
            Gl.DeleteShader(vertexShader);
            Gl.DeleteShader(fragmentShader); 

            

            
            
            

            Gl.BindVertexArray(0);
            Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
            Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

            _texture = Gl.GenTexture();
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2D, _texture);

            if (!File.Exists("Assets/GLEE.png"))
            {
                Console.WriteLine("buh");
            }

            // ImageResult.FromMemory reads the bytes of the .png file and returns all its information!
            ImageResult result = ImageResult.FromMemory(File.ReadAllBytes("Assets/GLEE.png"), ColorComponents.RedGreenBlueAlpha);
        
            Gl.TexImage2D<byte>(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)result.Width,
                (uint)result.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, result.Data);
            
            int wrapS = (int)TextureWrapMode.Repeat;
            int wrapT = (int)TextureWrapMode.Repeat;
            int minFilter = (int)TextureMinFilter.Nearest;
            int magFilter = (int)TextureMagFilter.Nearest;
            
            Gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, ref wrapS);
            Gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, ref wrapT);
            Gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, ref minFilter);
            Gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, ref magFilter);

            Gl.BindTexture(TextureTarget.Texture2D, 0);
        
            int texLoc = Gl.GetUniformLocation(_program, "uTexture");
            Gl.Uniform1(texLoc, 0);
        } 
    }
}