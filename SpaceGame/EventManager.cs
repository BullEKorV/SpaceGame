using System;
using Raylib_cs;
using System.Collections.Generic;

class EventManager
{
    public static List<Effect> allEffects = new List<Effect>();
    public static void NewEffect(float timeToDespawn, float x, float y, float size, Texture2D texture)
    {
        var rnd = new Random();
        int rotation = rnd.Next(0, 360);
        allEffects.Add(new Effect(timeToDespawn, x, y, rotation, size, 255, texture));
    }
    public static void ManagerCode()
    {
        // Console.WriteLine(allEffects.Count);

        EffectBehaviour();
    }
    static void EffectBehaviour()
    {
        for (int i = 0; i < allEffects.Count; i++)
        {
            if (allEffects[i].timeToDespawn < Raylib.GetTime())
            {
                allEffects.RemoveAt(i);
                return;
            }
            allEffects[i].rotation += 1;
            if (allEffects[i].rotation > 360)
                allEffects[i].rotation -= 360;
            allEffects[i].size *= 1.03f;
            allEffects[i].transparency *= 0.92f;
        }
    }
}

class Effect
{
    public float timeToDespawn;
    public float x;
    public float y;
    public float rotation;
    public float size;
    public float transparency;
    public Texture2D texture;
    public Effect(float timeToDespawn, float x, float y, float rotation, float size, float transparency, Texture2D texture)
    {
        this.timeToDespawn = timeToDespawn;
        this.x = x;
        this.y = y;
        this.rotation = rotation;
        this.size = size;
        this.transparency = transparency;
        this.texture = texture;
    }
}