using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroesandGoblins
{
    [Serializable]
    abstract class Enemy : Character
    {
        Random random = new Random();
        public Enemy(int x, int y, int damage, int hp, int maxHP, char symbol) : base(x, y, symbol)
        {
            Damage = damage;
            HP = hp;
            MaxHP = maxHP;
        }
        public override string ToString()
        {
            return nameof(Enemy) + " at [" + X + "," + Y + "] Damage:" + damage;
        }
    }
}
