using System;
using Raylib_cs;
class SpaceShip
{
    public static SpaceShip playerShip;
    public float x;
    public float y;
    public int width;
    public int height;
    public float xVelocity;
    public float yVelocity;
    public float rotation;
    public int health;
    public int maxHealth;
    public int timeSinceShot;
    private bool left, right, up, down;
    private float speed = 0.22f;
    public SpaceShip(float x, float y, float rotation, int maxHealth)
    {
        this.x = x;
        this.y = y;
        this.rotation = rotation;
        this.maxHealth = maxHealth;
        this.health = maxHealth;

        playerShip = this;
    }
    public void PlayerControl()
    {
        // Make ship look at mouse
        playerShip.rotation = Program.LookAt(playerShip.x + Raylib.GetScreenWidth() / 2, playerShip.y - Raylib.GetScreenHeight() / 2, playerShip.x + Raylib.GetMouseX(), playerShip.y - Raylib.GetMouseY());

        // Spawn bullet
        if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON) && playerShip.timeSinceShot > 20)
        {
            Bullet.SpawnBullet(playerShip.x, playerShip.y, playerShip.rotation, playerShip.height / 2);
            playerShip.timeSinceShot = 0;
        }
        playerShip.timeSinceShot++;

        // Check keypresses
        KeyPresses();

        // Calculate velocity
        CalculateVelocity();

        // Constraint velocities
        // if (playerShip.xVelocity < -5) // Constraint max velocity
        //     playerShip.xVelocity = -5;
        // else if (playerShip.xVelocity > 5) // Constraint max velocity
        //     playerShip.xVelocity = 5;

        // if (playerShip.yVelocity < -5) // Constraint max velocity
        //     playerShip.yVelocity = -5;
        // else if (playerShip.yVelocity > 5) // Constraint max velocity
        //     playerShip.yVelocity = 5;

        // Calculate new position
        var newPos = Program.CalculatePosition(playerShip.x, playerShip.y, playerShip.xVelocity, playerShip.yVelocity);
        playerShip.x = newPos.x;
        playerShip.y = newPos.y;

        playerShip.xVelocity *= 0.96f;
        playerShip.yVelocity *= 0.96f;

        // Check collision
        if (Program.CheckCollision(playerShip.x, playerShip.y, playerShip.width, Bullet.allBullets))
            playerShip.health--;
    }
    public void KeyPresses()
    {
        if (Raylib.IsKeyDown(KeyboardKey.KEY_W))
            up = true;
        else up = false;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
            down = true;
        else down = false;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
            left = true;
        else left = false;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
            right = true;
        else right = false;
    }
    public void CalculateVelocity()
    {
        if (up)
            yVelocity += speed;
        if (down)
            yVelocity -= speed;
        if (left)
            xVelocity -= speed;
        if (right)
            xVelocity += speed;
    }
}