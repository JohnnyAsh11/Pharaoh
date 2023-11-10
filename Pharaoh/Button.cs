using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pharaoh
{
    /// <summary>
    /// buttons on the menu and UI
    /// </summary>
    public class Button
    {

        //Fields:
        private Texture2D asset;
        private Rectangle position;
        private bool isHovered;

        //properties: - NONE -

        //Constructors:
        /// <summary>
        /// Parameterized constructor for the button class
        /// </summary>
        /// <param name="normalAsset">Normal display asset for the button</param>
        /// <param name="hoverAsset">Hovered asset for hovering of the button</param>
        /// <param name="position">Rectangle position of the button</param>
        public Button(Texture2D asset, Rectangle position)
        {
            this.asset = asset;
            this.position = position;
            this.isHovered = false;
        }

        //Methods:
        /// <summary>
        /// per frame update method for the button class
        /// </summary>
        /// <returns>whether or not hte button has been pressed</returns>
        public bool Update()
        {
            MouseState mState = Mouse.GetState();

            //checking if the bounds of the button contain the mouse
            if (position.Contains(mState.Position))
            {
                isHovered = true;

                //if the user click while in bounds of the box then return true
                if (mState.LeftButton == ButtonState.Pressed)
                {
                    return true;
                }
                return false;
            }
            else
            {
                isHovered = false;
                return false;
            }
        }

        /// <summary>
        /// Draw method for the button class
        /// </summary>
        public void Draw()
        {
            Globals.SB.Begin();
            if (isHovered)
            {
                //Drawing the hovered over button
                Globals.SB.Draw(
                    asset,
                    position,
                    new Rectangle(0, 210, asset.Width, asset.Height / 2),
                    Color.White);
            }
            else if (!isHovered)
            {
                //drawing the non-hovered over button
                Globals.SB.Draw(
                    asset,
                    position,
                    new Rectangle(0, 0, asset.Width, asset.Height / 2),
                    Color.White);
            }
            Globals.SB.End();
        }

    }
}
