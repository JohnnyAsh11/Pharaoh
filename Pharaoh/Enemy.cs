using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Pharaoh
{
    public delegate GraphVertex GetContainingVertex(Point center);

    public delegate List<Projectile> GetProjectiles();

    /// <summary>
    /// Enumeration to track the states of the Enemy class
    /// </summary>
    public enum EnemyState
    {
        Walking,
        Attacking,
        Death,
        Resurrect
    }

    /// <summary>
    /// In game skeleton enemy class
    /// </summary>
    public class Enemy : GameObject
    {

        //Fields:
        private int health;
        private bool isAttacking;
        private bool fillHealth;
        private bool isHit;
        private int hitTimer;

        //movement fields
        private int movementSpeed;
        private bool hasStartingPoint;
        private Point startingPoint;
        private bool reachedLocation;

        private int leftDistance;
        private int rightDistance;

        //animation fields
        private int xAnimation;
        private int animationTimer;
        private EnemyState eState;

        private bool isDead;
        private int resurrectTimer;

        //false = right, true = left
        private bool movementDirection;

        //events
        public event GetCollidables GetCollidableRectangles;
        public event GetPosition GetPlayerPosition;
        public event GetProjectiles GetPlayerProjectiles;

        //Properties:
        //get property for the area of effect of the enemy's attack
        public Rectangle AttackRange
        {
            get
            {
                if (isAttacking)
                {
                    return new Rectangle(
                        position.X + 15, 
                        position.Y + 20, 
                        175, 
                        30);
                }
                else
                {
                    return Rectangle.Empty;
                }
            }
        }

        //Constructors:
        /// <summary>
        /// parameterized constructor for the enemy class
        /// </summary>
        /// <param name="position">starting position of the enemy</param>
        /// <param name="rightDistance">right movement distance from the starting position</param>
        /// <param name="leftDistance">left movement distance from the starting position</param>
        public Enemy(Rectangle position, int rightDistance, int leftDistance)
            : base(Globals.GameTextures["SkeletonSprite"], position)
        {
            this.health = 100;
            this.isAttacking = false;
            this.fillHealth = false;
            this.isHit = false;

            this.movementSpeed = 5;
            this.hasStartingPoint = false;
            this.reachedLocation = false;
            this.rightDistance = rightDistance;
            this.leftDistance = leftDistance;

            this.xAnimation = 0;
            this.isDead = false;
            this.animationTimer = 5;
            this.resurrectTimer = 300;

            this.eState = EnemyState.Walking;
            this.movementDirection = false;
        }

        //Methods:
        /// <summary>
        /// Per frame update method for the enemy class
        /// </summary>
        public override void Update()
        {
            //saving the center point of the Enemy to a variable
            Point center = position.Center;

            //getting the player's rectangle
            Rectangle playerPosition = Rectangle.Empty;
            if (GetPlayerPosition != null)
            {
                playerPosition = GetPlayerPosition();
            }

            //getting the player's list of projectiles
            List<Projectile> projectiles = null!;
            if (GetPlayerProjectiles != null)
            {
                projectiles = GetPlayerProjectiles();

                //checking the projectiles for impact with the enemy
                foreach (Projectile proj in projectiles)
                {
                    if (proj.Position.Intersects(position) && health > 0)
                    {
                        //letting the projectile know that it hit something
                        proj.Hit = true;

                        //deducting health
                        health -= 25;

                        //letting the enemy know it was hit
                        isHit = true;

                        //timer for reseting the hit boolean -> MonoGame runs at a constant 60fps
                        hitTimer = 10;  // 1/6 of a second
                    }
                }
            }

            //making the enemy effected by gravity
            position.Y += (int)Globals.Gravity;

            #region Level collisions
            if (GetCollidableRectangles != null)
            {
                //getting the rectangles that can be collided with
                List<Rectangle> collidables = GetCollidableRectangles();


                //general algorithm for collision system:
                //   - creates 6 points located around the enemy
                //   - Checks whether those points are within any collidable rectangles
                //   - Reacts accordingly based on which point is where

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

                //looping through the collidable rectangles
                foreach (Rectangle collidable in collidables)
                {
                    //checking against the points around the enemy
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

            #region General FSM
            switch (eState)
            {
                case EnemyState.Walking:
                    #region Walking
                    //if the enemy already has a starting point
                    if (!hasStartingPoint)
                    {
                        //if not, then set it to the current position
                        startingPoint = position.Center;

                        //set the bool checking for the starting point to true
                        hasStartingPoint = true;
                    }

                    //checks whether or not it has traveled to one side, if not,
                    // move to the other side
                    if (!reachedLocation)
                    {
                        //setting left/right for animations
                        movementDirection = false;

                        //move in the right direction
                        position.X += movementSpeed;

                        //checking if the enemy reached its location
                        if (position.X >= startingPoint.X + rightDistance)
                        {
                            //if so, let the enemy know it made it for the next iteration
                            reachedLocation = true;
                        }
                    }
                    else
                    {
                        //setting left/right for animations
                        movementDirection = true;

                        //move in the left direction
                        position.X -= movementSpeed;

                        //checking if the enemy reached its location
                        if (position.X <= startingPoint.X - leftDistance)
                        {
                            //if so, let the enemy know it made it for the next iteration
                            reachedLocation = false;
                        }
                    }
                    #endregion

                    //checks the distance between the player and the enemy to
                    // determine whether to attack
                    if (Globals.Distance(playerPosition.Center, 
                                         new Point(
                                              position.X - position.Width / 2 + 150, 
                                              position.Y + position.Height / 2)) 
                                         < 100)
                    {
                        //reset the animation source framing
                        xAnimation = 0;

                        //reset the animation timer
                        animationTimer = 5;

                        //change state
                        eState = EnemyState.Attacking;
                    }
                    else if (health <= 0)
                    {
                        //reset animation source framing
                        xAnimation = 0;

                        //reset the animation timer
                        animationTimer = 5;

                        //change the state
                        eState = EnemyState.Death;
                    }

                    break;
                case EnemyState.Attacking:

                    //if the enemy's health becomes less than 0
                    if (health <= 0)
                    {
                        //reset the animation frame
                        xAnimation = 0;

                        //reset the animation timer
                        animationTimer = 5;

                        //change states
                        eState = EnemyState.Death;
                    }

                    break;
                case EnemyState.Death:

                    //if when the enemy dies they are attacking, stop the attack
                    if (isAttacking)
                    {
                        isAttacking = false;
                    }

                    break;
            }            
            #endregion
        }

        /// <summary>
        /// Overridden draw method for the Enemy class
        /// </summary>
        public override void Draw()
        {
            int sourceRect = 128;

            //creating the source rectangle for the animation frames
            Rectangle animationPosition = 
                new Rectangle(
                    position.X,
                    position.Y - 50,
                    200,
                    200);

            Color drawColor = Color.White;

            //if the enemy is hit, add a red hue
            if (isHit)
            {
                drawColor = Color.Red;
            }

            //remove from the hit timer variable
            hitTimer--;
            if (hitTimer == 0)
            {
                isHit = false;
            }

            if (eState == EnemyState.Walking)
            {
                #region walking
                if (!movementDirection)
                {
                    Globals.SB.Draw(
                        asset,
                        animationPosition,
                        new Rectangle(xAnimation, 256, sourceRect, sourceRect),
                        drawColor);
                }
                if (movementDirection)
                {
                    Globals.SB.Draw(
                        asset,
                        animationPosition,
                        new Rectangle(xAnimation, 256, sourceRect, sourceRect),
                        drawColor,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.FlipHorizontally,
                        0f);
                }

                //Animation timer setup:
                // subtract from the timer
                // > if the timer reaches 0
                //   > reset the timer
                //   > move the source frame
                //   > check if the source frame is going to become
                //     greater than the spritesheet
                //      > if it is then reset the source animation
                animationTimer--;
                if (animationTimer == 0)
                {
                    xAnimation += 128;
                    animationTimer = 5;

                    if (xAnimation >= 1536)
                    {
                        xAnimation = 0;
                    }                    
                }
                #endregion
            }
            else if (eState == EnemyState.Attacking)
            {
                #region attacking
                if (!movementDirection)
                {
                    Globals.SB.Draw(
                        asset,
                        animationPosition,
                        new Rectangle(xAnimation, 0, sourceRect, sourceRect),
                        drawColor);
                }
                if (movementDirection)
                {
                    Globals.SB.Draw(
                        asset,
                        animationPosition,
                        new Rectangle(xAnimation, 0, sourceRect, sourceRect),
                        drawColor,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.FlipHorizontally,
                        0f);
                }

                //Animation timer setup:
                // subtract from the timer
                // > if the timer reaches 0
                //   > reset the timer
                //   > move the source frame
                //   > check if the source frame is going to become
                //     greater than the spritesheet
                //      > if it is then reset the source animation
                animationTimer--;
                if (animationTimer == 0)
                {
                    xAnimation += 128;
                    animationTimer = 5;

                    if (xAnimation >= 1664)
                    {
                        animationTimer = 5;
                        xAnimation = 0;
                        eState = EnemyState.Walking;
                    }                    
                }

                if (xAnimation == 512 || 
                    xAnimation == 640 || 
                    xAnimation == 768 ||
                    xAnimation == 896 ||
                    xAnimation == 1024 ||
                    xAnimation == 1152)
                {
                    isAttacking = true;
                }
                else
                {
                    isAttacking = false;
                }
                #endregion
            }
            else if (eState == EnemyState.Death)
            {
                #region Death Animation
                if (!isDead)
                {
                    if (!movementDirection)
                    {
                        Globals.SB.Draw(
                            asset,
                            animationPosition,
                            new Rectangle(xAnimation, 128, sourceRect, sourceRect),
                            drawColor);
                    }
                    if (movementDirection)
                    {
                        Globals.SB.Draw(
                            asset,
                            animationPosition,
                            new Rectangle(xAnimation, 128, sourceRect, sourceRect),
                            drawColor,
                            0f,
                            Vector2.Zero,
                            SpriteEffects.FlipHorizontally,
                            0f);
                    }

                    //Animation timer setup:
                    // subtract from the timer
                    // > if the timer reaches 0
                    //   > reset the timer
                    //   > move the source frame
                    //   > check if the source frame is going to become
                    //     greater than the spritesheet
                    //      > if it is then reset the source animation
                    animationTimer--;
                    if (animationTimer == 0)
                    {
                        xAnimation += 128;
                        animationTimer = 5;

                        if (xAnimation >= 1664)
                        {
                            xAnimation = 0;

                            //set the death variable to true
                            isDead = true;
                        }
                    }
                }
                else
                {
                    if (!movementDirection)
                    {
                        Globals.SB.Draw(
                            asset,
                            animationPosition,
                            new Rectangle(1536, 128, sourceRect, sourceRect),
                            drawColor);
                    }
                    if (movementDirection)
                    {
                        Globals.SB.Draw(
                            asset,
                            animationPosition,
                            new Rectangle(1536, 128, sourceRect, sourceRect),
                            drawColor,
                            0f,
                            Vector2.Zero,
                            SpriteEffects.FlipHorizontally,
                            0f);
                    }

                    resurrectTimer--;

                    if (resurrectTimer <= 0)
                    {
                        resurrectTimer = 300;
                        animationTimer = 5;
                        xAnimation = 1664;
                        eState = EnemyState.Resurrect;
                    }
                }
                #endregion
            }
            else if (eState == EnemyState.Resurrect)
            {
                #region resurrect
                if (!movementDirection)
                {
                    Globals.SB.Draw(
                        asset,
                        animationPosition,
                        new Rectangle(xAnimation, 128, sourceRect, sourceRect),
                        drawColor);
                }
                if (movementDirection)
                {
                    Globals.SB.Draw(
                        asset,
                        animationPosition,
                        new Rectangle(xAnimation, 128, sourceRect, sourceRect),
                        drawColor,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.FlipHorizontally,
                        0f);
                }

                //Animation timer setup:
                // subtract from the timer
                // > if the timer reaches 0
                //   > reset the timer
                //   > move the source frame
                //   > check if the source frame is going to become
                //     greater than the spritesheet
                //      > if it is then reset the source animation
                //      > set the death variable to false
                //      > reset health
                //      > fill the health
                //      > return to a default state (walking)
                animationTimer--;
                if (animationTimer == 0)
                {
                    xAnimation -= 128;
                    animationTimer = 5;

                    if (xAnimation <= 0)
                    {
                        xAnimation = 0;
                        isDead = false;
                        health = 0;
                        fillHealth = true;
                        eState = EnemyState.Walking;
                    }
                }
                #endregion
            }

            //fills the enemy's health back up after resurrection
            //  > done in increments to allow the player a chance to kill it 
            //    quickly with preparation and a lot of skill
            if (fillHealth)
            {
                health += 3;

                if (health >= 100)
                {
                    health = 100;
                    fillHealth = false;
                }
            }

            //drawing the enemy's health bar
            //back box
            Globals.SB.Draw(
                Globals.GameTextures["Square"],
                new Rectangle(position.X + 50, position.Y - 20, 100, 5),
                Color.White);
            //actual red, variable, health
            Globals.SB.Draw(
                Globals.GameTextures["Square"],
                new Rectangle(position.X + 50, position.Y - 20, health, 5),
                Color.Red);
        }

    }
}
