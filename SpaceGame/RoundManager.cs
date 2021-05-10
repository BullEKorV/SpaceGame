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
    public static bool bossAlive = false;
    public static Round currentRound;
    private static int timeBetweenRounds = 3;
    public static void SpawnEnemies()
    {
        // Spawn dummy Enemy first round
        if (currentRound.round == 0 && Enemy.allEnemies.Count == 0)
            new Enemy(EnemyType.Dummy);

        var rnd = new Random();

        // Spawn new enemy
        if (Raylib.GetTime() > currentRound.timeTillNextSpawn && Enemy.allEnemies.Count < 10)
        {
            if (RoundManager.EnemiesLeft() > 0)
            {
                bool enemySpawned = false;

                while (!enemySpawned)
                {
                    int enemyToSpawn = rnd.Next(0, 3);

                    if (enemyToSpawn == 0 && currentRound.enemies.easy > 0)
                    {
                        enemySpawned = true;
                        new Enemy(EnemyType.Easy);
                        RoundManager.currentRound.enemies.easy--;
                    }
                    else if (enemyToSpawn == 1 && currentRound.enemies.hard > 0)
                    {
                        enemySpawned = true;
                        new Enemy(EnemyType.Hard);
                        currentRound.enemies.hard--;
                    }
                    else if (enemyToSpawn == 2 && currentRound.enemies.kamikaze > 0)
                    {
                        enemySpawned = true;
                        new Enemy(EnemyType.Kamikaze);
                        currentRound.enemies.kamikaze--;
                    }
                }
            }
            else if (currentRound.enemies.boss > 0)
            {
                bossAlive = true;
                new BossSun();
                RoundManager.currentRound.enemies.boss--;
            }
            int extraTime = 0;
            if (EnemiesLeft() == 0 && currentRound.enemies.boss > 0)
                extraTime = 5;
            currentRound.timeTillNextSpawn = (float)Raylib.GetTime() + currentRound.spawnRate + extraTime;
        }
    }
    public static void GetCurrentRound(int round)
    {
        currentRound = GetLevelsJson()[round];
        currentRound.timeTillNextSpawn = (float)Raylib_cs.Raylib.GetTime() + currentRound.spawnRate;
    }
    public static int EnemiesLeft()
    {
        return currentRound.enemies.easy + currentRound.enemies.hard + currentRound.enemies.kamikaze;
    }
    public static void RoundCompleted()
    {
        if (currentRound.round == 0)
            EventManager.allTexts.Clear();
        if (EnemiesLeft() == 0 && Enemy.allEnemies.Count == 0 && bossAlive == false && currentRound.enemies.boss == 0)
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
    public int kamikaze { get; set; }
    public int boss { get; set; }
}