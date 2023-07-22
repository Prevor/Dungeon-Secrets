using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyRoom : Room
{
    private List<EnemyToSpawn> blacklistedEnemies;

    public override void Create(Vector2Int centerPosition, HashSet<Vector2Int> floorsPositions)
    {
        Init(centerPosition, floorsPositions);

        SpawnInteriorObjectRandomly(roomData.SpawnableInteriorObjects[0]);
        SpawnInteriorObjectRandomly(roomData.SpawnableInteriorObjects[1]); 

        //SpawnInteriorObjectRandomly(roomData.SpawnableInteriorObjects[2]); 
        //SpawnInteriorObjectRandomly(roomData.SpawnableInteriorObjects[3]); 

        SpawnEnemies();
    }

    #region SPAWNING ENEMIES
    protected void SpawnEnemies()
    {
        if (!roomData.CanSpawnEnemies) return;

        if (roomData.SpawnableEnemies.Count == 0)
            throw new Exception(gameObject.name + " Room has no 'spawnable enemies' attached, but trying to spawn them!");

        // spawn enemies
        blacklistedEnemies = new List<EnemyToSpawn>();

        for (int i = 1; i < 3; i++)
        {
            InteriorGenerator.SpawnEnemy(GetRandomEnemyToSpawn(), availableFloorsPositions, gameObject.transform);
        }
    }

    private EnemyToSpawn GetRandomEnemyToSpawn()
    {
        if (roomData.SpawnableEnemies.Count == 0)
            return null;

        var random = new System.Random();
        var availableEnemies = roomData.SpawnableEnemies.Except(blacklistedEnemies).ToList();

        if (availableEnemies.Count == 0)
            return null;

        var randomEnemyIndex = random.Next(0, availableEnemies.Count);
        var randomEnemy = availableEnemies[randomEnemyIndex];
        blacklistedEnemies.Add(randomEnemy);

        return randomEnemy;
    }
    #endregion

}