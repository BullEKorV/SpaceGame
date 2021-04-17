using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

class RoundHandler
{
    public static Round currentRound;
    public static void GetCurrentRound(int round)
    {
        currentRound = JSONreader.GetAllRounds()[round - 1];
        currentRound.timeTillNextSpawn = (float)Raylib_cs.Raylib.GetTime() + currentRound.spawnRate;
    }
}
class JSONreader
{
    public static List<Round> GetAllRounds()
    {
        string response = File.ReadAllText("rounds.json");

        List<Round> rounds = JsonConvert.DeserializeObject<List<Round>>(response);

        return rounds;
    }
}
public class Round
{
    public float timeTillNextSpawn;
    public int round { get; set; }
    public int enemies { get; set; }
    public float spawnRate { get; set; }
}