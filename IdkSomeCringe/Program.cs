using IdkSomeCringe.LowLevel;

namespace IdkSomeCringe
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int height = 30;
            int width = 90;

            Console.SetWindowSize(width, height);
            Console.SetBufferSize(width + 1, height + 1);

            Console.CursorVisible = false;
            Console.Title = "Snake idk";

            Console.OutputEncoding = System.Text.Encoding.Unicode;

            PrintManager.Init(width, height, (index, count) =>
            {
                double prog = index / (double)Math.Clamp(count - 1, 1, count);
                return new CharInfo(PrintManager.GetSnakePartChar(index), PrintManager.ProcessGradient(prog), ConsoleColor.Black);
            });

            while (true)
            {
                PrintManager.Update();
                Thread.Sleep(1);
            }
        }
    }
}
