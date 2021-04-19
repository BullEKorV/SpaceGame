using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

class EnemyShip
{
    public static List<EnemyShip> allEnemies = new List<EnemyShip>();

    // What type of enemy
    public EnemyType type;

    //Position variables
    public float x, y;

    // Size variables
    public int width, height;

    // Velocity variables
    private float velocity, speed;

    // Rotation
    public float rotation;

    // Health variables
    public int health, maxHealth;

    // Shooting variables
    private int timeSinceShot, fireRate, damage;
    public EnemyShip(float x, float y, int maxHealth, float speed, int damage, int fireRate, EnemyType type)
    {
        this.x = x;
        this.y = y;
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
    static void EnemyAI(EnemyShip enemy)
    {
        enemy.rotation = Program.LookAt(enemy.x, enemy.y, PlayerShip.ship.x, PlayerShip.ship.y);

        float distanceToPlayer = Math.Abs(enemy.y - PlayerShip.ship.y) + Math.Abs(enemy.x - PlayerShip.ship.x);

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
        else
        {
            enemy.timeSinceShot++;
            if (enemy.timeSinceShot > enemy.fireRate)
            {
                if (enemy.type == EnemyType.Easy)
                    Bullet.SpawnBullet(enemy.x, enemy.y, enemy.rotation, enemy.height / 2 + 15, 20, enemy.damage);
                else if (enemy.type == EnemyType.Hard)
                {
                    // Shoot 2 bullets
                    var leftCords = Program.CalculatePositionVelocity(enemy.x, enemy.y, 40, enemy.rotation - 90);
                    var rightCords = Program.CalculatePositionVelocity(enemy.x, enemy.y, 40, enemy.rotation + 90);

                    Bullet.SpawnBullet(leftCords.x, leftCords.y, enemy.rotation, enemy.height / 2 + 15, 20, enemy.damage);
                    Bullet.SpawnBullet(rightCords.x, rightCords.y, enemy.rotation, enemy.height / 2 + 15, 20, enemy.damage);
                }
                enemy.timeSinceShot = 0;
            }
        }

        // Check if dead
        if (enemy.health <= 0)
        {
            EnemyDead(enemy);
        }

        // Calculate new position
        var newPos = Program.CalculatePositionVelocity(enemy.x, enemy.y, enemy.velocity, enemy.rotation);
        enemy.x = newPos.x;
        enemy.y = newPos.y;

        enemy.velocity *= 0.97f;

        // Check if collision with bullet
        enemy.health -= Program.CheckBulletCollision(enemy.x, enemy.y, enemy.width);
    }
    static void EnemyDead(EnemyShip enemy)
    {
        allEnemies.Remove(enemy);

        // Check if should update to new round
        RoundManager.NewRound();
    }
    static void SpawnEnemy(Texture2D enemyTexture, EnemyType type)
    {
        var rnd = new Random();

        int side = rnd.Next(1, 5); // 1 up, 2 down, 3 left, 4 right
        float enemyX = 0;
        float enemyY = 0;

        switch (side)
        {
            case 1:
                enemyY = PlayerShip.ship.y + Raylib.GetScreenHeight() / 2 + 200;
                enemyX = PlayerShip.ship.x + rnd.Next(-Raylib.GetScreenWidth() / 2, Raylib.GetScreenWidth() / 2);
                break;
            case 2:
                enemyY = PlayerShip.ship.y - Raylib.GetScreenHeight() / 2 - 200;
                enemyX = PlayerShip.ship.x + rnd.Next(-Raylib.GetScreenWidth() / 2, Raylib.GetScreenWidth() / 2);
                break;
            case 3:
                enemyX = PlayerShip.ship.x - Raylib.GetScreenWidth() / 2 - 200;
                enemyY = PlayerShip.ship.y + rnd.Next(-Raylib.GetScreenHeight() / 2, Raylib.GetScreenHeight() / 2);
                break;
            case 4:
                enemyX = PlayerShip.ship.x + Raylib.GetScreenWidth() / 2 + 200;
                enemyY = PlayerShip.ship.y + rnd.Next(-Raylib.GetScreenHeight() / 2, Raylib.GetScreenHeight() / 2);
                break;
            default:
                break;
        }

        int maxHealth = 0;
        float speed = 0;
        int damage = 0;
        int fireRate = 0;

        // Give special enemies special stats 
        if (type == EnemyType.Easy) // Find better system
        {
            maxHealth = 100;
            speed = 0.2f;
            damage = 5;
            fireRate = 30;
        }
        if (type == EnemyType.Hard)
        {
            maxHealth = 150;
            speed = 0.1f;
            damage = 3;
            fireRate = 20;
        }

        new EnemyShip(enemyX, enemyY, maxHealth, speed, damage, fireRate, type);

        allEnemies[allEnemies.Count - 1].width = enemyTexture.width;
        allEnemies[allEnemies.Count - 1].height = enemyTexture.height;

    }
}
enum EnemyType
{
    Easy,
    Hard
}