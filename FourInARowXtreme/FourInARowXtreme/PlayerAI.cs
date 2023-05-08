using System;
using System.Threading;

namespace Connect4Xtreme
{
    public class PlayerAI : Player
    {
        public override ConsoleColor Color
        {
            get { return color; }
            set
            {
                ConsoleColor[] optionsColors = { ConsoleColor.Green, ConsoleColor.Cyan,
                    ConsoleColor.Red, ConsoleColor.Yellow };
                do
                {
                    color = optionsColors[Game.random.Next(optionsColors.Length)];
                } while (color == value);
            }
        }

        public PlayerAI(Game game) : base(game)
        {
            coordType = Game.CoordType.CPU;
            name = "CPU";
        }

        public override int Tirar()
        {
            int res;

            do
            {
                res = Game.random.Next(1, Game.COLUMNS);

            } while (game.Taulell[res - 1, 0] != Game.CoordType.None);

            Console.SetCursorPosition(10, 21);
            Console.Write("A quina columna vols tirar? ");
            Thread.Sleep(1500);
            Console.Write(res);
            Thread.Sleep(1500);

            return res - 1;
        }

        public override Vector2 Eliminar()
        {
            Vector2 ans = player.Coords[Game.random.Next(player.Coords.Count)];

            Console.SetCursorPosition(10, 21);
            Console.Write("Si qualsevol de les coordenades és 0, vol dir que no vols eliminar res...");

            Console.SetCursorPosition(10, 22);
            Console.Write("Introdueix la columna: ");
            Thread.Sleep(1500);
            Console.Write(ans.x + 1);

            Console.SetCursorPosition(10, 23);
            Console.Write("Introdueix la fila: ");
            Thread.Sleep(1500);
            Console.Write(ans.y + 1);

            Thread.Sleep(1500);
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

        public override void Win()
        {
            Thread.Sleep(4000);
            Console.Clear();
            Game.FileText("LooseText.txt", 17, 10);
            Thread.Sleep(1000);
            Input.GetKeyInfo();
        }
    }
}
