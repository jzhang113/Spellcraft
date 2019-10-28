using GoRogue;
using GoRogue.GameFramework;
using Spellcraft.Components;
using System.Drawing;

namespace Spellcraft
{
    static class TerrainFactory
    {
        public static IGameObject Floor(Coord position)
        {
            var floor = new GameObject(position, 0, null, true, true, true);
            floor.AddComponent(new DrawComponent('.', Color.White));
            return floor;
        }

        public static IGameObject Wall(Coord position)
        {
            var wall = new GameObject(position, 0, null, true, false, false);
            wall.AddComponent(new DrawComponent('#', Color.White));
            return wall;
        }
    }
}
