using System;
using System.Drawing;

using Silk.NET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Voxelia
{
    class Program
    {   
        private static uint _vao;  
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

        private static void OnRender(double deltaTime)
        {
            Gl.Clear(ClearBufferMask.ColorBufferBit);
        }

        private static void OnLoad() 
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
        }
    }
}