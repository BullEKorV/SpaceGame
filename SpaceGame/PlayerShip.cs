using System;
using Raylib_cs;
class PlayerShip
{
    public static PlayerShip ship;
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
    public PlayerShip(float x, float y, float rotation, int maxHealth)
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
        if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON) && ship.timeSinceShot > 20)
        {
            Bullet.SpawnBullet(ship.x, ship.y, ship.rotation, ship.height / 2);
            ship.timeSinceShot = 0;
        }
        ship.timeSinceShot++;

        // Check keypresses
        KeyPresses();

        // Calculate velocity
        CalculateVelocity();

        // Constraint velocities
        // if (ship.xVelocity < -5) // Constraint max velocity
        //     ship.xVelocity = -5;
        // else if (ship.xVelocity > 5) // Constraint max velocity
        //     ship.xVelocity = 5;

        // if (ship.yVelocity < -5) // Constraint max velocity
        //     ship.yVelocity = -5;
        // else if (ship.yVelocity > 5) // Constraint max velocity
        //     ship.yVelocity = 5;

        // Calculate new position
        var newPos = Program.CalculatePosition(ship.x, ship.y, ship.xVelocity, ship.yVelocity);
        ship.x = newPos.x;
        ship.y = newPos.y;

        ship.xVelocity *= 0.96f;
        ship.yVelocity *= 0.96f;

        // Check collision
        if (Program.CheckCollision(ship.x, ship.y, ship.width, Bullet.allBullets))
            ship.health--;
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