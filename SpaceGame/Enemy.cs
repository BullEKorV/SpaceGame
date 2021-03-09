using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

class Enemy
{
    public static List<SpaceShip> SpawnEnemy(List<SpaceShip> enemies, SpaceShip playerShip, Texture2D enemyTexture)
    {
        float enemyX = -200;
        float enemyY = -200;
        float enemyRotation = Program.LookAt(enemyX, enemyY, playerShip.x, playerShip.y);
        int enemyHealth = 100;
        ShipType enemyType = ShipType.Enemy;


        enemies.Add(new SpaceShip(enemyX, enemyY, enemyRotation, enemyHealth, enemyType));

        enemies[enemies.Count - 1].width = enemyTexture.width;
        enemies[enemies.Count - 1].height = enemyTexture.height;

        return enemies;
    }
}