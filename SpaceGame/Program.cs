using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

class Program
{
    static void Main(string[] args)
    {
        SpaceShip playerShip = new SpaceShip(0, 0, 90, 100, ShipType.Mouse);

        List<Bullet> bullets = new List<Bullet>();

        Raylib.InitWindow(1900, 1000, "SpaceGame");
        Raylib.SetTargetFPS(120);

        Dictionary<String, Texture2D> Textures = LoadTextures(); // Game Textures
        
        // Give width and height to playership
        playerShip.width = Textures["PlayerShip"].width;
        playerShip.height = Textures["PlayerShip"].height;

        bool gameActive = true;
        while (gameActive)
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);

            // Control player and spawn bullets
            var playerControl = PlayerControl(playerShip, bullets);
            playerShip = playerControl.Item1;
            bullets = playerControl.Item2;

            BulletScript.MoveBullets(bullets, playerShip); // Update bullets position

            RenderWorld(Textures, playerShip, bullets);
            CheckCollision(bullets, playerShip);

            // Console.WriteLine(playerShip.health + "" + playerShip.maxHealth);

            // Console.WriteLine((int)playerShip.x + " " + (int)playerShip.y + " " + playerShip.rotation);

            Raylib.EndDrawing();
        }
    }
    static void CheckCollision(List<Bullet> bullets, SpaceShip playerShip)
    {
        double radians = (Math.PI / 180) * playerShip.rotation;

        float p1X = (float)((playerShip.x - (playerShip.x + playerShip.width / 2)) * Math.Cos(radians) - (playerShip.y - (playerShip.y + playerShip.height / 2)) * Math.Sin(radians));
        float p1Y = (float)((playerShip.x - (playerShip.x + playerShip.width / 2)) * Math.Sin(radians) + (playerShip.y - (playerShip.y + playerShip.height / 2)) * Math.Cos(radians));

        float p2X = (float)((playerShip.x + playerShip.width - (playerShip.x + playerShip.width / 2)) * Math.Cos(radians) - (playerShip.y - (playerShip.y + playerShip.height / 2)) * Math.Sin(radians));
        float p2Y = (float)((playerShip.x + playerShip.width - (playerShip.x + playerShip.width / 2)) * Math.Sin(radians) + (playerShip.y - (playerShip.y + playerShip.height / 2)) * Math.Cos(radians));

        float p3X = (float)((playerShip.x - (playerShip.x + playerShip.width / 2)) * Math.Cos(radians) - (playerShip.y + playerShip.height - (playerShip.y + playerShip.height / 2)) * Math.Sin(radians));
        float p3Y = (float)((playerShip.x - (playerShip.x + playerShip.width / 2)) * Math.Sin(radians) + (playerShip.y + playerShip.height - (playerShip.y + playerShip.height / 2)) * Math.Cos(radians));

        float p4X = (float)((playerShip.x + playerShip.width - (playerShip.x + playerShip.width / 2)) * Math.Cos(radians) - (playerShip.y + playerShip.height - (playerShip.y + playerShip.height / 2)) * Math.Sin(radians));
        float p4Y = (float)((playerShip.x + playerShip.width - (playerShip.x + playerShip.width / 2)) * Math.Sin(radians) + (playerShip.y + playerShip.height - (playerShip.y + playerShip.height / 2)) * Math.Cos(radians));

        Raylib.DrawRectangle((int)p1X + Raylib.GetScreenWidth() / 2, (int)p1Y + Raylib.GetScreenHeight() / 2, 10, 10, Color.RED);
        Raylib.DrawRectangle((int)p2X + Raylib.GetScreenWidth() / 2, (int)p2Y + Raylib.GetScreenHeight() / 2, 10, 10, Color.DARKGRAY);
        Raylib.DrawRectangle((int)p3X + Raylib.GetScreenWidth() / 2, (int)p3Y + Raylib.GetScreenHeight() / 2, 10, 10, Color.ORANGE);
        Raylib.DrawRectangle((int)p4X + Raylib.GetScreenWidth() / 2, (int)p4Y + Raylib.GetScreenHeight() / 2, 10, 10, Color.BLUE);

        foreach (var bullet in bullets)
        {
            float distanceBetweenCirclesSquared =
            (bullet.x - playerShip.x) * (bullet.x - playerShip.x) +
            (bullet.y - playerShip.y) * (bullet.y - playerShip.y);

            // Raylib.DrawCircle((int)(bullet.x - playerShip.x + Raylib.GetScreenWidth() / 2), (int)(-bullet.y + playerShip.y + Raylib.GetScreenHeight() / 2), 10, Color.BROWN);
            // Raylib.DrawCircle((int)Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2, playerShip.width / 2, Color.RED);


            if (distanceBetweenCirclesSquared < (playerShip.width / 2 + 10) * (playerShip.width / 2 + 10))
                playerShip.health--;
        }
    }
    static (SpaceShip, List<Bullet>) PlayerControl(SpaceShip playerShip, List<Bullet> bullets)
    {
        // Arrow control
        if (playerShip.type == ShipType.Arrow)
        {
            if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT))
                playerShip.rotationVelocity += 0.2f;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT))
                playerShip.rotationVelocity -= 0.2f;
        }
        // Mouse control
        else if (playerShip.type == ShipType.Mouse)
        {
            // Make ship look at player
            playerShip.rotation = LookAt(playerShip.x + Raylib.GetScreenWidth() / 2, playerShip.y - Raylib.GetScreenHeight() / 2, playerShip.x + Raylib.GetMouseX(), playerShip.y - Raylib.GetMouseY());
        }

        // Spawn bullet
        if ((Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON) && playerShip.type == ShipType.Mouse) || (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE) && playerShip.type == ShipType.Arrow))
        {
            BulletScript.SpawnBullet(bullets, playerShip.x, playerShip.y, playerShip.rotation, playerShip.height / 2);
        }

        // Calculate velocity
        if ((Raylib.IsKeyDown(KeyboardKey.KEY_SPACE) && playerShip.type == ShipType.Mouse) || (Raylib.IsKeyDown(KeyboardKey.KEY_UP) && playerShip.type == ShipType.Arrow))
            playerShip.velocity += 0.05f;
        else playerShip.velocity *= 0.95f;
        if (playerShip.velocity > 5) // Constraint max velocity
            playerShip.velocity = 5;

        // Calculate new rotation
        playerShip.rotation = CalculateRotation(playerShip.rotation, playerShip.rotationVelocity);
        playerShip.rotationVelocity *= 0.95f; // Slow down rotation

        // Calculate new position
        var newPos = CalculatePosition(playerShip.x, playerShip.y, playerShip.velocity, playerShip.rotation);
        playerShip.x = newPos.x;
        playerShip.y = newPos.y;

        return (playerShip, bullets);
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
    static float CalculateRotation(float rotation, float rotationVelocity)
    {
        rotation += rotationVelocity;

        // Keep rotation in range
        if (rotation > 360)
            rotation -= 360;
        if (rotation < 0)
            rotation += 360;

        return rotation;
    }
    static public (float x, float y) CalculatePosition(float x, float y, float velocity, float rotation)
    {
        double radians = (Math.PI / 180) * rotation;

        x = (float)(x + velocity * Math.Sin(radians));

        y = (float)(y + velocity * Math.Cos(radians));

        return (x, y);
    }
    static void RenderWorld(Dictionary<String, Texture2D> Textures, SpaceShip playerShip, List<Bullet> bullets) // RenderWorld
    {
        // https://www.raylib.com/examples/web/textures/loader.html?name=textures_srcrec_dstrec

        Raylib.DrawRectangle((int)(-playerShip.x - 50f), (int)(playerShip.y - 50f), 100, 100, Color.GREEN);

        // Draw player
        DrawObjectRotation(Textures["PlayerShip"], 0, 0, playerShip.rotation);

        // Draw bullets
        foreach (var bullet in bullets)
        {
            DrawObjectRotation(Textures["Laser"], (int)bullet.x - (int)playerShip.x, -(int)bullet.y + (int)playerShip.y, bullet.rotation);
        }

        // Draw player health bar
        DrawHealthBar(0, 0, playerShip.width, playerShip.height, playerShip.health, playerShip.maxHealth);
    }
    static void DrawHealthBar(float x, float y, int width, int height, int health, int maxHealth)
    {
        float percentOfHealthLeft = ((float)health / (float)maxHealth);

        int borderOffset = 3;

        Raylib.DrawRectangle((int)x - width / 2 + Raylib.GetScreenWidth() / 2, (int)y - height / 2 + Raylib.GetScreenHeight() / 2, width, 30, Color.WHITE);

        Raylib.DrawRectangle((int)x - width / 2 + Raylib.GetScreenWidth() / 2 + borderOffset, (int)y - height / 2 + Raylib.GetScreenHeight() / 2 + borderOffset, (int)(width * percentOfHealthLeft - (borderOffset * 2)), 30 - borderOffset * 2, Color.RED);
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
        Textures.Add("Laser", Raylib.LoadTexture("Textures/Laser.png")); // Player ship

        return Textures;
    }
}
class SpaceShip
{
    public float x;
    public float y;
    public int width;
    public int height;
    public float velocity;
    public float rotationVelocity;
    public float rotation;
    public int health;
    public int maxHealth;
    public ShipType type;
    public SpaceShip(float x, float y, float rotation, int maxHealth, ShipType type)
    {
        this.x = x;
        this.y = y;
        this.rotation = rotation;
        this.maxHealth = maxHealth;
        this.health = maxHealth;
        this.type = type;
    }
}

enum ShipType
{
    Arrow,
    Mouse,
}