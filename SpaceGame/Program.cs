using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

class Program
{
    static void Main(string[] args)
    {
        SpaceShip playerShip = new SpaceShip(0, 0, 0, 90, 100, "mouse");

        Raylib.InitWindow(1000, 1000, "SpaceGame");
        Raylib.SetTargetFPS(120);

        Dictionary<String, Texture2D> Textures = LoadTextures(); // Game Textures

        bool gameActive = true;
        while (gameActive)
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.WHITE);

            PlayerControl(playerShip);

            CalculatePosition(playerShip);

            RenderWorld(Textures, playerShip);

            // Console.WriteLine((int)playerShip.x + " " + (int)playerShip.y + " " + playerShip.rotation);

            Raylib.EndDrawing();
        }
    }
    static SpaceShip PlayerControl(SpaceShip playerShip)
    {
        // Arrow control
        if (playerShip.controllerType == "arrow")
        {
            float turnSpeed = 2f;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT))
                playerShip.rotation += turnSpeed;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT))
                playerShip.rotation -= turnSpeed;

            if (playerShip.rotation > 360)
                playerShip.rotation = 0;
            if (playerShip.rotation < 0)
                playerShip.rotation = 360;
        }
        // Mouse and wasd control
        else if (playerShip.controllerType == "mouse")
        {
            float deltaY = (Raylib.GetMouseY() + playerShip.y) - (playerShip.y + Raylib.GetScreenHeight() / 2); // Calculate Delta y

            float deltaX = (Raylib.GetMouseX() + playerShip.x) - (playerShip.x + Raylib.GetScreenWidth() / 2); // Calculate delta x

            float angle = (float)(Math.Atan2(deltaY, deltaX) * 180.0 / Math.PI) + 90; // Find angle
            // Console.WriteLine(deltaX + " " + deltaY);

            if (angle < 0)
                angle = 360 - Math.Abs(angle);

            playerShip.rotation = angle;
        }

        // Calculate velocity
        if (Raylib.IsKeyDown(KeyboardKey.KEY_SPACE) || Raylib.IsKeyDown(KeyboardKey.KEY_UP))
        {
            playerShip.velocity += 0.05f;
        }
        else playerShip.velocity *= 0.96f;

        return playerShip;
    }
    static SpaceShip CalculatePosition(SpaceShip playerShip)
    {
        if (playerShip.velocity > 5)
            playerShip.velocity = 5;

        double radians = (Math.PI / 180) * playerShip.rotation;

        playerShip.x = (float)(playerShip.x + playerShip.velocity * Math.Sin(radians));

        playerShip.y = (float)(playerShip.y + playerShip.velocity * Math.Cos(radians));

        return playerShip;
    }
    static void RenderWorld(Dictionary<String, Texture2D> Textures, SpaceShip playerShip) // RenderWorld
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
    public float rotation;
    public int health;
    public string controllerType;
    public SpaceShip(float x, float y, float velocity, float rotation, int health, string controllerType)
    {
        this.x = x;
        this.y = y;
        this.rotation = rotation;
        this.health = health;
        this.controllerType = controllerType;
    }
}