using System;
using System.Collections.Generic;
using Raylib_cs;
class BulletScript
{
    public static List<Bullet> SpawnBullet(List<Bullet> bullets, float x, float y, float rotation, int shipHeight)
    {
        float xPos = Program.CalculatePosition(x + Raylib.GetScreenWidth() / 2, -y + Raylib.GetScreenHeight() / 2, -shipHeight, -rotation).x;
        float yPos = Program.CalculatePosition(x + Raylib.GetScreenWidth() / 2, -y + Raylib.GetScreenHeight() / 2, -shipHeight, -rotation).y;

        bullets.Add(new Bullet(xPos, yPos, 10, 20, "player"));


        return bullets;
    }
}

class Bullet
{
    public float x;
    public float y;
    public float velocity;
    public string type;
    public float timeSpawned;
    public Bullet(float x, float y, float velocity, float timeSpawned, string type)
    {
        this.x = x;
        this.y = y;
        this.velocity = velocity;
        this.timeSpawned = timeSpawned;
        this.type = type;
    }
}