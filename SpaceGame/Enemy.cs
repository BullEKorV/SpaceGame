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
    public int health, maxHealth, damage;

    // Timer variables
    private float timeTillNextShoot, fireRate;
    public Enemy(Vector2 pos, int maxHealth, float speed, int damage, float fireRate, EnemyType type)
    {
        this.pos = pos;
        this.maxHealth = maxHealth;
        this.health = maxHealth;
        this.speed = speed;
        this.damage = damage;
        this.fireRate = fireRate;
        this.type = type;

        allEnemies.Add(this);
    }
    public static void EnemyLogic(Dictionary<String, Texture2D> Textures)
    {
        var rnd = new Random();

        // Spawn new enemy
        if (RoundManager.EnemiesLeft() > 0 && Raylib.GetTime() > RoundManager.currentRound.timeTillNextSpawn)
        {
            bool enemySpawned = false;

            while (!enemySpawned)
            {
                int enemyToSpawn = rnd.Next(0, 2);

                if (enemyToSpawn == 0 && RoundManager.currentRound.enemies.easy > 0)
                {
                    enemySpawned = true;
                    SpawnEnemy(Textures["EnemyShipEasy"], EnemyType.Easy);
                    RoundManager.currentRound.enemies.easy--;
                }
                if (enemyToSpawn == 1 && RoundManager.currentRound.enemies.hard > 0)
                {
                    enemySpawned = true;
                    SpawnEnemy(Textures["EnemyShipHard"], EnemyType.Hard);
                    RoundManager.currentRound.enemies.hard--;
                }
            }
            RoundManager.currentRound.timeTillNextSpawn = (float)Raylib.GetTime() + RoundManager.currentRound.spawnRate;
        }

        for (int i = 0; i < allEnemies.Count; i++)
        {
            EnemyAI(allEnemies[i]);
        }
    }
    static void EnemyAI(Enemy enemy)
    {
        enemy.rotation = Program.LookAt(enemy.pos, Player.ship.pos);

        float distanceToPlayer = Vector2.Distance(enemy.pos, Player.ship.pos);

        // Move closer to the player
        if (distanceToPlayer > 550)
        {
            enemy.velocity += enemy.speed;
            // if (enemy.velocity > 3)
            //     enemy.velocity = 3;
        }
        else if (distanceToPlayer < 350)
        {
            enemy.velocity -= enemy.speed * 2;
            // if (enemy.velocity < -6)
            //     enemy.velocity = -6;
        }

        // Calculate new position
        enemy.pos = Program.CalculatePositionVelocity(enemy.pos, enemy.velocity, enemy.rotation);

        enemy.velocity *= 0.97f;

        // Shooting logic
        if (distanceToPlayer < 650)
        {
            if (Raylib.GetTime() > enemy.timeTillNextShoot)
            {
                if (enemy.type == EnemyType.Easy)
                    Bullet.SpawnBullet(enemy.pos, enemy.rotation, enemy.height / 2 + 10, 20, enemy.damage, false, false);
                else if (enemy.type == EnemyType.Hard)
                {
                    // Shoot 2 bullets
                    Vector2 leftCords = Program.CalculatePositionVelocity(enemy.pos, 40, enemy.rotation - 90);
                    Vector2 rightCords = Program.CalculatePositionVelocity(enemy.pos, 40, enemy.rotation + 90);

                    Bullet.SpawnBullet(leftCords, enemy.rotation, enemy.height / 2 + 15, 25, enemy.damage, false, false);
                    Bullet.SpawnBullet(rightCords, enemy.rotation, enemy.height / 2 + 15, 25, enemy.damage, false, false);
                }
                enemy.timeTillNextShoot = (float)Raylib.GetTime() + enemy.fireRate;
            }
        }

        // Check if dead
        if (enemy.health <= 0)
        {
            EnemyDead(enemy);
        }

        // Check if collision with bullet
        enemy.health -= Program.CheckBulletCollision(enemy.pos, enemy.width, false);
    }
    static void EnemyDead(Enemy enemy)
    {
        allEnemies.Remove(enemy);

        // Check if should update to new round
        RoundManager.RoundCompleted();
    }
    static void SpawnEnemy(Texture2D enemyTexture, EnemyType type)
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

        // Give special enemies special stats 
        if (type == EnemyType.Easy) // Find better system
        {
            maxHealth = 100;
            speed = 0.2f;
            damage = 10;
            fireRate = 0.2f;
        }
        if (type == EnemyType.Hard)
        {
            maxHealth = 150;
            speed = 0.1f;
            damage = 6;
            fireRate = 0.5f;
        }

        new Enemy(pos, maxHealth, speed, damage, fireRate, type);

        allEnemies[allEnemies.Count - 1].width = enemyTexture.width;
        allEnemies[allEnemies.Count - 1].height = enemyTexture.height;

    }
}
enum EnemyType
{
    Easy,
    Hard
}