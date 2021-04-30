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
    private float velocity, rotationSpeed, rotationDirection;

    // Rotation
    public float rotation;

    // Stats
    public int health, maxHealth;

    // Timer variables
    private float timeTillNextShoot;
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

        // Give special enemies special stats 
        if (type == EnemyType.Easy)
        {
            maxHealth = 100;
            this.rotationSpeed = rnd.Next(30, 50);
            this.rotationDirection = rnd.Next(0, 2);
        }
        else if (type == EnemyType.Hard)
            maxHealth = 200;
        else if (type == EnemyType.Kamikaze)
            maxHealth = 50;
        else if (type == EnemyType.Dummy)
        {
            maxHealth = 100;
            pos = new Vector2(200, 0);
        }

        this.pos = pos;
        this.maxHealth = maxHealth;
        this.health = maxHealth;
        this.type = type;
        this.width = Program.allTextures["EnemyEasy"].width;
        this.height = Program.allTextures["EnemyEasy"].height;

        allEnemies.Add(this);
    }
    public static void EnemyLogic()
    {
        for (int i = 0; i < allEnemies.Count; i++)
        {
            if (allEnemies[i].type == EnemyType.Easy)
                EnemyAIEasy(allEnemies[i]);
            else if (allEnemies[i].type == EnemyType.Hard)
                EnemyAIHard(allEnemies[i]);
            else if (allEnemies[i].type == EnemyType.Kamikaze)
                EnemyAIKamikaze(allEnemies[i]);
            else
                EnemyAIDummy(allEnemies[i]);

        }
        if (RoundManager.bossAlive == true)
        {
            if (BossSun.boss != null)
                BossSun.AI();
        }
    }
    static void EnemyAIEasy(Enemy enemy)
    {
        var rnd = new Random();

        enemy.rotation = Program.LookAt(enemy.pos, Player.ship.pos);

        float distanceToPlayer = Vector2.Distance(enemy.pos, Player.ship.pos);

        int shootingReach = 400;
        float fireRate = 0.22f;
        float speed = 9f;
        int damage = 4;

        // Calculate new position
        float rotationAroundPlayer = Program.LookAt(Player.ship.pos, enemy.pos);

        if (enemy.rotationDirection == 0)
            rotationAroundPlayer += enemy.rotationSpeed;
        else
            rotationAroundPlayer -= enemy.rotationSpeed;

        Vector2 newPos = Program.CalculatePositionVelocity(Player.ship.pos, shootingReach, rotationAroundPlayer);

        enemy.pos = Program.CalculatePositionVelocity(enemy.pos, speed, Program.LookAt(enemy.pos, newPos));

        // Shooting logic
        if (distanceToPlayer < shootingReach * 1.2f && Raylib.GetTime() > enemy.timeTillNextShoot)
        {
            new Bullet(enemy.pos, enemy.rotation, enemy.height / 2 + 10, 20, damage, false, false, false);
            enemy.timeTillNextShoot = (float)Raylib.GetTime() + fireRate;
        }

        // Check if collision with bullet
        enemy.health -= Program.CheckBulletCollision(enemy.pos, enemy.width, false);

        // Check if dead
        if (enemy.health <= 0)
        {
            EnemyDead(enemy);
        }
    }

    static void EnemyAIHard(Enemy enemy)
    {
        enemy.rotation = Program.LookAt(enemy.pos, Player.ship.pos);

        float distanceToPlayer = Vector2.Distance(enemy.pos, Player.ship.pos);

        int shootingReach = 650;
        float fireRate = 0.5f;
        float speed = 0.18f;
        int damage = 20;

        // Move closer to the player
        if (distanceToPlayer > shootingReach)
        {
            enemy.velocity += speed;
        }
        else if (distanceToPlayer < shootingReach / 2)
        {
            enemy.velocity -= speed * 2;
        }

        // Calculate new position
        enemy.pos = Program.CalculatePositionVelocity(enemy.pos, enemy.velocity, enemy.rotation);

        enemy.velocity *= 0.96f;

        // Shooting logic
        if (distanceToPlayer < shootingReach && Raylib.GetTime() > enemy.timeTillNextShoot)
        {
            // Shoot 2 bullets
            Vector2 leftCords = Program.CalculatePositionVelocity(enemy.pos, 40, enemy.rotation - 90);
            Vector2 rightCords = Program.CalculatePositionVelocity(enemy.pos, 40, enemy.rotation + 90);

            new Bullet(leftCords, enemy.rotation, enemy.height / 2 + 15, 35, damage, false, false, false);
            new Bullet(rightCords, enemy.rotation, enemy.height / 2 + 15, 35, damage, false, false, false);

            enemy.timeTillNextShoot = (float)Raylib.GetTime() + fireRate;
        }

        // Check if collision with bullet
        enemy.health -= Program.CheckBulletCollision(enemy.pos, enemy.width, false);

        // Check if dead
        if (enemy.health <= 0)
        {
            EnemyDead(enemy);
        }
    }
    static void EnemyAIKamikaze(Enemy enemy)
    {
        enemy.rotation = Program.LookAt(enemy.pos, Player.ship.pos);

        float distanceToPlayer = Vector2.Distance(enemy.pos, Player.ship.pos);

        float speed = 0.35f;
        int damage = 100;
        int shootingReach = 50;

        // Calculate new position
        enemy.velocity += speed;
        enemy.pos = Program.CalculatePositionVelocity(enemy.pos, enemy.velocity, enemy.rotation);
        enemy.velocity *= 0.96f;

        // Shooting logic
        if (distanceToPlayer < shootingReach)
        {
            Player.ship.TakeDamage(damage);

            EnemyDead(enemy);
            // return;
        }

        // Check if collision with bullet
        enemy.health -= Program.CheckBulletCollision(enemy.pos, enemy.width, false);

        // Check if dead
        if (enemy.health <= 0)
        {
            EnemyDead(enemy);
        }
    }
    static void EnemyAIDummy(Enemy enemy)
    {
        enemy.rotation = Program.LookAt(enemy.pos, Player.ship.pos);

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
    Kamikaze,
    Dummy
}