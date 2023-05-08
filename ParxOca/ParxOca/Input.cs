using System;

public static class Input
{
	public static char GetInputChar()
	{
		ConsoleKeyInfo key;
		
		while (Console.KeyAvailable)
    		Console.ReadKey(true);
		
		key = Console.ReadKey(true);
		
		return key.KeyChar;
	}
	
	public static ConsoleKey GetInputKey()
	{
		ConsoleKeyInfo key;
		
		while (Console.KeyAvailable)
    		Console.ReadKey(true);
		
		key = Console.ReadKey(true);
		
		return key.Key;
	}
	
	public static int GetInputNum()
	{
		ConsoleKeyInfo key;
		
		while (Console.KeyAvailable)
    		Console.ReadKey(true);
		
		key = Console.ReadKey(true);
		
		return (int)Char.GetNumericValue(key.KeyChar);
	}
}
