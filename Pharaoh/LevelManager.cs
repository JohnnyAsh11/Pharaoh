using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Data;
using System.Transactions;
using System.Diagnostics;

namespace Pharaoh
{
    /// <summary>
    /// manages all aspects of the game levels
    /// </summary>
    public class LevelManager
    {

        //Fields:
        private int level;
        private string enemyFilepath;

        private Graph graph;

        private Camera camera;
        private Player player;

        private PuzzleManager pManager;
        private EnemyManager eManager;
        private bool isEnemiesInstatiated;

        //Properties:
        //returns a reference to the player object
        public Player Player
        {
            get { return player; }
        }

        //gets the current level of the game
        public int Level
        {
            get { return level; }
        }

        //Constructors:
        /// <summary>
        /// Default constructor for the LevelManager class
        /// </summary>
        public LevelManager()
        {
            this.level = 1;

            this.player = new Player();
            this.camera = new Camera();

            this.graph = new Graph("../../../Level1/Level1Collidables.txt", "../../../Level1/TexturesLevel1.txt");
            this.pManager = new PuzzleManager("../../../Level1/Level1Puzzle.txt", player, graph);

            this.eManager = new EnemyManager();
            this.enemyFilepath = "../../../Level1/EnemyLevel1.txt";
            this.isEnemiesInstatiated = false;

            player.GetCollidableRectangles += graph.GiveCollidables;
        }

        //Methods:
        /// <summary>
        /// per frame update method for the LevelManager
        /// </summary>
        public bool Update()
        {
            //instantiating enemies if they are not already
            if (!isEnemiesInstatiated)
            {
                //instantiating the enemies
                eManager.InstantiateEnemies(graph, player, enemyFilepath);

                //subscribing the enemy hitboxes to the player
                player.GetEnemyAttackBoxes += eManager.GetAttackHitboxes;
                isEnemiesInstatiated = true;
            }

            pManager.Update();
            player.Update();
            eManager.Update();
            camera.FollowObject(player);

            //checking to see if the player is in the win tile
            GraphVertex winTile = null!;
            if ((winTile = graph.FindWinTile()) != null)
            {
                if (winTile.Contains(player.Position.Center))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// controls the current files that all objects are using
        /// </summary>
        public void NextLevel()
        {
            //moving the level counter up 1
            level++;

            //instantiating a new graph with the new collidable points/tile textures
            graph = new Graph($"../../../Level{level}/Level{level}Collidables.txt", 
                              $"../../../Level{level}/TexturesLevel{level}.txt");

            //enemy filepath changes
            isEnemiesInstatiated = false;
            this.enemyFilepath = $"../../../Level{level}/EnemyLevel{level}.txt";

            //reinstantiating the player
            player = new Player();

            //instantiating a new puzzleManager
            pManager = new PuzzleManager($"../../../Level{level}/Level{level}Puzzle.txt", player , graph);

            player.GetCollidableRectangles += graph.GiveCollidables;
        }

        /// <summary>
        /// resets the game back to level 1
        /// </summary>
        public void Reset()
        {
            this.level = 1;

            this.player = new Player();
            this.camera = new Camera();

            this.graph = new Graph("../../../Level1/Level1Collidables.txt", "../../../Level1/TexturesLevel1.txt");
            this.pManager = new PuzzleManager("../../../Level1/Level1Puzzle.txt", player, graph);

            this.eManager = new EnemyManager();
            this.enemyFilepath = "../../../Level1/EnemyLevel1.txt";
            this.isEnemiesInstatiated = false;

            player.GetCollidableRectangles += graph.GiveCollidables;
        }

        /// <summary>
        /// Draw method for the LevelManager class
        /// </summary>
        public void Draw()
        {
            Globals.SB.Begin(transformMatrix: camera.Transform);

            graph.Draw();
            pManager.Draw();
            eManager.Draw();
            player.Draw();

            Globals.SB.End();
        }

    }
}
