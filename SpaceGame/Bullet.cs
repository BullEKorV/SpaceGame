using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;
class Bullet
{
    public static List<Bullet> allBullets = new List<Bullet>();
    public bool isPlayer, isExplosive;

    // Position variables
    public Vector2 pos;

    // Rotation variable
    public float rotation;

    // Velocity variables
    private float speed, xVelocity, yVelocity;

    // Damage
    public int damage;
    public Bullet(Vector2 pos, float rotation, float speed, int damage, bool isPlayer, bool isExplosive)
    {
        this.rotation = rotation;
        this.pos = pos;
        this.speed = speed;
        this.damage = damage;
        this.isPlayer = isPlayer;
        this.isExplosive = isExplosive;

        allBullets.Add(this);
    }
    public static void Move()
    {
        for (int i = 0; i < allBullets.Count; i++)
        {
            allBullets[i].pos = Program.CalculatePosition(allBullets[i].pos, allBullets[i].xVelocity, allBullets[i].yVelocity);

            // Delete bullet if too far away                                    CHANGE MAKE USE VECTOR
            if (Vector2.Distance(allBullets[i].pos, Player.ship.pos) > 1400)
            {
                allBullets.Remove(allBullets[i]);
            }
        }
    }
    public static void SpawnBullet(Vector2 pos, float rotation, int shipHeight, float speed, int damage, bool isPlayer, bool isExplosive)
    {
        Vector2 newPos = Program.CalculatePositionVelocity(pos, shipHeight, rotation);

        // allBullets.Add(new Bullet(xPos, yPos, rotation, 10, "player"));
        new Bullet(newPos, rotation, speed, damage, isPlayer, isExplosive);

        // Calculate x and y velocity
        var newVelocity = Program.CalculatePositionVelocity(new Vector2(0, 0), allBullets[allBullets.Count - 1].speed, rotation);
        float xVelocity = newVelocity.X;
        float yVelocity = newVelocity.Y;

        allBullets[allBullets.Count - 1].xVelocity = xVelocity;
        allBullets[allBullets.Count - 1].yVelocity = yVelocity;
    }
}