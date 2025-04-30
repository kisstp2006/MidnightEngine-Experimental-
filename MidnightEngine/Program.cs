using MidnightEngine.Engine.OpenGL;
namespace MidnightEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Loading Core...");
            Console.WriteLine("Creating Window...");

            GLRenderer renderer = new GLRenderer();
            renderer.InitializeAndShowWindow(1280 , 720 , "Midnight Engine", true);
        }
    }
}
