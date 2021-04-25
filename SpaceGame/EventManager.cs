using System;
using System.Numerics;
using Raylib_cs;
using System.Collections.Generic;

class EventManager
{
    public static List<Effect> allEffects = new List<Effect>();
    public static List<TextBox> allTexts = new List<TextBox>();
    public static void ManagerCode()
    {
        // Console.WriteLine(allEffects.Count);

        TextBehaviour();

        Console.WriteLine(allEffects.Count);
        EffectBehaviour();
    }
    static void TextBehaviour()
    {
        for (int i = 0; i < allTexts.Count; i++)
        {
            if (allTexts[i].timeToDespawn < Raylib.GetTime())
            {
                allTexts.RemoveAt(i);
                return;
            }
        }
    }
    static void EffectBehaviour()
    {
        // Limit effects to 20 effects
        // while (allEffects.Count > 20)
        // {
        //     allEffects.RemoveAt(0);
        // }

        foreach (Effect effect in allEffects)
        {
            if (effect.timeToDespawn < Raylib.GetTime())
            {
                allEffects.Remove(effect);
                return;
            }
            effect.rotation += 1;
            if (effect.rotation > 360)
                effect.rotation -= 360;
            effect.size += 0.02f;
            effect.transparency -= 5;
            if (effect.transparency <= 1)
            {
                allEffects.Remove(effect);
                return;
            }
        }
    }
}

class Effect
{
    public float timeToDespawn;
    public Vector2 pos;
    public float rotation;
    public float size;
    public float transparency;
    public Texture2D texture;
    public Effect(float timeToDespawn, Vector2 pos, float size, float transparency, Texture2D texture)
    {
        this.timeToDespawn = timeToDespawn;
        this.pos = pos;
        this.size = size;
        this.transparency = transparency;
        this.texture = texture;

        var rnd = new Random();
        int rotation = rnd.Next(0, 360);
        this.rotation = rotation;

        bool canSpawn = true;

        // Deletes if already effect there
        for (int i = 0; i < EventManager.allEffects.Count; i++)
        {
            if (Vector2.Distance(pos, EventManager.allEffects[i].pos) < 25)
            {
                canSpawn = false;
            }
        }
        if (canSpawn)
            EventManager.allEffects.Add(this);
    }
}
class TextBox
{
    public float timeToDespawn;
    public Vector2 pos;
    public int fontSize;
    public string text;
    public Color color;
    public TextBox(float timeToDespawn, Vector2 pos, int fontSize, string text, Color color)
    {
        this.timeToDespawn = timeToDespawn;
        this.pos = pos;
        this.fontSize = fontSize;
        this.text = text;
        this.color = color;

        EventManager.allTexts.Add(this);
    }
}