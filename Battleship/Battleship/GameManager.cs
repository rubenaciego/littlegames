using System;
using System.Threading;

public static class GameManager
{
	public static bool running;
	public const int columns = 16;
	public const int rows = 9;
	public static Player localPlayer;
	public static Player playerAI;
	public static int turn = 0; // if it's 0 plays the first player and if it's 1 the other
	
	private const int marginDistanceX = 6;
	private const int marginDistanceY = 2;
	private const int marginX = 5;
	private const int marginY = 3;
	private const int width = 98;
	private const int height = 20;
	
	public static void InitSinglePlayer()
	{
		// Initialization
		Console.Clear();
		running = true;
		
		RenderScenary();
		localPlayer = new Player(true);
		localPlayer.Win("HAS GUANYAT!!!");
		
		RenderLowerScenary();
		playerAI = new PlayerAI();
		
		UpdateHUD();
	}

	private static void RenderScenary()
	{
		for (int i = 1; i <= 10; i++)
		{
			for (int j = 2; j <= width; j++)
			{
				int a = i * 2;
				Console.SetCursorPosition(j, a);
				Console.Write('_');
			}
		}
		
		for (int j = 2; j <= width; j += 6)
		{
			for (int i = 3; i <= height; i++)
			{
				Console.SetCursorPosition(j, i);
				Console.WriteLine('│');
			}
		}
		
		for (int i = 0; i < columns; i++)
		{
			Coord c = GetReadlCoord(new Coord(i, 0));
			Console.SetCursorPosition(c.x, 1);
			Console.Write(i);
		}
		
		for (int i = 0; i < rows; i++)
		{
			Coord c = GetReadlCoord(new Coord(0, i));
			Console.SetCursorPosition(1, c.y);
			Console.Write(i);
		}
	}
	
	public static void GameUpdate()
	{
		// Main game loop
		UpdateHUD();
		
		if (turn == 0)
			localPlayer.Play();
		else if (turn == 1)
			playerAI.Play();
		
		Render();
		UpdateHUD();
		CheckGameOver();
	}	
	
	public static void Render()
	{
		localPlayer.RenderBoats();
		playerAI.RenderBoats();
	}
	
	public static Coord GetReadlCoord(Coord coordenate)
	{
		Coord coord;
		
		coord.x = (coordenate.x * marginDistanceX) + marginX;
		coord.y = (coordenate.y * marginDistanceY) + marginY;
		return coord;
	}
	
	private static void RenderLowerScenary()
	{
		for (int i = 1; i <= 10; i++)
		{
			for (int j = 2; j <= width; j++)
			{
				int a = i * 2 + 1;
				Console.SetCursorPosition(j, a + 21);
				Console.Write('_');
			}
		}
		
		for (int j = 2; j <= width; j += 6)
		{
			for (int i = 4; i <= height + 1; i++)
			{
				Console.SetCursorPosition(j, i + 21);
				Console.WriteLine('│');
			}
		}
		
		for (int i = 0; i < columns; i++)
		{
			Coord c = GetReadlCoord(new Coord(i, 0));
			Console.SetCursorPosition(c.x, 2 + 21);
			Console.Write(i);
		}
		
		for (int i = 0; i < rows; i++)
		{
			Coord c = GetReadlCoord(new Coord(0, i));
			Console.SetCursorPosition(1, c.y + 22);
			Console.Write(i);
		}
	}
	
	public static void UpdateHUD()
	{
		// Updates HUD
		
		Console.SetCursorPosition(100, 5);
		Console.Write("Vaixells que queden:");
		
		Console.SetCursorPosition(100, 6);
		Console.Write("  Grossos: {0}/2", localPlayer.boatsAlive[2]);
		
		Console.SetCursorPosition(100, 7);
		Console.Write("  Mitjans: {0}/3", localPlayer.boatsAlive[1]);
		
		Console.SetCursorPosition(100, 8);
		Console.Write("  Petits: {0}/5", localPlayer.boatsAlive[0]);
		
		
		Console.SetCursorPosition(100, 27);
		Console.Write("Vaixells que queden:");
		
		Console.SetCursorPosition(100, 28);
		Console.Write("  Grossos: {0}/2", playerAI.boatsAlive[2]);
		
		Console.SetCursorPosition(100, 29);
		Console.Write("  Mitjans: {0}/3", playerAI.boatsAlive[1]);
		
		Console.SetCursorPosition(100, 30);
		Console.Write("  Petits: {0}/5", playerAI.boatsAlive[0]);
	}
	
	public static void PrintMessage(string msg, bool upperScreen)
	{
		ClearScr(upperScreen);
		Thread.Sleep(100);
		
		if (upperScreen)
			Console.SetCursorPosition(105, 11);
		else
			Console.SetCursorPosition(105, 33);
		
		Console.Write(msg);
	}
	
	public static void PrintMessage()
	{
		Console.SetCursorPosition(100, 33);
		Console.Write(@" _   __     _          ____           ___                  __  ______");
		Console.SetCursorPosition(100, 34);
		Console.Write(@"| | / /__ _(_)_ _____ / / / ___ ___  / _/__  ___  ______ _/ /_/ / / /");
		Console.SetCursorPosition(100, 35);
		Console.Write(@"| |/ / _ `/ /\ \ / -_) / / / -_) _ \/ _/ _ \/ _ \(_-< _ `/ __/_/_/_/");
		Console.SetCursorPosition(100, 36);
		Console.Write(@"|___/\_,_/_//_\_\\__/_/_/  \__/_//_/_/ \___/_//_/___|_,_/\__(_|_|_)");                                                                     
	}
	
	public static void ClearScr(bool upperScreen)
	{
		for (int i = 100; i < Console.BufferWidth; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				Console.SetCursorPosition(i, j + (upperScreen ? 11 : 33));
				Console.Write(' ');
			}
		}
	}
	
	private static void CheckGameOver()
	{
		bool localPlayerWins = true;
		bool playerIAWins = true;
		
		foreach (Boat b in localPlayer.bigBoats)
		{
			if (b.isDestroyed == false)
				playerIAWins = false;
		}
		
		foreach (Boat b in localPlayer.mediumBoats)
		{
			if (b.isDestroyed == false)
				playerIAWins = false;
		}
		
		foreach (Boat b in localPlayer.smallBoats)
		{
			if (b.isDestroyed == false)
				playerIAWins = false;
		}
		
		foreach (Boat b in playerAI.bigBoats)
		{
			if (b.isDestroyed == false)
				localPlayerWins = false;
		}
		
		foreach (Boat b in playerAI.mediumBoats)
		{
			if (b.isDestroyed == false)
				localPlayerWins = false;
		}
		
		foreach (Boat b in playerAI.smallBoats)
		{
			if (b.isDestroyed == false)
				localPlayerWins = false;
		}
		
		if (playerIAWins)
		{
			running = false;
			playerAI.Win("HAS PERDUT!!! :(");
		}
		else if (localPlayerWins)
		{
			running = false;
			localPlayer.Win("HAS GUANYAT!!!");
		}
	}
}
