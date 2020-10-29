using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroesandGoblins
{
    [Serializable]
    abstract class Tile
    {
        private protected int x, y;
        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }

        public TileType thisTile;
        public TileType ThisTile { get => thisTile; set => thisTile = value; }
        public enum TileType
        {
            Hero,
            Goblin,
            Mage,
            Gold,
            Weapon,
            Empty,
            Obstacle
        }

        public Tile(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    [Serializable]
    class EmptyTile : Tile
    {
        public EmptyTile(int x, int y) : base(x, y)
        {
            ThisTile = TileType.Empty;
        }
    }
    [Serializable]
    class Obstacle : Tile
    {
        public Obstacle(int x, int y) : base(x, y)
        {
            ThisTile = TileType.Obstacle;
        }
    }
}
