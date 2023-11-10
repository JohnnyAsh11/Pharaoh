using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pharaoh
{

    public enum GameState
    {
        Menu,
        Settings,
        Pause,
        Game,
        GameOver,
        GameWin
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Dictionary<string, Texture2D> gameTextures;

        //tracks the states of the game
        private GameState gameState;

        private KeyboardState kbState;
        private KeyboardState prevKBState;

        private Button startButton;
        private Button settingsButton;
        private Button returnButton;

        private LevelManager lManager;
        private BackgroundManager parallaxManager;
        private BackgroundManager titleManager;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            //changing the window sizing to be our preferred size
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 960;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //giving the globals static class necessary data
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            gameTextures = new Dictionary<string, Texture2D>();
            Globals.SB = _spriteBatch;
            Globals.Graphics = _graphics;

            #region Loading Textures

            //general game textures
            gameTextures["WhiteDebugImage"] = Content.Load<Texture2D>("WhiteDebugImage");
            gameTextures["SkeletonSprite"] = Content.Load<Texture2D>("Skeleton enemy");
            gameTextures["Square"] = Content.Load<Texture2D>("GraySquare");
            gameTextures["Tileset"] = Content.Load<Texture2D>("SpookyTileset");
            gameTextures["PlayerSprite"] = Content.Load<Texture2D>("Necromancer_creativekind-Sheet");
            gameTextures["ProjectileSprite"] = Content.Load<Texture2D>("WaterBall - Startup and Infinite");
            gameTextures["Key"] = Content.Load<Texture2D>("key_01c");
            gameTextures["Lock"] = Content.Load<Texture2D>("book_01f");
            gameTextures["MossBrick"] = Content.Load<Texture2D>("Moss_Bricks");
            gameTextures["EndOfLevel"] = Content.Load<Texture2D>("Totem_Mage_Spritesheet");

            //game direction textures
            gameTextures["Level1Directions"] = Content.Load<Texture2D>("Level1Directions");
            gameTextures["Level1InteractionDirections"] = Content.Load<Texture2D>("Level1InteractionDirections");
            gameTextures["Level1"] = Content.Load<Texture2D>("Level1");
            gameTextures["Level2"] = Content.Load<Texture2D>("Level2");
            gameTextures["Level2Directions"] = Content.Load<Texture2D>("Level2Directions");
            gameTextures["Level3"] = Content.Load<Texture2D>("Level3");
            gameTextures["Level4"] = Content.Load<Texture2D>("Level4");
            gameTextures["Level5"] = Content.Load<Texture2D>("Level5");

            //menu texture loading
            gameTextures["Title"] = Content.Load<Texture2D>("PharaohTitle");
            gameTextures["StartButton"] = Content.Load<Texture2D>("StartButton");
            gameTextures["SettingsButton"] = Content.Load<Texture2D>("SettingsButton");

            #endregion

            #region Parallax

            parallaxManager = new BackgroundManager();
            titleManager = new BackgroundManager();

            //loading in the parallax textures
            gameTextures["DebugImage"] = Content.Load<Texture2D>("DebugImage");
            gameTextures["P1"] = Content.Load<Texture2D>("Layer_0000_9");
            gameTextures["P2"] = Content.Load<Texture2D>("Layer_0001_8");
            gameTextures["P3"] = Content.Load<Texture2D>("Layer_0002_7");
            gameTextures["P4"] = Content.Load<Texture2D>("Layer_0003_6");
            gameTextures["P5"] = Content.Load<Texture2D>("Layer_0004_Lights");
            gameTextures["P6"] = Content.Load<Texture2D>("Layer_0005_5");
            gameTextures["P7"] = Content.Load<Texture2D>("Layer_0006_4");
            gameTextures["P8"] = Content.Load<Texture2D>("Layer_0007_Lights");
            gameTextures["P9"] = Content.Load<Texture2D>("Layer_0008_3");
            gameTextures["P10"] = Content.Load<Texture2D>("Layer_0009_2");
            gameTextures["P11"] = Content.Load<Texture2D>("Layer_0010_1");
            gameTextures["P12"] = Content.Load<Texture2D>("Layer_0011_0");

            //giving globals the game textures hash table
            Globals.GameTextures = gameTextures;

            //adding the parallax layers to the game parallax
            parallaxManager.AddLayer(
                new Layer(
                    Globals.GameTextures["P1"],     //Texture
                    1.1f,                           //Layer depth
                    1.0f));                         //the scale/speed at which the layer is moving

            //applying the same method as above, below
            parallaxManager.AddLayer(new Layer(Globals.GameTextures["P2"], 1.0f, 1.0f));
            parallaxManager.AddLayer(new Layer(Globals.GameTextures["P3"], 0.9f, 1.0f));
            parallaxManager.AddLayer(new Layer(Globals.GameTextures["P4"], 0.8f, 1.0f));
            parallaxManager.AddLayer(new Layer(Globals.GameTextures["P5"], 0.7f, 0.5f));
            parallaxManager.AddLayer(new Layer(Globals.GameTextures["P6"], 0.6f, 0.5f));
            parallaxManager.AddLayer(new Layer(Globals.GameTextures["P7"], 0.5f, 0.5f));
            parallaxManager.AddLayer(new Layer(Globals.GameTextures["P8"], 0.4f, 0.5f));
            parallaxManager.AddLayer(new Layer(Globals.GameTextures["P9"], 0.3f, 0.25f));
            parallaxManager.AddLayer(new Layer(Globals.GameTextures["P10"], 0.2f, 0.25f));
            parallaxManager.AddLayer(new Layer(Globals.GameTextures["P11"], 0.1f, 0.25f));
            parallaxManager.AddLayer(new Layer(Globals.GameTextures["P12"], 0.0f, 0.25f));

            //repeating the process with the menu screen parallax
            titleManager.AddLayer(new Layer(Globals.GameTextures["P1"], 1.1f, 1.0f));
            titleManager.AddLayer(new Layer(Globals.GameTextures["P2"], 1.0f, 1.0f));
            titleManager.AddLayer(new Layer(Globals.GameTextures["P3"], 0.9f, 1.0f));
            titleManager.AddLayer(new Layer(Globals.GameTextures["P4"], 0.8f, 1.0f));
            titleManager.AddLayer(new Layer(Globals.GameTextures["P5"], 0.7f, 0.5f));
            titleManager.AddLayer(new Layer(Globals.GameTextures["P6"], 0.6f, 0.5f));
            titleManager.AddLayer(new Layer(Globals.GameTextures["P7"], 0.5f, 0.5f));
            titleManager.AddLayer(new Layer(Globals.GameTextures["P8"], 0.4f, 0.5f));
            titleManager.AddLayer(new Layer(Globals.GameTextures["P9"], 0.3f, 0.25f));
            titleManager.AddLayer(new Layer(Globals.GameTextures["P10"], 0.2f, 0.25f));
            titleManager.AddLayer(new Layer(Globals.GameTextures["P11"], 0.1f, 0.25f));
            titleManager.AddLayer(new Layer(Globals.GameTextures["P12"], 0.0f, 0.25f));

            //special level messages
            //Level 1:
            parallaxManager.AddLayer(new Layer(Globals.GameTextures["Level1"], 
                0.0f, 3f, new Point(500, 200), new Point(200, 35), 1));
            parallaxManager.AddLayer(new Layer(Globals.GameTextures["Level1Directions"], 
                0.0f, 2f, new Point(500, 400), new Point(400, 84), 1));
            parallaxManager.AddLayer(new Layer(Globals.GameTextures["Level1InteractionDirections"], 
                0.0f, 2f, new Point(1500, 500), new Point(548, 100), 1));

            //Level 2:
            parallaxManager.AddLayer(new Layer(Globals.GameTextures["Level2"], 
                0.0f, 3f, new Point(500, 200), new Point(200, 35), 2));
            parallaxManager.AddLayer(new Layer(Globals.GameTextures["Level2Directions"], 
                0.0f, 2f, new Point(800, 400), new Point(320, 60), 2));

            //Level 3:
            parallaxManager.AddLayer(new Layer(Globals.GameTextures["Level3"],
                0.0f, 3f, new Point(500, 200), new Point(200, 35), 3));

            //Level 4:
            parallaxManager.AddLayer(new Layer(Globals.GameTextures["Level4"],
                0.0f, 3f, new Point(500, 200), new Point(200, 35), 4));

            //Level 5:
            parallaxManager.AddLayer(new Layer(Globals.GameTextures["Level5"],
                0.0f, 3f, new Point(500, 200), new Point(200, 35), 5));

            #endregion


            //--------------------------------------------
            //    specific menu button instantiation
            //--------------------------------------------
            //615 x 420
            startButton = new Button(
                Globals.GameTextures["StartButton"],  
                new Rectangle(650, 600, 308, 105));

            //930 x 420
            settingsButton = new Button(
                Globals.GameTextures["SettingsButton"],
                new Rectangle(575, 750, 465, 105));

            returnButton = new Button(
                Globals.GameTextures["WhiteDebugImage"],
                new Rectangle(50, 50, 400, 100));
            //--------------------------------------------

            lManager = new LevelManager();
            parallaxManager.GetPlayerPosition += lManager.Player.GivePosition;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            kbState = Keyboard.GetState();

            //the main game finite state machine
            switch (gameState)
            {
                case GameState.Menu:

                    //updating the title manager
                    titleManager.MenuUpdate(gameTime);

                    //checking the buttons to know if we are changing states
                    if (startButton.Update())
                    {
                        gameState = GameState.Game;
                    }
                    else if (settingsButton.Update())
                    {
                        gameState = GameState.Settings;
                    }
                    break;
                case GameState.Settings:

                    //updating the parallax
                    titleManager.MenuUpdate(gameTime);

                    //checking the buttons to know if we are changing states
                    if (returnButton.Update())
                    {
                        gameState = GameState.Menu;
                    }
                    break;
                case GameState.Pause:

                    //game pause state checks to re-enter gameplay
                    if (kbState.IsKeyDown(Keys.Enter) &&
                        prevKBState.IsKeyUp(Keys.Enter))
                    {
                        gameState = GameState.Game;
                    }

                    break;
                case GameState.Game:

                    //checking for pauses and if the game was won
                    if (kbState.IsKeyDown(Keys.Enter) &&
                        prevKBState.IsKeyUp(Keys.Enter))
                    {
                        gameState = GameState.Pause;
                    }
                    else if (lManager.Level == 10)
                    {
                        gameState = GameState.GameWin;
                    }

                    parallaxManager.Update(gameTime);

                    //if the update method returns true
                    if (lManager.Update())
                    {
                        //move to the next level
                        lManager.NextLevel();

                        if (lManager.Level == 5)
                        {
                            break;
                        }

                        //reseting the parallax positions
                        parallaxManager.ResetLayers();

                        //resubscribing to events
                        parallaxManager.GetPlayerPosition += lManager.Player.GivePosition;
                    }

                    //once the death animation is completed
                    if (lManager.Player.DeathAnimationDone || lManager.Level > 4)
                    {
                        //reset the level manager and return to the menu
                        lManager.Reset();
                        parallaxManager.ResetLayers();
                        parallaxManager.GetPlayerPosition += lManager.Player.GivePosition;
                        gameState = GameState.Menu;
                    }

                    break;
                //case GameState.GameOver:
                //    break;
                case GameState.GameWin:

                    lManager.Reset();
                    gameState = GameState.Menu;

                    break;
            }            

            prevKBState = kbState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkOrchid);            

            if (gameState == GameState.Menu)
            {
                titleManager.Draw(0);

                _spriteBatch.Begin();
                _spriteBatch.Draw(
                    Globals.GameTextures["Title"],
                    new Rectangle(390, 25, 820, 400),
                    Color.White);
                _spriteBatch.End();

                startButton.Draw();
                settingsButton.Draw();
            }
            else if (gameState == GameState.Settings)
            {
                titleManager.Draw(0);
                returnButton.Draw();
            }
            else if (gameState == GameState.Game ||
                     gameState == GameState.Pause)
            {
                parallaxManager.Draw(lManager.Level);
                lManager.Draw();

                if (gameState == GameState.Pause)
                {
                    //draw pause state buttons here
                }
            }


            base.Draw(gameTime);
        }
    }
}