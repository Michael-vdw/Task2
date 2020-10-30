using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeroesandGoblins
{
    public partial class Form1 : Form
    {
        private static readonly char cHero = 'H';
        private static readonly char cGoblin = 'G';
        private static readonly char cMage = 'M';
        private static readonly char cGold = '$';
        private static readonly char cEmpty = '.';
        private static readonly char cObstacle = 'X';
        GameEngine gameEngine = new GameEngine();
        [Serializable]
        private class Goblin : Enemy
        {
            public Goblin(int x, int y) : base(x, y, 1, 10, 10, 'G')
            {
                thisTile = TileType.Goblin;
            }

            public override Movement ReturnMove(Movement move)
            {
                Random random = new Random();
                int randomroll = random.Next(1, 5);

                while (vision[randomroll].thisTile != TileType.Empty)
                {
                    randomroll = random.Next(1, 5);
                }
                return (Movement)randomroll;
            }
        }
        [Serializable]
        class Mage : Enemy
        {
            public Mage(int x, int y) : base(x, y, 5, 5, 5, 'M')
            {
                thisTile = TileType.Mage;
            }

            public override Movement ReturnMove(Movement move)
            {
                return Movement.NoMove;
            }

            public override bool CheckRange(Character target)
            {
                for (int i = 0; i > 8; i++)
                {
                    if (vision[i].ThisTile == TileType.Hero || vision[i].ThisTile == TileType.Empty)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        [Serializable]
        private class Hero : Character
        {
            public Hero(int x, int y, int hp) : base(x, y, 'H')
            {
                this.hp = hp;
                this.maxHP = hp;
                this.damage = 2;
                thisTile = TileType.Hero;
            }

            public override Movement ReturnMove(Movement move)
            {
                if (vision[Convert.ToInt32(move)].thisTile != TileType.Empty)
                {
                    return Movement.NoMove;
                }
                else
                {
                    return move;
                }
            }

            public override string ToString()
            {
                return "Player stats: \nHP:" + hp + "/" + maxHP + "\nDamage:" + damage + "\nCoordinates:" + "[" + x + "," + y + "]" + "\nGold:" + gold; 
            }
        }
        [Serializable]
        class Map
        {
            private Tile[,] tileMap;
            private Hero player;
            private Enemy[] enemies;
            private Item[] items;
            private int minWidth, maxWidth, minHeight, maxHeight, height, width, i;
            Random randomnum = new Random();

            public int MinWidth { get => minWidth; set => minWidth = value; }
            public int MaxWidth { get => maxWidth; set => maxWidth = value; }
            public int MinHeight { get => minHeight; set => minHeight = value; }
            public int MaxHeight { get => maxHeight; set => maxHeight = value; }
            public int Height { get => height; set => height = value; }
            public int Width { get => width; set => width = value; }
            public Hero Player { get => player; set => player = value; }
            public Enemy[] Enemies { get => enemies; set => enemies = value; }
            public Tile[,] TileMap { get => tileMap; set => tileMap = value; }
            public Item[] Items { get => items; set => items = value; }

            public Map(int minwidth, int maxwidth, int minheight, int maxheight, int enemynum, int gold)
            {
                Height = randomnum.Next(minheight, maxheight);
                Width = randomnum.Next(minwidth, maxwidth);

                items = new Item[gold];
                tileMap = new Tile[width, height];
                enemies = new Enemy[enemynum];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (x == 0 || y == 0 || x == width -1 || y == height -1)
                        {
                            tileMap[x, y] = new Obstacle(x, y);
                        }
                        else
                        {
                            tileMap[x, y] = new EmptyTile(x, y);
                        }      
                    }
                }

                Create(Tile.TileType.Hero);

                i = 0;
                while(i < enemynum)
                {
                    int randomEnemy = randomnum.Next(1, 3);
                    if (randomEnemy == 1)
                    {
                        Create(Tile.TileType.Goblin);
                    }
                    else
                    {
                        Create(Tile.TileType.Mage);
                    }
                    i++;
                }

                i = 0;
                while (i < gold)
                {
                    Create(Tile.TileType.Gold);
                    i++;
                }

                UpdateVision();
            }
            public void UpdateVision()
            {
                player.Vision[0] = tileMap[player.X, player.Y - 1];
                player.Vision[1] = tileMap[player.X, player.Y + 1];
                player.Vision[2] = tileMap[player.X - 1, player.Y];
                player.Vision[3] = tileMap[player.X + 1, player.Y];
                player.Vision[4] = tileMap[player.X - 1, player.Y - 1];
                player.Vision[5] = tileMap[player.X + 1, player.Y - 1];
                player.Vision[6] = tileMap[player.X + 1, player.Y + 1];
                player.Vision[7] = tileMap[player.X - 1, player.Y - 1];

                for (int i = 0; i < enemies.Length; i++)
                {
                    enemies[i].Vision[0] = tileMap[enemies[i].X, enemies[i].Y - 1];
                    enemies[i].Vision[1] = tileMap[enemies[i].X, enemies[i].Y + 1];
                    enemies[i].Vision[2] = tileMap[enemies[i].X - 1, enemies[i].Y];
                    enemies[i].Vision[3] = tileMap[enemies[i].X + 1, enemies[i].Y];
                    enemies[i].Vision[4] = tileMap[enemies[i].X - 1, enemies[i].Y - 1];
                    enemies[i].Vision[5] = tileMap[enemies[i].X + 1, enemies[i].Y - 1];
                    enemies[i].Vision[6] = tileMap[enemies[i].X + 1, enemies[i].Y + 1];
                    enemies[i].Vision[7] = tileMap[enemies[i].X - 1, enemies[i].Y - 1];
                }
            }

            public Item GetItemAtPosition(int x, int y)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].X == x && items[i].Y == y)
                    {
                        Item tempgold;
                        tempgold = items[i];
                        items[i] = null;
                        return tempgold;
                    }
                }
                return null;
            }

            private Tile Create(Tile.TileType type)
            {
                int x = randomnum.Next(1, width);
                int y = randomnum.Next(1, height);

                while (tileMap[x,y].ThisTile != Tile.TileType.Empty)
                {
                    x = randomnum.Next(1, width);
                    y = randomnum.Next(1, height);
                }
                if (type == Tile.TileType.Hero)
                {
                    player = new Hero(x, y, 40);
                    tileMap[player.X, player.Y] = player;
                    return player;
                }
                if (type == Tile.TileType.Gold)
                {
                    items[i] = new Gold(x, y);
                    tileMap[items[i].X, items[i].Y] = items[i];
                    return items[i];
                }
                if (type == Tile.TileType.Goblin)
                {
                    enemies[i] = new Goblin(x, y);
                    tileMap[enemies[i].X, enemies[i].Y] = enemies[i];
                    return enemies[i];
                }
                if (type == Tile.TileType.Mage)
                {
                    enemies[i] = new Mage(x, y);
                    tileMap[enemies[i].X, enemies[i].Y] = enemies[i];
                    return enemies[i];
                }
                return new EmptyTile(x, y);
            }
        }
        [Serializable]
        class GameEngine
        {
            private Map engineMap;
            private Hero player;
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("D:\\OneDrive\\OneDrive - Vega School\\Vega\\Game Development\\Project\\Task2\\GitRepo\\Task2\\HeroesandGoblins\\MapSaveFile.txt", FileMode.Create, FileAccess.Write);
            //private static readonly char 
            public Map EngineMap { get => engineMap; set => engineMap = value; }
            public Hero Player { get => player; set => player = value; }

            public GameEngine() 
            {
                engineMap = new Map(10,15,10,15,5, 5);
                player = engineMap.Player;
            }

            public void Save()
            {
                formatter.Serialize(stream, engineMap);
                stream.Close();
            }

            public void Load()
            {
                stream = new FileStream(@"D:\\OneDrive\\OneDrive - Vega School\\Vega\\Game Development\\Project\\Task2\\GitRepo\\Task2\\HeroesandGoblins\\MapSaveFile.txt", FileMode.Open, FileAccess.Read);
                engineMap = (Map) formatter.Deserialize(stream);
            }

            public void EnemyAttacks()
            {
                for (int i = 0; i < engineMap.Enemies.Length; i++)
                {
                    if (engineMap.Enemies[i].thisTile == Tile.TileType.Goblin)
                    {
                        for (int ivision = 0; ivision < 4; ivision++)
                        {
                            if (engineMap.Enemies[i].Vision[ivision].thisTile == Tile.TileType.Hero)
                            {
                                engineMap.Enemies[i].Attack(player);
                            }
                        }
                    }
                    if (engineMap.Enemies[i].thisTile == Tile.TileType.Mage)
                    {
                        for (int ivision = 0; ivision < 8; ivision++)
                        {
                            if (engineMap.Enemies[i].Vision[ivision].thisTile == Tile.TileType.Hero)
                            {
                                engineMap.Enemies[i].Attack(player);
                            }

                            if (engineMap.Enemies[i].Vision[ivision].thisTile == Tile.TileType.Goblin)
                            {
                                for (int a = 0; a < engineMap.Enemies.Length; a++)
                                {
                                    if (engineMap.Enemies[a].X == engineMap.Enemies[i].Vision[ivision].X && engineMap.Enemies[a].Y == engineMap.Enemies[i].Vision[ivision].Y)
                                    {
                                        engineMap.Enemies[i].Attack(EngineMap.Enemies[a]);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            public void MoveEnemies()
            {
                Random randomMove = new Random();
                for (int i = 0; i < engineMap.Enemies.Length;)
                {
                    if (engineMap.Enemies[i].thisTile == Tile.TileType.Goblin)
                    {
                        int movenum = randomMove.Next(1, 5);
                        switch (movenum)
                        {
                            case 1:
                                if (engineMap.Enemies[i].Vision[0].thisTile == Tile.TileType.Empty)
                                {
                                    engineMap.Enemies[i].Move(Character.Movement.Up);
                                    engineMap.TileMap[engineMap.Enemies[i].X, engineMap.Enemies[i].Y] = engineMap.Enemies[i];
                                    engineMap.TileMap[engineMap.Enemies[i].X, engineMap.Enemies[i].Y + 1] = new EmptyTile(engineMap.Enemies[i].X, engineMap.Enemies[i].Y + 1);
                                    engineMap.UpdateVision();
                                    i++;
                                }
                                break;
                            case 2:
                                if (engineMap.Enemies[i].Vision[1].thisTile == Tile.TileType.Empty)
                                {
                                    engineMap.Enemies[i].Move(Character.Movement.Down);
                                    engineMap.TileMap[engineMap.Enemies[i].X, engineMap.Enemies[i].Y] = engineMap.Enemies[i];
                                    engineMap.TileMap[engineMap.Enemies[i].X, engineMap.Enemies[i].Y - 1] = new EmptyTile(engineMap.Enemies[i].X, engineMap.Enemies[i].Y - 1);
                                    engineMap.UpdateVision();
                                    i++;
                                }
                                break;
                            case 3:
                                if (engineMap.Enemies[i].Vision[2].thisTile == Tile.TileType.Empty)
                                {
                                    engineMap.Enemies[i].Move(Character.Movement.Left);
                                    engineMap.TileMap[engineMap.Enemies[i].X, engineMap.Enemies[i].Y] = engineMap.Enemies[i];
                                    engineMap.TileMap[engineMap.Enemies[i].X + 1, engineMap.Enemies[i].Y] = new EmptyTile(engineMap.Enemies[i].X + 1, engineMap.Enemies[i].Y);
                                    engineMap.UpdateVision();
                                    i++;
                                }
                                break;
                            case 4:
                                if (engineMap.Enemies[i].Vision[3].thisTile == Tile.TileType.Empty)
                                {
                                    engineMap.Enemies[i].Move(Character.Movement.Right);
                                    engineMap.TileMap[engineMap.Enemies[i].X, engineMap.Enemies[i].Y] = engineMap.Enemies[i];
                                    engineMap.TileMap[engineMap.Enemies[i].X - 1, engineMap.Enemies[i].Y] = new EmptyTile(engineMap.Enemies[i].X - 1, engineMap.Enemies[i].Y);
                                    engineMap.UpdateVision();
                                    i++;
                                }
                                break;
                        }
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            public bool MovePlayer(Character.Movement move)
            {
                if (move == Character.Movement.Down)
                {
                    if (player.Vision[1].thisTile == Tile.TileType.Empty)
                    {
                        player.Move(Character.Movement.Down);
                        EngineMap.TileMap[player.X, player.Y] = player; 
                        EngineMap.TileMap[player.X, player.Y - 1] = new EmptyTile(player.X, player.Y - 1);
                        EngineMap.UpdateVision();
                        return true;
                    }
                    if(player.Vision[1].thisTile == Tile.TileType.Gold)
                    {
                        for (int i = 0; i < engineMap.Items.Length; i++)
                        {
                            if (engineMap.Items[i].X == player.X && EngineMap.Items[i].Y - 1 == player.Y)
                            {
                                player.Pickup(engineMap.Items[i]);
                            }
                        }
                        player.Move(Character.Movement.Down);
                        EngineMap.TileMap[player.X, player.Y] = player;
                        EngineMap.TileMap[player.X, player.Y - 1] = new EmptyTile(player.X, player.Y - 1);
                        EngineMap.UpdateVision();
                        return true;
                    }
                    else
                    {
                        //MessageBox.Show("Path Blocked","Cannot move here");
                        return false;
                    }
                }
                if (move == Character.Movement.Right)
                {
                    if (player.Vision[3].thisTile == Tile.TileType.Empty)
                    {
                        player.Move(Character.Movement.Right);
                        EngineMap.TileMap[player.X, player.Y] = player;
                        EngineMap.TileMap[player.X - 1, player.Y] = new EmptyTile(player.X - 1, player.Y);
                        EngineMap.UpdateVision();
                        return true;
                    }
                    if (player.Vision[3].thisTile == Tile.TileType.Gold)
                    {
                        for (int i = 0; i < engineMap.Items.Length; i++)
                        {
                            if (engineMap.Items[i].X - 1 == player.X && EngineMap.Items[i].Y == player.Y)
                            {
                                player.Pickup(engineMap.Items[i]);
                            }
                        }
                        player.Move(Character.Movement.Right);
                        EngineMap.TileMap[player.X, player.Y] = player;
                        EngineMap.TileMap[player.X - 1, player.Y] = new EmptyTile(player.X - 1, player.Y);
                        EngineMap.UpdateVision();
                        return true;
                    }
                    else
                    {
                        //MessageBox.Show("Path Blocked", "Cannot move here");
                        return false;
                    }
                }
                if (move == Character.Movement.Left)
                {
                    if (player.Vision[2].thisTile == Tile.TileType.Empty)
                    {
                        player.Move(Character.Movement.Left);
                        EngineMap.TileMap[player.X, player.Y] = player;
                        EngineMap.TileMap[player.X + 1, player.Y] = new EmptyTile(player.X + 1, player.Y);
                        EngineMap.UpdateVision();
                        return true;
                    }
                    if (player.Vision[2].thisTile == Tile.TileType.Gold)
                    {
                        for (int i = 0; i < engineMap.Items.Length; i++)
                        {
                            if (engineMap.Items[i].X + 1 == player.X && EngineMap.Items[i].Y == player.Y)
                            {
                                player.Pickup(engineMap.Items[i]);
                            }
                        }
                        player.Move(Character.Movement.Left);
                        EngineMap.TileMap[player.X, player.Y] = player;
                        EngineMap.TileMap[player.X + 1, player.Y] = new EmptyTile(player.X + 1, player.Y);
                        EngineMap.UpdateVision();
                        return true;
                    }
                    else
                    {
                        //MessageBox.Show("Path Blocked", "Cannot move here");
                        return false;
                    }
                }
                if (move == Character.Movement.Up)
                {
                    if (player.Vision[0].thisTile == Tile.TileType.Empty)
                    {
                        player.Move(Character.Movement.Up);
                        EngineMap.TileMap[player.X, player.Y] = player;                      
                        EngineMap.TileMap[player.X, player.Y + 1] = new EmptyTile(player.X, player.Y + 1);
                        EngineMap.UpdateVision();
                        return true;
                    }
                    if (player.Vision[0].thisTile == Tile.TileType.Gold)
                    {
                        for (int i = 0; i < engineMap.Items.Length; i++)
                        {
                            if (engineMap.Items[i].X == player.X && EngineMap.Items[i].Y + 1 == player.Y)
                            {
                                player.Pickup(engineMap.Items[i]);
                            }
                        }
                        player.Move(Character.Movement.Up);
                        EngineMap.TileMap[player.X, player.Y] = player;
                        EngineMap.TileMap[player.X, player.Y + 1] = new EmptyTile(player.X, player.Y + 1);
                        EngineMap.UpdateVision();
                        return true;
                    }
                    else
                    {
                        //MessageBox.Show("Path Blocked", "Cannot move here");
                        return false;
                    }
                }
                return false;
            }
        }
        [Serializable]
        class Gold : Item
        {
            public Gold(int x, int y) : base(x,y)
            {
                thisTile = Tile.TileType.Gold;
            }
        }

        public void mapDraw()
        {
            labelMap.Text = "";

            for (int y = 0; y < gameEngine.EngineMap.Height; y++)
            {
                for (int x = 0; x < gameEngine.EngineMap.Width; x++)
                {
                    if (gameEngine.EngineMap.TileMap[x, y].ThisTile == Tile.TileType.Empty)
                    {
                        labelMap.Text += cEmpty;
                    }
                    if (gameEngine.EngineMap.TileMap[x, y].ThisTile == Tile.TileType.Goblin)
                    {
                        labelMap.Text += cGoblin;
                    }
                    if (gameEngine.EngineMap.TileMap[x, y].ThisTile == Tile.TileType.Mage)
                    {
                        labelMap.Text += cMage;
                    }
                    if (gameEngine.EngineMap.TileMap[x, y].ThisTile == Tile.TileType.Hero)
                    {
                        labelMap.Text += cHero;
                    }
                    if (gameEngine.EngineMap.TileMap[x, y].ThisTile == Tile.TileType.Obstacle)
                    {
                        labelMap.Text += cObstacle;
                    }
                    if (gameEngine.EngineMap.TileMap[x, y].ThisTile == Tile.TileType.Gold)
                    {
                        labelMap.Text += cGold;
                    }
                }
                labelMap.Text += "\n";
            }
            lblStats.Text = gameEngine.Player.ToString();
        }

        public Form1()
        {
            InitializeComponent();
            rtbAttack.Text = "";
            mapDraw();
            for (int i = 0; i < gameEngine.EngineMap.Enemies.Length; i++)
            {
                cbxEnemies.Items.Add(gameEngine.EngineMap.Enemies[i]);
            }  
        }

        private void btnUp_Click_1(object sender, EventArgs e)
        {
            gameEngine.MovePlayer(Character.Movement.Up);
            gameEngine.MoveEnemies();
            gameEngine.EnemyAttacks();
            mapDraw();
        }

        private void btnRight_Click_1(object sender, EventArgs e)
        {
            gameEngine.MovePlayer(Character.Movement.Right);
            gameEngine.MoveEnemies();
            gameEngine.EnemyAttacks();
            mapDraw();
        }

        private void btnDown_Click_1(object sender, EventArgs e)
        {
            gameEngine.MovePlayer(Character.Movement.Down);
            gameEngine.MoveEnemies();
            gameEngine.EnemyAttacks();
            mapDraw();
        }

        private void btnleft_Click_1(object sender, EventArgs e)
        {
            gameEngine.MovePlayer(Character.Movement.Left);
            gameEngine.MoveEnemies();
            gameEngine.EnemyAttacks();
            mapDraw();
        }

        private void btnAttack_Click(object sender, EventArgs e)
        {
            if (gameEngine.Player.CheckRange((Character)cbxEnemies.SelectedItem) == true)
            {
                gameEngine.Player.Attack((Character)cbxEnemies.SelectedItem);
                rtbAttack.Text += "Attacked successfully\n";
                gameEngine.EnemyAttacks();
            }
            else
            {
                rtbAttack.Text += "Out of range\n";
            }
            for (int i = 0; i < gameEngine.EngineMap.Enemies.Length; i++)
            {
                if (gameEngine.EngineMap.Enemies[i].HP < 1)
                {
                    gameEngine.EngineMap.TileMap[gameEngine.EngineMap.Enemies[i].X, gameEngine.EngineMap.Enemies[i].Y] = new EmptyTile(gameEngine.EngineMap.Enemies[i].X, gameEngine.EngineMap.Enemies[i].Y);
                    cbxEnemies.Items.Remove(cbxEnemies.SelectedItem);
                }
            }
            mapDraw();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            gameEngine.Save();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            gameEngine.Load();
            mapDraw();
        }
    }
}
