using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Pharaoh
{
    /// <summary>
    /// Abstract Object class
    /// </summary>
    public abstract class GameObject
    {

        //Fields:
        protected Texture2D asset;
        protected Rectangle position;

        //Properties:
        //get/set properties for the positon's X and Y coordinates
        public int X
        {
            get { return position.X; }
            set { position.X = value; }
        }

        public int Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        public Rectangle Position { get { return position; } }

        //Constructors:
        /// <summary>
        /// Parameterized constructor for the GameObject class
        /// </summary>
        /// <param name="asset">Texture asset for the GameObject</param>
        /// <param name="position">Rectangle position of the Object</param>
        public GameObject(Texture2D asset, Rectangle position)
        {
            this.asset = asset;
            this.position = position;
        }

        //Methods:
        /// <summary>
        /// per frame update method for GameObejcts
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Draw method for GameObjects
        /// </summary>
        public virtual void Draw()
        {
            Globals.SB.Draw(
                asset,
                position,
                Color.White);
        }

        /// <summary>
        /// gives the GameObject's rectangle to any class that may need it
        /// </summary>
        /// <returns>the GameObject's Rectangle position</returns>
        public virtual Rectangle GivePosition()
        {
            return position;
        }

    }
}
