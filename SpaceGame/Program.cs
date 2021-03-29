using System;
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
    public static bool CheckCollision(float xPos, float yPos, int size, List<Bullet> bullets)
    {
        // double radians = (Math.PI / 180) * testShip.rotation;

        // float p1X = (float)((testShip.x - (testShip.x + testShip.width / 2)) * Math.Cos(radians) - (testShip.y - (testShip.y + testShip.height / 2)) * Math.Sin(radians));
        // float p1Y = (float)((testShip.x - (testShip.x + testShip.width / 2)) * Math.Sin(radians) + (testShip.y - (testShip.y + testShip.height / 2)) * Math.Cos(radians));

        // float p2X = (float)((testShip.x + testShip.width - (testShip.x + testShip.width / 2)) * Math.Cos(radians) - (testShip.y - (testShip.y + testShip.height / 2)) * Math.Sin(radians));
        // float p2Y = (float)((testShip.x + testShip.width - (testShip.x + testShip.width / 2)) * Math.Sin(radians) + (testShip.y - (testShip.y + testShip.height / 2)) * Math.Cos(radians));

        // float p3X = (float)((testShip.x - (testShip.x + testShip.width / 2)) * Math.Cos(radians) - (testShip.y + testShip.height - (testShip.y + testShip.height / 2)) * Math.Sin(radians));
        // float p3Y = (float)((testShip.x - (testShip.x + testShip.width / 2)) * Math.Sin(radians) + (testShip.y + testShip.height - (testShip.y + testShip.height / 2)) * Math.Cos(radians));

        // float p4X = (float)((testShip.x + testShip.width - (testShip.x + testShip.width / 2)) * Math.Cos(radians) - (testShip.y + testShip.height - (testShip.y + testShip.height / 2)) * Math.Sin(radians));
        // float p4Y = (float)((testShip.x + testShip.width - (testShip.x + testShip.width / 2)) * Math.Sin(radians) + (testShip.y + testShip.height - (testShip.y + testShip.height / 2)) * Math.Cos(radians));

        // Raylib.DrawRectangle((int)p1X + Raylib.GetScreenWidth() / 2, (int)p1Y + Raylib.GetScreenHeight() / 2, 10, 10, Color.RED);
        // Raylib.DrawRectangle((int)p2X + Raylib.GetScreenWidth() / 2, (int)p2Y + Raylib.GetScreenHeight() / 2, 10, 10, Color.DARKGRAY);
        // Raylib.DrawRectangle((int)p3X + Raylib.GetScreenWidth() / 2, (int)p3Y + Raylib.GetScreenHeight() / 2, 10, 10, Color.ORANGE);
        // Raylib.DrawRectangle((int)p4X + Raylib.GetScreenWidth() / 2, (int)p4Y + Raylib.GetScreenHeight() / 2, 10, 10, Color.BLUE);

        for (int i = 0; i < bullets.Count; i++)
        {
            float distanceBetweenCirclesSquared =
            (bullets[i].x - xPos) * (bullets[i].x - xPos) +
            (bullets[i].y - yPos) * (bullets[i].y - yPos);

            // Raylib.DrawCircle((int)(bullet.x - testShip.x + Raylib.GetScreenWidth() / 2), (int)(-bullet.y + testShip.y + Raylib.GetScreenHeight() / 2), 10, Color.BROWN);
            // Raylib.DrawCircle((int)Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2, testShip.width / 2, Color.RED);

            if (distanceBetweenCirclesSquared < (size / 2 + 10) * (size / 2 + 10))
            {
                bullets.Remove(bullets[i]);
                return true;
            }
        }
        return false;
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