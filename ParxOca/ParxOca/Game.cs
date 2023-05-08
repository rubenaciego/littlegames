using System;
using System.IO;
using System.Threading;

public class Game
{
	public enum Casella
	{
		PARXOCA_A_PARXOCA = 5, MORT = 27, MIGDIADA = 37, FINAL = 40,
		ENDAVANTAR = 3, PONT = 7, RETROCEDIR = -3, NORMAL = 0
	}
	
	private bool isRunning;
	private bool turn;
	private Player player;
	private PlayerAI playerAI;
	private Casella[] taulell;
	
	public static Random random;
	public static bool cursorVisible;
	
	static Game()
	{
		random = new Random();
		cursorVisible = false;
	}
	
	public Game()
	{
		isRunning = true;
		turn = true;
		taulell = new Casella[40];
		
		for (int i = 1; i <= taulell.Length; i++)
		{
			if (i == 40)
				taulell[i - 1] = Casella.FINAL;
			else if (i % 5 == 0)
				taulell[i - 1] = Casella.PARXOCA_A_PARXOCA;
			else if (i == 7 || i == 17)
				taulell[i - 1] = Casella.PONT;
			else if (i == 27)
				taulell[i - 1] = Casella.MORT;
			else if (i == 37)
				taulell[i - 1] = Casella.MIGDIADA;
			else if (i % 8 == 0)
				taulell[i - 1] = random.NextDouble() > 0.5 ? Casella.ENDAVANTAR : Casella.RETROCEDIR;
			else
				taulell[i - 1] = Casella.NORMAL;
		}
		
		player = new Player(taulell);
		playerAI = new PlayerAI(player, taulell);
		player.PlayerCPU = playerAI;
	}
	
	public void Start()
	{
		DrawMenu();
		DrawScenary(2, 2);
		
		ConsoleColor color = IO.AskColor("Quin color vols per les teves fitxes?", 60, 25);
		player.SetupFitxes(color);
		playerAI.SetupFitxes(color);
		
		player.DrawFitxes();
		playerAI.DrawFitxes();
		
		while (isRunning)
		{
			DrawUI();
			
			if (turn)
				player.Play();
			else
				playerAI.Play();
			
			Thread.Sleep(300);
			
			turn = !turn;
			isRunning = !(player.Won || playerAI.Won);
			
			if (player.Won)
				Player.Win("WinText.txt");
			else if (playerAI.Won)
				Player.Win("LooseText.txt");
		}
	}
	
	public static int TirarDau(int x, int y)
	{
		Game.cursorVisible = true;
		Console.SetCursorPosition(x, y);
		Console.Write("Polsa una tecla per a tirar el dau...");
		
		while (Console.KeyAvailable)
			Console.ReadKey(true);
		
		Console.ReadKey(true);
		
		Game.cursorVisible = false;
		
		for (int i = 0; i < 50; i++)
		{
			Console.SetCursorPosition(x, y);
			Console.Write("Has tret un... {0}                       ", random.Next(1, 7));
			Thread.Sleep(5);
		}
		
		int num = random.Next(1, 7);
		
		Console.SetCursorPosition(x, y);
		Console.Write("Has tret un... {0}!!!                  ", num);
		
		Thread.Sleep(1500);
		
		return num;
	}
	
	private void DrawMenu()
	{		
		string[] oca = File.ReadAllLines("OcaASCII.txt");
		
		for (int i = 0; i < oca.Length; i++)
		{
			Console.SetCursorPosition(5, 2 + i);
			Console.Write(oca[i]);
		}
		
		string[] text = File.ReadAllLines("ParxOcaText.txt");
		
		for (int i = 0; i < text.Length; i++)
		{
			Console.SetCursorPosition(70, 3 + i);
			Console.Write(text[i]);
		}
		
		while (Console.KeyAvailable)
			Console.ReadKey(true);
		
		Console.ReadKey(true);
		Console.Clear();
	}
	
	private void DrawScenary(int x, int y)
	{
		// Primer dibuixem el taulell
		for (int i = 0; i <= taulell.Length * 4; i++)
		{
			Console.SetCursorPosition(i + x, y);
			Console.Write('█');
			
			Console.SetCursorPosition(i + x, y + 6);
			Console.Write('█');
			
			if (i % 8 == 0)
			{
				for (int j = 0; j < 6; j++)
				{
					Console.SetCursorPosition(i + x, j + y);
					Console.Write('█');
				}
			}
		}
		
		for (int i = 0; i <= taulell.Length * 4; i++)
		{
			Console.SetCursorPosition(i + x, y + 10);
			Console.Write('█');
			
			Console.SetCursorPosition(i + x, y + 16);
			Console.Write('█');
			
			if (i % 8 == 0)
			{
				for (int j = 0; j < 6; j++)
				{
					Console.SetCursorPosition(i + x, j + y + 10);
					Console.Write('█');
				}
			}
		}
		
		// Ara dibuixem a sobre el número de cada casella
		for (int i = 1; i <= taulell.Length; i++)
		{
			if (i <= 20)
				Console.SetCursorPosition(i * 8 - 3, 1);
			else
				Console.SetCursorPosition((i - 20) * 8 - 3, 11);
			
			switch (taulell[i - 1])
			{
				case Casella.NORMAL:
					Console.ForegroundColor = ConsoleColor.White;
					break;
				case Casella.FINAL:
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				case Casella.MIGDIADA:
					Console.ForegroundColor = ConsoleColor.Cyan;
					break;
				case Casella.ENDAVANTAR:
					Console.ForegroundColor = ConsoleColor.Green;
					break;
				case Casella.MORT:
					Console.ForegroundColor = ConsoleColor.Red;
					break;
				case Casella.PARXOCA_A_PARXOCA:
					Console.ForegroundColor = ConsoleColor.Magenta;
					break;
				case Casella.RETROCEDIR:
					Console.BackgroundColor = ConsoleColor.Red;
					Console.ForegroundColor = ConsoleColor.White;
					break;
				case Casella.PONT:
					Console.BackgroundColor = ConsoleColor.Magenta;
					Console.ForegroundColor = ConsoleColor.White;
					break;
			}
			
			Console.Write(i);
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.BackgroundColor = ConsoleColor.Black;
		}
	}
	
	private void DrawUI()
	{
		Console.SetCursorPosition(100, 20);
		Console.Write("Instruccions:");
		Console.SetCursorPosition(103, 21);
		Console.Write("- Les caselles ");
		Console.ForegroundColor = ConsoleColor.White;
		Console.Write("blanques ");
		Console.ForegroundColor = ConsoleColor.Gray;
		Console.Write("són les normals");
		Console.SetCursorPosition(103, 22);
		Console.Write("- Guanya qui arribi a la ");
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.Write("casella 40 ");
		Console.ForegroundColor = ConsoleColor.Gray;
		Console.Write("o es passi");
		Console.SetCursorPosition(103, 23);
		Console.Write("- Si caus en una casella de color ");
		Console.ForegroundColor = ConsoleColor.Green;
		Console.Write("verd ");
		Console.ForegroundColor = ConsoleColor.Gray;
		Console.Write("avances 3 caselles");
		Console.SetCursorPosition(103, 24);
		Console.Write("- Les caselles de color ");
		Console.ForegroundColor = ConsoleColor.Magenta;
		Console.Write("magenta ");
		Console.ForegroundColor = ConsoleColor.Gray;
		Console.Write("són de ParxOca a ParxOca i");
		Console.SetCursorPosition(105, 25);
		Console.Write("tiro perquè em toca");
		Console.SetCursorPosition(103, 26);
		Console.Write("- La casella ");
		Console.ForegroundColor = ConsoleColor.Red;
		Console.Write("vermella ");
		Console.ForegroundColor = ConsoleColor.Gray;
		Console.Write("és la mort, tornes a començar");
		Console.SetCursorPosition(103, 27);
		Console.Write("- La casella ");
		Console.ForegroundColor = ConsoleColor.Cyan;
		Console.Write("blava ");
		Console.ForegroundColor = ConsoleColor.Gray;
		Console.Write("és la migdiada, dos torns sense tirar");
		Console.SetCursorPosition(103, 28);
		Console.Write("- Les ");
		Console.ForegroundColor = ConsoleColor.White;
		Console.BackgroundColor = ConsoleColor.Magenta;
		Console.Write("caselles 7 i 17");
		Console.ForegroundColor = ConsoleColor.Gray;
		Console.BackgroundColor = ConsoleColor.Black;
		Console.Write(" són el pont, si caus en una vas");
		Console.SetCursorPosition(105, 29);
		Console.Write("a l'altre");
		Console.SetCursorPosition(103, 30);
		Console.Write("- Les ");
		Console.ForegroundColor = ConsoleColor.White;
		Console.BackgroundColor = ConsoleColor.Red;
		Console.Write("caselles amb fons vermell");
		Console.ForegroundColor = ConsoleColor.Gray;
		Console.BackgroundColor = ConsoleColor.Black;
		Console.Write(" et fan retrocedir 3 caselles");
		Console.SetCursorPosition(103, 31);
		Console.Write("- Si una de les teves fitxes cau on hi havia una fitxa del");
		Console.SetCursorPosition(105, 32);
		Console.Write("contrari la mates i avances 10 caselles, de la mateixa");
		Console.SetCursorPosition(105, 33);
		Console.Write("manera si ho fa el contrari");
		Console.SetCursorPosition(103, 34);
		Console.Write("- Si treus un 6 tornes a tirar");
		
		Console.SetCursorPosition(2, 21);
		Console.Write("Jugador 1:          Jugador 2:");	
		
		Console.SetCursorPosition(8, 22);
		Console.Write(player.Morts[0] + " matades");
		Console.SetCursorPosition(8, 23);
		Console.Write(player.Morts[1] + " matades");
		Console.SetCursorPosition(28, 22);
		Console.Write(playerAI.Morts[0] + " matades");
		Console.SetCursorPosition(28, 23);
		Console.Write(playerAI.Morts[1] + " matades");
	}
	
	public static int ProceedInput(int caselles, Fitxa fitxa, Fitxa fitxa2, Casella[] taulell, Fitxa[] fitxes, bool askInput,Player player, PlayerAI playerAI)
	{
		Console.SetCursorPosition(50, 26);
		Console.Write("                                         ");
		
		fitxa.Casella += caselles;
		fitxa.Draw();
		
		caselles = 0;
		
		Thread.Sleep(1000);
		int returnValue = 0;
		int finish = 0;
		bool fitxa2Avançada;
		
		do
		{
			fitxa2Avançada = false;
			
			foreach (Fitxa i in fitxes)
			{
				if (fitxa.Casella == i.Casella && fitxa.Casella != 0)
				{
					for (int j = 0; j < player.Fitxes.Length; j++)
					{
						if (player.Fitxes[j] == fitxa)
							player.Morts[j]++;
					}
					
					for (int j = 0; j < playerAI.Fitxes.Length; j++)
					{
						if (playerAI.Fitxes[j] == fitxa)
							playerAI.Morts[j]++;
					}
					
					finish = -1;
					Console.SetCursorPosition(50, 26);
					Console.Write("Has matat una fitxa de l'altre jugador!");
					Thread.Sleep(1300);
					Console.SetCursorPosition(50, 27);
					Console.Write("Avances 10 caselles!");
					
					Console.SetCursorPosition((i.Casella > 20 ? i.Casella - 20 : i.Casella) * 8 - 3, (i.Casella > 20 ? 13 : 3) + i.CasellaPos);
					Console.Write("  ");
					i.Casella = 0;
					Thread.Sleep(1000);
					
					Console.SetCursorPosition(50, 27);
					Console.Write("Quina fitxa vols moure, la 1ª o la 2ª? ");
				
					int num;
					
					if (askInput)
					{
						do
						{
							Game.cursorVisible = true;
							num = Input.GetInputNum();
							
							if (num != -1)
								Console.Write(num);
						
							if (num != 1 && num != 2)
							{
								Game.cursorVisible = false;
								
								Console.SetCursorPosition(50, 28);
								Console.Write("Ha d'introduïr 1 o 2");
							
								Thread.Sleep(1000);
								
								Console.SetCursorPosition(50, 28);
								Console.Write("                          ");
								
								Console.SetCursorPosition(89, 27);
								Console.Write("             ");
								Console.SetCursorPosition(89, 27);
							}
						} while (num != 1 && num != 2);
					}
					else
					{
						num = playerAI.DecidirFitxa(10);
				
						Thread.Sleep(1000);
						Console.Write(num);
					}
					
					cursorVisible = false;
					Thread.Sleep(800);
					
					Console.SetCursorPosition(50, 27);
					Console.Write("                                           ");
					
					Fitxa f;
					
					if (num == 1)
					{
						if (fitxa.CasellaPos == 0 || fitxa.CasellaPos == 2)
							f = fitxa;
						else
						{
							f = fitxa2;
							fitxa2Avançada = true;
						}
					}
					else
					{
						if (fitxa.CasellaPos == 1 || fitxa.CasellaPos == 3)
							f = fitxa;
						else
						{
							f = fitxa2;
							fitxa2Avançada = true;
						}
					}
					
					if (f.Casella != 0)
					{
						bool down = f.Casella > 20;
				
						Console.SetCursorPosition((down ? f.Casella - 20 : f.Casella) * 8 - 3, (down ? 13 : 3) + f.CasellaPos);
						Console.Write("  ");
					}
					
					f.Casella += 10;
					
					i.Draw();
					f.Draw();
					
					Thread.Sleep(1200);
					
					Console.SetCursorPosition(50, 26);
					Console.Write("                                         ");
					Console.SetCursorPosition(50, 27);
					Console.Write("                                         ");
					
					returnValue = 1;
					
					if (fitxa2Avançada)
					{
						f = fitxa;
						fitxa = fitxa2;
						fitxa2 = f;
					}
				}
				else if (finish == -1)
					finish = 0;
			}
			
			Thread.Sleep(300);
			
			if (finish != -1)
			{
				switch (taulell[fitxa.Casella + caselles - 1])
				{
					case Casella.FINAL:
						finish = 2;
						break;
					case Casella.MIGDIADA:
						fitxa.TornsSenseTirar = 2;
						Console.SetCursorPosition(50, 26);
						Console.Write("Et quedes dos torns sense tirar");
						returnValue = 3;
						
						finish++;
						
						Thread.Sleep(600);
						break;
					case Casella.ENDAVANTAR:
						Console.SetCursorPosition(50, 26);
						Console.Write("Endavantes tres caselles!");
						caselles += 3;
						Thread.Sleep(1300);
						
						finish = 0;
						break;
					case Casella.MORT:
						Console.SetCursorPosition(50, 26);
						Console.Write("Has caigut a la mort! Tornes a començar.");
						caselles = -fitxa.Casella;
						
						finish = 2;
						
						Thread.Sleep(1300);
						break;
					case Casella.PARXOCA_A_PARXOCA:
						int seguentCasella = 0;
						
						for (int i = fitxa.Casella; i < taulell.Length; i++)
						{
							if (taulell[i] == Casella.PARXOCA_A_PARXOCA)
							{
								seguentCasella = i + 1;
								break;
							}
						}
						
						if (seguentCasella != 0)
						{
							if (finish == 0)
								caselles = seguentCasella - fitxa.Casella;
							
							Console.SetCursorPosition(50, 26);
							Console.Write("De parxoca a parxoca i tornes a tirar!");
							returnValue = 2;
							
							Thread.Sleep(600);
						}
						
						finish++;
						
						break;
					case Casella.RETROCEDIR:
						Console.SetCursorPosition(50, 26);
						Console.Write("Retrocedeixes 3 caselles!");
						caselles = -3;
						
						finish = 0;
						
						Thread.Sleep(1300);
						
						break;
					case Casella.PONT:
						Console.SetCursorPosition(50, 26);
						Console.Write("Has caigut al pont! Vas a la casella ");
						
						if (finish == 0)
						{
							if (fitxa.Casella == 7)
								caselles = 10;
							else if (fitxa.Casella == 17)
								caselles = -10;
						}
						
						Console.Write(fitxa.Casella + caselles);
						
						Thread.Sleep(600);
						finish++;
						
						break;
						
					default:
						finish++;
						break;
				}
			}
			
			if (fitxa.Casella != 0)
			{
				Console.SetCursorPosition((fitxa.Casella > 20 ? fitxa.Casella - 20 : fitxa.Casella) * 8 - 3, (fitxa.Casella > 20 ? 13 : 3) + fitxa.CasellaPos);
				Console.Write("  ");
			}
			
			fitxa.Casella += caselles;
			fitxa.Draw();
			
			caselles = 0;
			
		} while (finish != 2);
		
		return returnValue;
	}
}
