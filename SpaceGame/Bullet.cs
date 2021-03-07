using System;
using System.Collections.Generic;
using Raylib_cs;
class BulletScript
{
    public static List<Bullet> MoveBullets(List<Bullet> bullets, SpaceShip playerShip)
    {
        foreach (Bullet bullet in bullets)
        {
            bullet.velocity = 0;
            // Delete bullet if too far away
            if (Math.Abs(bullet.y - playerShip.y) > 1200 || Math.Abs(bullet.x - playerShip.x) > 1500)
            {
                bullets.Remove(bullet);
                break;
            }

            // bullet.rotation = Program.LookAt(bullet.x, bullet.y, playerShip.x, playerShip.y);

            var newPos = Program.CalculatePosition(bullet.x, bullet.y, bullet.velocity, bullet.rotation);
            float xPos = newPos.x;
            float yPos = newPos.y;

            bullet.x = xPos;
            bullet.y = yPos;
            // Console.WriteLine(xPos + " " + yPos + " " + playerShip.x + " " + playerShip.y);
        }
        return bullets;
    }
    public static List<Bullet> SpawnBullet(List<Bullet> bullets, float x, float y, float rotation, int shipHeight)
    {
        var pos = Program.CalculatePosition(x, y, shipHeight, rotation);
        float xPos = pos.x;
        float yPos = pos.y;

        bullets.Add(new Bullet(xPos, yPos, rotation, 14, "player"));

        return bullets;
    }
}

class Bullet
{
    public float x;
    public float y;
    public float rotation;
    public float velocity;
    public string type;
    public Bullet(float x, float y, float rotation, float velocity, string type)
    {
        this.rotation = rotation;
        this.x = x;
        this.y = y;
        this.velocity = velocity;
        this.type = type;
    }
}