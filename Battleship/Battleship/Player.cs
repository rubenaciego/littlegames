using System;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

public class Player
{	
	public Boat[] bigBoats;
	public Boat[] mediumBoats;
	public Boat[] smallBoats;
	public int[] boatsAlive;
	protected bool localPlayer;
	
	public Player(bool _localPlayer)
	{
		localPlayer = _localPlayer;
		bigBoats = new Boat[2];
		mediumBoats = new Boat[3];
		smallBoats = new Boat[5];
		boatsAlive = new int[3];
		
		boatsAlive[0] = (int)BoatType.Big;
		boatsAlive[1] = (int)BoatType.Medium;
		boatsAlive[2] = (int)BoatType.Small;
		
		if (localPlayer)
			SetBoatsPosition();
	}
	
	public void Win(string msg)
	{
		Console.Clear();
		ConsoleUtils.SetConsoleFont("DejaVu Sans Mono", 50);
		
		Thread printThread = new Thread(()=>
		{		
			while (true)
			{
				Console.SetCursorPosition(18, 5);
				Console.Write(msg);
				Thread.Sleep(800);
				Console.SetCursorPosition(18, 5);
				Console.Write("                    ");
				Thread.Sleep(300);
			}
		});
		
		printThread.Start();
		Console.ReadKey(true);
		printThread.Abort();
	}
	
	public virtual void Play()
	{
		UpdateBoatCount();
		GameManager.UpdateHUD();
		
		bool finish = false;
		
		Coord c = new Coord(0, 0);
		
		Coord aux = GameManager.GetReadlCoord(c);
		Console.SetCursorPosition(aux.x, aux.y + PlayerAI.offset);
		Console.Write('_');
		
		while (!finish)
		{
			ConsoleKey key = Input.GetInputKey();
			
			aux = GameManager.GetReadlCoord(c);
			Console.SetCursorPosition(aux.x, aux.y + PlayerAI.offset);
			Console.Write(' ');
			
			switch (key)
			{
				case ConsoleKey.UpArrow:
					if (c.y > 0)
						c.y--;
					break;
				case ConsoleKey.DownArrow:
					if (c.y < GameManager.rows - 1)
						c.y++;
					break;
				case ConsoleKey.RightArrow:
					if (c.x < GameManager.columns - 1)
						c.x++;
					break;
				case ConsoleKey.LeftArrow:
					if (c.x > 0)
						c.x--;
					break;
				case ConsoleKey.Enter:
					Console.SetCursorPosition(100, 30);
					Console.Write("                ");
					
					switch (GameManager.playerAI.CheckBoatCollision(c))
					{
						case BoatState.Water:
							((PlayerAI)GameManager.playerAI).AddWaterCoord(c);
							GameManager.ClearScr(false);
							finish = true;
							break;
							
						case BoatState.Touched:
							GameManager.PrintMessage("Vaixell tocat!", false);
							
							finish = true;
							break;
							
						case BoatState.Sinked:
							//GameManager.PrintMessage("Baixell enfonsat!!!", false);
							GameManager.PrintMessage();
							finish = true;
							break;
					}
						
					break;
			}
			
			Console.ForegroundColor = ConsoleColor.White;
			aux = GameManager.GetReadlCoord(c);
			Console.SetCursorPosition(aux.x, aux.y + PlayerAI.offset);
			if (!finish)
				Console.Write('_');
			
			GameManager.playerAI.RenderBoats();
		}
		
		GameManager.turn = 1;
		UpdateBoatCount();
	}
	
	protected void UpdateBoatCount()
	{
		boatsAlive[2] = 2;
		boatsAlive[1] = 3;
		boatsAlive[0] = 5;
		
		foreach (Boat b in bigBoats)
		{
			if (b.isDestroyed)
				boatsAlive[2]--;
		}
		
		foreach (Boat b in mediumBoats)
		{
			if (b.isDestroyed)
				boatsAlive[1]--;
		}
		
		foreach (Boat b in smallBoats)
		{
			if (b.isDestroyed)
				boatsAlive[0]--;
		}
	}
	
	public virtual BoatState CheckBoatCollision(Coord c)
	{
		foreach (Boat b in bigBoats)
		{	
			for (int i = 0; i < (int)b.boatType; i++)
			{	
				Coord bPos = b.position;
				if (b.isVertical)
					bPos.y += i;
				else
					bPos.x += i;
				
				if (c == bPos)
					return b.TakeDamage(c);
			}
		}
		
		foreach (Boat b in mediumBoats)
		{				
			for (int i = 0; i < (int)b.boatType; i++)
			{	
				Coord bPos = b.position;
				if (b.isVertical)
					bPos.y += i;
				else
					bPos.x += i;
				
				if (c == bPos)
					return b.TakeDamage(c);
			}
		}
		
		foreach (Boat b in smallBoats)
		{
			for (int i = 0; i < (int)b.boatType; i++)
			{	
				Coord bPos = b.position;
				if (b.isVertical)
					bPos.y += i;
				else
					bPos.x += i;
				
				if (c == bPos)
					return b.TakeDamage(c);
			}
		}
	
		return BoatState.Water;
	}
	
	private void SetBoatsPosition()
	{
		for (int i = 0; i < bigBoats.Length; i++)
		{
			Coord c;
			c.x = 0; c.y = 0;
			bigBoats[i] = new Boat(BoatType.Big, c, ConsoleColor.Green);
			SetBoatPosition(bigBoats[i]);
		}
		
		for (int i = 0; i < mediumBoats.Length; i++)
		{
			Coord c;
			c.x = 0; c.y = 0;
			mediumBoats[i] = new Boat(BoatType.Medium, c, ConsoleColor.Blue);
			SetBoatPosition(mediumBoats[i]);
		}
		
		for (int i = 0; i < smallBoats.Length; i++)
		{
			Coord c;
			c.x = 0; c.y = 0;
			smallBoats[i] = new Boat(BoatType.Small, c, ConsoleColor.Yellow);
			SetBoatPosition(smallBoats[i]);
		}
	}
	
	void SetBoatPosition(Boat b)
	{	
		bool finish = false;
		
		while (!finish)
		{
			b.Render();
			RenderBoats();
			
			ConsoleKey key = Input.GetInputKey();
			
			switch (key)
			{
				case ConsoleKey.LeftArrow:
					if (b.position.x > 0)
					{
						b.Flush();
						b.position.x--;
					}
					break;
				case ConsoleKey.RightArrow:
					if (!b.isVertical)
					{
						if ((b.position.x + (int)b.boatType) < GameManager.columns)
						{
							b.Flush();
							b.position.x++;
						}
					}
					else
					{
						if (b.position.x < GameManager.columns - 1)
						{
							b.Flush();
							b.position.x++;
						}
					}
					break;
				case ConsoleKey.UpArrow:
					if (b.position.y >= 1)
					{
						b.Flush();
						b.position.y--;
					}
					break;
				case ConsoleKey.DownArrow:
					if (!b.isVertical)
					{
						if (b.position.y < GameManager.rows - 1)
						{
							b.Flush();
							b.position.y++;
						}
					}
					else
					{
						if ((b.position.y + (int)b.boatType) < GameManager.rows)
						{
							b.Flush();
							b.position.y++;
						}
					}
					break;
				case ConsoleKey.R:
					b.Flush();
					
					if (b.isVertical && ((b.position.x + (int)b.boatType) > GameManager.columns - 1))
					{
					    b.position.x = (GameManager.columns) - (int)b.boatType;
					}
					
					else if (!b.isVertical && ((b.position.y + (int)b.boatType) > GameManager.rows - 1))
					{
						b.position.y = (GameManager.rows) - (int)b.boatType;
					}
					
					b.isVertical = !b.isVertical;
					break;
				case ConsoleKey.Enter:
					
					if (!CanPlaceBoat(b))
					{
						ConsoleUtils.MessageBoxW((int)ConsoleUtils.GetConsoleWindow(),
						            "No pots posar un baixell a sobre o al costat d'un altre!",
						            "Alerta", (uint) 0x00000000L | (uint)0x00000010L | (uint)0x00010000L);
						break;
					}
					
					b.placed = true;
					finish = true;
					break;
			}
			
			b.Render();
			RenderBoats();
		}
		
	}
	
	protected virtual bool CanPlaceBoat(Boat b)
	{
		bool first = CanPlaceBoatHere(b);
		b.position.x++;
		
		bool second = CanPlaceBoatHere(b);
		b.position.x -= 2;
		
		bool third = CanPlaceBoatHere(b);
		b.position.x++;
		b.position.y++;
		
		bool fourth = CanPlaceBoatHere(b);
		b.position.y -= 2;
		
		bool fifth = CanPlaceBoatHere(b);
		b.position.y++;
		
		b.Render();
		
		return first && second && third && fourth && fifth;
	}
	
	protected bool CanPlaceBoatHere(Boat boat)
	{
		// Detects if the boat can be placed on it's coordeantes
		for (int j = 0; j < (int)boat.boatType; j++)
		{
			Coord boatPos = boat.position;
			if (boat.isVertical)
				boatPos.y += j;
			else
				boatPos.x += j;
			
			foreach (Boat b in bigBoats)
			{
				if (boat == b ||  b == null || b.placed == false)
					continue;
				
				for (int i = 0; i < (int)b.boatType; i++)
				{	
					Coord bPos = b.position;
					if (b.isVertical)
						bPos.y += i;
					else
						bPos.x += i;
					
					if (bPos == boatPos)
						return false;
				}
			}
			foreach (Boat b in mediumBoats)
			{
				if (boat == b ||  b == null || b.placed == false)
					continue;
				
				for (int i = 0; i < (int)b.boatType; i++)
				{	
					Coord bPos = b.position;
					if (b.isVertical)
						bPos.y += i;
					else
						bPos.x += i;
					
					if (bPos == boatPos)
						return false;
				}
			}
			foreach (Boat b in smallBoats)
			{
				if (boat == b ||  b == null || b.placed == false)
					continue;
				
				for (int i = 0; i < (int)b.boatType; i++)
				{	
					Coord bPos = b.position;
					if (b.isVertical)
						bPos.y += i;
					else
						bPos.x += i;
					
					if (bPos == boatPos)
						return false;
				}
			}
		}
		
		return true;
	}
	
	public virtual void RenderBoats()
	{
		foreach (Boat b in bigBoats)
		{
			if (b != null && b.placed)
				b.Render();
		}
		
		foreach (Boat b in mediumBoats)
		{
			if (b != null && b.placed)
				b.Render();
		}
		
		foreach (Boat b in smallBoats)
		{
			if (b != null && b.placed)
				b.Render();
		}
	}
}
