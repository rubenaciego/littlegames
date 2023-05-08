using System;

public class Boat
{
	public BoatType boatType;
	public bool[] partsAlive;
	public bool isDestroyed;
	public bool isVertical;
	public Coord position;
	public bool placed;
	public bool secondScenary;
	
	private int partsIterator;
	private ConsoleColor boatColor;
	
	public Boat(BoatType b, Coord pos, ConsoleColor color)
	{
		position = pos;
		boatType = b;
		secondScenary = false;
		boatColor = color;
		placed = false;
		isDestroyed = false;
		partsAlive = new bool[(int)boatType];
		isVertical = false;
		partsIterator = 0;
		
		for (int i = 0; i < partsAlive.Length; i++)
			partsAlive[i] = true;
	}
	
	public BoatState TakeDamage(Coord c)
	{
		int index = GetBoatPartIndex(c);
		
		if (isDestroyed || partsIterator >= partsAlive.Length)
			return BoatState.Sinked;
		
		if (partsAlive[index] == false)
			return BoatState.Touched;
		
		partsAlive[index] = false;
		partsIterator++;
		
		if (partsIterator == partsAlive.Length)
		{
			isDestroyed = true;
			return BoatState.Sinked;
		}
		
		return BoatState.Touched;
	}
	
	public void Flush()
	{
		Coord c = GameManager.GetReadlCoord(position);
		
		if (secondScenary)
			c.y += PlayerAI.offset;
		
		Console.SetCursorPosition(c.x, c.y);
		Console.Write(' ');
		
		for (int i = 1; i < partsAlive.Length; i++)
		{
			c = position;
			
			if (isVertical)
				c.y += i;
			else
				c.x += i;
			
			c = GameManager.GetReadlCoord(c);
			
			if (secondScenary)
				c.y += PlayerAI.offset;
			
			Console.SetCursorPosition(c.x, c.y);
			Console.Write(' ');
		}
	}
	
	public void Render()
	{
		if (!secondScenary && !partsAlive[0])
				Console.ForegroundColor = ConsoleColor.Red;
		else
			Console.ForegroundColor = boatColor;
		
		Coord c = GameManager.GetReadlCoord(position);
		
		if (secondScenary)
			c.y += PlayerAI.offset;
		
		Console.SetCursorPosition(c.x, c.y);
		
		if (isDestroyed)
			Console.Write('X');
		else if (secondScenary && !partsAlive[0])
			Console.Write('*');
		else if (!secondScenary)
			Console.Write('*');
		
		for (int i = 1; i < partsAlive.Length; i++)
		{
			c = position;
			
			if (isVertical)
				c.y += i;
			else
				c.x += i;
			
			c = GameManager.GetReadlCoord(c);
			
			if (secondScenary)
				c.y += PlayerAI.offset;
			
			if (secondScenary && partsAlive[i])
				continue;
			
			Console.SetCursorPosition(c.x, c.y);
			Console.ForegroundColor = boatColor;
			
			if (!secondScenary && !partsAlive[i])
				Console.ForegroundColor = ConsoleColor.Red;
			
			if (isDestroyed)
				Console.Write('X');
			else
				Console.Write('*');
			
			Console.ForegroundColor = boatColor;
		}
		
		Console.ForegroundColor = ConsoleColor.White;
	}
	
	public int GetBoatPartIndex(Coord c)
	{
		for (int i = 0; i < (int)this.boatType; i++)
		{
			Coord aux = position;
			
			if (isVertical)
				aux.y += i;
			else
				aux.x += i;
			
			if (c == aux)
				return i;
		}
		
		return 0;
	}
}

public enum BoatType
{
	Small = 2,
	Medium = 3,
	Big = 5,
}

public enum BoatState
{
	Water,
	Touched,
	Sinked
}
