using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroesandGoblins
{
    [Serializable]
    abstract class Item : Tile
    {
        public Item(int x, int y) : base(x,y)
        {

        }
        public override string ToString()
        {
            return nameof(Item);
        }
    }
}
