using System;
using System.Collections.Generic;
using Raylib_cs;
class Bullet
{
    public static List<Bullet> allBullets = new List<Bullet>();

    // Position variables
    public float x, y;

    // Rotation variable
    public float rotation;

    // Velocity variables
    private float speed, xVelocity, yVelocity;

    // Damage
    public int damage;
    public Bullet(float x, float y, float rotation, float speed, int damage)
    {
        this.rotation = rotation;
        this.x = x;
        this.y = y;
        this.speed = speed;
        this.damage = damage;

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
    public static void SpawnBullet(float x, float y, float rotation, int shipHeight, float speed, int damage)
    {
        var pos = Program.CalculatePositionVelocity(x, y, shipHeight, rotation);
        float xPos = pos.x;
        float yPos = pos.y;

        // allBullets.Add(new Bullet(xPos, yPos, rotation, 10, "player"));
        new Bullet(xPos, yPos, rotation, speed, damage);

        // Calculate x and y velocity
        var newVelocity = Program.CalculatePositionVelocity(0, 0, allBullets[allBullets.Count - 1].speed, rotation);
        float xVelocity = newVelocity.x;
        float yVelocity = newVelocity.y;

        allBullets[allBullets.Count - 1].xVelocity = xVelocity;
        allBullets[allBullets.Count - 1].yVelocity = yVelocity;
    }
}