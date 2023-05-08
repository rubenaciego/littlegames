using System;
using System.Threading;

public static class IO
{
	public static void ClearScreen(int initialX, int finalX, int initialY, int finalY)
	{
		for (int i = initialX; i < finalX; i++)
		{
			for (int j = initialY; j < finalY; j++)
			{
				Console.SetCursorPosition(i, j);
				Console.Write(' ');
			}
		}
	}
	
	public static int AskUser(string msg, int minValue, int maxValue)
    {
        int returnValue = 0;
        bool finish;

        do
        {
            finish = true;
            Console.WriteLine(msg);

            try
            {
                returnValue = int.Parse(Console.ReadLine());

                if (returnValue < minValue || returnValue > maxValue)
                    throw new OverflowException();
            }

            catch (OverflowException)
            {
                Console.WriteLine("Número massa gran o petit. Ha de ser un número entre {0} i {1}.",
                    minValue, maxValue);
                finish = false;
            }
            catch (FormatException)
            {
                Console.WriteLine("Has d'introduir un número entre {0} i {1}",
                    minValue, maxValue);
                finish = false;
            }

            if (!finish)
            {
                Console.Write("Polsa una tecla per a continuar...");
                Console.ReadKey(true);
                Console.WriteLine('\n');
            }

        } while (!finish);

        Console.WriteLine();

        return returnValue;
    }

    public static double AskUser(string msg, double minValue, double maxValue)
    {
        double returnValue = 0;
        bool finish;

        do
        {
            finish = true;
            Console.Write(msg);

            try
            {
                returnValue = double.Parse(Console.ReadLine());

                if (returnValue < minValue || returnValue > maxValue)
                    throw new OverflowException();
            }

            catch (OverflowException)
            {
                Console.WriteLine("Número massa gran o petit. Ha de ser un número entre {0} i {1}.",
                    minValue, maxValue);
                finish = false;
            }
            catch (FormatException)
            {
                Console.WriteLine("Has d'introduir un número enter entre {0} i {1}",
                    minValue, maxValue);
                finish = false;
            }

            if (!finish)
            {
                Console.Write("Polsa una tecla per a continuar...");
                Console.ReadKey(true);
                Console.WriteLine('\n');
            }

        } while (!finish);

        Console.WriteLine();

        return returnValue;
    }
    
    public static int ShowMenu(string msg, params string[] options)
    {
    	int returnValue = 0;
        bool finish;

        do
        {
            finish = true;
            
             Console.WriteLine(msg);

	        for (int i = 0; i < options.Length; i++)
	            Console.WriteLine("{0}. {1}", i + 1, options[i]);
	        
	        Console.Write("Escull una opció: ");

            try
            {
                returnValue = int.Parse(Console.ReadLine());

                if (returnValue < 1 || returnValue > options.Length)
                    throw new OverflowException();
            }

            catch (OverflowException)
            {
                Console.WriteLine("Número massa gran o petit. Ha de ser un número enter entre {0} i {1}.",
                    1, options.Length);
                finish = false;
            }
            catch (FormatException)
            {
                Console.WriteLine("Has d'introduir un número enter entre {0} i {1}",
                    1, options.Length);
                finish = false;
            }

            if (!finish)
            {
                Console.Write("Polsa una tecla per a continuar...");
                Console.ReadKey(true);
                Console.WriteLine('\n');
            }

        } while (!finish);

        Console.WriteLine();
        
        return returnValue;
    }

    public static ConsoleColor AskColor(string message, int x, int y)
    {
        int returnValue = 0;
        bool finish;

        string[] options = { "Verd", "Blau", "Vermell", "Groc" };
        ConsoleColor[] optionsColors = { ConsoleColor.Green, ConsoleColor.Cyan,
            ConsoleColor.Red, ConsoleColor.Yellow };

        do
        {
            finish = true;

            Console.SetCursorPosition(x, y);
            Console.Write(message);

            for (int i = 0; i < options.Length; i++)
            {
                Console.SetCursorPosition(x, y + i + 1);
                Console.Write("{0}. ", i + 1);
                Console.ForegroundColor = optionsColors[i];
                Console.Write(options[i]);
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            Console.SetCursorPosition(x, y + options.Length + 1);
            Console.Write("Escull una opció: ");

            Connect4Xtreme.Game.cursorVisible = true;
            returnValue = Input.GetInputNum();
            Connect4Xtreme.Game.cursorVisible = false;

            if (returnValue != -1)
                Console.Write(returnValue);

            if (returnValue < 1 || returnValue > options.Length)
            {
                finish = false;
                Console.SetCursorPosition(x, y + options.Length + 2);
                Console.Write("Has d'introduir un número entre {0} i {1}", 1, options.Length);
                Thread.Sleep(1000);
                Console.SetCursorPosition(x, y + options.Length + 2);
                Console.Write("                                     ");
                Console.SetCursorPosition(x, y + options.Length + 1);
                Console.Write("Escull una opció:     ");
            }
            else
            {
                Console.SetCursorPosition(x, y + options.Length + 2);
                Console.Write("Fitxes de color: ");
                Console.ForegroundColor = optionsColors[returnValue - 1];
                Console.Write(options[returnValue - 1]);
                Console.ForegroundColor = ConsoleColor.Gray;
            }

        } while (!finish);

        Thread.Sleep(1000);

        for (int i = y; i <= y + options.Length + 2; i++)
        {
            Console.SetCursorPosition(x, i);
            Console.Write("                                     ");
        }

        return optionsColors[returnValue - 1]; ;
    }

    public static int AskInt(string message, int min, int max, int x, int y)
    {
        Console.SetCursorPosition(x, y);
        Console.Write(message);

        int resposta = -1;

        do
        {
            Connect4Xtreme.Game.cursorVisible = true;
            resposta = Input.GetInputNum();
            Connect4Xtreme.Game.cursorVisible = false;

            if (resposta != -1)
                Console.Write(resposta);

            if (!(resposta >= min && resposta <= max))
            {
                Console.SetCursorPosition(x, y + 1);
                Console.Write("Has d'introduïr un número entre {0} i {1}.", min, max);
                Thread.Sleep(1000);
                Console.SetCursorPosition(x, y + 1);
                Console.Write("                                      ");
                Console.SetCursorPosition(x + message.Length, y);
                Console.Write(' ');
                Console.SetCursorPosition(x + message.Length, y);
            }

        } while (!(resposta >= min && resposta <= max));

        Thread.Sleep(500);

        return resposta;
    }
}
