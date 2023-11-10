using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Pharaoh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Diagnostics;

namespace Pharaoh
{
    public class ParallaxManager
    {

        //Fields:
        private List<Layer> layers;

        //Properties: - NONE -

        //Constructors:
        /// <summary>
        /// default constructor for the ParallaxManager class
        /// </summary>
        public ParallaxManager()
        {
            this.layers = new List<Layer>();
        }

        //Methods:
        /// <summary>
        /// method to add layers to the background parallax
        /// </summary>
        /// <param name="layer">Layer being added</param>
        public void AddLayer(Layer layer)
        {
            layers.Add(layer);
        }

        /// <summary>
        /// per frame update method for the ParallaxManager class
        /// </summary>
        /// <param name="movement">How much the player has moved</param>
        /// <param name="gameTime">GameTime object used to track elapsed time</param>
        public void Update(float movement, GameTime gameTime)
        {
            foreach (Layer layer in layers)
            {
                layer.Update(movement, gameTime);
            }
        }

        /// <summary>
        /// Draw method for the layers in the ParallaxManager object
        /// </summary>
        /// <param name="currentLevel">the current level of the game</param>
        public void Draw(int currentLevel)
        {
            Globals.SB.Begin(sortMode: SpriteSortMode.FrontToBack);
            foreach (Layer layer in layers)
            {
                if (layer.IsFullscreen)
                {
                    layer.Draw();
                }
            }
            Globals.SB.End();

            Globals.SB.Begin();
            foreach (Layer layer in layers)
            {
                if (!layer.IsFullscreen && layer.Level == currentLevel)
                {
                    layer.Draw();
                }
            }
            Globals.SB.End();
        }

        /// <summary>
        /// resets all layers in the parallax
        /// </summary>
        public void ResetLayers()
        {
            foreach(Layer layer in layers)
            {
                layer.ResetPosition();
            }
        }

    }
}
