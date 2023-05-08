using System;

public class Fitxa
{
	private int casella;
	private ConsoleColor color;
	private readonly int casellaPos;
	private static int numCasella;
	private Game.Casella[] taulell;
	private int tornsSenseTirar;
	
	public int TornsSenseTirar
	{
		get { return tornsSenseTirar; }
		set { tornsSenseTirar = value; }
	}
	
	public int Casella
	{
		get { return casella; }
		set { casella = value; }
	}
	
	public int CasellaPos
	{
		get { return casellaPos; }
	}
	
	public Fitxa(ConsoleColor color, Game.Casella[] taulell)
	{
		casellaPos = numCasella;
		casella = 0;
		tornsSenseTirar = 0;
		this.color = color;
		this.taulell = taulell;
		numCasella++;
	}
	
	public void Draw()
	{			
		if (casella == 0)
		{
			Console.ForegroundColor = color;		
			Console.SetCursorPosition(casellaPos > 1 ? 25 : 5,
			                          (casellaPos == 1 || casellaPos == 3) ? 23 : 22);
			Console.Write("▄▄");
			
			Console.ForegroundColor = ConsoleColor.Gray;
		}
		else
		{
			if (casella > 40)
				casella = 40;
			
			bool down = casella > 20;
			
			Console.ForegroundColor = color;		
			Console.SetCursorPosition((down ? casella - 20 : casella) * 8 - 3, (down ? 13 : 3) + casellaPos);
			Console.Write("▄▄");
			
			Console.ForegroundColor = ConsoleColor.Gray;
		}
	}
	
	public int Avança(int casellesAvançar, Fitxa[] fitxes, Fitxa altreFitxa, bool askInput,Player player, PlayerAI p)
	{
		if (casella != 0)
		{
			bool down = casella > 20;
			
			Console.SetCursorPosition((down ? casella - 20 : casella) * 8 - 3, (down ? 13 : 3) + casellaPos);
			Console.Write("  ");
		}
		
		return Game.ProceedInput(casellesAvançar, this, altreFitxa, taulell, fitxes, askInput,player, p);
	}
}
