using System;
using Raylib_cs;
using System.Collections.Generic;
class Star
{
    public static List<Star> allStars = new List<Star>();
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

        allStars.Add(this);
    }

    public static void StarLogic()
    {
        SpawnStars();
        DeleteStars();
    }

    public static void SpawnStars()
    {
        int maxStars = 100;
        for (int i = allStars.Count; i < maxStars; i++)
        {
            var rnd = new Random();

            float tempX = rnd.Next((int)SpaceShip.playerShip.x - Raylib.GetScreenWidth() / 2, (int)SpaceShip.playerShip.x + Raylib.GetScreenWidth() / 2);
            float tempY = rnd.Next((int)SpaceShip.playerShip.y - Raylib.GetScreenHeight() / 2, (int)SpaceShip.playerShip.y + Raylib.GetScreenHeight() / 2);

            int tempSize = rnd.Next(10, 20);

            int tempRotation = rnd.Next(0, 360);

            allStars.Add(new Star(tempX, tempY, tempSize, tempRotation));
        }
    }
    public static void DeleteStars()
    {
        for (int i = 0; i < allStars.Count; i++)
        {
            if (Math.Abs(allStars[i].y - SpaceShip.playerShip.y) > Raylib.GetScreenHeight() / 2 + 100 || Math.Abs(allStars[i].x - SpaceShip.playerShip.x) > Raylib.GetScreenWidth() / 2 + 100)
            {
                allStars.Remove(allStars[i]);
            }
        }
    }
}