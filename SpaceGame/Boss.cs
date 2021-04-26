using System;
using Raylib_cs;
using System.Numerics;
class BossSun
{
    public static BossSun ship;

    //Position variables
    public Vector2 pos;

    // Size variables
    public int width, height;

    // Velocity variables
    private float speed;

    // Stats
    public int health, maxHealth, damage;

    // Timer variables
    private float timeTillNextShoot, fireRate;

    // Sun variables
    private float sunRotation, sunTimeTillNextShoot, sunFireRate;
    public BossSun()
    {
        var rnd = new Random();

        int side = rnd.Next(1, 5); // 1 up, 2 down, 3 left, 4 right

        Vector2 pos = new Vector2(0, 0);

        switch (side)
        {
            case 1:
                pos = new Vector2(Player.ship.pos.X + rnd.Next(-Raylib.GetScreenWidth() / 2, Raylib.GetScreenWidth() / 2), Player.ship.pos.Y + Raylib.GetScreenHeight() / 2 + 200);
                break;
            case 2:
                pos = new Vector2(Player.ship.pos.X + rnd.Next(-Raylib.GetScreenWidth() / 2, Raylib.GetScreenWidth() / 2), Player.ship.pos.Y - Raylib.GetScreenHeight() / 2 - 200);
                break;
            case 3:
                pos = new Vector2(Player.ship.pos.X - Raylib.GetScreenWidth() / 2 - 200, Player.ship.pos.Y + rnd.Next(-Raylib.GetScreenHeight() / 2, Raylib.GetScreenHeight() / 2));
                break;
            case 4:
                pos = new Vector2(Player.ship.pos.X + Raylib.GetScreenWidth() / 2 + 200, Player.ship.pos.Y + rnd.Next(-Raylib.GetScreenHeight() / 2, Raylib.GetScreenHeight() / 2));
                break;
            default:
                break;
        }

        int maxHealth = 200;
        float speed = 0.003f;
        int damage = 10;
        float fireRate = 0.2f;
        float sunFireRate = 1f;

        this.pos = pos;
        this.maxHealth = maxHealth;
        this.health = maxHealth;
        this.speed = speed;
        this.damage = damage;
        this.fireRate = fireRate;
        this.sunFireRate = sunFireRate;
        this.width = Program.allTextures["BossSun"].width;
        this.height = Program.allTextures["BossSun"].height;

        ship = (this);
    }
    public static void AI()
    {
        // float distanceToPlayer = Vector2.Distance(enemy.pos, Player.ship.pos);

        // Walk towards player
        Vector2 velocity = ship.pos - Player.ship.pos;
        velocity *= 0.003f;
        ship.pos -= velocity;

        // Sun logic
        if (Raylib.GetTime() > ship.sunTimeTillNextShoot)
        {
            for (int i = 0; i < 10; i++)
                Bullet.SpawnBullet(new Vector2(ship.pos.X, ship.pos.Y + 150), ship.sunRotation + 36 * i, 50, 10, ship.damage, false, false);
            ship.sunTimeTillNextShoot = (float)Raylib.GetTime() + ship.sunFireRate;
        }
        ship.sunRotation++;

        // Check if collision with bullet
        ship.health -= Program.CheckBulletCollision(ship.pos, ship.width, false);
    }
    public static void DrawBoss()
    {
        Program.DrawObjectRotation(Program.allTextures["BossSun"], ship.pos - Player.ship.pos, 0, 1, 255);
        Program.DrawObjectRotation(Program.allTextures["Sun"], new Vector2(ship.pos.X, ship.pos.Y + 150) - Player.ship.pos, ship.sunRotation, 1, 255);
    }
}