using System;
using System.Numerics;
using Raylib_cs;
using System.Collections.Generic;
class Star
{
    public static Dictionary<String, Star[]> allStarsChunks = new Dictionary<String, Star[]>();
    public Vector2 pos;
    public int size;
    public int rotation;
    public Star(Vector2 pos, int size, int rotation)
    {
        this.pos = pos;
        this.size = size;
        this.rotation = rotation;
    }
    static int chunkX = 1;
    static int chunkY = 1;
    public static void StarLogic()
    {
        // TO DO... Only play when entering new chunk                  CHANGE MAKE USE VECTOR
        if (chunkX != (int)Player.ship.pos.X / Raylib.GetScreenWidth() || chunkY != (int)Player.ship.pos.Y / Raylib.GetScreenHeight())
        {
            chunkX = (int)Player.ship.pos.X / Raylib.GetScreenWidth();
            chunkY = (int)Player.ship.pos.Y / Raylib.GetScreenHeight();
            SpawnStars();
        }
    }

    public static void SpawnStars()
    {
        var rnd = new Random();

        int screenWidth = Raylib.GetScreenWidth();
        int screenHeight = Raylib.GetScreenHeight();

        Vector2 tempPos;
        int tempSize;
        int tempRotation;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (!allStarsChunks.ContainsKey((chunkX + x) + "-" + (chunkY + y)))
                {
                    int starsPerChunk = rnd.Next(7, 18);
                    Star[] allStarsInChunk = new Star[starsPerChunk];
                    for (int i = 0; i < starsPerChunk; i++)
                    {
                        tempPos.X = rnd.Next((chunkX + x) * screenWidth, (chunkX + x) * screenWidth + screenWidth);
                        tempPos.Y = rnd.Next((chunkY + y) * -screenHeight, (chunkY + y) * -screenHeight + screenHeight);

                        tempSize = rnd.Next(6, 24);

                        tempRotation = rnd.Next(0, 360);

                        allStarsInChunk[i] = new Star(tempPos, tempSize, tempRotation);
                    }
                    allStarsChunks.Add((chunkX + x) + "-" + (chunkY + y), allStarsInChunk); // New star
                }
            }
        }
    }
    public static void DrawStars()
    {
        // Console.WriteLine(chunkX);
        for (int i = 0; i < allStarsChunks.Count; i++)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    // if (allStarsChunks.ContainsKey((chunkX + x) + "-" + (chunkY + y)))
                    // {
                    foreach (Star star in allStarsChunks[(chunkX + x) + "-" + (chunkY + y)])
                    {
                        // Program.* MAKE BETTER USE VECTOR
                        Raylib.DrawRectangle((int)star.pos.X - (int)Player.ship.pos.X, (int)star.pos.Y + (int)Player.ship.pos.Y, star.size, star.size, Color.YELLOW);
                    }
                    // }
                }
            }
        }
    }
}