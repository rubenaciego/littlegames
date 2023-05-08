using System;

public static class Input
{
	public static char GetInputChar()
	{
		return GetKeyInfo().KeyChar;
	}
	
	public static ConsoleKey GetInputKey()
	{		
		return GetKeyInfo().Key;
	}
	
	public static int GetInputNum()
	{		
		return (int)Char.GetNumericValue(GetKeyInfo().KeyChar);
	}

    public static ConsoleKeyInfo GetKeyInfo()
    {
        while (Console.KeyAvailable)
            Console.ReadKey(true);

        return Console.ReadKey(true);
    }
}
