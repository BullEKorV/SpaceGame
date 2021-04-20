using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

class Program
{
    static void Main(string[] args)
    {
        new Player(0, 0, 90, 100);

        Raylib.InitWindow(1900, 1000, "SpaceGame");
        Raylib.SetTargetFPS(120);

        Dictionary<String, Texture2D> Textures = LoadTextures(); // Game Textures

        // Give width and height to Player
        Player.ship.width = Textures["PlayerShip"].width;
        Player.ship.height = Textures["PlayerShip"].height;

        RoundManager.GetCurrentRound(1);

        while (!Raylib.WindowShouldClose())
        {
            // Round test
            // RoundManager.NewRound();

            // Draw logic
            Star.StarLogic();

            // Control player and spawn bullets
            Player.ship.PlayerControl();

            // Enemy AI
            Enemy.EnemyLogic(Textures);

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
    public static int CheckBulletCollision(float xPos, float yPos, int size, bool isPlayer)
    {
        for (int i = 0; i < Bullet.allBullets.Count; i++)
        {
            float distanceBetweenCirclesSquared =
            (Bullet.allBullets[i].x - xPos) * (Bullet.allBullets[i].x - xPos) +
            (Bullet.allBullets[i].y - yPos) * (Bullet.allBullets[i].y - yPos);

            if (distanceBetweenCirclesSquared < (size / 2 + 10) * (size / 2 + 10))
            {
                if (Bullet.allBullets[i].isPlayer != isPlayer)
                {
                    // For explosive bullets
                    if (Bullet.allBullets[i].isExplosive == true)
                    {
                        for (int y = 0; y < Enemy.allEnemies.Count; y++)
                        {
                            distanceBetweenCirclesSquared =
                            (Bullet.allBullets[i].x - Enemy.allEnemies[y].x) * (Bullet.allBullets[i].x - Enemy.allEnemies[y].x) +
                            (Bullet.allBullets[i].y - Enemy.allEnemies[y].y) * (Bullet.allBullets[i].y - Enemy.allEnemies[y].y);

                            int explosionSize = 150;
                            if (distanceBetweenCirclesSquared < (size / 2 + explosionSize) * (size / 2 + explosionSize))
                            {
                                // Calculate explosion range damage dropoff
                                float damageMultiplier = distanceBetweenCirclesSquared / ((size / 2 + explosionSize) * (size / 2 + explosionSize));

                                Enemy.allEnemies[y].health -= (int)(Bullet.allBullets[i].damage * (1 - damageMultiplier));
                            }
                        }
                    }
                    // Return damaga value to hit enemy
                    int damageDealt = Bullet.allBullets[i].damage;
                    Bullet.allBullets.Remove(Bullet.allBullets[i]);
                    return damageDealt;
                }
                Bullet.allBullets.Remove(Bullet.allBullets[i]);
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
        if (rotation < 0)
            rotation = 360 - Math.Abs(rotation);

        if (rotation > 360)
            rotation -= 360;

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

        Raylib.DrawRectangle((int)(-Player.ship.x - 50f), (int)(Player.ship.y - 50f), 100, 100, Color.GREEN);

        // Draw stars
        Star.DrawStars();

        // Draw player
        DrawObjectRotation(Textures["PlayerShip"], 0, 0, Player.ship.rotation);

        foreach (var enemy in Enemy.allEnemies)
        {
            if (enemy.type == EnemyType.Easy)
                DrawObjectRotation(Textures["EnemyShipEasy"], (int)enemy.x - (int)Player.ship.x, -(int)enemy.y + (int)Player.ship.y, enemy.rotation);
            else if (enemy.type == EnemyType.Hard)
                DrawObjectRotation(Textures["EnemyShipHard"], (int)enemy.x - (int)Player.ship.x, -(int)enemy.y + (int)Player.ship.y, enemy.rotation);
        }

        // Draw bullets
        foreach (var bullet in Bullet.allBullets)
        {
            DrawObjectRotation(Textures["Laser"], (int)bullet.x - (int)Player.ship.x, -(int)bullet.y + (int)Player.ship.y, bullet.rotation);
        }

        // Draw player health bar
        DrawHealthBar(0, 0, Player.ship.width, Player.ship.height, Player.ship.health, Player.ship.maxHealth);

        // Draw enemies health bar
        foreach (var enemy in Enemy.allEnemies)
        {
            DrawHealthBar(enemy.x - (int)Player.ship.x, -enemy.y + (int)Player.ship.y, enemy.width, enemy.height, enemy.health, enemy.maxHealth);
        }

        // Display round
        Raylib.DrawText(RoundManager.currentRound.round.ToString(), Raylib.GetScreenWidth() / 2, 35, 30, Color.WHITE);

        // Display FPS
        Raylib.DrawText(Raylib.GetFPS().ToString(), 10, 10, 30, Color.WHITE);

        // Display player stats
        Raylib.DrawText(Player.ship.health.ToString() + ":" + Player.ship.maxHealth.ToString(), Raylib.GetScreenWidth() / 2, 10, 30, Color.WHITE);

        // Display enemy amounts
        Raylib.DrawText(Enemy.allEnemies.Count.ToString(), Raylib.GetScreenWidth() - 100, 10, 30, Color.WHITE);

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
        Textures.Add("EnemyShipEasy", Raylib.LoadTexture("Textures/EnemyShipEasy.png")); // Enemy ship easy
        Textures.Add("EnemyShipHard", Raylib.LoadTexture("Textures/EnemyShipHard.png")); // Enemy ship hard
        Textures.Add("Laser", Raylib.LoadTexture("Textures/Laser.png")); // Bullet

        return Textures;
    }
}