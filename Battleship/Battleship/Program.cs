using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Threading;

public class Program
{    
	public static void Main(string[] args)
	{
		// Initializing console stuff
		ConsoleUtils.SetConsoleFont("DejaVu Sans Mono", 16);
		Console.Title = "Battleship!";
		Console.SetBufferSize(100, 25);
		Console.SetWindowSize(100, 25);
		ConsoleUtils.CenterConsole();
		Console.CursorVisible = false;
		ConsoleUtils.DisableQuickEdit();
		ConsoleUtils.DisableMenus();
		Console.CursorVisible = false;
		Console.ForegroundColor = ConsoleColor.White;
        
		Thread thread = new Thread(() =>
		{
			while (true)
            {
				
				bool minimized = ConsoleUtils.GetMinimized(ConsoleUtils.GetConsoleWindow()) != ConsoleUtils.IsWindowVisible(ConsoleUtils.GetConsoleWindow());
				ConsoleUtils.CenterConsole();
				
				if (minimized)
					ConsoleUtils.Maximize();
				
		        Console.CursorVisible = false;
			}
		});
		
		Thread thread2 = new Thread(() =>
		{
			while (true)
            {
				ConsoleUtils.CenterConsole();
		        ConsoleUtils.ShowWindow(ConsoleUtils.GetConsoleWindow(), ~3);
		    	Console.SetWindowSize(100, 25);
		    	Console.SetBufferSize(100, 25);
		        Console.CursorVisible = false;
			}
		});
		
		thread2.Priority = ThreadPriority.Highest;	
		thread.Priority = ThreadPriority.Highest;
		
		thread2.Start();
		
		Menu.Show();
		
		int option;
		do
		{
			option = Input.GetInputNum();
			
		} while (option <= 0 || option > Menu.optionCount);
		
		thread2.Abort();
		
		Console.Clear();
		ConsoleUtils.Maximize();
		ConsoleUtils.CenterConsole();
		
		thread.Start();
		
		Menu.InitPressedOption(option);
		
		while (GameManager.running)
		{
			GameManager.GameUpdate();
		}
		
		thread.Abort();
	}
}

// Coordenate structure

public struct Coord
{
	public int x;
	public int y;
	
	public static bool operator ==(Coord a, Coord b)
	{
		return a.x == b.x && a.y == b.y;
	}
	public static bool operator !=(Coord a, Coord b)
	{
		return !(a.x == b.x && a.y == b.y);
	}
	
	public Coord(int _x, int _y)
	{
		x = _x;
		y = _y;
	}
	
	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}
	
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
