using BearLib;
using GoRogue.GameFramework;
using GoRogue.MapGeneration;
using GoRogue.MapViews;
using GoRogue.Random;
using Spellcraft.Components;
using Spellcraft.Entities;
using Spellcraft.Shards;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Spellcraft
{
    internal class Game
    {
        public const int WinWidth = 30;
        public const int WinHeight = 30;
        public static readonly TimeSpan FrameRate = new TimeSpan(TimeSpan.TicksPerSecond / 60);

        public const int Width = 10;
        public const int Height = 10;
        internal static Map Map;
        internal static IGameObject Player;
        internal static AnimationHandler Animations;

        private static IList<IShard> _cards;
        private static SpellStack _stack;

        private static List<Type> _cardList;

        private static void Main(string[] args)
        {
            Terminal.Open();
            Terminal.Set(
                $"window: size={WinWidth}x{WinHeight}," +
                $"cellsize=auto, title='GeomanceRL';");
            Terminal.Set("font: square.ttf, size = 12x12;");
            Terminal.Set("input.filter = [keyboard]");

            _cardList = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IShard).IsAssignableFrom(p) && p.IsClass && p != typeof(Move) && p != typeof(Shards.Empty))
                .ToList();

            Animations = new AnimationHandler();

            Map = new Map(Width, Height, 1, GoRogue.Distance.MANHATTAN);

            ISettableMapView<bool> mapview = new ArrayMap<bool>(Width, Height);
            QuickGenerators.GenerateRectangleMap(mapview);

            Map.ApplyTerrainOverlay(mapview, (pos, val) => val ? TerrainFactory.Floor(pos) : TerrainFactory.Wall(pos));

            Player = EntityFactory.Player(new GoRogue.Coord(5, 5));
            Map.AddEntity(Player);

            _cards = new List<IShard>();
            _stack = new SpellStack(Player);

            Render();
            Run();
        }

        private static void Run()
        {
            const int updateLimit = 10;
            bool exiting = false;
            DateTime currentTime = DateTime.UtcNow;
            var accum = new TimeSpan();

            TimeSpan maxDt = FrameRate * updateLimit;

            while (!exiting)
            {
                DateTime newTime = DateTime.UtcNow;
                TimeSpan frameTime = newTime - currentTime;
                if (frameTime > maxDt)
                {
                    frameTime = maxDt;
                }

                currentTime = newTime;
                accum += frameTime;

                while (accum >= FrameRate)
                {
                    if (Terminal.HasInput())
                    {
                        exiting = Update(Terminal.Read());
                        RunSystems();
                    }

                    accum -= FrameRate;
                }

                double remaining = accum / FrameRate;
                Animations.Run(frameTime, remaining);
                Render();
            }

            Terminal.Close();
        }

        private static bool Update(int input)
        {
            switch (input)
            {
                case Terminal.TK_CLOSE:
                case Terminal.TK_ESCAPE:
                    return true;
                case Terminal.TK_UP:
                    _stack.Add(new Move(MoveDir.N));
                    break;
                case Terminal.TK_DOWN:
                    _stack.Add(new Move(MoveDir.S));
                    break;
                case Terminal.TK_LEFT:
                    _stack.Add(new Move(MoveDir.W));
                    break;
                case Terminal.TK_RIGHT:
                    _stack.Add(new Move(MoveDir.E));
                    break;
                case Terminal.TK_ENTER:
                case Terminal.TK_SPACE:
                    if (_cards.Count < 9)
                    {
                        IShard card = RandomShard();
                        _cards.Add(card);
                    }

                    _stack.Resolve();
                    _stack.Clear();
                    return false;
            }

            int num = input - 0x1E;
            if (num >= 0 && num < 9)
            {
                if (_cards.Count > num)
                {
                    IShard itm = _cards[num];
                    if (_stack.Add(itm))
                        _cards.RemoveAt(num);
                }
            }

            return false;
        }

        private static IShard RandomShard() => (IShard)Activator.CreateInstance(_cardList[SingletonRandom.DefaultRNG.Next(_cardList.Count)]);

        private static void RunSystems()
        {
            // death system
            var cleanup = new List<IGameObject>();
            Map.Entities
               .Select(spatialTuple => spatialTuple.Item)
               .ForEach(entity =>
               {
                   if (entity.GetComponent<HealthComponent>()
                             ?.Health < 0)
                   {
                       cleanup.Add(entity);
                   }
               });

            cleanup.ForEach(entity => Map.RemoveEntity(entity));
        }

        private static void Render()
        {
            Terminal.Clear();

            Map.Positions()
                .Select(pos => Map.Terrain[pos])
                .ForEach(terrain =>
                    terrain.GetComponent<DrawComponent>()
                           ?.Draw());

            Map.Entities
                .Select(spatialTuple => spatialTuple.Item)
                .ForEach(entity =>
                    entity.GetComponent<DrawComponent>()
                          ?.Draw());

            int handY = Height + 1;
            Terminal.Print(new Rectangle(0, handY, 5, 1), "Hand:");
            foreach ((IShard card, int idx) in _cards.Select((v, i) => (v, i)))
            {
                Terminal.Put(idx * 2 + 1, handY + 1, idx + '1');
                Terminal.Put(idx * 2 + 2, handY + 2, '┬');
                Terminal.Put(idx * 2 + 2, handY + 4, '┴');
                Terminal.Put(idx * 2 + 1, handY + 3, card.Symbol);
            }
            if (_cards.Count > 0)
            {
                Terminal.Put(0, handY + 2, '┌');
                Terminal.Put(0, handY + 4, '└');
                Terminal.Put(_cards.Count * 2, handY + 2, '┐');
                Terminal.Put(_cards.Count * 2, handY + 4, '┘');
            }

            int stackX = 0;
            int stackY = Height + 6;
            _stack.Draw(stackX, stackY);

            Animations.Draw();

            Terminal.Refresh();
        }
    }
}