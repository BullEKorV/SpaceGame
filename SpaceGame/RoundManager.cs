using System;
using System.Numerics;
using Raylib_cs;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

class RoundManager
{
    public static float timeTillNextRound;
    public static bool roundActive = true;
    public static Round currentRound;
    private static int timeBetweenRounds = 3;
    public static void GetCurrentRound(int round)
    {
        currentRound = GetLevelsJson()[round];
        currentRound.timeTillNextSpawn = (float)Raylib_cs.Raylib.GetTime() + currentRound.spawnRate;
    }
    public static int EnemiesLeft()
    {
        return currentRound.enemies.easy + currentRound.enemies.hard;
    }
    public static void RoundCompleted()
    {
        if (EnemiesLeft() == 0 && Enemy.allEnemies.Count == 0)
        {
            if (currentRound.round < GetLevelsJson().Count - 1)
            {
                new TextBox((float)Raylib.GetTime() + timeBetweenRounds + 2, new Vector2(Raylib.GetScreenWidth() / 2 - 100, 20), 70, "Round " + (currentRound.round + 1), Color.WHITE);

                roundActive = false;
                timeTillNextRound = (float)Raylib.GetTime() + timeBetweenRounds;
            }
            else
            {
                new TextBox((float)Raylib.GetTime() + 100000, new Vector2(Raylib.GetScreenWidth() / 2 - 100, 20), 40, "No more rounds", Color.WHITE);
            }
        }
    }
    public static void NewRound()
    {
        roundActive = true;

        GetCurrentRound(currentRound.round + 1);

        new TextBox((float)Raylib.GetTime() + 2, new Vector2(Raylib.GetScreenWidth() / 2 - 65, 90), 50, "BEGIN!", Color.WHITE);
    }
    static List<Round> GetLevelsJson()
    {
        string response = File.ReadAllText("rounds.json");

        List<Round> rounds = JsonConvert.DeserializeObject<List<Round>>(response);

        return rounds;
    }
}
class Round
{
    public float timeTillNextSpawn;
    public int round { get; set; }
    public EnemiesDifficulties enemies { get; set; }
    public float spawnRate { get; set; }
}
class EnemiesDifficulties
{
    public int easy { get; set; }
    public int hard { get; set; }
}