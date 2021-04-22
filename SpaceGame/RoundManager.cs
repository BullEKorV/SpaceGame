using System;
using System.Numerics;
using Raylib_cs;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

class RoundManager
{
    public static float timeTillNextRound;
    public static Round currentRound;
    public static void GetCurrentRound(int round)
    {
        currentRound = GetLevelsJson()[round - 1];
        currentRound.timeTillNextSpawn = (float)Raylib_cs.Raylib.GetTime() + currentRound.spawnRate;
    }
    public static int EnemiesLeft()
    {
        return currentRound.enemies.easy + currentRound.enemies.hard;
    }
    public static void RoundCompleted()
    {

    }
    public static void NewRound()
    {
        if (EnemiesLeft() == 0 && Enemy.allEnemies.Count == 0)
        {
            if (currentRound.round < GetLevelsJson().Count)
            {
                new TextBox((float)Raylib.GetTime() + 5, new Vector2(Raylib.GetScreenWidth() / 2 - 100, 30), 50, "Round " + currentRound.round, Color.WHITE);
                GetCurrentRound(currentRound.round + 1);
            }
        }
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