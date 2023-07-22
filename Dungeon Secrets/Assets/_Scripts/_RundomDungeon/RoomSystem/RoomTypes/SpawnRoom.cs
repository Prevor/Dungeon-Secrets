using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SpawnRoom : EnemyRoom
{
    [Space]
    [SerializeField] private GameObject spawnPoint;

    public override void Create(Vector2Int centerPosition, HashSet<Vector2Int> floorsPositions)
    {
        Init(centerPosition, floorsPositions);

        GameObject game = Instantiate(spawnPoint, roomCenterPosition + InteriorGenerator.placementOffset, quaternion.identity, gameObject.transform);
        game.name = game.name.Replace("(Clone)", "");

        SpawnInteriorObjectNearWall(roomData.SpawnableInteriorObjects[0]);
        SpawnInteriorObjectRandomly(roomData.SpawnableInteriorObjects[1]);
        // SpawnInteriorObjectRandomly(roomData.SpawnableInteriorObjects[2]);

        SpawnEnemies();
    }
}