using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

class Enemy
{
    public static List<Enemy> allEnemies = new List<Enemy>();

    // What type of enemy
    public EnemyType type;

    //Position variables
    public Vector2 pos;

    // Size variables
    public int width, height;

    // Velocity variables
    private float velocity, speed;

    // Rotation
    public float rotation;

    // Stats
    public int health, maxHealth, damage, shootingReach;

    // Timer variables
    private float timeTillNextShoot, fireRate;
    public Enemy(EnemyType type)
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

        int maxHealth = 0;
        float speed = 0;
        int damage = 0;
        float fireRate = 0;
        int shootingReach = 0;

        // Give special enemies special stats 
        if (type == EnemyType.Easy) // Find better system
        {
            maxHealth = 100;
            speed = 0.21f;
            damage = 10;
            fireRate = 0.2f;
            shootingReach = 400;
        }
        else if (type == EnemyType.Hard)
        {
            maxHealth = 200;
            speed = 0.14f;
            damage = 6;
            fireRate = 0.4f;
            shootingReach = 550;
        }
        else if (type == EnemyType.Dummy)
        {
            maxHealth = 200;
            speed = 0.0f;
            damage = 0;
            fireRate = 0.4f;
            shootingReach = 0;
            pos = new Vector2(200, 0);
        }

        this.pos = pos;
        this.maxHealth = maxHealth;
        this.health = maxHealth;
        this.speed = speed;
        this.damage = damage;
        this.fireRate = fireRate;
        this.type = type;
        this.shootingReach = shootingReach;
        this.width = Program.allTextures["EnemyEasy"].width;
        this.height = Program.allTextures["EnemyEasy"].height;

        allEnemies.Add(this);
    }
    public static void EnemyLogic()
    {
        for (int i = 0; i < allEnemies.Count; i++)
        {
            EnemyAI(allEnemies[i]);
        }
        if (RoundManager.bossAlive == true)
        {
            if (BossSun.boss != null)
                BossSun.AI();
        }
    }
    static void EnemyAI(Enemy enemy)
    {
        enemy.rotation = Program.LookAt(enemy.pos, Player.ship.pos);

        float distanceToPlayer = Vector2.Distance(enemy.pos, Player.ship.pos);

        // Move closer to the player
        if (distanceToPlayer > enemy.shootingReach)
        {
            enemy.velocity += enemy.speed;
            // if (enemy.velocity > 3)
            //     enemy.velocity = 3;
        }
        else if (distanceToPlayer < enemy.shootingReach / 2)
        {
            enemy.velocity -= enemy.speed * 2;
            // if (enemy.velocity < -6)
            //     enemy.velocity = -6;
        }

        // Calculate new position
        enemy.pos = Program.CalculatePositionVelocity(enemy.pos, enemy.velocity, enemy.rotation);

        enemy.velocity *= 0.96f;

        // Shooting logic
        if (distanceToPlayer < enemy.shootingReach)
        {
            if (Raylib.GetTime() > enemy.timeTillNextShoot)
            {
                if (enemy.type == EnemyType.Easy)
                    new Bullet(enemy.pos, enemy.rotation, enemy.height / 2 + 10, 20, enemy.damage, false, false, false);
                else if (enemy.type == EnemyType.Hard)
                {
                    // Shoot 2 bullets
                    Vector2 leftCords = Program.CalculatePositionVelocity(enemy.pos, 40, enemy.rotation - 90);
                    Vector2 rightCords = Program.CalculatePositionVelocity(enemy.pos, 40, enemy.rotation + 90);

                    new Bullet(leftCords, enemy.rotation, enemy.height / 2 + 15, 25, enemy.damage, false, false, false);
                    new Bullet(rightCords, enemy.rotation, enemy.height / 2 + 15, 25, enemy.damage, false, false, false);
                }
                enemy.timeTillNextShoot = (float)Raylib.GetTime() + enemy.fireRate;
            }
        }

        // Check if collision with bullet
        enemy.health -= Program.CheckBulletCollision(enemy.pos, enemy.width, false);

        // Check if dead
        if (enemy.health <= 0)
        {
            EnemyDead(enemy);
        }
    }
    static void EnemyDead(Enemy enemy)
    {
        allEnemies.Remove(enemy);

        // Check if should update to new round
        RoundManager.RoundCompleted();
    }
}
enum EnemyType
{
    Easy,
    Hard,
    Dummy
}