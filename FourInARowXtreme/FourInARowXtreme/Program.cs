using System;
using System.Threading;

namespace Connect4Xtreme
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    ConsoleUtils.ShowWindow(ConsoleUtils.GetConsoleWindow(), ~3);
                    Console.SetWindowSize(120, 30);
                    Console.SetBufferSize(120, 30);
                    Console.CursorVisible = Game.cursorVisible;
                    Thread.Sleep(10);
                }
            });

            thread.Start();
            Console.Title = "Four in a Row Extreme Edition";
            ConsoleUtils.DisableMenus();
            ConsoleUtils.CenterConsole();
            ConsoleUtils.SetConsoleFont("Consolas", 16);

            Game game = new Game();
            game.Run();

            thread.Abort();
        }
    }
}
