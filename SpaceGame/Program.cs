using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

class Program
{
    static void Main(string[] args)
    {
        SpaceShip playerShip = new SpaceShip(0, 0, 90, 100, "arrow");

        List<Bullet> bullets = new List<Bullet>();

        Raylib.InitWindow(1000, 1000, "SpaceGame");
        Raylib.SetTargetFPS(120);

        Dictionary<String, Texture2D> Textures = LoadTextures(); // Game Textures

        bool gameActive = true;
        while (gameActive)
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.WHITE);

            var playerControl = PlayerControl(playerShip, bullets);
            playerShip = playerControl.Item1;
            bullets = playerControl.Item2;

            RenderWorld(Textures, playerShip, bullets);
            Console.WriteLine(bullets.Count);

            // Console.WriteLine((int)playerShip.x + " " + (int)playerShip.y + " " + playerShip.rotation);

            Raylib.EndDrawing();
        }
    }
    static List<Bullet> SpawnBullet(List<Bullet> bullets, float x, float y, float rotation, int shipHeight)
    {

        return bullets;
    }
    static (SpaceShip, List<Bullet>) PlayerControl(SpaceShip playerShip, List<Bullet> bullets)
    {
        // Arrow control
        if (playerShip.controllerType == "arrow")
        {
            if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT))
                playerShip.rotationVelocity += 0.2f;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT))
                playerShip.rotationVelocity -= 0.2f;
        }
        // Mouse control
        else if (playerShip.controllerType == "mouse")
        {
            float deltaY = (Raylib.GetMouseY() + playerShip.y) - (playerShip.y + Raylib.GetScreenHeight() / 2); // Calculate Delta y

            float deltaX = (Raylib.GetMouseX() + playerShip.x) - (playerShip.x + Raylib.GetScreenWidth() / 2); // Calculate delta x

            float angle = (float)(Math.Atan2(deltaY, deltaX) * 180.0 / Math.PI) + 90; // Find angle

            if (angle < 0)
                angle = 360 - Math.Abs(angle);

            playerShip.rotationVelocity = angle - playerShip.rotation;
        }

        // Spawn bullet
        if ((Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON) && playerShip.controllerType == "mouse") || (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE) && playerShip.controllerType == "arrow"))
        {
            BulletScript.SpawnBullet(bullets, playerShip.x, playerShip.y, playerShip.rotation, 100);
        }

        // Calculate velocity
        if ((Raylib.IsKeyDown(KeyboardKey.KEY_SPACE) && playerShip.controllerType == "mouse") || (Raylib.IsKeyDown(KeyboardKey.KEY_UP) && playerShip.controllerType == "arrow"))
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
        float sizeMultiplier = 0.7f;

        int shipWidth = Textures["PlayerShip"].width;
        int shipheight = Textures["PlayerShip"].height;

        Rectangle sourceRec = new Rectangle(0.0f, 0.0f, (float)shipWidth, (float)shipheight);

        Rectangle destRec = new Rectangle(Raylib.GetScreenWidth() / 2.0f, Raylib.GetScreenHeight() / 2.0f, shipWidth * sizeMultiplier, shipheight * sizeMultiplier);

        Vector2 origin = new Vector2((float)shipWidth * sizeMultiplier * 0.5f, (float)shipheight * sizeMultiplier * 0.5f);

        Raylib.DrawTexturePro(Textures["PlayerShip"], sourceRec, destRec, origin, playerShip.rotation, Color.WHITE);

        // Draw bullets
        foreach (var bullet in bullets)
        {
            Raylib.DrawRectangle((int)bullet.x - (int)playerShip.x, (int)bullet.y + (int)playerShip.y, 10, 10, Color.BLACK);
        }
    }
    static Dictionary<String, Texture2D> LoadTextures() // Load Textures
    {
        Dictionary<String, Texture2D> Textures = new Dictionary<string, Texture2D>();
        Textures.Add("PlayerShip", Raylib.LoadTexture("Textures/player.png")); // Player ship

        return Textures;
    }
}
class SpaceShip
{
    public float x;
    public float y;
    public float velocity;
    public float rotationVelocity;
    public float rotation;
    public int health;
    public string controllerType;
    public SpaceShip(float x, float y, float rotation, int health, string controllerType)
    {
        this.x = x;
        this.y = y;
        this.rotation = rotation;
        // this.velocity = velocity;
        // this.rotationVelocity = rotationVelocity;
        this.health = health;
        this.controllerType = controllerType;
    }
}