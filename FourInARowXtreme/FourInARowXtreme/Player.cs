using System;
using System.Threading;
using System.Collections.Generic;

namespace Connect4Xtreme
{
    public class Player
    {
        protected enum Direction { DOWN, RIGHT, DOWNRIGHT, DOWNLEFT }

        protected List<Vector2> coords;
        protected string name;
        protected ConsoleColor color;
        protected Game game;
        protected Game.CoordType coordType;
        protected Player player;

        public Player OtherPlayer
        {
            set { player = value; }
        }

        public virtual ConsoleColor Color
        {
            get { return color; }
            set { color = value; }
        }

        public List<Vector2> Coords
        {
            get { return coords; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Player(Game game)
        {
            this.game = game;
            coords = new List<Vector2>(10);
            coordType = Game.CoordType.Player;
        }

        public void Play(int delete)
        {
            Console.SetCursorPosition(5, 20);
            Console.Write("Torn de " + name + ", has tret un " + delete + ", ");

            if (delete == 3)
            {
                Console.Write("pots eliminar una fitxa:");
                Vector2 coord = Eliminar();

                if (coord.x != -1 && coord.y != -1)
                {
                    game.Taulell[coord.x, coord.y] = Game.CoordType.None;
                    game.DrawScenary();
                    ReplaceTokens();
                    game.DrawScenary();

                    if (game.CheckPlayersWin() != null)
                        return;
                }
            }
            else
            {
                Console.Write("no pots eliminar cap fitxa:");
            }

            CreateFitxa(Tirar());
        }

        public virtual Vector2 Eliminar()
        {
            Vector2 ans;

            Console.SetCursorPosition(10, 21);
            Console.Write("Si qualsevol de les coordenades és 0, vol dir que no vols eliminar res...");

            ans.x = IO.AskInt("Introdueix la columna: ", 0, Game.COLUMNS, 10, 22) - 1;
            ans.y = IO.AskInt("Introdueix la fila: ", 0, Game.ROWS, 10, 23) - 1;
            IO.ClearScreen(10, 100, 21, 24);

            for (int i = 0; i < coords.Count; i++)
            {
                if (coords[i] == ans)
                    coords.RemoveAt(i);
            }

            for (int i = 0; i < player.Coords.Count; i++)
            {
                if (player.Coords[i] == ans)
                    player.Coords.RemoveAt(i);
            }

            return ans;
        }

        public virtual int Tirar()
        {
            return IO.AskInt("A quina columna vols tirar? ", 1, Game.COLUMNS, 10, 21) - 1;
        }

        public Vector2 CreateFitxa(int num)
        {
            Vector2 fitxaCoord = new Vector2();
            fitxaCoord.x = num;

            for (int i = Game.ROWS - 1; i >= 0; i--)
            {
                if (game.Taulell[num, i] == Game.CoordType.None)
                {
                    fitxaCoord.y = i;
                    break;
                }
            }

            if (game.Taulell[fitxaCoord.x, fitxaCoord.y] == Game.CoordType.None)
            {
                game.Taulell[fitxaCoord.x, fitxaCoord.y] = coordType;
                coords.Add(fitxaCoord);
            }
            else
            {
                Console.SetCursorPosition(10, 24);
                Console.Write("Columna plena!");
                Thread.Sleep(1000);
            }

            IO.ClearScreen(0, Console.BufferWidth-1, 19, Console.BufferHeight-1);

            return fitxaCoord;
        }

        public bool CheckWin()
        {
            foreach (Vector2 i in coords)
            {
                for (int j = 0; j <= 3; j++)
                {
                    if (CheckCoord(i, (Direction)j, 0))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public virtual void Win()
        {
            Thread.Sleep(4000);
            Console.Clear();
            Game.FileText("WinText.txt", 13, 10);
            Thread.Sleep(1000);
            Input.GetKeyInfo();
        }

        protected void ReplaceTokens()
        {
            for (int i = 0; i < Game.COLUMNS; i++)
                CheckColumn(i);
        }

        protected void CheckColumn(int col)
        {
            for (int i = Game.ROWS - 2; i >= 0; i--)
            {
                if (game.Taulell[col, i + 1] == Game.CoordType.None &&
                    game.Taulell[col, i] != Game.CoordType.None)
                {
            		Vector2 coord = new Vector2(col, i);
            		
            		if (game.Taulell[col, i] != Game.CoordType.CPU)
            		{
            			for (int j = 0; j < player.coords.Count; j++)
            			{
            				if (player.coords[j] == coord)
            				{
            					player.coords.RemoveAt(j);
            					coord.y++;
            					player.coords.Add(coord);
            				}
            			}
            		}
            		else
            		{
            			for (int j = 0; j <coords.Count; j++)
            			{
            				if (coords[j] == coord)
            				{
            					coords.RemoveAt(j);
            					coord.y++;
            					coords.Add(coord);
            				}
            			}
            		}
            		
            		
                    game.Taulell[col, i + 1] = game.Taulell[col, i];
                    game.Taulell[col, i] = Game.CoordType.None;
                }
            }
        }

        protected bool CheckCoord(Vector2 fitxaCoord, Direction dir, int num)
        {
            /* Returns true if there are 4 tokens aligned */
            if (num == 3)
                return true;

            switch (dir)
            {
                case Direction.DOWN:
                    fitxaCoord.y++;
                    break;
                case Direction.RIGHT:
                    fitxaCoord.x++;
                    break;
                case Direction.DOWNRIGHT:
                    fitxaCoord.x++;
                    fitxaCoord.y++;
                    break;
                case Direction.DOWNLEFT:
                    fitxaCoord.x--;
                    fitxaCoord.y++;
                    break;
            }

            try
            {
                if (game.Taulell[fitxaCoord.x, fitxaCoord.y] != coordType)
                    return false;

                return CheckCoord(fitxaCoord, dir, num + 1);

            } catch (IndexOutOfRangeException) { return false; }
        }
    }
}
