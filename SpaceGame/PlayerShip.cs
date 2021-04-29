using System;
using System.Numerics;
using Raylib_cs;
class Player
{
    public static Player ship;

    //Position variables
    public Vector2 pos, velocity;

    // Rotation
    public float rotation;

    // Size variables
    public int width, height;

    // Velocity variables
    private float speed = 0.23f;

    // Stats
    public int health, maxHealth, score, damage = 15;

    // Timer variables
    public float timeTillLaser, timeTillExplosive, timeTillHealthRegen, laserFireRate = 0.15f, explosiveFireRate = 0.3f;

    public Player(Vector2 pos, float rotation, int maxHealth)
    {
        this.pos = pos;
        this.rotation = rotation;
        this.maxHealth = maxHealth;
        this.health = maxHealth;

        ship = this;
    }
    public void PlayerControl()
    {
        // Make ship look at mouse
        ship.rotation = Program.LookAt(new Vector2(ship.pos.X + Raylib.GetScreenWidth() / 2, ship.pos.Y - Raylib.GetScreenHeight() / 2), new Vector2(ship.pos.X + Raylib.GetMouseX(), ship.pos.Y - Raylib.GetMouseY()));

        // Spawn bullet
        if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON) && Raylib.GetTime() > ship.timeTillLaser)
        {
            Vector2 leftCords = Program.CalculatePositionVelocity(ship.pos, 40, ship.rotation - 90);
            Vector2 rightCords = Program.CalculatePositionVelocity(ship.pos, 40, ship.rotation + 90);

            new Bullet(leftCords, ship.rotation, ship.height / 2, 20, ship.damage, true, false, false);
            new Bullet(rightCords, ship.rotation, ship.height / 2, 20, ship.damage, true, false, false);

            ship.timeTillLaser = (float)Raylib.GetTime() + laserFireRate;
        }
        else if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_RIGHT_BUTTON) && Raylib.GetTime() > ship.timeTillExplosive)
        {
            new Bullet(ship.pos, ship.rotation, ship.height / 2, 7, ship.damage * 8, true, true, false);
            ship.timeTillExplosive = (float)Raylib.GetTime() + explosiveFireRate;
        }

        // Regen health after a while
        if (ship.health < ship.maxHealth && Raylib.GetTime() > ship.timeTillHealthRegen)
            ship.health++;

        // Check keypresses
        // KeyPresses();

        // Calculate velocity
        CalculateVelocity();

        // Calculate new position
        ship.pos = Program.CalculatePosition(ship.pos, ship.velocity);

        ship.velocity *= 0.96f;

        // Check collision
        int damageTaken = Program.CheckBulletCollision(ship.pos, ship.width, true);
        ship.health -= damageTaken;

        if (damageTaken > 0)
            ship.timeTillHealthRegen = (int)Raylib.GetTime() + 3;
    }
    public void TakeDamage(int damage)
    {
        ship.health -= damage;
        ship.timeTillHealthRegen = (int)Raylib.GetTime() + 3;
    }
    public void CalculateVelocity()
    {
        if (Raylib.IsKeyDown(KeyboardKey.KEY_W))
            ship.velocity.Y += speed;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
            ship.velocity.Y -= speed;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
            ship.velocity.X -= speed;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
            ship.velocity.X += speed;
    }
}