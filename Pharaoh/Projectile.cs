using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pharaoh
{
    public class Projectile : GameObject
    {

        //Fields:
        private int speed;
        private int range;

        //false = right, true = left
        private bool direction;
        private bool hit;

        private int xAnimation;
        private int yAnimation;
        private int animationTimer;

        //Properties:
        //get/set property for whether the projectile has hit something
        public bool Hit
        {
            get { return hit; }
            set { hit = value; }
        }

        //public Rectangle Position { get { return position; } }

        public int Range { get { return range; } }

        //Constructors:
        public Projectile(Rectangle position, bool direction)
            : base(Globals.GameTextures["ProjectileSprite"], position)
        {
            if (direction)
            {
                this.speed = -20;
            }
            else
            {
                this.speed = 20;
            }

            this.range = 35;
            this.xAnimation = 0;
            this.yAnimation = 64;
            this.animationTimer = 2;
            this.hit = false;
            
            this.direction = direction;
        }

        //Methods:
        /// <summary>
        /// per frame Update method for the Projectile class
        /// </summary>
        public void Update(List<Rectangle> collidables)
        {
            if (range >= 0 && !hit)
            {
                position.X += speed;

                range--;
            }

            foreach (Rectangle collidable in collidables)
            {
                if (position.Intersects(collidable))
                {
                    hit = true;
                    range = 0;
                }
            }
        }

        public override void Update()
        {
            
        }

        /// <summary>
        /// Draw method for the Projectile class
        /// </summary>
        public override void Draw()
        {
            //64 x 64
            if (!hit)
            {
                if (!direction)
                {
                    Globals.SB.Draw(
                        asset,
                        position,
                        new Rectangle(xAnimation, yAnimation, 64, 64),
                        Color.Red);
                }
                else
                {
                    Globals.SB.Draw(
                        asset,
                        position,
                        new Rectangle(xAnimation, yAnimation, 64, 64),
                        Color.Red,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.FlipHorizontally,
                        0f);
                }

                animationTimer--;
                if (animationTimer == 0)
                {
                    animationTimer = 2;
                    xAnimation += 64;

                    if (xAnimation == 320)
                    {
                        yAnimation += 64;
                        xAnimation = 0;

                        if (yAnimation >= 192)
                        {
                            yAnimation = 192;
                        }
                    }
                }
            }
        }
    }
}
