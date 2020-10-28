﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeroesandGoblins
{
    public partial class Form1 : Form
    {
        private static readonly char cHero = 'H';
        private static readonly char cGoblin = 'G';
        private static readonly char cEmpty = '.';
        private static readonly char cObstacle = 'X';
        GameEngine gameEngine = new GameEngine();

        private class Goblin : Enemy
        {
            public Goblin(int x, int y) : base(x, y, 1, 10, 10, 'G')
            {
                thisTile = TileType.Enemy;
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
                return "Player stats: \nHP:" + hp + "/" + maxHP + "\nDamage:" + damage + "\nCoordinates:" + "[" + x + "," + y + "]"; 
            }
        }

        class Map
        {
            private Tile[,] tileMap;
            private Hero player;
            private Enemy[] enemies;
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

            public Map(int minwidth, int maxwidth, int minheight, int maxheight, int enemynum)
            {
                Height = randomnum.Next(minheight, maxheight);
                Width = randomnum.Next(minwidth, maxwidth);

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
                    Create(Tile.TileType.Enemy);
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

                for (int i = 0; i > enemies.Length; i++)
                {
                    enemies[i].Vision[0] = tileMap[enemies[i].X, enemies[i].Y - 1];
                    enemies[i].Vision[1] = tileMap[enemies[i].X, enemies[i].Y + 1];
                    enemies[i].Vision[2] = tileMap[enemies[i].X - 1, enemies[i].Y];
                    enemies[i].Vision[3] = tileMap[enemies[i].X + 1, enemies[i].Y];
                }
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
                if (type == Tile.TileType.Enemy)
                {
                    enemies[i] = new Goblin(x, y);
                    tileMap[enemies[i].X, enemies[i].Y] = enemies[i];
                    return enemies[i];
                }
                return new EmptyTile(x, y);
            }
        }

        class GameEngine
        {
            private Map engineMap;
            private Hero player;
            //private static readonly char 
            public Map EngineMap { get => engineMap; set => engineMap = value; }
            public Hero Player { get => player; set => player = value; }

            public GameEngine() 
            {
                engineMap = new Map(10,15,10,15,5);
                player = engineMap.Player;
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
                    else
                    {
                        //MessageBox.Show("Path Blocked", "Cannot move here");
                        return false;
                    }
                }
                return false;
            }

            
        }

        class Gold : Item
        {
            private int gold;
            private Random goldRandom = new Random();

            public Gold(int x, int y) : base(x,y)
            {
                gold = goldRandom.Next(1, 6);
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
                    if (gameEngine.EngineMap.TileMap[x, y].ThisTile == Tile.TileType.Enemy)
                    {
                        labelMap.Text += cGoblin;
                    }
                    if (gameEngine.EngineMap.TileMap[x, y].ThisTile == Tile.TileType.Hero)
                    {
                        labelMap.Text += cHero;
                    }
                    if (gameEngine.EngineMap.TileMap[x, y].ThisTile == Tile.TileType.Obstacle)
                    {
                        labelMap.Text += cObstacle;
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
            mapDraw();
        }

        private void btnRight_Click_1(object sender, EventArgs e)
        {
            gameEngine.MovePlayer(Character.Movement.Right);
            mapDraw();
        }

        private void btnDown_Click_1(object sender, EventArgs e)
        {
            gameEngine.MovePlayer(Character.Movement.Down);
            mapDraw();
        }

        private void btnleft_Click_1(object sender, EventArgs e)
        {
            gameEngine.MovePlayer(Character.Movement.Left);
            mapDraw();
        }

        private void btnAttack_Click(object sender, EventArgs e)
        {
            if (gameEngine.Player.CheckRange((Character)cbxEnemies.SelectedItem) == true)
            {
                gameEngine.Player.Attack((Character)cbxEnemies.SelectedItem);
                rtbAttack.Text += "Attacked successfully\n";
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
    }
}
