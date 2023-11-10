using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharaoh
{
    public static class Globals
    {

        //Properties:
        // gets/sets property for the SpriteBatch class
        public static SpriteBatch SB { get; set; }

        public static GraphicsDeviceManager Graphics { get; set; }

        //gets/sets the dictionary holding all assets for the entire game
        public static Dictionary<string, Texture2D> GameTextures { get; set; }

        //gets the constant value for gravity
        public static float Gravity { get { return 25f; } }

        //Methods:
        /// <summary>
        /// Finds the distance between two points
        /// </summary>
        /// <param name="point1">the first point</param>
        /// <param name="point2">the second point</param>
        /// <returns>The distance between the two points</returns>
        public static double Distance(Point point1, Point point2)
        {
            double xValues;
            double yValues;
            double distance;

            xValues = Math.Pow((point2.X - point1.X), 2);
            yValues = Math.Pow((point2.Y - point1.Y), 2);
            distance = Math.Sqrt((xValues + yValues));

            return distance;
        }

    }
}
