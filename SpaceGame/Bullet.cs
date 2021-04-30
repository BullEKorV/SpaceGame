using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;
class Bullet
{
    public static List<Bullet> allBullets = new List<Bullet>();

    // Position variables
    public Vector2 pos, velocity;

    // Move variables
    public float rotation, speed;

    // Stats
    public bool isPlayer, isExplosive, isHoming;
    public int damage, health;
    public Bullet(Vector2 pos, float rotation, int offset, float speed, int damage, bool isPlayer, bool isExplosive, bool isHoming)
    {
        pos = Program.CalculatePositionVelocity(pos, offset, rotation);

        this.velocity = Program.CalculatePositionVelocity(new Vector2(0, 0), speed, rotation);
        this.rotation = rotation;
        this.pos = pos;
        this.speed = speed;
        this.damage = damage;
        this.isPlayer = isPlayer;
        this.isExplosive = isExplosive;
        this.isHoming = isHoming;

        if (isHoming)
            this.health = 4;

        allBullets.Add(this);
    }
    public static void Move()
    {
        for (int i = 0; i < allBullets.Count; i++)
        {
            if (allBullets[i].isHoming)
            {
                allBullets[i].rotation = Program.LookAt(allBullets[i].pos, Player.ship.pos);
                allBullets[i].velocity = Program.CalculatePositionVelocity(new Vector2(0, 0), allBullets[i].speed, allBullets[i].rotation);

                // Remove bullet if touching player bullet
                if (allBullets[i].isPlayer == false)
                {
                    List<Bullet> allPlayerBullets = allBullets.FindAll(x => x.isPlayer == true);

                    // Console.WriteLine(allPlayerBullets.Count);
                    for (int y = 0; y < allPlayerBullets.Count; y++)
                    {
                        if (Vector2.Distance(allBullets[i].pos, allPlayerBullets[y].pos) < 20)
                        {
                            if (allPlayerBullets[y].isExplosive)
                                allBullets[i].health -= 4;
                            else
                                allBullets[i].health--;
                            allBullets.Remove(allPlayerBullets[y]);
                        }
                    }
                }
            }

            // Rotate explosive bullets
            if (allBullets[i].isExplosive)
                allBullets[i].rotation += 6;
            if (allBullets[i].rotation > 360)
                allBullets[i].rotation -= 360;

            allBullets[i].pos = Program.CalculatePosition(allBullets[i].pos, allBullets[i].velocity);

            // Delete bullet if too far away
            if (Vector2.Distance(allBullets[i].pos, Player.ship.pos) > 1400 || (allBullets[i].health == 0 && allBullets[i].isHoming))
            {
                allBullets.Remove(allBullets[i]);
            }
        }
    }
}