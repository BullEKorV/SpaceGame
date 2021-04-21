using System;
using Raylib_cs;
class Player
{
    public static Player ship;

    //Position variables
    public float x, y;

    // Size variables
    public int width, height;

    // Velocity variables
    private float xVelocity, yVelocity, speed = 0.22f;

    // Rotation
    public float rotation;

    // Health variables
    public int health, maxHealth;

    // Shooting variables
    private int timeSinceLaser, timeSinceExplosive, shootSpeed = 15, damage = 15;

    // Moving variables
    private bool left, right, up, down;
    public Player(float x, float y, float rotation, int maxHealth)
    {
        this.x = x;
        this.y = y;
        this.rotation = rotation;
        this.maxHealth = maxHealth;
        this.health = maxHealth;

        ship = this;
    }
    public void PlayerControl()
    {
        // Make ship look at mouse
        ship.rotation = Program.LookAt(ship.x + Raylib.GetScreenWidth() / 2, ship.y - Raylib.GetScreenHeight() / 2, ship.x + Raylib.GetMouseX(), ship.y - Raylib.GetMouseY());

        // Spawn bullet
        if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON) && ship.timeSinceLaser > shootSpeed)
        {
            Bullet.SpawnBullet(ship.x, ship.y, ship.rotation, ship.height / 2, 20, ship.damage, true, false);
            ship.timeSinceLaser = 0;
        }
        else if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_RIGHT_BUTTON) && ship.timeSinceExplosive > shootSpeed * 10)
        {
            Bullet.SpawnBullet(ship.x, ship.y, ship.rotation, ship.height / 2, 7, ship.damage * 8, true, true);
            ship.timeSinceExplosive = 0;
        }
        ship.timeSinceLaser++;
        ship.timeSinceExplosive++;

        // Check keypresses
        KeyPresses();

        // Calculate velocity
        CalculateVelocity();

        // Calculate new position
        var newPos = Program.CalculatePosition(ship.x, ship.y, ship.xVelocity, ship.yVelocity);
        ship.x = newPos.x;
        ship.y = newPos.y;

        ship.xVelocity *= 0.96f;
        ship.yVelocity *= 0.96f;

        // Check collision
        ship.health -= Program.CheckBulletCollision(ship.x, ship.y, ship.width, true);
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