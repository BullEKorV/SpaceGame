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
    public int enemiesEasy { get; set; }
    public int enemiesHard { get; set; }
    public float spawnRate { get; set; }
}