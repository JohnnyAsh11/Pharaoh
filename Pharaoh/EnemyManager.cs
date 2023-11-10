using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Diagnostics;

namespace Pharaoh
{
    public delegate List<Point> GiveEnemyCenters();

    public delegate List<Rectangle> GetEnemyAttacks();

    /// <summary>
    /// Manager for the skeleton enemies of the game
    /// </summary>
    public class EnemyManager
    {

        //Fields:
        private int currentLevel;
        private List<Enemy> enemies;

        //Properties: - NONE -

        //Constructors:
        /// <summary>
        /// default constructor for the EnemyManager class
        /// </summary>
        public EnemyManager()
        {
            this.currentLevel = 0;
            this.enemies = new List<Enemy>();
        }

        //Methods:
        /// <summary>
        /// instantiates the enemies for a level of the game
        /// </summary>
        public void InstantiateEnemies(Graph graph, Player player, string filepath)
        {
            StreamReader reader = null!;

            //increasing the level count
            currentLevel++;

            //clearing the enemy list prior to instantiating
            enemies.Clear();

            try
            {
                reader = new StreamReader(filepath);
                string rawData;
                string[] splitData;

                while ((rawData = reader.ReadLine()!) != null)
                {
                    splitData = rawData.Split('|');

                    //actually instantiating the enemies and parsing
                    // the data from the level files
                    enemies.Add(new Enemy(new Rectangle(
                                 int.Parse(splitData[0]),
                                 int.Parse(splitData[1]),
                                 int.Parse(splitData[2]),
                                 int.Parse(splitData[3])),
                                 int.Parse(splitData[4]),
                                 int.Parse(splitData[5])));
                }
            }
            //performing proper exception handling
            catch (Exception error)
            {
                Debug.Print(error.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            //subscribing necessary methods to the enemy events
            foreach (Enemy enemy in enemies)
            {
                enemy.GetCollidableRectangles += graph.GiveCollidables;
                enemy.GetPlayerPosition += player.GivePosition;
                enemy.GetPlayerProjectiles += player.GiveProjectiles;
            }
        }

        /// <summary>
        /// draws all drawable enemies
        /// </summary>
        public void Draw()
        {
            //drawing the enemies in the enemy manager
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw();
            }
        }

        /// <summary>
        /// Updates all enemies in the enemy list
        /// </summary>
        public void Update()
        {
            //Updating all enemies in the list
            foreach (Enemy enemy in enemies)
            {
                enemy.Update();
            }
        }

        /// <summary>
        /// Gets all enemy attack hitboxes
        /// </summary>
        /// <returns>a list of all enemy attack hitbox ranges</returns>
        public List<Rectangle> GetAttackHitboxes()
        {
            List<Rectangle> attackHitboxes = new List<Rectangle>();

            //collecting all the enemy hitboxes
            foreach (Enemy enemy in enemies)
            {
                //adding them to the list
                attackHitboxes.Add(enemy.AttackRange);
            }

            return attackHitboxes;
        }

    }
}
