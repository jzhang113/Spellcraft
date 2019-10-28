using BearLib;
using GoRogue.GameFramework;
using GoRogue.MapGeneration;
using GoRogue.MapViews;
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
        public const int WinWidth = 20;
        public const int WinHeight = 20;

        public const int Width = 10;
        public const int Height = 10;
        internal static Map Map;
        internal static IGameObject Player;

        private static IList<IShard> _cards;
        private static IList<IShard> _stack;

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
                .Where(p => typeof(IShard).IsAssignableFrom(p) && p.IsClass)
                .ToList();

            Map = new Map(Width, Height, 1, GoRogue.Distance.CHEBYSHEV);
            _cards = new List<IShard>();
            _stack = new List<IShard>();

            ISettableMapView<bool> mapview = new ArrayMap<bool>(Width, Height);
            QuickGenerators.GenerateRectangleMap(mapview);

            Map.ApplyTerrainOverlay(mapview, (pos, val) => val ? TerrainFactory.Floor(pos) : TerrainFactory.Wall(pos));

            Player = EntityFactory.Player(new GoRogue.Coord(5, 5));
            Map.AddEntity(Player);

            Render();
            Run();
        }

        private static void Run()
        {
            var frameRate = new TimeSpan(TimeSpan.TicksPerSecond / 30);
            const int updateLimit = 10;
            bool exiting = false;
            DateTime currentTime = DateTime.UtcNow;
            var accum = new TimeSpan();

            TimeSpan maxDt = frameRate * updateLimit;

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

                while (accum >= frameRate)
                {
                    if (Terminal.HasInput())
                    {
                        exiting = Update(Terminal.Read());
                        RunSystems();
                    }

                    accum -= frameRate;
                }

                double remaining = accum / frameRate;
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
                case Terminal.TK_ENTER:
                    IShard card = RandomShard();
                    _cards.Add(card);
                    Resolve(_stack);

                    _stack.Clear();
                    return false;
            }

            int num = input - 0x1E;
            if (num >= 0 && num < 9)
            {
                if (_cards.Count > num)
                {
                    IShard itm = _cards[num];
                    _cards.RemoveAt(num);
                    _stack.Add(itm);
                }
            }

            return false;
        }

        private static void Resolve(IList<IShard> stack)
        {
            if (stack.Count == 0)
                return;

            SpellResolver spell = stack[0].Primary();
            stack.Skip(1)
                 .Aggregate(spell, (resolver, shard) => shard.Secondary(resolver))
                 .Resolve();
        }

        private static IShard RandomShard() => (IShard)Activator.CreateInstance(_cardList[GoRogue.Random.SingletonRandom.DefaultRNG.Next(_cardList.Count)]);

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

            Terminal.Print(new Rectangle(0, Height + 1, 5, 1), "Hand:");
            foreach ((IShard card, int idx) in _cards.Select((v, i) => (v, i)))
            {
                Terminal.Put(idx, Height + 2, idx + '1');
                Terminal.Put(idx, Height + 3, card.Symbol);
            }

            Terminal.Print(new Rectangle(0, Height + 5, 6, 1), "Stack:");
            foreach ((IShard card, int idx) in _stack.Select((v, i) => (v, i)))
            {
                Terminal.Put(idx, Height + 6, card.Symbol);
            }

            Terminal.Refresh();
        }
    }
}