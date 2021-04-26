using System;
using System.Numerics;
using Raylib_cs;
using System.Collections.Generic;
class Star
{
    public static Dictionary<String, Star[]> allStarsChunks = new Dictionary<String, Star[]>();
    public Vector2 pos;
    public float size;
    public int rotation;
    public Star(Vector2 pos, float size, int rotation)
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
        float tempSize;
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
                        tempPos.X = rnd.Next((chunkX + x) * screenWidth, (chunkX + x) * screenWidth + screenWidth) - Raylib.GetScreenWidth() / 2;
                        tempPos.Y = rnd.Next((chunkY + y) * screenHeight, (chunkY + y) * screenHeight + screenHeight) - Raylib.GetScreenHeight() / 2;

                        tempSize = rnd.Next(4, 13);
                        tempSize *= 0.1f;

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
                    foreach (Star star in allStarsChunks[(chunkX + x) + "-" + (chunkY + y)])
                    {
                        // Program.* MAKE BETTER USE VECTOR
                        Program.DrawObjectRotation(Program.allTextures["Star"], star.pos - Player.ship.pos, star.rotation, star.size, 255);
                    }
                }
            }
        }
    }
}