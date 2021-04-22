using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

class Program
{
    public static Dictionary<String, Texture2D> allTextures = LoadTextures(); // Game Textures
    static void Main(string[] args)
    {
        new Player(new Vector2(0, 0), 90, 100);

        Raylib.InitWindow(1900, 1000, "SpaceGame");
        Raylib.SetTargetFPS(120);

        // Give width and height to Player
        Player.ship.width = allTextures["PlayerShip"].width;
        Player.ship.height = allTextures["PlayerShip"].height;

        RoundManager.GetCurrentRound(1);

        while (!Raylib.WindowShouldClose())
        {
            // Event manager code
            EventManager.ManagerCode();

            // Draw logic
            Star.StarLogic();

            // Control player and spawn bullets
            Player.ship.PlayerControl();

            // Enemy AI
            Enemy.EnemyLogic(allTextures);

            // Update bullets position
            Bullet.Move();

            // Render Frame
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);

            // Render world
            RenderWorld(allTextures);

            Raylib.EndDrawing();
        }
    }
    public static int CheckBulletCollision(Vector2 pos, int size, bool isPlayer)
    {
        for (int i = 0; i < Bullet.allBullets.Count; i++)
        {
            float distanceBetweenCircles = Vector2.Distance(pos, Bullet.allBullets[i].pos);

            // float distanceBetweenCirclesSquared =
            // (Bullet.allBullets[i].x - xPos) * (Bullet.allBullets[i].x - xPos) +
            // (Bullet.allBullets[i].y - yPos) * (Bullet.allBullets[i].y - yPos);

            if (distanceBetweenCircles < size / 2 + 10)
            {
                if (Bullet.allBullets[i].isPlayer != isPlayer)
                {
                    // Bullet splatter
                    new Effect((float)(Raylib.GetTime() + 0.5), Bullet.allBullets[i].pos, 0.5f, 255f, allTextures["BulletHit"]);

                    // For explosive bullets
                    if (Bullet.allBullets[i].isExplosive == true)
                    {
                        for (int y = 0; y < Enemy.allEnemies.Count; y++)
                        {
                            distanceBetweenCircles = Vector2.Distance(Bullet.allBullets[i].pos, Enemy.allEnemies[y].pos);


                            int explosionSize = 250;
                            if (distanceBetweenCircles < size / 2 + explosionSize)
                            {
                                // Calculate explosion range damage dropoff
                                float damageMultiplier = distanceBetweenCircles / (size / 2 + explosionSize);

                                int damageTaken = (int)(Bullet.allBullets[i].damage * (1 - damageMultiplier));

                                // Console.WriteLine(damageTaken + " " + distanceBetweenCircles);

                                if (Bullet.allBullets[i].isPlayer == true)
                                    Player.ship.score += damageTaken;

                                Enemy.allEnemies[y].health -= damageTaken;

                                // Bullet splatter
                                new Effect((float)(Raylib.GetTime() + 0.5), Bullet.allBullets[i].pos, 1.5f, 255f, allTextures["BulletHit"]);
                            }
                        }
                    }
                    // Return damaga value to hit enemy
                    int damageDealt = Bullet.allBullets[i].damage;
                    if (Bullet.allBullets[i].isPlayer == true)
                        Player.ship.score += damageDealt; // Add player score
                    Bullet.allBullets.Remove(Bullet.allBullets[i]);
                    return damageDealt;
                }
                Bullet.allBullets.Remove(Bullet.allBullets[i]);
            }
        }
        return 0;
    }
    static public float LookAt(Vector2 pos1, Vector2 pos2)
    {
        float deltaY = pos1.Y - pos2.Y; // Calculate Delta y

        float deltaX = pos2.X - pos1.X; // Calculate delta x

        float angle = (float)(Math.Atan2(deltaY, deltaX) * 180.0 / Math.PI) + 90; // Find angle

        if (angle < 0)
            angle = 360 - Math.Abs(angle);

        return angle;
    }

    static public Vector2 CalculatePositionVelocity(Vector2 pos, float velocity, float rotation)
    {
        Vector2 newPos = pos;

        if (rotation < 0)
            rotation = 360 - Math.Abs(rotation);

        if (rotation > 360)
            rotation -= 360;

        double radians = (Math.PI / 180) * rotation;

        newPos.X = (float)(newPos.X + velocity * Math.Sin(radians));

        newPos.Y = (float)(newPos.Y + velocity * Math.Cos(radians));

        return newPos;
    }
    static public Vector2 CalculatePosition(Vector2 pos, float xVelocity, float yVelocity)
    {
        Vector2 newPos = new Vector2(pos.X + xVelocity, pos.Y + yVelocity);

        return newPos;
    }
    static void RenderWorld(Dictionary<String, Texture2D> Textures) // RenderWorld
    {
        // https://www.raylib.com/examples/web/textures/loader.html?name=textures_srcrec_dstrec

        Raylib.DrawRectangle((int)(-Player.ship.pos.X - 50f), (int)(Player.ship.pos.Y - 50f), 100, 100, Color.GREEN);

        // Draw stars
        Star.DrawStars();

        // Draw player
        DrawObjectRotation(Textures["PlayerShip"], new Vector2(0, 0), Player.ship.rotation, 1, 255);

        foreach (var enemy in Enemy.allEnemies)
        {
            if (enemy.type == EnemyType.Easy)
                DrawObjectRotation(Textures["EnemyShipEasy"], new Vector2(enemy.pos.X - Player.ship.pos.X, -enemy.pos.Y + Player.ship.pos.Y), enemy.rotation, 1, 255);
            else if (enemy.type == EnemyType.Hard)
                DrawObjectRotation(Textures["EnemyShipHard"], new Vector2(enemy.pos.X - Player.ship.pos.X, -enemy.pos.Y + Player.ship.pos.Y), enemy.rotation, 1, 255);
        }
        // (int)enemy.x - (int)Player.ship.x, -(int)enemy.y + (int)Player.ship.y

        // Draw bullets
        foreach (var bullet in Bullet.allBullets)
        {
            DrawObjectRotation(Textures["Laser"], new Vector2(bullet.pos.X - Player.ship.pos.X, -bullet.pos.Y + Player.ship.pos.Y), bullet.rotation, 1, 255);
        }

        // Draw player health bar
        DrawHealthBar(new Vector2(0, 0), Player.ship.width, Player.ship.height, Player.ship.health, Player.ship.maxHealth);

        // Draw enemies health bar
        foreach (var enemy in Enemy.allEnemies)
        {
            DrawHealthBar(new Vector2(enemy.pos.X - Player.ship.pos.X, -enemy.pos.Y + Player.ship.pos.Y), enemy.width, enemy.height, enemy.health, enemy.maxHealth);
        }

        // Display round
        Raylib.DrawText(RoundManager.currentRound.round.ToString(), Raylib.GetScreenWidth() / 2, 35, 30, Color.WHITE);

        // Display FPS
        Raylib.DrawText(Raylib.GetFPS().ToString(), Raylib.GetScreenWidth() - 50, 10, 30, Color.WHITE);

        // Display player stats
        Raylib.DrawText(Player.ship.health.ToString() + ":" + Player.ship.maxHealth.ToString(), Raylib.GetScreenWidth() / 2, 10, 30, Color.WHITE);

        // Display player score
        Raylib.DrawText("Score : " + Player.ship.score.ToString(), 20, 10, 40, Color.YELLOW);

        // // Display enemy amounts
        // Raylib.DrawText(Enemy.allEnemies.Count.ToString(), Raylib.GetScreenWidth() - 100, 10, 30, Color.WHITE);

        // // Display bullet amounts
        // Raylib.DrawText(Bullet.allBullets.Count.ToString(), Raylib.GetScreenWidth() - 100, 35, 30, Color.WHITE);

        // Draw effects
        foreach (Effect effect in EventManager.allEffects)
        {
            DrawObjectRotation(effect.texture, new Vector2(effect.pos.X - Player.ship.pos.X, -effect.pos.Y + Player.ship.pos.Y), effect.rotation, effect.size, effect.transparency);
        }
        foreach (TextBox text in EventManager.allTexts)
        {
            Raylib.DrawText(text.text, (int)text.pos.X, (int)text.pos.Y, text.fontSize, text.color);
            // Raylib.DrawTextEx(Font, )
        }
    }
    static void DrawHealthBar(Vector2 pos, int width, int height, int health, int maxHealth)
    {
        float percentOfHealthLeft = ((float)health / (float)maxHealth);

        int borderOffset = 3;

        Raylib.DrawRectangle((int)pos.X - width / 2 + Raylib.GetScreenWidth() / 2, (int)pos.Y - height / 2 + Raylib.GetScreenHeight() / 2, width, 30, Color.WHITE);

        Raylib.DrawRectangle((int)pos.X - width / 2 + Raylib.GetScreenWidth() / 2 + borderOffset, (int)pos.Y - height / 2 + Raylib.GetScreenHeight() / 2 + borderOffset, (int)((width - (borderOffset * 2)) * percentOfHealthLeft), 30 - borderOffset * 2, Color.RED);
    }
    static void DrawObjectRotation(Texture2D texture, Vector2 pos, float rotation, float size, float transparency)
    {
        float width = texture.width;
        float height = texture.height;

        Rectangle sourceRec = new Rectangle(0.0f, 0.0f, (float)width, (float)height);

        Rectangle destRec = new Rectangle(pos.X + Raylib.GetScreenWidth() / 2.0f, pos.Y + Raylib.GetScreenHeight() / 2.0f, width * size, height * size);

        Vector2 origin = new Vector2((float)width * size * 0.5f, (float)height * size * 0.5f);

        Color rl = new Color(255, 255, 255, (int)transparency);

        Raylib.DrawTexturePro(texture, sourceRec, destRec, origin, rotation, rl);

    }
    static Dictionary<String, Texture2D> LoadTextures() // Load Textures
    {
        Dictionary<String, Texture2D> Textures = new Dictionary<string, Texture2D>();
        Textures.Add("PlayerShip", Raylib.LoadTexture("Textures/PlayerShip.png")); // Player ship
        Textures.Add("EnemyShipEasy", Raylib.LoadTexture("Textures/EnemyShipEasy.png")); // Enemy ship easy
        Textures.Add("EnemyShipHard", Raylib.LoadTexture("Textures/EnemyShipHard.png")); // Enemy ship hard
        Textures.Add("Laser", Raylib.LoadTexture("Textures/Laser.png")); // Bullet

        // Effects
        Textures.Add("BulletHit", Raylib.LoadTexture("Textures/BulletHit.png")); // Bullet hit

        return Textures;
    }
}