using System;
using System.Threading;
using System.IO;

namespace Connect4Xtreme
{
    public class Game
    {
        public enum CoordType { None, Player, CPU }

        public const byte COLUMNS = 8;
        public const byte ROWS = 6;
        public static readonly Random random = new Random();
        public static bool cursorVisible = false;

        private CoordType[,] taulell;
        private bool running;
        private bool turn;
        private Player[] players;

        public CoordType[,] Taulell
        {
            get { return taulell; }
        }

        public Game()
        {
            players = new Player[2];
            players[0] = new Player(this);
            players[1] = new PlayerAI(this);
            taulell = new CoordType[COLUMNS, ROWS];

            players[0].OtherPlayer = players[1];
            players[1].OtherPlayer = players[0];
        }

        public void Run()
        {
            SplashScreen();

            running = true;
            turn = true;

            Player wonPlayer = null;

            do
            {
                /* Set the name and color for the players[0] */
                Console.SetCursorPosition(0, 2);
                Console.Write("Introdueix el nom del jugador: ");
                cursorVisible = true;
                players[0].Name = Console.ReadLine();

                if (players[0].Name == "")
                {
                    Console.Write("Has d'introduïr el teu nom. Polsa una tecla per continuar...");
                    Console.ReadKey(true);
                    Console.Clear();
                }

                cursorVisible = false;

            } while (players[0].Name == "");

            ConsoleColor color = IO.AskColor("De quin color vols les teves fitxes?", 40, 5);
            players[0].Color = color;
            players[1].Color = color;

            Console.Clear();
            DrawScenary();
            DrawUI();

            while (running)
            {
                int deleteFitxa = random.Next(1, 6);

                if (turn)
                    players[0].Play(deleteFitxa);
                else
                    players[1].Play(deleteFitxa);

                turn = !turn;

                DrawScenary();

                /* Check if someone has 4 tokens aligned */
                wonPlayer = CheckPlayersWin();

                if (wonPlayer != null)
                    running = false;
                
                if (TaulellFull())
                	running = false;
            }
            
            if (wonPlayer != null)
            	wonPlayer.Win();
            else
            	Empat();
        }
        
        public static void Empat()
        {
        	Thread.Sleep(4000);
            Console.Clear();
            Game.FileText("EmpatText.txt", 28, 10);
            Thread.Sleep(1000);
            Input.GetKeyInfo();
        }

        public static void FileText(string file, int x, int y)
        {
            string[] lines = File.ReadAllLines(file);

            for (int i = 0; i < lines.Length; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(lines[i]);
            }
        }

        public void DrawScenary()
        {
            for (int i = 0; i < COLUMNS; i++)
            {
                for (int j = 0; j < ROWS; j++)
                {
                    char key = ' ';

                    switch (taulell[i, j])
                    {
                        case CoordType.CPU:
                            key = 'O';
                            Console.ForegroundColor = players[1].Color;
                            break;
                        case CoordType.None:
                            key = '·';
                            break;
                        case CoordType.Player:
                            key = 'X';
                            Console.ForegroundColor = players[0].Color;
                            break;
                    }

                    DrawInCoord(i, j, key);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
        }

        public Player CheckPlayersWin()
        {
            if (players[0].CheckWin())
                return players[0];
            if (players[1].CheckWin())
                return players[1];

            return null;
        }

        private void DrawInCoord(int x, int y, char key)
        {
            Console.SetCursorPosition(7 * x + 35, y * 2 + 5);
            Console.Write(key);
        }

        private void SplashScreen()
        {
            FileText("TitleText.txt", 13, 3);
            FileText("XtremeText.txt", 40, 10);
            FileText("PressText.txt", 8, 20);

            Console.ReadKey(true);
            Console.Clear();
        }

        private void DrawUI()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write("Fitxes de {0}: ", players[0].Name);
            Console.ForegroundColor = players[0].Color;
            Console.WriteLine('X');
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Fitxes de {0}: ", players[1].Name);
            Console.ForegroundColor = players[1].Color;
            Console.Write('O');
            Console.ForegroundColor = ConsoleColor.Gray;

            for (int i = 0; i < 56; i++)
            {
                Console.SetCursorPosition(i + 32, 3);
                Console.Write('─');
                Console.SetCursorPosition(i + 32, 17);
                Console.Write('─');
            }

            for (int i = 0; i < 13; i++)
            {
                Console.SetCursorPosition(32, i + 4);
                Console.Write('│');
                Console.SetCursorPosition(88, i + 4);
                Console.Write('│');
            }

            Console.SetCursorPosition(88, 3);
            Console.Write('┐');
            Console.SetCursorPosition(88, 17);
            Console.Write('┘');
            Console.SetCursorPosition(32, 3);
            Console.Write('┌');
            Console.SetCursorPosition(32, 17);
            Console.Write('└');

            for (int i = 0; i < COLUMNS; i++)
            {
                Console.SetCursorPosition(7 * i + 35, 2);
                Console.Write(i + 1);
            }

            for (int i = 0; i < ROWS; i++)
            {
                Console.SetCursorPosition(30, i * 2 + 5);
                Console.Write(i + 1);
            }
        }
        
        public bool TaulellFull()
        {
        	for (int i = 0; i < taulell.GetLength(0); i++)
            {
            	if (taulell[i, 0] == CoordType.None)
            		return false;
            }
            
            return true;
        }
    }

    public struct Vector2
    {
        public int x;
        public int y;

        public Vector2(int x, int y)
        {
        	this.x = x;
        	this.y = y;
        }
        
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Vector2 operator++(Vector2 obj)
        {
            obj.x++;
            obj.y++;

            return obj;
        }

        public static bool operator==(Vector2 obj1, Vector2 obj2)
        {
            return obj1.x == obj2.x && obj1.y == obj2.y;
        }

        public static bool operator !=(Vector2 obj1, Vector2 obj2)
        {
            return obj1.x != obj2.x || obj1.y != obj2.y;
        }
    }
}
