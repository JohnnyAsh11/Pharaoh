using System;
using System.Collections.Generic;
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
    /// <summary>
    /// manager for the main puzzle mechanic
    /// </summary>
    public class PuzzleManager
    {
        //Fields:
        private List<Lock> locks;
        private List<Key> keys;

        //Properties: - NONE -

        //Constructors:
        /// <summary>
        /// Parameterized constructor for the PuzzleManager class
        /// </summary>
        /// <param name="filepath">File containing the data for this level's puzzle</param>
        /// <param name="player">reference to the player for event subscription</param>
        public PuzzleManager(string filepath, Player player, Graph graph)
        {
            locks = new List<Lock>();
            keys = new List<Key>();

            InstantiatePuzzle(filepath, player, graph);
        }

        //Methods:
        /// <summary>
        /// Instantiates all the puzzle object for a level
        /// </summary>
        /// <param name="filepath">file path for the object data in a given level</param>
        /// <param name="player">reference to the player for event subscription</param>
        private void InstantiatePuzzle(string filepath, Player player, Graph graph)
        {
            StreamReader reader = null!;

            try
            {
                string rawData;
                string[] splitData;
                reader = new StreamReader(filepath);

                //reading in the file's header
                reader.ReadLine();

                //instantiating the keys
                while ((rawData = reader.ReadLine()) != "LOCKS")
                {
                    splitData = rawData.Split('|');

                    keys.Add(new Key(
                                new Rectangle(
                                    int.Parse(splitData[0]),
                                    int.Parse(splitData[1]),
                                    int.Parse(splitData[2]),
                                    int.Parse(splitData[3])),
                                int.Parse(splitData[4])));
                }

                //instantiating the locks
                while ((rawData = reader.ReadLine()) != null)
                {
                    splitData = rawData.Split('|');

                    //default the lockdirection to up
                    LockDirection lockDirection = LockDirection.Up;
                    if (splitData[5] == "Down")
                    {
                        lockDirection = LockDirection.Down;
                    }
                    else if (splitData[5] == "Up")
                    {
                        lockDirection = LockDirection.Up;
                    }
                    else if (splitData[5] == "Left")
                    {
                        lockDirection = LockDirection.Left;
                    }
                    else if (splitData[5] == "Right")
                    {
                        lockDirection = LockDirection.Right;
                    }

                    locks.Add(new Lock(
                                new Rectangle(
                                    int.Parse(splitData[0]),
                                    int.Parse(splitData[1]),
                                    int.Parse(splitData[2]),
                                    int.Parse(splitData[3])),
                                int.Parse(splitData[4]),
                                lockDirection));
                }
            }
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

            if (keys.Count > 0 && locks.Count > 0)
            {
                foreach (Key puzzleKey in keys)
                {
                    puzzleKey.GetPlayerPosition += player.GivePosition;
                }

                foreach (Lock puzzleLock in locks)
                {
                    puzzleLock.GetKeys += this.GiveKeys;
                    puzzleLock.GetUnlockableVertex += graph.FindVertex;
                }
            }
        }

        /// <summary>
        /// per frame update method for the PuzzleManager class
        /// </summary>
        public void Update()
        {
            foreach (Lock puzzleLock in locks)
            {
                puzzleLock.Update();
            }

            foreach (Key puzzleKey in keys)
            {
                puzzleKey.Update();
            }
        }

        /// <summary>
        /// Draw method for all puzzle objects
        /// </summary>
        public void Draw()
        {
            foreach (Lock puzzleLock in locks)
            {
                puzzleLock.Draw();
            }

            foreach (Key puzzleKey in keys)
            {
                puzzleKey.Draw();
            }
        }

        /// <summary>
        /// method that gives out the references to key objects for the lock class
        /// </summary>
        /// <returns>a list of the current level's keys</returns>
        public List<Key> GiveKeys()
        {
            return keys;
        }
    }
}