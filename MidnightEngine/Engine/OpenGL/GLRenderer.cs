using Hexa.NET.GLFW;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.Backends.GLFW;
using Hexa.NET.ImGui.Backends.OpenGL3;
using Hexa.NET.OpenGL;
using HexaGen.Runtime;
using MidnightEngine.Editor.ImGui;
using System.Runtime.CompilerServices;
using GLFWwindowPtr = Hexa.NET.GLFW.GLFWwindowPtr;

namespace MidnightEngine.Engine.OpenGL
{
    internal class GLRenderer
    {
        private GLFWwindowPtr window;
        private ImGuiContext guiContext;
        private GL gl;

        private bool showDemoWindow = false;

        private MidnightEngine.Editor.ImGui.ImGuiManager imGuiManager;

        public void InitializeAndShowWindow(int width ,int height , string windowName , bool showDemoWindow )
        {
            this.showDemoWindow = showDemoWindow;
            GLFW.Init();

            GLFW.WindowHint(GLFW.GLFW_CONTEXT_VERSION_MAJOR, 4);
            GLFW.WindowHint(GLFW.GLFW_CONTEXT_VERSION_MINOR, 6);
            GLFW.WindowHint(GLFW.GLFW_OPENGL_PROFILE, GLFW.GLFW_OPENGL_CORE_PROFILE);
            GLFW.WindowHint(GLFW.GLFW_FOCUSED, 1);
            GLFW.WindowHint(GLFW.GLFW_RESIZABLE, 1);

            window = GLFW.CreateWindow(width, height, windowName, null, null);
            if (window.IsNull)
            {
                Console.WriteLine("Failed to create window.");
                GLFW.Terminate();
                return;
            }

            GLFW.MakeContextCurrent(window);

            gl = new GL(new GLFWGLContext(window));

            imGuiManager = new MidnightEngine.Editor.ImGui.ImGuiManager();
            imGuiManager.Initialize(gl, window);

            RunMainLoop();
        }

        private void RunMainLoop()
        {
            while (GLFW.WindowShouldClose(window) == 0)
            {
                GLFW.PollEvents();

                /*
                if (GLFW.GetKey(window, (int)GlfwKey.Escape) == GLFW.GLFW_PRESS)
                {
                    GLFW.SetWindowShouldClose(window, 1);
                }
                */
                GLFW.MakeContextCurrent(window);
                gl.ClearColor(0.15f, 0.15f, 0.15f, 1);
                gl.Clear(GLClearBufferMask.ColorBufferBit);

                // ImGui frame kezdése
                imGuiManager.NewFrame();

                // ImGui Demo UI elemek rajzolása
                if(this.showDemoWindow)
                imGuiManager.ShowDemoWindow();

                // UI elemek hozzáadása a saját logikád szerint...

                // ImGui renderelés
                imGuiManager.Render();


                GLFW.SwapBuffers(window);
            }
            imGuiManager.Shutdown();
            Cleanup();
        }

        private void Cleanup()
        {
            GLFW.DestroyWindow(window);
            GLFW.Terminate();
        }
    }

    internal unsafe class GLFWGLContext : IGLContext
    {
        private GLFWwindowPtr window;

        public GLFWGLContext(GLFWwindowPtr window)
        {
            this.window = window;
        }

        public nint Handle { get; }

        public bool IsCurrent { get; }

        public void Dispose()
        {
        }

        public nint GetProcAddress(string procName)
        {
            return (nint)GLFW.GetProcAddress(procName);
        }

        public bool IsExtensionSupported(string extensionName)
        {
            return GLFW.ExtensionSupported(extensionName) != 0;
        }

        public void MakeCurrent()
        {
            GLFW.MakeContextCurrent(window);
        }

        public void SwapBuffers()
        {
            GLFW.SwapBuffers(window);
        }

        public void SwapInterval(int interval)
        {
            GLFW.SwapInterval(interval);
        }

        public bool TryGetProcAddress(string procName, out nint procAddress)
        {
            procAddress = GetProcAddress(procName);
            return procAddress != 0;
        }
    }
}
