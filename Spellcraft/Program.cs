using BearLib;
using GoRogue.DiceNotation;
using GoRogue.GameFramework;
using GoRogue.MapGeneration;
using GoRogue.MapViews;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Spellcraft
{
    internal class Program
    {
        public const int WinWidth = 20;
        public const int WinHeight = 20;

        public const int Width = 10;
        public const int Height = 10;
        private static Map _map;
        private static IList<int> _cards;
        private static IList<char> _cardList;
        private static IList<int> _stack;

        private static void Main(string[] args)
        {
            Terminal.Open();
            Terminal.Set(
                $"window: size={WinWidth}x{WinHeight}," +
                $"cellsize=auto, title='GeomanceRL';");
            Terminal.Set("font: square.ttf, size = 12x12;");
            Terminal.Set("input.filter = [keyboard]");

            _map = new Map(Width, Height, 1, GoRogue.Distance.CHEBYSHEV);
            _cards = new List<int>();
            _stack = new List<int>();

            ISettableMapView<bool> mapview = new ArrayMap<bool>(Width, Height);
            QuickGenerators.GenerateRectangleMap(mapview);

            _map.ApplyTerrainOverlay(mapview, (pos, val) => new GameObject(pos, 0, null, true, val, !val));
            _map.AddEntity(new GameObject(new GoRogue.Coord(5, 5), 1, null));

            _cardList = new char[] { 'M', 'C', 'D', 'R', 'S', 'A' };

            Render();
            Run();
        }

        private static void Run()
        {
            bool exiting = false;

            while (!exiting)
            {
                int input = Terminal.Read();

                switch (input)
                {
                    case Terminal.TK_CLOSE:
                    case Terminal.TK_ESCAPE:
                        exiting = true;
                        break;
                }

                Update(input);
                Render();
            }

            Terminal.Close();
        }

        private static void Update(int input)
        {
            int num = input - 0x1E;

            if (num >= 0 && num < 9)
            {
                if (_cards.Count > num)
                {
                    int itm = _cards[num];
                    _cards.RemoveAt(num);
                    _stack.Add(itm);
                }
            }
            else
            {
                int card = Dice.Roll("1d6");
                _cards.Add(card);

                _stack.Clear();
            }
        }

        private static void Render()
        {
            Terminal.Clear();

            foreach (GoRogue.Coord pos in _map.Positions())
            {
                char symbol = _map.WalkabilityView[pos] ? '.' : '#';

                Terminal.Color("white");
                Terminal.Put(pos, symbol);
            }

            foreach (var v in _map.Entities)
            {
                Terminal.Put(v.Position, '@');
            }

            Terminal.Print(new Rectangle(0, Height + 1, 5, 1), "Hand:");
            foreach ((int ident, int idx) in _cards.Select((v, i) => (v, i)))
            {
                char card = _cardList[ident - 1];

                Terminal.Put(idx, Height + 2, idx + '1');
                Terminal.Put(idx, Height + 3, card);
            }

            Terminal.Print(new Rectangle(0, Height + 5, 6, 1), "Stack:");
            foreach ((int ident, int idx) in _stack.Select((v, i) => (v, i)))
            {
                char card = _cardList[ident - 1];

                Terminal.Put(idx, Height + 6, card);
            }

            Terminal.Refresh();
        }
    }
}