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
        for (int i = 0; i < allBullets.Count; i++)
        {
            allBullets[i].x += allBullets[i].xVelocity;
            allBullets[i].y += allBullets[i].yVelocity;

            // Delete bullet if too far away
            if (Math.Abs(allBullets[i].y - PlayerShip.ship.y) > 1200 || Math.Abs(allBullets[i].x - PlayerShip.ship.x) > 1500)
            {
                allBullets.Remove(allBullets[i]);
            }
        }
    }
    public static void SpawnBullet(float x, float y, float rotation, int shipHeight)
    {
        var pos = Program.CalculatePositionVelocity(x, y, shipHeight, rotation);
        float xPos = pos.x;
        float yPos = pos.y;

        // allBullets.Add(new Bullet(xPos, yPos, rotation, 10, "player"));
        new Bullet(xPos, yPos, rotation, 20, "player");

        // Calculate x and y velocity
        var newVelocity = Program.CalculatePositionVelocity(0, 0, allBullets[allBullets.Count - 1].velocity, rotation);
        float xVelocity = newVelocity.x;
        float yVelocity = newVelocity.y;

        allBullets[allBullets.Count - 1].xVelocity = xVelocity;
        allBullets[allBullets.Count - 1].yVelocity = yVelocity;
    }
}