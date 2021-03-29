using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

class EnemyShip
{
    public static List<EnemyShip> allEnemies = new List<EnemyShip>();
    public float x;
    public float y;
    public int width;
    public int height;
    public float velocity;
    public float rotation;
    public int health;
    public int maxHealth;
    public int timeSinceShot;
    private float speed = 0.02f;
    public EnemyShip(float x, float y, float rotation, int maxHealth)
    {
        this.x = x;
        this.y = y;
        this.rotation = rotation;
        this.maxHealth = maxHealth;
        this.health = maxHealth;

        allEnemies.Add(this);
    }
    public static void EnemyLogic(Dictionary<String, Texture2D> Textures)
    {
        if (allEnemies.Count < 1)
            SpawnEnemy(Textures["PlayerShip"]);

        // foreach (EnemyShip enemy in allEnemies)
        // {
        //     EnemyAI(enemy);
        // }
        for (int i = 0; i < allEnemies.Count; i++) // Why no work?
        {
            EnemyAI(allEnemies[i]);
        }
    }
    static void EnemyAI(EnemyShip enemy)
    {
        enemy.rotation = Program.LookAt(enemy.x, enemy.y, PlayerShip.ship.x, PlayerShip.ship.y);

        // Console.WriteLine(Math.Abs(enemy.y - playerShip.y) + Math.Abs(enemy.x - playerShip.x));

        float distanceToPlayer = Math.Abs(enemy.y - PlayerShip.ship.y) + Math.Abs(enemy.x - PlayerShip.ship.x);

        // Move closer to the player
        if (distanceToPlayer > 550)
        {
            enemy.velocity += enemy.speed * 4;
            if (enemy.velocity > 3)
                enemy.velocity = 3;
        }
        else if (distanceToPlayer < 350)
        {
            enemy.velocity -= enemy.speed * 7;
            if (enemy.velocity < -6)
                enemy.velocity = -6;
        }
        else
        {
            enemy.timeSinceShot++;
            if (enemy.timeSinceShot > 50)
            {
                Bullet.SpawnBullet(enemy.x, enemy.y, enemy.rotation, enemy.height / 2);
                enemy.timeSinceShot = 0;
            }
            enemy.velocity *= 0.98f;
        }

        // Check if dead
        if (enemy.health <= 0)
        {
            EnemyDead(enemy);
        }

        var newPos = Program.CalculatePositionVelocity(enemy.x, enemy.y, enemy.velocity, enemy.rotation);
        enemy.x = newPos.x;
        enemy.y = newPos.y;

        if (Program.CheckCollision(enemy.x, enemy.y, enemy.width))
            enemy.health--;
    }
    static void EnemyDead(EnemyShip enemy)
    {
        allEnemies.Remove(enemy);
    }
    static void SpawnEnemy(Texture2D enemyTexture)
    {
        float enemyX = -200;
        float enemyY = -200;
        float enemyRotation = 0;
        int enemyHealth = 100;

        new EnemyShip(enemyX, enemyY, enemyRotation, enemyHealth);

        allEnemies[allEnemies.Count - 1].width = enemyTexture.width;
        allEnemies[allEnemies.Count - 1].height = enemyTexture.height;
    }
}