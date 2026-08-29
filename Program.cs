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
using Voxelia.Engine.Rendering;

namespace Voxelia
{
    class Program
    {   
        private static uint _texture;
        private static uint _tbo;
        private static uint _ebo;
        private static uint _vao;  
        private static uint _vbo;
        public static GL Gl { get; private set; }   
        private static IWindow _window;
        private static ShaderProgram _shader;
        
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
            _shader.Use();
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

            _shader = new("Assets/Shaders/default.vert", "Assets/Shaders/default.frag");

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
        
            int texLoc = Gl.GetUniformLocation(_shader.Id, "uTexture");
            Gl.Uniform1(texLoc, 0);
        } 
    }
}