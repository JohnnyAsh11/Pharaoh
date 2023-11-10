using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Runtime.Intrinsics.X86;
using System.Diagnostics;

namespace Pharaoh
{
    /// <summary>
    /// Key class for the main puzzle mechanic
    /// </summary>
    public class Key : GameObject
    {

        //Fields:
        private bool isUsed;
        private bool isCollected;
        private Rectangle prevPlayerPosition;
        private Point playerFollowPoint;

        private Color drawColor;

        public event GetPosition GetPlayerPosition;

        //Properties:
        //get property for the key's color
        public Color KeyColor
        {
            get { return drawColor; }
        }

        //get/set property for whether the key has been used or not
        public bool IsUsed
        {
            get { return isUsed; }
            set { isUsed = value; }
        }

        //Constructors:
        /// <summary>
        /// Default constructor for the Key class 
        /// </summary>
        public Key(Rectangle position, int drawColor)
            : base(Globals.GameTextures["Key"], position)
        {
            this.isUsed = false;
            this.isCollected = false;
            this.playerFollowPoint = new Point(0, 0);

            if (drawColor == 0)
            {
                this.drawColor = Color.White;
            }
            else if (drawColor == 1)
            {
                this.drawColor = Color.Yellow;
            }
            else if (drawColor == 2)
            {
                this.drawColor = Color.Red;
            }
            else if (drawColor == 3)
            {
                this.drawColor = Color.HotPink;
            }
            else if (drawColor == 4)
            {
                this.drawColor = Color.Purple;
            }
            else if (drawColor == 5)
            {
                this.drawColor = Color.SkyBlue;
            }
            else if (drawColor == 6)
            {
                this.drawColor = Color.Orange;
            }
            else if (drawColor == 7)
            {
                this.drawColor = Color.Chartreuse;
            }
        }

        //Methods:
        /// <summary>
        /// Per frame Update method for the Key class
        /// </summary>
        public override void Update()
        {
            //only update if the key has not yet been used
            if (!isUsed)
            {
                //Getting the player's position Rectangle
                Rectangle playerPosition = Rectangle.Empty;
                if (GetPlayerPosition != null)
                {
                    playerPosition = GetPlayerPosition();
                }

                if (Globals.Distance(position.Center, playerPosition.Center) < 200 && !isCollected)
                {
                    isCollected = true;
                }

                if (isCollected)
                {
                    int maxRange = 50;
                    playerFollowPoint.X = playerPosition.X - 50;
                    playerFollowPoint.Y = playerPosition.Y - 50;

                    if (playerFollowPoint.X < position.X)
                    {
                        position.X -= 5;
                    }
                    else if (playerFollowPoint.X > position.X)
                    {
                        position.X += 5;
                    }

                    if (position.X < playerFollowPoint.X - maxRange)
                    {
                        position.X = playerFollowPoint.X - maxRange;
                    }
                    else if (position.X > playerFollowPoint.X + maxRange)
                    {
                        position.X = playerFollowPoint.X + maxRange;
                    }

                    if (playerFollowPoint.Y < position.Y)
                    {
                        position.Y -= 5;
                    }
                    else if (playerFollowPoint.Y > position.Y)
                    {
                        position.Y += 5;
                    }

                    if (position.Y < playerFollowPoint.Y - maxRange)
                    {
                        position.Y = playerFollowPoint.Y - maxRange;
                    }
                    else if (position.Y > playerFollowPoint.Y + maxRange)
                    {
                        position.Y = playerFollowPoint.Y + maxRange;
                    }
                }

                prevPlayerPosition = playerPosition;
            }
        }

        /// <summary>
        /// Draw method for the Key class
        /// </summary>
        public override void Draw()
        {
            if (!isUsed)
            {
                Globals.SB.Draw(
                    asset,
                    position,
                    drawColor);
            }
        }

    }
}
