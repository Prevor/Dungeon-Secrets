using System.Collections.Generic;
using UnityEngine;

public class TreasureRoom : EnemyRoom
{
    [Space]
    [SerializeField] private GameObject treasureChestPrefab;

    public override void Create(Vector2Int centerPosition, HashSet<Vector2Int> floorsPositions)
    {
        Init(centerPosition, floorsPositions);

        Instantiate(treasureChestPrefab, roomCenterPosition + InteriorGenerator.placementOffset, Quaternion.identity, gameObject.transform);

        SpawnInteriorObjectRandomly(roomData.SpawnableInteriorObjects[0]); 
        SpawnInteriorObjectRandomly(roomData.SpawnableInteriorObjects[1]);

        SpawnEnemies();
    }

}