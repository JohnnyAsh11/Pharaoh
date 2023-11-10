using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Pharaoh
{
    /// <summary>
    /// Class that handles creating the transformation matrix to move with a GameObject
    /// </summary>
    public class Camera
    {

        //Fields:
        private Matrix transformationMatrix;
        private Vector2 screenBounds;

        //Properties:
        public Matrix Transform { get { return transformationMatrix; } }

        //Constructors:
        /// <summary>
        /// Default constructor for the Camera class
        /// </summary>
        public Camera()
        {
            transformationMatrix = new Matrix();
            screenBounds = new Vector2(1600, 960);
        }

        //Methods:
        /// <summary>
        /// Updates the 
        /// </summary>
        /// <param name="gameObj"></param>
        public void FollowObject(GameObject gameObj)
        {
            //tracking the position of the player
            Matrix position = Matrix.CreateTranslation(
                -gameObj.X - gameObj.Position.Width / 2,
                -420,
                0);

            //calculating the offset based on the screen bounds
            Matrix offset = Matrix.CreateTranslation(
                screenBounds.X / 2,
                screenBounds.Y / 2,
                0);

            //multiplying both together to get the transformationMatrix
            transformationMatrix = position * offset;
        }

    }
}
