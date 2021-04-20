using System;
using Raylib_cs;
using System.Collections.Generic;
class Star
{
    public static Dictionary<String, Star[]> allStarsChunks = new Dictionary<String, Star[]>();
    public int x;
    public int y;
    public int size;
    public int rotation;
    public Star(int x, int y, int size, int rotation)
    {
        this.x = x;
        this.y = y;
        this.size = size;
        this.rotation = rotation;
    }
    static int chunkX = 1;
    static int chunkY = 1;
    public static void StarLogic()
    {
        // TO DO... Only play when entering new chunk
        if (chunkX != (int)Player.ship.x / Raylib.GetScreenWidth() || chunkY != (int)Player.ship.y / Raylib.GetScreenHeight())
        {
            chunkX = (int)Player.ship.x / Raylib.GetScreenWidth();
            chunkY = (int)Player.ship.y / Raylib.GetScreenHeight();
            SpawnStars();
        }
    }

    public static void SpawnStars()
    {
        var rnd = new Random();

        int screenWidth = Raylib.GetScreenWidth();
        int screenHeight = Raylib.GetScreenHeight();

        int tempX;
        int tempY;
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
                        tempX = rnd.Next((chunkX + x) * screenWidth, (chunkX + x) * screenWidth + screenWidth);
                        tempY = rnd.Next((chunkY + y) * -screenHeight, (chunkY + y) * -screenHeight + screenHeight);

                        tempSize = rnd.Next(6, 24);

                        tempRotation = rnd.Next(0, 360);

                        allStarsInChunk[i] = new Star(tempX, tempY, tempSize, tempRotation);
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
                        Raylib.DrawRectangle((int)star.x - (int)Player.ship.x, (int)star.y + (int)Player.ship.y, star.size, star.size, Color.YELLOW);
                    }
                    // }
                }
            }
        }
    }
}