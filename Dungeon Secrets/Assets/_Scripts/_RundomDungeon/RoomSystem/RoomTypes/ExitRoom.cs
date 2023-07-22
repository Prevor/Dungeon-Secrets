using System.Collections.Generic;
using UnityEngine;

public class ExitRoom : Room
{
    [Space]
    [SerializeField] private GameObject exitPrefab;

    public override void Create(Vector2Int centerPosition, HashSet<Vector2Int> floorsPositions)
    {
        Init(centerPosition, floorsPositions); 

        Instantiate(exitPrefab, roomCenterPosition + InteriorGenerator.placementOffset, Quaternion.identity, gameObject.transform); 

        SpawnInteriorObjectNearWall(roomData.SpawnableInteriorObjects[0]); 
        //SpawnInteriorObjectRandomly(roomData.SpawnableInteriorObjects[1]); 
    }
}