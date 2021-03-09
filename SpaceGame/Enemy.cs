using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

class EnemyScript
{
    public static (List<SpaceShip>, List<Bullet>) EnemyCode(List<SpaceShip> enemies, SpaceShip playerShip, List<Bullet> bullets, Dictionary<String, Texture2D> Textures)
    {
        // Spawn enemy
        if (enemies.Count < 5)
            SpawnEnemy(enemies, playerShip, Textures["PlayerShip"]);

        // Enemy AI
        for (int i = 0; i < enemies.Count; i++)
        {
            var data = EnemyAI(enemies[i], playerShip, bullets);
            enemies[i] = data.Item1;
            bullets = data.Item2;
        }

        return (enemies, bullets);
    }
    static (SpaceShip, List<Bullet>) EnemyAI(SpaceShip enemy, SpaceShip playerShip, List<Bullet> bullets)
    {
        enemy.rotation = Program.LookAt(enemy.x, enemy.y, playerShip.x, playerShip.y);

        enemy.timeSinceShot++;
        if (enemy.timeSinceShot > 30)
        {
            BulletScript.SpawnBullet(bullets, enemy.x, enemy.y, enemy.rotation, enemy.height);
            enemy.timeSinceShot = 0;
        }

        return (enemy, bullets);
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