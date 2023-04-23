using IdkSomeCringe.API;
using IdkSomeCringe.LowLevel;
using System.Drawing;

namespace IdkSomeCringe
{
    internal class Program
    {
        public static int Framerate { get; set; } = 1000;

        static void Main(string[] args)
        {
            int height = Console.WindowHeight = 30;
            int width = Console.WindowWidth = 90;

            Console.CursorVisible = false;
            Console.Title = "Idk";

            PrintManager.Init(width, height);

            while (true)
            {
                FastConsole.Write(PrintManager.Chars);
                PrintManager.Update();
                Thread.Sleep(1);
            }
        }
    }
}