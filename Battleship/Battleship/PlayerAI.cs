using System;
using System.Collections.Generic;

public class PlayerAI : Player
{
	public const int offset = 22;
	public Stack<Coord> waterCoords;

	// AI variables
	private bool previouslyTouched;
	private Coord touchedPosCoord, nextCoord;
	private Coord firstCoordAttacked;
	private int coordDir;
	private List<Coord> coordsNotAttacked;
	private int attackedCoordsIterator;
	private List<int> possibleDirections;
	
	protected override bool CanPlaceBoat(Boat b)
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
		
		return first && second && third && fourth && fifth;
	}
	
	public override void Play()
	{
		// AI for the CPU
		UpdateBoatCount();
		GameManager.UpdateHUD();
		
		if (attackedCoordsIterator >= 144)
		{
			GameManager.running = false;
			return;
		}
		
		Random rand = new Random();
		Coord c = new Coord(0, 0);
		
		
		if (previouslyTouched)
		{
			if (coordDir < 0)
			{
				bool finish = false;
				do
				{
					bool finish2 = false;
					do
					{
						coordDir = rand.Next(0, 4);
						
						foreach (int i in possibleDirections)
						{
							if (i == coordDir)
								finish2 = true;
						}
						
					} while (!finish2);
					
					nextCoord = touchedPosCoord;
					
					switch (coordDir)
					{
						case 0:
							nextCoord.y++;
							break;
							
						case 1:
							nextCoord.x++;
							break;
							
						case 2:
							nextCoord.y--;
							break;
							
						case 3:
							nextCoord.x--;
							break;
					}
					
					foreach (Coord i in coordsNotAttacked)
					{
						if (nextCoord == i)
							finish = true;
					}
					
				} while (!finish);
				
				switch (GameManager.localPlayer.CheckBoatCollision(nextCoord))
				{
					case BoatState.Water:
						possibleDirections.Remove(coordDir);
						Coord p = GameManager.GetReadlCoord(nextCoord);
						Console.SetCursorPosition(p.x, p.y);
						Console.ForegroundColor = ConsoleColor.Cyan;
						Console.Write('0');
						coordDir = -1;
						GameManager.ClearScr(true);
						
						break;
						
					case BoatState.Touched:
						touchedPosCoord = nextCoord;
						GameManager.PrintMessage("T'han tocat un vaixell!", true);
						break;
						
					case BoatState.Sinked:
						possibleDirections.Clear();
						
						for (int i = 0; i < 4; i++)
							possibleDirections.Add(i);
						
						touchedPosCoord = new Coord(0, 0);
						firstCoordAttacked = new Coord(0, 0);
						previouslyTouched = false;
						coordDir = -1;
						GameManager.PrintMessage("T'han enfonsat un vaixell!!!", true);
						
						break;
				}
			}
			
			else
			{
				nextCoord = touchedPosCoord;
				switch (coordDir)
				{
					case 0:
						nextCoord.y++;
						break;
						
					case 1:
						nextCoord.x++;
						break;
						
					case 2:
						nextCoord.y--;
						break;
						
					case 3:
						nextCoord.x--;
						break;
				}
				
				bool canAttack = false;
				
				foreach (Coord i in coordsNotAttacked)
				{
					if (i == nextCoord)
					{
						canAttack = true;
						break;
					}
				}
				
				if (!canAttack)
				{
					SwapDirection();
					
					switch (coordDir)
					{
						case 0:
							nextCoord.y++;
							break;
							
						case 1:
							nextCoord.x++;
							break;
							
						case 2:
							nextCoord.y--;
							break;
							
						case 3:
							nextCoord.x--;
							break;
					}
				}
					
				switch (GameManager.localPlayer.CheckBoatCollision(nextCoord))
				{
					case BoatState.Water:
						Coord p = GameManager.GetReadlCoord(nextCoord);
						Console.SetCursorPosition(p.x, p.y);
						Console.ForegroundColor = ConsoleColor.Cyan;
						Console.Write('0');
						
						SwapDirection();
						touchedPosCoord = nextCoord;
						GameManager.ClearScr(true);
						
						break;
						
					case BoatState.Touched:
						touchedPosCoord = nextCoord;
						GameManager.PrintMessage("T'han tocat un vaixell!", true);
						
						break;
						
					case BoatState.Sinked:
						possibleDirections.Clear();
						
						for (int i = 0; i < 4; i++)
							possibleDirections.Add(i);
						
						touchedPosCoord = new Coord(0, 0);
						firstCoordAttacked = new Coord(0, 0);
						previouslyTouched = false;
						coordDir = -1;
						GameManager.PrintMessage("T'han enfonsat un vaixell!", true);
										
						break;
				}
			}
			
			c = nextCoord;
		}
		else
		{
			int index = rand.Next(0, 144 - attackedCoordsIterator);
			c = coordsNotAttacked[index];
			
			Coord aux = GameManager.GetReadlCoord(c);
			Console.SetCursorPosition(aux.x, aux.y);
			
			switch (GameManager.localPlayer.CheckBoatCollision(c))
			{
				case BoatState.Water:
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.Write('0');
					previouslyTouched = false;
					GameManager.ClearScr(true);
					break;
					
				case BoatState.Touched:
					previouslyTouched = true;
					touchedPosCoord = c;
					firstCoordAttacked = c;
					GameManager.PrintMessage("T'han tocat un vaixell!", true);
					
					break;
					
				case BoatState.Sinked:
					previouslyTouched = false;
					firstCoordAttacked = new Coord(0, 0);
					touchedPosCoord = new Coord(0, 0);
					GameManager.PrintMessage("T'han enfonsat un vaixell!", true);
					
					break;
			}
		}
		
		coordsNotAttacked.Remove(c);
		attackedCoordsIterator++;
		Console.ForegroundColor = ConsoleColor.White;
		
		GameManager.turn = 0;
		UpdateBoatCount();
	}
	
	public void AddWaterCoord(Coord c)
	{
		bool isAlready = false;
		
		foreach (Coord i in waterCoords)
		{
			if (c == i)
				isAlready = true;
		}
		
		if (isAlready)
			return;
		
		waterCoords.Push(c);
	}
	
	public override BoatState CheckBoatCollision(Coord c)
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
		// Algorithm for setting the boat's position randomly
		
		Random rand = new Random();
		
		for (int i = 0; i < bigBoats.Length; i++)
		{
			Coord c;
			bool placed = false;
			bool vertical = false;
			
			do
			{
				c.x = rand.Next(0, 16);
				c.y = rand.Next(0, 9);
				
				if ((c.x + (int)BoatType.Big) > GameManager.columns - 1)
				{
					if ((c.y + (int)BoatType.Big) > GameManager.rows - 1)
					{
						placed = false;
					}
					else
					{
						vertical = true;
						placed = true;
					}
				}
				else
				{
					if ((c.y + (int)BoatType.Big) > GameManager.rows - 1)
					{
						vertical = false;
						placed = true;
					}
					else
					{
						vertical = rand.NextDouble() > 0.5;
						placed = true;
					}
				}
				
				Boat tempBoat = new Boat(BoatType.Big, c, ConsoleColor.White);
				tempBoat.isVertical = vertical;
				
				if (placed && !CanPlaceBoat(tempBoat))
				{
				    placed = false;	
				}
				
			} while (!placed);
			
			bigBoats[i] = new Boat(BoatType.Big, c, ConsoleColor.Red);
			bigBoats[i].secondScenary = true;
			bigBoats[i].isVertical = vertical;
			bigBoats[i].placed = true;
		}
		
		for (int i = 0; i < mediumBoats.Length; i++)
		{
			Coord c;
			bool placed = false;
			bool vertical = false;
			
			do
			{
				c.x = rand.Next(0, 16);
				c.y = rand.Next(0, 9);
				
				if ((c.x + (int)BoatType.Medium) > GameManager.columns - 1)
				{
					if ((c.y + (int)BoatType.Medium) > GameManager.rows - 1)
					{
						placed = false;
					}
					else
					{
						vertical = true;
						placed = true;
					}
				}
				else
				{
					if ((c.y + (int)BoatType.Medium) > GameManager.rows - 1)
					{
						vertical = false;
						placed = true;
					}
					else
					{
						vertical = rand.NextDouble() > 0.5;
						placed = true;
					}
				}
				
				Boat tempBoat = new Boat(BoatType.Medium, c, ConsoleColor.White);
				tempBoat.isVertical = vertical;
				
				if (placed && !CanPlaceBoat(tempBoat))
				{
				    placed = false;	
				}
				
			} while (!placed);
			
			mediumBoats[i] = new Boat(BoatType.Medium, c, ConsoleColor.Red);
			mediumBoats[i].secondScenary = true;
			mediumBoats[i].isVertical = vertical;
			mediumBoats[i].placed = true;
		}
		
		for (int i = 0; i < smallBoats.Length; i++)
		{
			Coord c;
			bool placed = false;
			bool vertical = false;
			
			do
			{
				c.x = rand.Next(0, 16);
				c.y = rand.Next(0, 9);
				
				if ((c.x + (int)BoatType.Small) > GameManager.columns - 1)
				{
					if ((c.y + (int)BoatType.Small) > GameManager.rows - 1)
					{
						placed = false;
					}
					else
					{
						vertical = true;
						placed = true;
					}
				}
				else
				{
					if ((c.y + (int)BoatType.Small) > GameManager.rows - 1)
					{
						vertical = false;
						placed = true;
					}
					else
					{
						vertical = rand.NextDouble() > 0.5;
						placed = true;
					}
				}
				
				Boat tempBoat = new Boat(BoatType.Small, c, ConsoleColor.White);
				tempBoat.isVertical = vertical;
				
				if (placed && !CanPlaceBoat(tempBoat))
				{
				    placed = false;	
				}
				
			} while (!placed);
			
			smallBoats[i] = new Boat(BoatType.Small, c, ConsoleColor.Red);
			smallBoats[i].secondScenary = true;
			smallBoats[i].isVertical = vertical;
			smallBoats[i].placed = true;
		}
	}
	
	public override void RenderBoats()
	{
		base.RenderBoats();
		
		Console.ForegroundColor = ConsoleColor.Cyan;
		
		foreach (Coord c in waterCoords)
		{
			Coord t = GameManager.GetReadlCoord(c);
			Console.SetCursorPosition(t.x, t.y + offset);
			Console.Write('0');
		}
		
		Console.ForegroundColor = ConsoleColor.White;
	}
	
	public PlayerAI() : base(false)
	{
		previouslyTouched = false;
		touchedPosCoord = new Coord(0, 0);
		coordDir = -1;
		waterCoords = new Stack<Coord>();
		coordsNotAttacked = new List<Coord>();
		attackedCoordsIterator = 0;
		possibleDirections = new List<int>(4);
		
		for (int i = 0; i < 4; i++)
		{
			possibleDirections.Add(i);
		}
		
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 9; j++)
				coordsNotAttacked.Add(new Coord(i, j));
		}
		
		SetBoatsPosition();
	}
	
	public void SwapDirection()
	{
		nextCoord = firstCoordAttacked;
		
		
		switch (coordDir)
		{
			case 0:
				coordDir = 2;
				break;
				
			case 1:
				coordDir = 3;
				break;
				
			case 2:
				coordDir = 0;
				break;
				
			case 3:
				coordDir = 1;
				break;
		}
	}
}
