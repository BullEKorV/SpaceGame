using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

class Program
{
    public static Dictionary<String, Texture2D> allTextures = LoadTextures(); // Game Textures
    static void Main(string[] args)
    {
        Raylib.InitWindow(1900, 1000, "SpaceGame");
        Raylib.SetTargetFPS(120);
        Raylib.ToggleFullscreen();

        new Player(new Vector2(0, 0), 90, 500);

        RoundManager.GetCurrentRound(0);
        new TextBox(999, new Vector2(Raylib.GetScreenWidth() / 3 + 120, 25), 40, "Welcome to SpaceGame!", Color.WHITE);
        new TextBox(999, new Vector2(Raylib.GetScreenWidth() / 5, 80), 25, "You move around with WASD\nYou shoot lasers with left mouse button and explosive bullets with right mouse button\nYou'll have to dodge the enemies which comes in rounds with increasing difficulty.\nKill the dummy enemy when you're ready!", Color.WHITE);

        while (!Raylib.WindowShouldClose())
        {
            // New round after pause
            if (Raylib.GetTime() > RoundManager.timeTillNextRound && !RoundManager.roundActive)
                RoundManager.NewRound();

            // Spawn enemies during round
            if (RoundManager.roundActive)
                RoundManager.SpawnEnemies();

            // Event manager code
            EventManager.ManagerCode();

            // Draw logic
            Star.StarLogic();

            // Update bullets position
            Bullet.Move();

            // Control player and spawn bullets
            if (Player.ship.isDead == false)
            {
                Player.ship.PlayerControl();

                // Enemy AI
                Enemy.EnemyLogic();
            }
            else
            {
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_R))
                    ResetGame();
            }

            // Render Frame
            Raylib.BeginDrawing();
            Raylib.ClearBackground(new Color(2, 2, 2, 255));

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

            if (distanceBetweenCircles < size / 2 + 15)
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
    static public Vector2 CalculatePosition(Vector2 pos, Vector2 velocity)
    {
        Vector2 newPos = Vector2.Add(pos, velocity);

        return newPos;
    }
    static void RenderWorld(Dictionary<String, Texture2D> Textures) // RenderWorld
    {
        // https://www.raylib.com/examples/web/textures/loader.html?name=textures_srcrec_dstrec

        // Raylib.DrawRectangle((int)(-Player.ship.pos.X - 50f), (int)(Player.ship.pos.Y - 50f), 100, 100, Color.GREEN);

        // Draw stars
        Star.DrawStars();

        // Draw bosses
        if (RoundManager.bossAlive)
        {
            if (BossSun.boss != null)
            {
                BossSun.DrawBoss();
                DrawHealthBar(new Vector2(BossSun.boss.pos.X - Player.ship.pos.X, -BossSun.boss.pos.Y + Player.ship.pos.Y), BossSun.boss.width, BossSun.boss.height, BossSun.boss.health, BossSun.boss.maxHealth);
            }
        }

        // Draw all the enemies
        foreach (Enemy enemy in Enemy.allEnemies)
        {
            if (enemy.type == EnemyType.Easy)
                DrawObjectRotation(Textures["EnemyEasy"], enemy.pos - Player.ship.pos, enemy.rotation, 1, 255);
            else if (enemy.type == EnemyType.Hard)
                DrawObjectRotation(Textures["EnemyHard"], enemy.pos - Player.ship.pos, enemy.rotation, 1, 255);
            else if (enemy.type == EnemyType.Kamikaze)
                DrawObjectRotation(Textures["EnemyKamikaze"], enemy.pos - Player.ship.pos, enemy.rotation, 1, 255);
            else if (enemy.type == EnemyType.Dummy)
                DrawObjectRotation(Textures["EnemyDummy"], enemy.pos - Player.ship.pos, enemy.rotation, 1, 255);

        }
        // Draw all the enemy healthbars
        foreach (Enemy enemy in Enemy.allEnemies)
        {
            DrawHealthBar(new Vector2(enemy.pos.X - Player.ship.pos.X, -enemy.pos.Y + Player.ship.pos.Y), enemy.width, enemy.height, enemy.health, enemy.maxHealth);
        }

        // Draw bullets
        foreach (var bullet in Bullet.allBullets)
        {
            if (bullet.isExplosive)
                DrawObjectRotation(Textures["Bomb"], bullet.pos - Player.ship.pos, bullet.rotation, 1, 255);

            else
                DrawObjectRotation(Textures["Laser"], bullet.pos - Player.ship.pos, bullet.rotation, 1, 255);
        }

        // Draw player
        DrawObjectRotation(Textures["PlayerShip"], new Vector2(0, 0), Player.ship.rotation, 1, 255);

        // Draw player health bar
        DrawHealthBar(new Vector2(0, 0), Player.ship.width, Player.ship.height, Player.ship.health, Player.ship.maxHealth);

        // Display round
        // Raylib.DrawText("Round: " + RoundManager.currentRound.round.ToString(), Raylib.GetScreenWidth() / 2, 0, 15, Color.WHITE);

        // Display FPS
        Raylib.DrawText(Raylib.GetFPS().ToString(), Raylib.GetScreenWidth() - 50, 10, 30, Color.WHITE);

        // Display player stats
        // Raylib.DrawText(Player.ship.health.ToString() + ":" + Player.ship.maxHealth.ToString(), Raylib.GetScreenWidth() - 120, 50, 30, Color.WHITE);

        // Display player score
        Raylib.DrawText("Score : " + Player.ship.score.ToString(), 20, 10, 40, Color.YELLOW);

        // // Display enemy amounts
        // Raylib.DrawText(Enemy.allEnemies.Count.ToString(), Raylib.GetScreenWidth() - 100, 10, 30, Color.WHITE);

        // // Display bullet amounts
        // Raylib.DrawText(Bullet.allBullets.Count.ToString(), Raylib.GetScreenWidth() - 100, 35, 30, Color.WHITE);

        // Draw effects
        foreach (Effect effect in EventManager.allEffects)
        {
            DrawObjectRotation(effect.texture, effect.pos - Player.ship.pos, effect.rotation, effect.size, effect.transparency);
        }
        foreach (TextBox text in EventManager.allTexts)
        {
            Raylib.DrawText(text.text, (int)text.pos.X, (int)text.pos.Y, text.fontSize, text.color);
        }
    }
    static void DrawHealthBar(Vector2 pos, int width, int height, int health, int maxHealth)
    {
        float percentOfHealthLeft = ((float)health / (float)maxHealth);

        int borderOffset = 2;

        Raylib.DrawRectangle((int)pos.X - width / 2 + Raylib.GetScreenWidth() / 2, (int)pos.Y - height + Raylib.GetScreenHeight() / 2, width, 20, Color.WHITE);

        Raylib.DrawRectangle((int)pos.X - width / 2 + Raylib.GetScreenWidth() / 2 + borderOffset, (int)pos.Y - height + Raylib.GetScreenHeight() / 2 + borderOffset, (int)((width - (borderOffset * 2)) * percentOfHealthLeft), 20 - borderOffset * 2, Color.RED);
    }
    public static void DrawObjectRotation(Texture2D texture, Vector2 pos, float rotation, float size, float transparency)
    {
        float width = texture.width;
        float height = texture.height;

        Rectangle sourceRec = new Rectangle(0.0f, 0.0f, (float)width, (float)height);

        Rectangle destRec = new Rectangle(pos.X + Raylib.GetScreenWidth() / 2.0f, -pos.Y + Raylib.GetScreenHeight() / 2.0f, width * size, height * size);

        Vector2 origin = new Vector2((float)width * size * 0.5f, (float)height * size * 0.5f);

        Color rl = new Color(255, 255, 255, (int)transparency);

        Raylib.DrawTexturePro(texture, sourceRec, destRec, origin, rotation, rl);
    }
    private static void ResetGame()
    {
        new Player(new Vector2(0, 0), 90, 500);
        EventManager.allTexts.Clear();
        Enemy.allEnemies.Clear();
        RoundManager.GetCurrentRound(0);
        RoundManager.roundActive = true;
        RoundManager.bossAlive = false;
        Star.allStarsChunks.Clear();
        Star.SpawnStars();
        new TextBox(999, new Vector2(Raylib.GetScreenWidth() / 3 + 120, 25), 40, "Welcome to SpaceGame!", Color.WHITE);
        new TextBox(999, new Vector2(Raylib.GetScreenWidth() / 5, 80), 25, "You move around with WASD\nYou shoot lasers with left mouse button and explosive bullets with right mouse button\nYou'll have to dodge the enemies which comes in rounds with increasing difficulty.\nKill the dummy enemy when you're ready!", Color.WHITE);
    }
    static Dictionary<String, Texture2D> LoadTextures() // Load Textures
    {
        Dictionary<String, Texture2D> Textures = new Dictionary<string, Texture2D>();
        Textures.Add("PlayerShip", Raylib.LoadTexture("Textures/PlayerShip.png")); // Player ship
        Textures.Add("EnemyEasy", Raylib.LoadTexture("Textures/EnemyEasy.png")); // Enemy ship easy
        Textures.Add("EnemyHard", Raylib.LoadTexture("Textures/EnemyHard.png")); // Enemy ship hard
        Textures.Add("EnemyKamikaze", Raylib.LoadTexture("Textures/EnemyKamikaze.png")); // Enemy kamikaze
        Textures.Add("EnemyDummy", Raylib.LoadTexture("Textures/EnemyDummy.png")); // Enemy dummy

        Textures.Add("BossSun", Raylib.LoadTexture("Textures/BossSun.png")); // Boss
        Textures.Add("Sun", Raylib.LoadTexture("Textures/Sun.png")); // Boss sun

        Textures.Add("Laser", Raylib.LoadTexture("Textures/Laser.png")); // Bullet
        Textures.Add("Bomb", Raylib.LoadTexture("Textures/BulletExplosive.png")); // Bomb

        Textures.Add("Star", Raylib.LoadTexture("Textures/Star.png")); // Star

        // Effects
        Textures.Add("BulletHit", Raylib.LoadTexture("Textures/BulletHit.png")); // Bullet hit

        return Textures;
    }
}