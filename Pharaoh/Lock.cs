using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pharaoh
{

    public enum LockDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public delegate List<Key> GetKeys();

    /// <summary>
    /// The main puzzle's lock mechanic
    /// </summary>
    public class Lock : GameObject
    {

        //Fields:
        private bool isUnlocked;
        private Color drawColor;
        private LockDirection openDirection;

        public event GetKeys GetKeys;
        public event GetContainingVertex GetUnlockableVertex;

        //Properties:
        //get property for the Lock's draw color
        public Color LockColor
        {
            get { return drawColor; }
        }

        //Constructors:
        /// <summary>
        /// Parameterized constructor for the Lock class
        /// </summary>
        /// <param name="position">Rectangle position of the Lock</param>
        /// <param name="drawColor">color of the lock (matches with same color key)</param>
        /// <param name="openDirection">direction that the lock is being opened</param>
        public Lock(Rectangle position, int drawColor, LockDirection openDirection)
            : base(Globals.GameTextures["Lock"], position)
        {
            this.isUnlocked = false;
            this.openDirection = openDirection;

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
        /// Per frame update method for the Lock class
        /// </summary>
        public override void Update()
        {
            #region key/lock interaction
            //retrieveing all key positions
            List<Key> keyPositions = null!;

            if (GetKeys != null)
            {
                keyPositions = GetKeys();
            }

            //finding the matching key and checking its position
            if (keyPositions != null)
            {
                Key matchingKey = null!;

                //looping through keys
                foreach (Key key in keyPositions)
                {
                    if (key.KeyColor == drawColor)
                    {
                        //once the match is found, save it and break from the loop
                        matchingKey = key;
                        break;
                    }
                }

                if (matchingKey != null!)
                {
                    //creating two points for the key/lock locations
                    Point keyLocation = matchingKey.Position.Center;
                    Point lockLocation = position.Center;

                    //finding their distance with Globals
                    double distance = Globals.Distance(keyLocation, lockLocation);

                    if (distance < 125)
                    {
                        KeyboardState kbState = Keyboard.GetState();

                        if (kbState.IsKeyDown(Keys.E))
                        {
                            matchingKey.IsUsed = true;
                            isUnlocked = true;
                        }
                    }
                }
            }
            #endregion

            #region graph interaction

            if (GetUnlockableVertex != null && isUnlocked)
            {
                //getting the node that contains the center of the lock's position
                GraphVertex containingVertex = GetUnlockableVertex(position.Center);

                //reference for the vertex being unlocked
                GraphVertex openedVertex = null!;
                if (openDirection == LockDirection.Up)
                {
                    openedVertex = containingVertex.Up;
                }
                else if (openDirection == LockDirection.Down)
                {
                    openedVertex = containingVertex.Down;
                }
                else if (openDirection == LockDirection.Left)
                {
                    openedVertex = containingVertex.Left;
                }
                else if (openDirection == LockDirection.Right)
                {
                    openedVertex = containingVertex.Right;
                }

                if (openedVertex != null!)
                {
                    openedVertex.IsWall = false;
                    openedVertex.Tile = Tile.Empty;
                }
            }

            #endregion

            //special code for level testing that unlocks all Locks
            //KeyboardState kbStateDebug = Keyboard.GetState();
            //if (kbStateDebug.IsKeyDown(Keys.Q))
            //{
            //    isUnlocked = true;
            //}
        }

        /// <summary>
        /// per frame draw method for the Lock class
        /// </summary>
        public override void Draw()
        {
            Globals.SB.Draw(
                asset,
                position,
                drawColor);
        }
    }
}
