using System;
using Raylib_cs;
using System.Collections.Generic;
class Star
{
    public static Dictionary<String, List<Star>> allStarsChunks = new Dictionary<String, List<Star>>();
    public float x;
    public float y;
    public int size;
    public int rotation;
    public Star(float x, float y, int size, int rotation)
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
        if (chunkX != (int)Math.Round(SpaceShip.playerShip.x / Raylib.GetScreenWidth()) || chunkY != (int)Math.Round(SpaceShip.playerShip.y / Raylib.GetScreenHeight()))
        {
            chunkX = (int)Math.Round(SpaceShip.playerShip.x / Raylib.GetScreenWidth());
            chunkY = (int)Math.Round(SpaceShip.playerShip.y / Raylib.GetScreenHeight());
            SpawnStars(chunkX, chunkY);
        }

        DrawStars(chunkX, chunkY);

        // Console.WriteLine(Raylib.GetFPS());

        // Console.WriteLine(allStarsChunks.Count);
    }

    public static void SpawnStars(int chunkX, int chunkY)
    {
        // Console.WriteLine(Raylib.GetFrameTime());
        var rnd = new Random();
        int maxStarsPerChunk = 10;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (!allStarsChunks.ContainsKey((chunkX + x) + "-" + (chunkY + y)))
                {
                    List<Star> allStarsInChunk = new List<Star>();
                    for (int i = 0; i < maxStarsPerChunk; i++)
                    {
                        float tempX = rnd.Next((chunkX + x) * Raylib.GetScreenWidth(), (chunkX + x) * Raylib.GetScreenWidth() + Raylib.GetScreenWidth());
                        float tempY = rnd.Next((chunkY + y) * -Raylib.GetScreenHeight(), (chunkY + y) * -Raylib.GetScreenHeight() + Raylib.GetScreenHeight());

                        int tempSize = rnd.Next(6, 24);

                        int tempRotation = rnd.Next(0, 360);

                        allStarsInChunk.Add(new Star(tempX, tempY, tempSize, tempRotation));
                    }
                    allStarsChunks.Add((chunkX + x) + "-" + (chunkY + y), allStarsInChunk); // New star
                }
            }
        }
    }
    public static void DrawStars(int chunkX, int chunkY)
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
                        Raylib.DrawRectangle((int)star.x - (int)SpaceShip.playerShip.x, (int)star.y + (int)SpaceShip.playerShip.y, star.size, star.size, Color.YELLOW);
                    }
                    // }
                }
            }
        }
    }
}