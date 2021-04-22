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
        // for (int i = 0; i < allEffects.Count; i++)
        // {
        //     if (allEffects[i].timeToDespawn < Raylib.GetTime())
        //     {
        //         allEffects.RemoveAt(i);
        //         return;
        //     }
        //     allEffects[i].rotation += 1;
        //     if (allEffects[i].rotation > 360)
        //         allEffects[i].rotation -= 360;
        //     allEffects[i].size *= 1.03f;
        //     allEffects[i].transparency *= 0.92f;
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
            effect.size *= 1.03f;
            effect.transparency *= 0.92f;
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