using System;

public static class Input
{
	public static char GetInputChar()
	{
		ConsoleKeyInfo key;
		
		do
		{
			key = Console.ReadKey(true);
		} while (Console.KeyAvailable);
		
		return key.KeyChar;
	}
	
	public static ConsoleKey GetInputKey()
	{
		ConsoleKeyInfo key;
		
		do
		{
			key = Console.ReadKey(true);
		} while (Console.KeyAvailable);
		
		return key.Key;
	}
	
	public static int GetInputNum()
	{
		ConsoleKeyInfo key;
		
		do
		{
			key = Console.ReadKey(true);
		} while (Console.KeyAvailable);
		
		return (int)Char.GetNumericValue(key.KeyChar);
	}
}
