using System;
using System.Collections.Generic;
using Raylib_cs;
class Bullet
{
    public static List<Bullet> allBullets = new List<Bullet>();
    public float x;
    public float y;
    public float rotation;
    public float velocity;
    public float xVelocity;
    public float yVelocity;
    public string type;
    public Bullet(float x, float y, float rotation, float velocity, string type)
    {
        this.rotation = rotation;
        this.x = x;
        this.y = y;
        this.velocity = velocity;
        this.type = type;
        allBullets.Add(this);
    }
    public static void Move()
    {
        foreach (Bullet bullet in allBullets)
        {
            // Delete bullet if too far away
            if (Math.Abs(bullet.y - SpaceShip.playerShip.y) > 1200 || Math.Abs(bullet.x - SpaceShip.playerShip.x) > 1500)
            {
                allBullets.Remove(bullet);
                break;
            }

            bullet.x += bullet.xVelocity;
            bullet.y += bullet.yVelocity;
            // Console.WriteLine(xPos + " " + yPos + " " + playerShip.x + " " + playerShip.y);
        }
    }
    public static void SpawnBullet(float x, float y, float rotation, int shipHeight)
    {
        var pos = Program.CalculatePositionVelocity(x, y, shipHeight, rotation);
        float xPos = pos.x;
        float yPos = pos.y;

        allBullets.Add(new Bullet(xPos, yPos, rotation, 10, "player"));

        // Calculate x and y velocity
        var newVelocity = Program.CalculatePositionVelocity(0, 0, allBullets[allBullets.Count - 1].velocity, rotation);
        float xVelocity = newVelocity.x;
        float yVelocity = newVelocity.y;

        allBullets[allBullets.Count - 1].xVelocity = xVelocity;
        allBullets[allBullets.Count - 1].yVelocity = yVelocity;
    }
}