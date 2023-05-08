using System;
using System.IO;
using System.Threading;

public class Player
{
	protected Fitxa[] fitxes;
	protected int[] morts;
	protected Game.Casella[] taulell;
	protected bool won;
	
	private PlayerAI playerAI;
	
	public PlayerAI PlayerCPU
	{
		set { playerAI = value; }
	}
	
	public int[] Morts
	{
		get { return morts; }
		set { morts = value; }
	}
	
	public bool Won
	{
		get { return won; }
		set { won = value; }
	}
	
	public Fitxa[] Fitxes
	{
		get { return fitxes; }
	}
	
	public Player(Game.Casella[] taulell)
	{
		fitxes = new Fitxa[2];
		morts = new int[2];
		this.taulell = taulell;
		won = false;
	}
	
	public virtual void SetupFitxes(ConsoleColor color)
	{
		for (int i = 0; i < fitxes.Length; i++)
			fitxes[i] = new Fitxa(color, taulell);
	}
	
	public virtual void Play()
	{	
		bool dosFitxesSenseTirar = true;
		
		for (int i = 0; i < fitxes.Length; i++)
		{
			if (fitxes[i].TornsSenseTirar > 0)
				fitxes[i].TornsSenseTirar--;
			else
				dosFitxesSenseTirar = false;
		}
		
		if (dosFitxesSenseTirar)
		{
			Console.SetCursorPosition(50, 26);
			Console.Write("No pots moure cap fitxa!! S'acaba el teu torn");
			
			Thread.Sleep(1000);
			
			Console.SetCursorPosition(50, 26);
			Console.Write("                                             ");
		}
		
		Console.SetCursorPosition(50, 23);
		Console.Write("El teu torn:     ");
		
		int avança = 0;
		bool finish;
		
		do
		{
			finish = true;
			
			avança = Game.TirarDau(50, 25);
			Console.SetCursorPosition(50, 26);
			Console.Write("Quina fitxa vols moure, la 1ª o la 2ª? ");
			
			int num;
			
			do
			{
				Game.cursorVisible = true;
				num = Input.GetInputNum();
				
				if (num != -1)
					Console.Write(num);
				
				if (num != 1 && num != 2)
				{
					Game.cursorVisible = false;
					
					Console.SetCursorPosition(50, 27);
					Console.Write("Ha d'introduïr 1 o 2");
					
					Thread.Sleep(1000);
					
					Console.SetCursorPosition(50, 27);
					Console.Write("                     ");
					
					Console.SetCursorPosition(89, 26);
					Console.Write("  ");
					Console.SetCursorPosition(89, 26);
				}
				
				if ((num == 1 || num == 2) && fitxes[num - 1].TornsSenseTirar > 0)
				{
					Game.cursorVisible = false;
					
					Console.SetCursorPosition(50, 27);
					Console.Write("Aquesta fitxa encara ha d'estar " + fitxes[num - 1].TornsSenseTirar +
					             " torns sense tirar");
					
					Thread.Sleep(1500);
					
					Console.SetCursorPosition(50, 27);
					Console.Write("                                                   ");
					
					Console.SetCursorPosition(89, 26);
					Console.Write("  ");
					Console.SetCursorPosition(89, 26);
				}
				
			} while ((num != 1 && num != 2) || fitxes[num - 1].TornsSenseTirar > 0);
			
			Game.cursorVisible = false;
			Thread.Sleep(600);
			
			int caselles = fitxes[num - 1].Avança(avança, playerAI.fitxes, fitxes[num == 1 ? 1 : 0], true,this, playerAI);
			
			if (caselles == 2)
				finish = false;
			else if (caselles == 3)
				fitxes[num - 1].TornsSenseTirar = 2;
			
			Console.SetCursorPosition(28, 22);
			Console.Write(morts[0] + " matades");
			Console.SetCursorPosition(28, 23);
			Console.Write(morts[1] + " matades");
			
			Console.SetCursorPosition(50, 25);
			Console.Write("                      ");
			Console.SetCursorPosition(50, 26);
			Console.Write("                                         ");
			
			DrawFitxes();
			
			foreach (Fitxa i in fitxes)
			{
				if (i.Casella >= 40)
					this.won = true;
			}
			
			if (avança == 6 && !this.won && caselles != 2)
			{
				Console.SetCursorPosition(50, 25);
				Console.Write("Has tret un 6! Tornes a tirar!");
				Thread.Sleep(1000);
				finish = false;
			}
		} while (!finish && !this.won);
		
		Console.SetCursorPosition(50, 23);
		Console.Write("            ");
	}
	
	public void DrawFitxes()
	{
		for (int i = 0; i < fitxes.Length; i++)
			fitxes[i].Draw();
	}
	
	public static void Win(string file)
	{
		Thread.Sleep(500);
		Console.Clear();
		
		int x = 0;
		int y = 0;
		
		if (file == "LooseText.txt")
		{
			x = 40;
			y = 15;
		}
		else if (file == "WinText.txt")
		{
			x = 13;
			y = 14;
		}
		
		string[] text = File.ReadAllLines(file);
		
		for (int i = 0; i < text.Length; i++)
		{
			Console.SetCursorPosition(x, y + i);
			Console.Write(text[i]);
		}
		
		while (Console.KeyAvailable)
			Console.ReadKey(true);
		
		Console.ReadKey(true);
	}
}
