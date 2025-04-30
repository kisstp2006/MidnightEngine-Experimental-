using Hexa.NET.GLFW;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.Backends.GLFW;
using Hexa.NET.ImGui.Backends.OpenGL3;
using Hexa.NET.OpenGL;
using System.Runtime.CompilerServices;
using GLFWwindowPtr = Hexa.NET.GLFW.GLFWwindowPtr;

namespace MidnightEngine.Editor.ImGui
{
    internal class ImGuiManager
    {
        private ImGuiContextPtr guiContext;
        private GL gl;
        private GLFWwindowPtr window;
        private string glslVersion;

        public ImGuiManager()
        {
            glslVersion = "#version 330 core";
        }

        public void Initialize(GL gl, GLFWwindowPtr window)
        {
            this.gl = gl;
            this.window = window;

            // ImGui inicializálása
            guiContext = Hexa.NET.ImGui.ImGui.CreateContext();
            Hexa.NET.ImGui.ImGui.SetCurrentContext(guiContext);
            Hexa.NET.ImGui.ImGui.StyleColorsDark();

            // ImGui IO konfigurálása
            var io = Hexa.NET.ImGui.ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;     // Billentyűzet vezérlés engedélyezése
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;      // Gamepad vezérlés engedélyezése
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;         // Dokkolás engedélyezése
            io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;       // Multi-viewport engedélyezése
            io.ConfigViewportsNoAutoMerge = false;
            io.ConfigViewportsNoTaskBarIcon = false;

            // ImGui backend inicializálása
            ImGuiImplGLFW.SetCurrentContext(guiContext);

            if (!ImGuiImplGLFW.InitForOpenGL(Unsafe.BitCast<GLFWwindowPtr, Hexa.NET.ImGui.Backends.GLFW.GLFWwindowPtr>(window), true))
            {
                throw new Exception("Failed to init ImGui Impl GLFW");
            }

            ImGuiImplOpenGL3.SetCurrentContext(guiContext);
            if (!ImGuiImplOpenGL3.Init(glslVersion))
            {
                throw new Exception("Failed to init ImGui Impl OpenGL3");
            }
        }

        public void NewFrame()
        {
            ImGuiImplOpenGL3.NewFrame();
            ImGuiImplGLFW.NewFrame();
            Hexa.NET.ImGui.ImGui.NewFrame();
        }

        public void Render()
        {
            Hexa.NET.ImGui.ImGui.Render();
            Hexa.NET.ImGui.ImGui.EndFrame();

            GLFW.MakeContextCurrent(window);
            ImGuiImplOpenGL3.RenderDrawData(Hexa.NET.ImGui.ImGui.GetDrawData());

            var io = Hexa.NET.ImGui.ImGui.GetIO();
            if ((io.ConfigFlags & ImGuiConfigFlags.ViewportsEnable) != 0)
            {
                Hexa.NET.ImGui.ImGui.UpdatePlatformWindows();
                Hexa.NET.ImGui.ImGui.RenderPlatformWindowsDefault();
            }

            GLFW.MakeContextCurrent(window);
        }

        public void Shutdown()
        {
            ImGuiImplOpenGL3.Shutdown();
            ImGuiImplGLFW.Shutdown();
            Hexa.NET.ImGui.ImGui.DestroyContext();
        }

        // ImGui UI rajzolási függvények
        public void ShowDemoWindow()
        {
            Hexa.NET.ImGui.ImGui.ShowDemoWindow();
        }

        // További hasznos UI függvények
        public bool BeginWindow(string name, ref bool open, ImGuiWindowFlags flags = ImGuiWindowFlags.None)
        {
            return Hexa.NET.ImGui.ImGui.Begin(name, ref open, flags);
        }

        public void EndWindow()
        {
            Hexa.NET.ImGui.ImGui.End();
        }

        public bool Button(string label)
        {
            return Hexa.NET.ImGui.ImGui.Button(label);
        }

        public bool Checkbox(string label, ref bool value)
        {
            return Hexa.NET.ImGui.ImGui.Checkbox(label, ref value);
        }

        public bool SliderFloat(string label, ref float value, float min, float max)
        {
            return Hexa.NET.ImGui.ImGui.SliderFloat(label, ref value, min, max);
        }

        public bool ColorEdit3(string label, ref System.Numerics.Vector3 color)
        {
            return Hexa.NET.ImGui.ImGui.ColorEdit3(label, ref color);
        }

        public void Text(string text)
        {
            Hexa.NET.ImGui.ImGui.Text(text);
        }
    }
}