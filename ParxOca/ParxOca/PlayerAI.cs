using System;
using System.Threading;

public class PlayerAI : Player
{	
	private enum PrioritatCasella
	{
		PARXOCA_A_PARXOCA = 6, MORT = 0, MIGDIADA = 3, FINAL = 9,
		ENDAVANTAR = 5, PONT = 1, RETROCEDIR = 2, NORMAL = 4
	}
	
	private Player player;
	
	public PlayerAI(Player player, Game.Casella[] taulell) : base(taulell)
	{
		this.player = player;
	}
	
	public override void SetupFitxes(ConsoleColor color)
	{
		ConsoleColor colorFitxa = ConsoleColor.White;
		
		do
		{
			switch (Game.random.Next(1, 5))
	        {
	        	case 1:
	        		colorFitxa = ConsoleColor.Green;
	        		break;
	        	case 2:
	        		colorFitxa = ConsoleColor.Cyan;
	        		break;
	        	case 3:
	        		colorFitxa = ConsoleColor.Red;
	        		break;
	        	case 4:
	        		colorFitxa = ConsoleColor.Yellow;
	        		break;
	        }
		} while (colorFitxa == color);
				
		for (int i = 0; i < fitxes.Length; i++)
			fitxes[i] = new Fitxa(colorFitxa, taulell);
	}
	
	public override void Play()
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
		Console.Write("El torn de la CPU:");
		
		int avança = 0;
		bool finish;
		
		do
		{
			finish = true;
			Game.cursorVisible = true;
			Console.SetCursorPosition(50, 25);
			Console.Write("Polsa una tecla per tirar el dau...");
			Thread.Sleep(1500);
			Game.cursorVisible = false;
			
			for (int i = 0; i < 50; i++)
			{
				Console.SetCursorPosition(50, 25);
				Console.Write("Has tret un... {0}                       ", Game.random.Next(1, 7));
				Thread.Sleep(5);
			}
			
			avança = Game.random.Next(1, 7);
			
			Console.SetCursorPosition(50, 25);
			Console.Write("Has tret un... {0}!!!                  ", avança);
			
			Thread.Sleep(1700);
			
			Game.cursorVisible = true;
			Console.SetCursorPosition(50, 26);
			Console.Write("Quina fitxa vols moure, la 1ª o la 2ª? ");
			
			int num = DecidirFitxa(avança);
			
			Thread.Sleep(1500);
			Console.Write(num);
			Game.cursorVisible = false;
			Thread.Sleep(1000);
			
			int caselles = fitxes[num - 1].Avança(avança, player.Fitxes, fitxes[num == 1 ? 1 : 0], false,player, this);
			
			if (caselles == 2)
				finish = false;
			
			Console.SetCursorPosition(28, 22);
			Console.Write(morts[0] + " matades");
			Console.SetCursorPosition(28, 23);
			Console.Write(morts[1] + " matades");
			
			Console.SetCursorPosition(50, 25);
			Console.Write("                          ");
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
				Thread.Sleep(1200);
				finish = false;
			}
		} while (!finish && !this.won);
		
		Console.SetCursorPosition(50, 23);
		Console.Write("                   ");
	}
	
	public int DecidirFitxa(int avançar)
	{
		if (fitxes[0].TornsSenseTirar > 0)
			return 1;
		else if (fitxes[1].TornsSenseTirar > 0)
			return 2;
		
		int fitxaPos = fitxes[0].Casella + avançar - 1;
		int fitxa2Pos = fitxes[1].Casella + avançar - 1;
		
		PrioritatCasella fitxa1;
		PrioritatCasella fitxa2;
		
		if (fitxaPos < 40)
			fitxa1 = taulell[fitxaPos].ToString().ToEnum<PrioritatCasella>();
		else
			fitxa1 = PrioritatCasella.FINAL;
		
		if (fitxa2Pos < 40)
			fitxa2 = taulell[fitxa2Pos].ToString().ToEnum<PrioritatCasella>();
		else
			fitxa2 = PrioritatCasella.FINAL;
		
		foreach (Fitxa i in player.Fitxes)
		{
			if (fitxes[0].Casella + avançar == i.Casella)
				fitxa1 = (PrioritatCasella)8;
			if (fitxes[1].Casella + avançar == i.Casella)
				fitxa2 = (PrioritatCasella)8;
		}
		
		if (fitxa1 == PrioritatCasella.PONT && fitxes[0].Casella < 7)
			fitxa1 = (PrioritatCasella)7;
		
		if (fitxa2 == PrioritatCasella.PONT && fitxes[1].Casella < 7)
			fitxa2 = (PrioritatCasella)7;
		
		if ((int)fitxa1 > (int)fitxa2)
		{
			return 1;
		}
		else if (fitxa1 == fitxa2)
		{
			if (fitxes[0].Casella > fitxes[1].Casella)
				return 1;
			else
				return 2;
		}
		else
		{
			return 2;
		}
	}
}
