using System;
using Raylib_cs;
using System.Collections.Generic;

class EventManager
{
    static List<Effect> allEffects = new List<Effect>();
    public static void NewEffect(float timeToDespawn, float x, float y, int size, Texture2D texture)
    {
        allEffects.Add(new Effect(timeToDespawn, x, y, size, texture));
    }
    public static void ManagerCode()
    {
        Console.WriteLine(allEffects.Count);
        DeleteEffects();

    }
    static void DeleteEffects()
    {
        for (int i = 0; i < allEffects.Count; i++)
        {
            if (allEffects[i].timeToDespawn < Raylib.GetTime())
            {
                allEffects.RemoveAt(i);
            }
        }
    }
    public static void DrawEffects()
    {
        foreach (Effect effect in allEffects)
        {
            Raylib.DrawTexture(effect.texture, (int)effect.x - (int)Player.ship.x + Raylib.GetScreenWidth() / 2, -(int)effect.y + (int)Player.ship.y + Raylib.GetScreenHeight() / 2, Color.WHITE);
        }
    }
}

class Effect
{
    public float timeToDespawn;
    public float x;
    public float y;
    public int size;
    public Texture2D texture;
    public Effect(float timeToDespawn, float x, float y, int size, Texture2D texture)
    {
        this.timeToDespawn = timeToDespawn;
        this.x = x;
        this.y = y;
        this.size = size;
        this.texture = texture;
    }
}