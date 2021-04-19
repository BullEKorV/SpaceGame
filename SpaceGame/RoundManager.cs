using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

class RoundManager
{
    public static Round currentRound;
    public static void GetCurrentRound(int round)
    {
        currentRound = GetLevelsJson()[round - 1];
        currentRound.timeTillNextSpawn = (float)Raylib_cs.Raylib.GetTime() + currentRound.spawnRate;
    }
    public static int EnemiesLeft()
    {
        // int enemiesLefts = currentRound.enemies.easy + currentRound.enemies.hard;
        return currentRound.enemies.easy + currentRound.enemies.hard;
    }
    public static void NewRound()
    {
        if (EnemiesLeft() == 0 && EnemyShip.allEnemies.Count == 0)
        {
            if (currentRound.round < GetLevelsJson().Count)
                GetCurrentRound(currentRound.round + 1);
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