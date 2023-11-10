using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharaoh
{
    public enum Tile
    {
        Empty,      //0
        TopLeft,    //1
        TopRight,   //2
        TopGrass,   //3
        Rock,       //4
        UnderLeft,  //5
        UnderRight, //6
        MossBricks, //7
        UnderMiddle //8
    }

    public class GraphVertex
    {

        //Fields:
        private Rectangle tilePosition;
        private bool isWall;
        private Texture2D tileset;
        private Tile tile;

        private bool isWinTile;
        private int xAnimation;
        private int animationTimer;

        private GraphVertex up;
        private GraphVertex down;
        private GraphVertex left;
        private GraphVertex right;

        //Properties:
        #region properties
        public bool IsWall
        {
            get { return isWall; }
            set { isWall = value; }
        }
        public Rectangle TilePosition
        {
            get { return tilePosition; }
        }
        public GraphVertex Up
        {
            get { return up; }
            set { up = value; }
        }
        public GraphVertex Down
        {
            get { return down; }
            set { down = value; }
        }
        public GraphVertex Left
        {
            get { return left; }
            set { left = value; }
        }
        public GraphVertex Right
        {
            get { return right; }
            set { right = value; }
        }
        #endregion

        //Texture property
        //get/set property for the GraphVertex's Tile
        public Tile Tile
        {
            get { return tile; }
            set { tile = value; }
        }

        //returns whether or not this is the tile that wins that level
        public bool IsWinTile
        {
            get { return isWinTile; }
        }

        //Contructors:
        /// <summary>
        /// Parameterized constructor for the graph node class
        /// </summary>
        /// <param name="isWall">tells whether or not the wall is there</param>
        /// <param name="x">x position of the wall</param>
        /// <param name="y">y position of the wall</param>
        public GraphVertex(bool isWall, int x, int y)
        {
            this.isWall = isWall;
            this.tilePosition = new Rectangle(x, y, 100, 100);
            this.tileset = Globals.GameTextures["Tileset"];

            up = null!;
            down = null!;
            left = null!;
            right = null!;
            tile = Tile.TopGrass;

            this.isWinTile = false;
        }

        /// <summary>
        /// Parameterized constructor for the graph node class
        /// </summary>
        /// <param name="isWall">tells whether or not the wall is there</param>
        /// <param name="x">x position of the wall</param>
        /// <param name="y">y position of the wall</param>
        /// <param name="isWinTile">determines if this is the tile that the player enters to win that level</param>
        public GraphVertex(bool isWall, int x, int y, bool isWinTile)
        {
            this.isWall = isWall;
            this.tilePosition = new Rectangle(x, y, 100, 100);
            this.tileset = Globals.GameTextures["Tileset"];

            up = null!;
            down = null!;
            left = null!;
            right = null!;
            tile = Tile.TopGrass;

            this.isWinTile = isWinTile;
            xAnimation = 0;
            animationTimer = 5;
        }


        //Methods:
        /// <summary>
        /// draw method for the GraphNode class
        /// </summary>
        public void Draw()
        {
            //for positioning debugging purposes
            //if (isWall)
            //{
            //    Globals.SB.Draw(
            //        Globals.GameTextures["DebugImage"],
            //        tilePosition,
            //        Color.White);
            //}

            if (tile == Tile.TopLeft)
            {
                Globals.SB.Draw(
                    tileset,
                    tilePosition,
                    new Rectangle(96, 0, 96, 96),
                    Color.White);
            }
            else if (tile == Tile.TopRight)
            {
                Globals.SB.Draw(
                    tileset,
                    tilePosition,
                    new Rectangle(192, 0, 96, 96),
                    Color.White);
            }
            else if (tile == Tile.TopGrass)
            {
                Globals.SB.Draw(
                    tileset,
                    tilePosition,
                    new Rectangle(144, 0, 96, 96),
                    Color.White);
            }
            else if (tile == Tile.Rock)
            {
                Globals.SB.Draw(
                    tileset,
                    tilePosition,
                    new Rectangle(144, 96, 96, 96),
                    Color.White);
            }
            else if (tile == Tile.UnderLeft)
            {
                Globals.SB.Draw(
                    tileset,
                    tilePosition,
                    new Rectangle(480, 96, 96, 96),
                    Color.White);
            }
            else if (tile == Tile.UnderRight)
            {
                Globals.SB.Draw(
                    tileset,
                    tilePosition,
                    new Rectangle(864, 96, 96, 96),
                    Color.White);
            }
            else if (tile == Tile.MossBricks)
            {
                Globals.SB.Draw(
                    Globals.GameTextures["MossBrick"],
                    tilePosition,
                    Color.White);
            }
            if (tile == Tile.UnderMiddle)
            {
                Globals.SB.Draw(
                    tileset,
                    tilePosition,
                    new Rectangle(640, 128, 96, 96),
                    Color.White);
            }
            //draw the special animation for the win tile
            else if (isWinTile)
            {
                Globals.SB.Draw(
                    Globals.GameTextures["EndOfLevel"],
                    tilePosition,
                    new Rectangle(xAnimation, 0, 64, 64),
                    Color.White);

                animationTimer--;
                if (animationTimer <= 0)
                {
                    xAnimation += 64;
                    animationTimer = 5;

                    if (xAnimation >= 896)
                    {
                        xAnimation = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a given point is within the GraphNode's Tile
        /// </summary>
        /// <param name="point">Point being checked for whether it is contained</param>
        /// <returns>whether or not the point is contained</returns>
        public bool Contains(Point point)
        {
            if (tilePosition.Contains(point))
            {
                return true;
            }
            return false;
        }

    }
}
