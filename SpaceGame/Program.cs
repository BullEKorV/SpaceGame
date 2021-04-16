﻿using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

class Program
{
    static void Main(string[] args)
    {
        new PlayerShip(0, 0, 90, 100);

        Raylib.InitWindow(1900, 1000, "SpaceGame");
        Raylib.SetTargetFPS(120);

        Dictionary<String, Texture2D> Textures = LoadTextures(); // Game Textures

        // Give width and height to playership
        PlayerShip.ship.width = Textures["PlayerShip"].width;
        PlayerShip.ship.height = Textures["PlayerShip"].height;


        while (!Raylib.WindowShouldClose())
        {
            // Draw logic
            Star.StarLogic();

            // Control player and spawn bullets
            PlayerShip.ship.PlayerControl();

            // Enemy AI
            EnemyShip.EnemyLogic(Textures);

            // Update bullets position
            Bullet.Move();

            // Render Frame
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);

            // Render world
            RenderWorld(Textures);

            Raylib.EndDrawing();
        }
    }
    public static int CheckBulletCollision(float xPos, float yPos, int size)
    {
        for (int i = 0; i < Bullet.allBullets.Count; i++)
        {
            float distanceBetweenCirclesSquared =
            (Bullet.allBullets[i].x - xPos) * (Bullet.allBullets[i].x - xPos) +
            (Bullet.allBullets[i].y - yPos) * (Bullet.allBullets[i].y - yPos);

            if (distanceBetweenCirclesSquared < (size / 2 + 10) * (size / 2 + 10))
            {
                int damageDealt = Bullet.allBullets[i].damage;
                Bullet.allBullets.Remove(Bullet.allBullets[i]);
                return damageDealt;
            }
        }
        return 0;
    }
    static public float LookAt(float x1, float y1, float x2, float y2)
    {
        float deltaY = y1 - y2; // Calculate Delta y

        float deltaX = x2 - x1; // Calculate delta x

        float angle = (float)(Math.Atan2(deltaY, deltaX) * 180.0 / Math.PI) + 90; // Find angle

        if (angle < 0)
            angle = 360 - Math.Abs(angle);

        return angle;
    }

    static public (float x, float y) CalculatePositionVelocity(float x, float y, float velocity, float rotation)
    {
        double radians = (Math.PI / 180) * rotation;

        x = (float)(x + velocity * Math.Sin(radians));

        y = (float)(y + velocity * Math.Cos(radians));

        return (x, y);
    }
    static public (float x, float y) CalculatePosition(float x, float y, float xVelocity, float yVelocity)
    {
        x += xVelocity;
        y += yVelocity;

        return (x, y);
    }
    static void RenderWorld(Dictionary<String, Texture2D> Textures) // RenderWorld
    {
        // https://www.raylib.com/examples/web/textures/loader.html?name=textures_srcrec_dstrec

        Raylib.DrawRectangle((int)(-PlayerShip.ship.x - 50f), (int)(PlayerShip.ship.y - 50f), 100, 100, Color.GREEN);

        // Draw stars
        Star.DrawStars();

        // Draw player
        DrawObjectRotation(Textures["PlayerShip"], 0, 0, PlayerShip.ship.rotation);

        foreach (var enemy in EnemyShip.allEnemies)
        {
            DrawObjectRotation(Textures["PlayerShip"], (int)enemy.x - (int)PlayerShip.ship.x, -(int)enemy.y + (int)PlayerShip.ship.y, enemy.rotation);
        }

        // Draw bullets
        foreach (var bullet in Bullet.allBullets)
        {
            DrawObjectRotation(Textures["Laser"], (int)bullet.x - (int)PlayerShip.ship.x, -(int)bullet.y + (int)PlayerShip.ship.y, bullet.rotation);
        }

        // Draw player health bar
        DrawHealthBar(0, 0, PlayerShip.ship.width, PlayerShip.ship.height, PlayerShip.ship.health, PlayerShip.ship.maxHealth);

        // Draw enemies health bar
        foreach (var enemy in EnemyShip.allEnemies)
        {
            DrawHealthBar(enemy.x - (int)PlayerShip.ship.x, -enemy.y + (int)PlayerShip.ship.y, enemy.width, enemy.height, enemy.health, enemy.maxHealth);
        }

        // Display FPS
        Raylib.DrawText(Raylib.GetFPS().ToString(), 10, 10, 30, Color.WHITE);

        // Display player stats
        Raylib.DrawText(PlayerShip.ship.health.ToString() + ":" + PlayerShip.ship.maxHealth.ToString(), Raylib.GetScreenWidth() / 2, 10, 30, Color.WHITE);

        // Display enemy amounts
        Raylib.DrawText(EnemyShip.allEnemies.Count.ToString(), Raylib.GetScreenWidth() - 100, 10, 30, Color.WHITE);

        // Display bullet amounts
        Raylib.DrawText(Bullet.allBullets.Count.ToString(), Raylib.GetScreenWidth() - 100, 35, 30, Color.WHITE);

    }
    static void DrawHealthBar(float x, float y, int width, int height, int health, int maxHealth)
    {
        float percentOfHealthLeft = ((float)health / (float)maxHealth);

        int borderOffset = 3;

        Raylib.DrawRectangle((int)x - width / 2 + Raylib.GetScreenWidth() / 2, (int)y - height / 2 + Raylib.GetScreenHeight() / 2, width, 30, Color.WHITE);

        Raylib.DrawRectangle((int)x - width / 2 + Raylib.GetScreenWidth() / 2 + borderOffset, (int)y - height / 2 + Raylib.GetScreenHeight() / 2 + borderOffset, (int)((width - (borderOffset * 2)) * percentOfHealthLeft), 30 - borderOffset * 2, Color.RED);
    }
    static void DrawObjectRotation(Texture2D texture, float x, float y, float rotation)
    {
        int width = texture.width;
        int height = texture.height;

        Rectangle sourceRec = new Rectangle(0.0f, 0.0f, (float)width, (float)height);

        Rectangle destRec = new Rectangle(x + Raylib.GetScreenWidth() / 2.0f, y + Raylib.GetScreenHeight() / 2.0f, width, height);

        Vector2 origin = new Vector2((float)width * 0.5f, (float)height * 0.5f);

        Raylib.DrawTexturePro(texture, sourceRec, destRec, origin, rotation, Color.WHITE);
    }
    static Dictionary<String, Texture2D> LoadTextures() // Load Textures
    {
        Dictionary<String, Texture2D> Textures = new Dictionary<string, Texture2D>();
        Textures.Add("PlayerShip", Raylib.LoadTexture("Textures/PlayerShip.png")); // Player ship
        Textures.Add("Laser", Raylib.LoadTexture("Textures/Laser.png")); // Bullet
        Textures.Add("Star", Raylib.LoadTexture("Textures/Star.png")); // Star

        return Textures;
    }
}