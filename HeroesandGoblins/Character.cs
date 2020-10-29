using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroesandGoblins
{
    [Serializable]
    abstract class Character : Tile
    {
        private protected int hp, maxHP, damage, gold;
        private protected char symbol;
        private protected Tile[] vision = new Tile[8];

        public int HP { get => hp; set => hp = value; }
        public int MaxHP { get => maxHP; set => maxHP = value; }
        public int Damage { get => damage; set => damage = value; }
        public int Gold { get => gold; set => gold = value; }
        public char Symbol { get => symbol; set => symbol = value; }
        public Tile[] Vision { get => vision; set => vision = value; }
        public enum Movement
        {
            NoMove,
            Up,
            Down,
            Left,
            Right
        }

        public Character(int x, int y, char symbol) : base(x, y)
        {

        }

        public virtual void Attack(Character target)
        {
            target.hp -= Damage;
        }

        public bool IsDead()
        {
            if (hp < 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool CheckRange(Character target)
        {
            if (DistanceTo(target) < 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int DistanceTo(Character target)
        {
            return Math.Abs(target.X - this.X) + Math.Abs(target.Y - this.Y);
        }

        public void Move(Movement move)
        {
            if (move == Movement.Up)
            {
                y--;
            }
            if (move == Movement.Down)
            {
                y++;
            }
            if (move == Movement.Left)
            {
                x--;
            }
            if (move == Movement.Right)
            {
                x++;
            }
        }

        public void Pickup(Item i)
        {
            Random goldRandom = new Random();
            if (i.thisTile == Tile.TileType.Gold)
            {
                Gold += goldRandom.Next(1, 6);
            }
        }

        public abstract Movement ReturnMove(Movement move);

        public abstract override string ToString();
    }
}
