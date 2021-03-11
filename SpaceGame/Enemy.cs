using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

class EnemyScript
{
    public static List<SpaceShip> EnemyCode(List<SpaceShip> enemies, SpaceShip playerShip, Dictionary<String, Texture2D> Textures)
    {
        // Spawn enemy
        if (enemies.Count < 2)
            SpawnEnemy(enemies, playerShip, Textures["PlayerShip"]);

        // Enemy AI
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i] = EnemyAI(enemies[i], playerShip);
        }

        return enemies;
    }
    static SpaceShip EnemyAI(SpaceShip enemy, SpaceShip playerShip)
    {
        enemy.rotation = Program.LookAt(enemy.x, enemy.y, playerShip.x, playerShip.y);

        // Console.WriteLine(Math.Abs(enemy.y - playerShip.y) + Math.Abs(enemy.x - playerShip.x));

        float distanceToPlayer = Math.Abs(enemy.y - playerShip.y) + Math.Abs(enemy.x - playerShip.x);

        // Move closer to the player
        if (distanceToPlayer > 550)
        {
            enemy.velocity += 0.05f;
            if (enemy.velocity > 3)
                enemy.velocity = 3;
        }
        else if (distanceToPlayer < 350)
        {
            enemy.velocity -= 0.05f;
            if (enemy.velocity < -3)
                enemy.velocity = -3;
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

        var newPos = Program.CalculatePosition(enemy.x, enemy.y, enemy.velocity, enemy.rotation);
        enemy.x = newPos.x;
        enemy.y = newPos.y;


        return enemy;
    }
    public static List<SpaceShip> SpawnEnemy(List<SpaceShip> enemies, SpaceShip playerShip, Texture2D enemyTexture)
    {
        float enemyX = -200;
        float enemyY = -200;
        float enemyRotation = 0;
        int enemyHealth = 100;
        ShipType enemyType = ShipType.Enemy;

        enemies.Add(new SpaceShip(enemyX, enemyY, enemyRotation, enemyHealth, enemyType));

        enemies[enemies.Count - 1].width = enemyTexture.width;
        enemies[enemies.Count - 1].height = enemyTexture.height;

        return enemies;
    }
}