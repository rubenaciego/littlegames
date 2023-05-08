using System;
using System.Threading;

public static class Program
{
	public static void Main(string[] args)
	{		
		Thread thread = new Thread(() =>
		{
			while (true)
		    {
				ConsoleUtils.ShowWindow(ConsoleUtils.GetConsoleWindow(), ~3);
				Console.WindowWidth = 165;
				Console.WindowHeight = 40;
		    	Console.SetBufferSize(165, 40);
				Console.CursorVisible = Game.cursorVisible; 
				Thread.Sleep(10);
		    }
		});
		
		thread.Start();
		Console.Title = "ParxOca";
		ConsoleUtils.DisableMenus();
		ConsoleUtils.DisableQuickEdit();
		ConsoleUtils.CenterConsole();
		ConsoleUtils.SetConsoleFont("Consolas", 16);
		
		Game game = new Game();
		game.Start();
		
		thread.Abort();
	}
	
	public static T ToEnum<T>(this string text)
	{
    	return (T) Enum.Parse(typeof(T), text, true);
	}
}
