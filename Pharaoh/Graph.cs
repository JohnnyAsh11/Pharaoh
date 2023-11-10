using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Pharaoh
{
    /// <summary>
    /// graph containing all information for the collidable ground and platforms
    /// </summary>
    public class Graph
    {

        //Fields:
        private GraphVertex[,] vertices;
        private List<Rectangle> collidableWalls;
        private int mazeSizeX;
        private int mazeSizeY;

        //Properties: - NONE -

        //Constructors:
        /// <summary>
        /// default contstructor of the Graph class
        /// </summary>
        public Graph(string collidingFilepath, string tilesFilepath)
        {
            collidableWalls = new List<Rectangle>();
            mazeSizeX = 120;
            mazeSizeY = 9;
            vertices = new GraphVertex[mazeSizeX, mazeSizeY];

            ReadFile(collidingFilepath);
            LoadTileAssets(tilesFilepath);
        }
        
        //Methods:
        /// <summary>
        /// loads in the level of a specified filePath
        /// </summary>
        /// <param name="filePath">the string filepath being loaded in</param>
        public void ReadFile(string filePath)
        {
            StreamReader reader = null!;
            string rawData;
            string[] splitData;
            int loopCounter = 0;
            int tileX = 0;
            int tileY = 0;

            try
            {
                reader = new StreamReader(filePath);

                while ((rawData = reader.ReadLine()!) != null)
                {
                    splitData = rawData.Split('|');

                    for (int i = 0; i < mazeSizeX; i++)
                    {
                        //not collidable
                        if (int.Parse(splitData[i]) == 0)
                        {
                            vertices[i, loopCounter] = new GraphVertex(false, tileX, tileY);
                        }
                        //collidable
                        else if (int.Parse(splitData[i]) == 1)
                        {
                            vertices[i, loopCounter] = new GraphVertex(true, tileX, tileY);
                        }
                        //win tile
                        else if (int.Parse(splitData[i]) == 2)
                        {
                            vertices[i, loopCounter] = new GraphVertex(false, tileX, tileY, true);
                        }

                        tileX += 100;
                    }

                    tileX = 0;
                    tileY += 100;
                    loopCounter++;
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

            //connecting all vertices together
            ConnectAllAdjacency();
        }

        /// <summary>
        /// loads all the textures for the blocks in a level
        /// </summary>
        /// <param name="filepath">Filepath for the file containing texture information</param>
        public void LoadTileAssets(string filepath)
        {
            StreamReader reader = null!;

            try
            {
                reader = new StreamReader(filepath);
                string rawData;
                string[] splitData;
                int loopCounter = 0;

                while ((rawData = reader.ReadLine()!) != null)
                {
                    splitData = rawData.Split('|');

                    for (int x = 0; x < mazeSizeX; x++)
                    {
                        int tile = int.Parse(splitData[x]);
                        vertices[x, loopCounter].Tile = (Tile)tile;
                    }

                    loopCounter++;
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
        }

        /// <summary>
        /// connects adjacency of all vertices in the graph
        /// </summary>
        private void ConnectAllAdjacency()
        {
            if (vertices != null)
            {
                for (int x = 0; x < mazeSizeX; x++)
                {
                    for (int y = 0; y < mazeSizeY; y++)
                    {
                        if (x + 1 < mazeSizeX && x + 1 >= 0)
                        {
                            vertices[x, y].Right = vertices[x + 1, y];
                        }
                        if (x - 1 < mazeSizeX && x - 1 >= 0)
                        {
                            vertices[x, y].Left = vertices[x - 1, y];
                        }
                        if (y + 1 < mazeSizeY && y + 1 >= 0)
                        {
                            vertices[x, y].Down = vertices[x, y + 1];
                        }
                        if (y - 1 < mazeSizeY && y - 1 >= 0)
                        {
                            vertices[x, y].Up = vertices[x, y - 1];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// draw method for the Graph class
        /// </summary>
        public void Draw()
        {
            if (vertices != null)
            {
                for (int x = 0; x < mazeSizeX; x++)
                {
                    for (int y = 0; y < mazeSizeY; y++)
                    {
                        vertices[x, y].Draw();
                    }
                }
            }
        }

        /// <summary>
        /// finds all walls that the player needs to collid with
        /// </summary>
        private void RetrieveCollidables()
        {
            collidableWalls.Clear();

            if (vertices != null)
            {
                for (int x = 0; x < mazeSizeX; x++)
                {
                    for (int y = 0; y < mazeSizeY; y++)
                    {
                        if (vertices[x, y].IsWall)
                        {
                            collidableWalls.Add(vertices[x, y].TilePosition);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// searches through the Graph to find the GraphNode that contains the point
        /// </summary>
        /// <param name="point">Point being looked for</param>
        /// <returns>The GraphNode containing the point</returns>
        public GraphVertex FindVertex(Point point)
        {
            if (vertices != null)
            {
                for (int x = 0; x < mazeSizeX; x++)
                {
                    for (int y = 0; y < mazeSizeY; y++)
                    {
                        if (vertices[x, y].Contains(point))
                        {
                            return vertices[x, y];
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Finds the win tile in the graph
        /// </summary>
        /// <returns>the win tile of the graph</returns>
        public GraphVertex FindWinTile()
        {
            if (vertices != null)
            {
                for (int x = 0; x < mazeSizeX; x++)
                {
                    for (int y = 0; y < mazeSizeY; y++)
                    {
                        if (vertices[x, y].IsWinTile)
                        {
                            return vertices[x, y];
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// gives the collidables walls to the player class
        /// </summary>
        /// <returns>the collidable walls list</returns>
        public List<Rectangle> GiveCollidables()
        {
            RetrieveCollidables();
            return collidableWalls;
        }

    }
}
