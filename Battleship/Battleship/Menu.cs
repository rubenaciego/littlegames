using System;

public static class Menu
{
	public static int optionCount = 1;
	
	public static void Show()
	{
		DrawTitle(50, 2);
		DrawBoat(2);
		
		Console.SetCursorPosition(75, 15);
		Console.Write("1. Solo play");
	}
	
	public static void InitPressedOption(int num)
	{
		switch (num)
		{
			case 1:
				GameManager.InitSinglePlayer();
				break;
		}
	}
	
	private static void DrawBoat(int y)
	{
		Console.SetCursorPosition(0, y);
		Console.WriteLine(@"
                   |    |    |
                  )_)  )_)  )_)
                 )___))___))___)
                )____)____)_____)
              _____|____|____|_____
     ---------\                   /---------
           ^^^^^ ^^^^^^^^^^^^^^^^^^^^^
           ^^^^      ^^^^     ^^^    ^^
                 ^^^^      ^^^");
		Console.Write(@"
		
                                |    |    |
                               )_)  )_)  )_)
                              )___))___))___)
                             )____)____)_____)
                           _____|____|____|_____
                  ---------\                   /---------
                        ^^^^^ ^^^^^^^^^^^^^^^^^^^^^
                        ^^^^      ^^^^     ^^^    ^^
                              ^^^^      ^^^");
	}
	
	private static void DrawTitle(int x, int y)
	{
		Console.SetCursorPosition(x, y);
		Console.Write(@"______       _   _   _          _     _       _ ");
		Console.SetCursorPosition(x, y + 1);
		Console.Write(@"| ___ \     | | | | | |        | |   (_)     | |");
		Console.SetCursorPosition(x, y + 2);
		Console.Write(@"| |_/ / __ _| |_| |_| | ___ ___| |__  _ _ __ | |");
		Console.SetCursorPosition(x, y + 3);
		Console.Write(@"| ___ \/ _` | __| __| |/ _ / __| '_ \| | '_ \| |");
		Console.SetCursorPosition(x, y + 4);
		Console.Write(@"| |_/ | (_| | |_| |_| |  __\__ | | | | | |_) |_|");
		Console.SetCursorPosition(x, y + 5);
		Console.Write(@"\____/ \__,_|\__|\__|_|\___|___|_| |_|_| .__/(_)");
		Console.SetCursorPosition(x, y + 6);
		Console.Write(@"                                       | |      ");
		Console.SetCursorPosition(x, y + 7);
		Console.Write(@"                                       |_|      ");
	}
}
