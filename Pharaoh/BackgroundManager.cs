using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Pharaoh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharaoh
{

    public delegate Rectangle GetPosition();

    /// <summary>
    /// Class that manages the Parallaxing background of the game
    /// </summary>
    public class BackgroundManager
    {

        //Fields:
        private ParallaxManager pManager;
        private float speed;
        private float movement;
        private KeyboardState kbState;
        private KeyboardState prevKbState;
        private Point prevLocation;

        public event GetPosition GetPlayerPosition;

        //Properties: - NONE -

        //Constructors:
        /// <summary>
        /// Default constructor of the Background Manager
        /// </summary>
        public BackgroundManager()
        {
            pManager = new ParallaxManager();
            speed = 200f;
        }

        //Methods:
        /// <summary>
        /// used to add layers into the ParallaxManager Field
        /// </summary>
        /// <param name="layer">layer being added</param>
        public void AddLayer(Layer layer)
        {
            //adding the layer
            pManager.AddLayer(layer);
        }

        /// <summary>
        /// Per frame update method for the GameManager class
        /// </summary>
        /// <param name="gameTime">GameTime obj used to track elapsed time</param>
        public void Update(GameTime gameTime)
        {
            kbState = Keyboard.GetState();

            //making sure the event is not null
            if (GetPlayerPosition != null)
            {
                Rectangle playerRect = GetPlayerPosition();

                //checking which way the player is moving and altering the direction of 
                //  the speed field accordingly
                if (playerRect.Center.X > prevLocation.X)
                {
                    movement = -speed;
                }
                else if (playerRect.Center.X < prevLocation.X)
                {
                    movement = speed;
                }
                else
                {
                    //if the player is not moving then make the parallax still
                    movement = 0; 
                }

                prevLocation = playerRect.Center;
            }

            //updating the Parallax
            pManager.Update(movement, gameTime);
        }

        /// <summary>
        /// Update method for the menu Parallax
        /// </summary>
        /// <param name="gameTime">GameTime used to track the total elapsed time</param>
        public void MenuUpdate(GameTime gameTime)
        {
            kbState = Keyboard.GetState();

            //fun little add-in that makes it so when you press the space bar
            //  the parallax on the menu screens flips directions
            if (kbState.IsKeyDown(Keys.Space) &&
                prevKbState.IsKeyUp(Keys.Space))
            {
                if (movement < 0)
                {
                    movement = speed;
                }
                else
                {
                    movement = -speed;
                }
            }
            else if (movement == 0)
            {
                movement = -speed;
            }

            prevKbState = kbState;

            //updating the actual parallax object
            pManager.Update(movement, gameTime);
        }

        /// <summary>
        /// standard draw method for the GameManager class
        /// </summary>
        public void Draw(int currentLevel)
        {
            //calling the draw method on the parallax manager
            pManager.Draw(currentLevel);
        }

        /// <summary>
        /// resets all layers in the parallax
        /// </summary>
        public void ResetLayers()
        {
            //calling the layer reset method on the parallax
            pManager.ResetLayers();
        }

    }
}
