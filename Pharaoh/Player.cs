using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Xna.Framework.Graphics;
using System.Net;
using System.Reflection.Metadata;

namespace Pharaoh
{
    public delegate List<Rectangle> GetCollidables();

    public enum PlayerState
    {
        Walking,
        Attacking,
        Death,
        Idle,
        Hit
    }

    /// <summary>
    /// player controller of the game
    /// </summary>
    public class Player : GameObject
    {

        //Fields:
        private int hitCooldown;
        private int movementSpeed;
        private int health;
        private bool isJumping;
        private bool isDead;
        private int jumpCounter;

        private PlayerState pState;
        private int xAnimation;
        private int animationTimer;
        private bool animationCompleted;
        //false = right, true = left
        private bool moveDirection;

        private List<Projectile> projectiles;
        private int projCooldown;
        private bool createdProj;

        public event GetCollidables GetCollidableRectangles;
        public event GetEnemyAttacks GetEnemyAttackBoxes;

        //Properties:
        //get property to give out the player's health total
        public bool DeathAnimationDone
        {
            get { return animationCompleted; }
        }

        //Constructors:
        /// <summary>
        /// Default constructor for the player class
        /// </summary>
        public Player()
            : base(Globals.GameTextures["PlayerSprite"], Rectangle.Empty)
        {
            this.movementSpeed = 15;
            this.health = 100;
            this.jumpCounter = 0;
            this.xAnimation = 0;
            this.animationTimer = 5;
            this.hitCooldown = 0;
            this.isDead = false;
            this.animationCompleted = false;
            projectiles = new List<Projectile>();
            this.projCooldown = 0;
            this.createdProj = false;

            //starting off facing left
            this.moveDirection = true;
            this.pState = PlayerState.Idle;
            
            StreamReader reader = null!;
            try
            {
                string rawData;
                string[] splitData;
                reader = new StreamReader("../../../player.txt");

                while ((rawData = reader.ReadLine()) != null)
                {
                    splitData = rawData.Split('|');

                    position = new Rectangle(
                        int.Parse(splitData[0]),
                        int.Parse(splitData[1]),
                        int.Parse(splitData[2]),
                        int.Parse(splitData[3]));
                }
            }
            catch (Exception error)
            {
                Debug.Print($"Player class: {error.Message}");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        //Methods:
        /// <summary>
        /// Per frame update method for the player class
        /// </summary>
        public override void Update()
        {
            KeyboardState kbState = Keyboard.GetState();

            switch (pState)
            {
                case PlayerState.Walking:
                    #region basic movement / idle state change
                    if (kbState.IsKeyDown(Keys.D))
                    {
                        position.X += movementSpeed;
                        moveDirection = false;
                    }
                    else if (kbState.IsKeyDown(Keys.A))
                    {
                        position.X -= movementSpeed;
                        moveDirection = true;
                    }
                    else
                    {
                        xAnimation = 0;
                        animationTimer = 5;
                        pState = PlayerState.Idle;
                    }
                    #endregion

                    if (kbState.IsKeyDown(Keys.Space) && !createdProj && projCooldown <= 0)
                    {
                        animationTimer = 5;
                        xAnimation = 0;
                        pState = PlayerState.Attacking;
                    }

                    break;
                case PlayerState.Attacking:

                    //continue to allow basic movement during attacking
                    if (kbState.IsKeyDown(Keys.D))
                    {
                        position.X += movementSpeed;
                    }
                    else if (kbState.IsKeyDown(Keys.A))
                    {
                        position.X -= movementSpeed;
                    }

                    if (!createdProj && xAnimation == 1440)
                    {
                        if (moveDirection)
                        {
                            projectiles.Add(
                                new Projectile(
                                    new Rectangle(
                                        position.X,
                                        position.Y,
                                        75,
                                        75),
                                    true));
                        }
                        else
                        {
                            projectiles.Add(
                                new Projectile(
                                    new Rectangle(
                                        position.X,
                                        position.Y,
                                        75,
                                        75),
                                    false));
                        }

                        createdProj = true;
                        projCooldown = 40;
                    }

                    break;
                case PlayerState.Idle:

                    if (kbState.IsKeyDown(Keys.D) ||
                        kbState.IsKeyDown(Keys.A))
                    {
                        animationTimer = 5;
                        xAnimation = 0;
                        pState = PlayerState.Walking;
                    }
                    else if (kbState.IsKeyDown(Keys.Space) && !createdProj && projCooldown <= 0)
                    {
                        animationTimer = 5;
                        xAnimation = 0;
                        pState = PlayerState.Attacking;
                    }                   

                    break;
            }            

            #region Jumping
            if (GetCollidableRectangles != null)
            {
                List<Rectangle> collidables = GetCollidableRectangles();
                Point bottom = new Point(
                        position.X + (position.Width / 2),
                        position.Y + position.Height);

                foreach (Rectangle collidable in collidables)
                {
                    if (collidable.Contains(bottom))
                    {
                        if (kbState.IsKeyDown(Keys.W) && 
                            pState != PlayerState.Hit)
                        {
                            isJumping = true;
                            jumpCounter = 15;
                            break;
                        }
                    }
                }

                jumpCounter--;
                if (jumpCounter <= 0)
                {
                    isJumping = false;
                    jumpCounter = 0;
                }
            }

            if (!isJumping && jumpCounter == 0 &&
                pState != PlayerState.Attacking &&
                pState != PlayerState.Death)
            {
                position.Y += (int)Globals.Gravity;
            }
            else if (isJumping && 
                pState != PlayerState.Attacking &&
                pState != PlayerState.Death)
            {
                position.Y -= (int)Globals.Gravity;
            }
            #endregion

            if (projectiles.Count > 0)
            {
                List<Rectangle> collidables = null;
                if (GetCollidableRectangles != null)
                {
                    collidables = GetCollidableRectangles();
                }

                for (int i = 0; i < projectiles.Count; i++)
                {
                    projectiles[i].Update(collidables);

                    if (projectiles[i].Range <= 0 ||
                        projectiles[i].Hit == true)
                    {
                        projectiles.RemoveAt(i);
                    }
                }
            }
            projCooldown--;
            if (projCooldown <= 0)
            {                
                projCooldown = 0;
                createdProj = false;
            }

            #region taking skeleton damage
            if (GetEnemyAttackBoxes != null! && pState != PlayerState.Death)
            {
                List<Rectangle> enemyAttacks = GetEnemyAttackBoxes();

                if (hitCooldown <= 0)
                {
                    foreach (Rectangle attack in enemyAttacks)
                    {
                        if (position.Intersects(attack))
                        {
                            health -= 20;
                            hitCooldown = 60;
                            xAnimation = 0;
                            animationTimer = 5;
                            pState = PlayerState.Hit;
                        }
                    }
                }
                else
                {
                    hitCooldown--;
                    if (hitCooldown < 0)
                    {
                        hitCooldown = 0;
                    }
                }                
            }
            #endregion

            if (health <= 0 && !isDead)
            {
                xAnimation = 0;
                animationTimer = 5;
                pState = PlayerState.Death;
                isDead = true;
            }

            #region level collisions
            if (GetCollidableRectangles != null)
            {
                List<Rectangle> walls = GetCollidableRectangles();

                foreach (Rectangle collidable in walls)
                {
                    Point top = new Point(
                        position.X + (position.Width / 2),
                        position.Y);
                    Point bottom = new Point(
                        position.X + (position.Width / 2),
                        position.Y + position.Height);

                    Point left = new Point(
                        position.X,
                        position.Y + (position.Height / 2));
                    Point right = new Point(
                        position.X + position.Width,
                        position.Y + (position.Height / 2));

                    Point bottomLeft = new Point(
                        position.X + 10,
                        position.Y + position.Height);
                    Point bottomRight = new Point(
                        position.X + position.Width - 10,
                        position.Y + position.Height);

                    if (collidable.Contains(top))
                    {
                        position.Y = collidable.Y + collidable.Height;
                    }
                    else if (collidable.Contains(bottom))
                    {
                        position.Y = collidable.Y - 100;
                    }
                    else if (collidable.Contains(left))
                    {
                        position.X = collidable.X + collidable.Width;
                    }
                    else if (collidable.Contains(right))
                    {
                        position.X = collidable.X - 100;
                    }
                    else if (collidable.Contains(bottomRight) ||
                             collidable.Contains(bottomLeft))
                    {
                        position.Y = collidable.Y - 100;
                    }
                }
            }
            #endregion

            if (position.Y < -30)
            {
                position.Y = -30;
            }
        }

        /// <summary>
        /// Draw method for the Player class
        /// </summary>
        public override void Draw()
        {
            //width x height
            //2720 x 896
            //x distance per frame = 160 pixels
            //y distance per frame = 128 pixels

            //rectangle for the animation
            Rectangle animationPosition =
                new Rectangle(
                    position.X - 163,
                    position.Y - 188,
                    400,
                    320);
            
            //variable for the draw color for ease of use
            Color drawColor = Color.White;

            if (pState == PlayerState.Hit)
            {
                //true = left, false = right
                if (moveDirection)
                {
                    Globals.SB.Draw(
                        asset,
                        animationPosition,
                        new Rectangle(xAnimation, 640, 160, 128),
                        drawColor,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.FlipHorizontally,
                        0f);
                }
                else
                {
                    Globals.SB.Draw(
                        asset,
                        animationPosition,
                        new Rectangle(xAnimation, 640, 160, 128),
                        drawColor);
                }

                animationTimer--;
                if (animationTimer <= 0)
                {
                    animationTimer = 5;
                    xAnimation += 160;
                    if (xAnimation >= 800)
                    {
                        xAnimation = 0;
                        animationTimer = 5;
                        pState = PlayerState.Idle;
                    }
                }
            }
            else if (pState == PlayerState.Walking)
            {
                //true = left, false = right
                if (moveDirection)
                {
                    Globals.SB.Draw(
                        asset,
                        animationPosition,
                        new Rectangle(xAnimation, 128, 160, 128),
                        drawColor,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.FlipHorizontally,
                        0f);
                }
                else
                {
                    Globals.SB.Draw(
                        asset,
                        animationPosition,
                        new Rectangle(xAnimation, 128, 160, 128),
                        drawColor);
                }

                animationTimer--;
                if (animationTimer <= 0)
                {
                    animationTimer = 5;
                    xAnimation += 160;
                    if (xAnimation >= 1280)
                    {
                        xAnimation = 0;
                    }
                }
            }
            else if (pState == PlayerState.Idle)
            {
                //true = left, false = right
                if (moveDirection)
                {
                    Globals.SB.Draw(
                        asset,
                        animationPosition,
                        new Rectangle(xAnimation, 0, 160, 128),
                        drawColor,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.FlipHorizontally,
                        0f);
                }
                else
                {
                    Globals.SB.Draw(
                        asset,
                        animationPosition,
                        new Rectangle(xAnimation, 0, 160, 128),
                        drawColor);
                }

                animationTimer--;
                if (animationTimer <= 0)
                {
                    animationTimer = 5;
                    xAnimation += 160;
                    if (xAnimation >= 1280)
                    {
                        xAnimation = 0;
                    }
                }
            }
            else if (pState == PlayerState.Attacking)
            {
                if (moveDirection)
                {
                    Globals.SB.Draw(
                        asset,
                        animationPosition,
                        new Rectangle(xAnimation, 256, 160, 128),
                        drawColor,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.FlipHorizontally,
                        0f);
                }
                else
                {
                    Globals.SB.Draw(
                        asset,
                        animationPosition,
                        new Rectangle(xAnimation, 256, 160, 128),
                        drawColor);
                }

                animationTimer--;
                if (animationTimer <= 0)
                {
                    animationTimer = 5;
                    xAnimation += 160;
                    if (xAnimation >= 2080)
                    {
                        xAnimation = 0;
                        animationTimer = 5;
                        pState = PlayerState.Idle;
                    }
                }
            }
            else if (pState == PlayerState.Death)
            {
                if (moveDirection)
                {
                    Globals.SB.Draw(
                        asset,
                        animationPosition,
                        new Rectangle(xAnimation, 768, 160, 128),
                        drawColor,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.FlipHorizontally,
                        0f);
                }
                else
                {
                    Globals.SB.Draw(
                        asset,
                        animationPosition,
                        new Rectangle(xAnimation, 768, 160, 128),
                        drawColor);
                }

                animationTimer--;
                if (animationTimer <= 0)
                {
                    animationTimer = 5;
                    xAnimation += 160;
                    if (xAnimation >= 1440)
                    {
                        animationCompleted = true;
                    }
                }

            }

            if (projectiles.Count > 0)
            {
                foreach (Projectile proj in projectiles)
                {
                    proj.Draw();
                }
            }

            //drawing UI elements
            //health bar:
            Globals.SB.Draw(
                Globals.GameTextures["Square"],
                new Rectangle(position.X - 740, -25, 400, 25),
                Color.DarkGray);
            Globals.SB.Draw(
                Globals.GameTextures["Square"],
                new Rectangle(position.X - 740, -25, health * 4, 25),
                Color.Red);

            //Projectile cooldown
            Globals.SB.Draw(
                Globals.GameTextures["Square"],
                new Rectangle(position.X - 740, 10, 60, 60),
                Color.Blue);
            Globals.SB.Draw(
                Globals.GameTextures["Square"],
                new Rectangle(position.X - 740, 10, 60, projCooldown),
                Color.DarkGray);
        }

        /// <summary>
        /// method to give other classes access to the projectiles made by the player
        /// </summary>
        /// <returns>a list of projectiles</returns>
        public List<Projectile> GiveProjectiles()
        {
            return projectiles;
        }

    }
}
