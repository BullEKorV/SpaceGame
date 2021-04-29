using System;
using Raylib_cs;
using System.Numerics;
class BossDash
{
    public static BossDash boss;

    //Position variables
    public Vector2 pos;

    // Size variables
    public int width, height;

    // Velocity variables
    private float speed;

    // Stats
    public int health, maxHealth, damage;

    // Timer variables
    private float timeTillNextDash, timeTillNextAttack, dashRate;
    private bool isDashing = false;
    public BossDash()
    {
        var rnd = new Random();

        int side = rnd.Next(1, 3); // 1 up, 2 down, 3 left, 4 right

        Vector2 pos = new Vector2(0, 0);

        switch (side)
        {
            case 1:
                pos = new Vector2(Player.ship.pos.X - Raylib.GetScreenWidth() / 2 - 200, Player.ship.pos.Y + rnd.Next(-Raylib.GetScreenHeight() / 2, Raylib.GetScreenHeight() / 2));
                break;
            case 2:
                pos = new Vector2(Player.ship.pos.X + Raylib.GetScreenWidth() / 2 + 200, Player.ship.pos.Y + rnd.Next(-Raylib.GetScreenHeight() / 2, Raylib.GetScreenHeight() / 2));
                break;
            default:
                break;
        }

        int maxHealth = 500;
        float speed = 0.1f;
        int damage = 20;
        float dashRate = 3.5f;

        this.pos = pos;
        this.maxHealth = maxHealth;
        this.health = maxHealth;
        this.speed = speed;
        this.damage = damage;
        this.dashRate = dashRate;
        this.width = Program.allTextures["BossSun"].width;
        this.height = Program.allTextures["BossSun"].height;

        boss = (this);
    }
    public static void AI()
    {
        float distanceToPlayer = Vector2.Distance(boss.pos, Player.ship.pos);

        // Walk towards player
        Vector2 velocity = Vector2.Subtract(boss.pos, Player.ship.pos) * 0.003f;
        boss.pos -= velocity;


        if (Raylib.GetTime() > boss.timeTillNextDash)
        {

        }


        if (distanceToPlayer < boss.width / 2 + Player.ship.width / 2)
            Player.ship.health -= 2;

        // Check if collision with bullet
        boss.health -= Program.CheckBulletCollision(boss.pos, boss.width, false);

        if (boss.health <= 0)
        {
            EnemyDead(boss);
        }
    }
    static void EnemyDead(BossDash boss)
    {
        boss = null; // BETTER WAY???

        RoundManager.bossAlive = false;

        // Check if should update to new round
        RoundManager.RoundCompleted();
    }
    public static void DrawBoss()
    {
        Program.DrawObjectRotation(Program.allTextures["BossSun"], boss.pos - Player.ship.pos, 0, 1, 255);
    }
}

class BossSun
{
    public static BossSun boss;

    //Position variables
    public Vector2 pos;

    // Size variables
    public int width, height;

    // Velocity variables
    private float speed;

    // Stats
    public int health, maxHealth, damage;

    // Timer variables
    private float timeTillNextMissile, fireRate;

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

        int maxHealth = 1500;
        float speed = 0.004f;
        int damage = 10;
        float fireRate = 1.5f;
        float sunFireRate = 0.08f;

        this.pos = pos;
        this.maxHealth = maxHealth;
        this.health = maxHealth;
        this.speed = speed;
        this.damage = damage;
        this.fireRate = fireRate;
        this.sunFireRate = sunFireRate;
        this.width = Program.allTextures["BossSun"].width;
        this.height = Program.allTextures["BossSun"].height;

        boss = (this);
    }
    public static void AI()
    {
        float distanceToPlayer = Vector2.Distance(boss.pos, Player.ship.pos);

        // Walk towards player
        Vector2 velocity = Vector2.Subtract(boss.pos, Player.ship.pos) * 0.003f;
        boss.pos -= velocity;

        // Sun logic
        if (Raylib.GetTime() > boss.sunTimeTillNextShoot)
        {
            for (int i = 0; i < 8; i++)
                new Bullet(new Vector2(boss.pos.X, boss.pos.Y + 150), boss.sunRotation + 45 * i, 50, 10, boss.damage, false, false, false);
            boss.sunTimeTillNextShoot = (float)Raylib.GetTime() + boss.sunFireRate;
        }
        boss.sunRotation += 0.55f;

        // Homing missiles
        if (Raylib.GetTime() > boss.timeTillNextMissile)
        {
            new Bullet(new Vector2(boss.pos.X, boss.pos.Y), 0, 0, 6, boss.damage * 3, false, true, true);
            boss.timeTillNextMissile = (float)Raylib.GetTime() + boss.fireRate;
        }

        if (distanceToPlayer < boss.width / 2 + Player.ship.width / 2)
            Player.ship.health -= 2;

        // Check if collision with bullet
        boss.health -= Program.CheckBulletCollision(boss.pos, boss.width, false);

        if (boss.health <= 0)
        {
            EnemyDead(boss);
        }
    }

    static void EnemyDead(BossSun boss)
    {
        boss = null; // BETTER WAY???

        RoundManager.bossAlive = false;

        // Check if should update to new round
        RoundManager.RoundCompleted();
    }
    public static void DrawBoss()
    {
        Program.DrawObjectRotation(Program.allTextures["BossSun"], boss.pos - Player.ship.pos, 0, 1, 255);
        Program.DrawObjectRotation(Program.allTextures["Sun"], new Vector2(boss.pos.X, boss.pos.Y + 150) - Player.ship.pos, boss.sunRotation, 1, 255);
    }
}