
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Цей клас генерує об'єкти інтер'єру всередині кімнат
/// Потрібно додати цей компонент до об'єкта генератора рівнів.
/// </summary>
[RequireComponent(typeof(RoomSystem))]
public class InteriorGenerator : MonoBehaviour
{
    public static readonly Vector2 placementOffset = new Vector2(0.5f, 0.5f);

    #region PUBLIC METHODS
    /// <summary>
    /// Створює об'єкт інтер'єру у випадковому місці
    /// </summary>
    /// <param name="objectToSpawn"> об'єкт інтер'єру для розмноження </param>
    /// <param name="availablePositions"> доступні позиції кімнат на підлозі </param>
    /// <param name="parent"> параметри кімнпти </param>
    public static void SpawnInteriorObjectRandomly(InteriorObject objectToSpawn, List<Vector2Int> availablePositions, Transform parent)
    {
        var quantity = Random.Range(objectToSpawn.MinQuantity, objectToSpawn.MaxQuantity + 1);
        var availablePositionsSet = new HashSet<Vector2Int>(availablePositions);

        for (int i = 0; i < quantity; i++)
        {
            int limiter = 10;
            while (true)
            {
                var spawnPosition = availablePositions[Random.Range(0, availablePositions.Count)];
                var neighbours = new List<Vector2Int>();

                if (objectToSpawn.X > 1 && objectToSpawn.Y > 1) // x & y bigger than 1
                {
                    var current = spawnPosition;

                    for (int y = 1; y <= objectToSpawn.Y; y++)
                    {
                        for (int x = 0; x < objectToSpawn.X; x++)
                            neighbours.Add(spawnPosition + new Vector2Int(x, 0));

                        current = new Vector2Int(spawnPosition.x, current.y - 1);
                    }
                }
                else if (objectToSpawn.X > 1 && objectToSpawn.Y <= 1) // x bigger than 1
                {
                    for (int x = 0; x < objectToSpawn.X; x++)
                        neighbours.Add(spawnPosition + MainAlgorithms.Direction2D.FourDirectionsList[1] * x);
                }
                else if (objectToSpawn.X <= 1 && objectToSpawn.Y > 1) // y bigger than 1
                {
                    for (int y = 0; y < objectToSpawn.Y; y++)
                        neighbours.Add(spawnPosition + MainAlgorithms.Direction2D.FourDirectionsList[2] * y);
                }
                else if (objectToSpawn.X <= 1 && objectToSpawn.Y <= 1) // x & y equals 1
                {
                    SpawnInteriorObjectInPlace(objectToSpawn, spawnPosition, availablePositions, neighbours, parent);
                    break;
                }

                if (availablePositionsSet.IsSubsetOf(neighbours))
                {
                    SpawnInteriorObjectInPlace(objectToSpawn, spawnPosition, availablePositions, neighbours, parent);
                    break;
                }

                if (limiter <= 0)
                    break;

                limiter--;
            }
        }
    }

    public static void SpawnInteriorObjectNearWall(InteriorObject objectToSpawn, List<Vector2Int> availablePositions, Transform parent)
    {
        var quantity = Random.Range(objectToSpawn.MinQuantity, objectToSpawn.MaxQuantity + 1);

        for (int i = 0; i < quantity; i++)
        {
            var limiter = 50;

            while (limiter >= 0)
            {
                var spawnPosition = availablePositions[Random.Range(0, availablePositions.Count)];
                var neighbours = new List<Vector2Int>();

                if (objectToSpawn.X > 1 && objectToSpawn.Y > 1) // x & y bigger than 1
                {
                    var current = spawnPosition;

                    for (int y = 1; y <= objectToSpawn.Y; y++)
                    {
                        for (int x = 0; x < objectToSpawn.X; x++)
                            neighbours.Add(new Vector2Int(current.x + x, current.y));

                        current = new Vector2Int(spawnPosition.x, current.y - 1);
                    }
                }
                else if (objectToSpawn.X > 1 && objectToSpawn.Y <= 1) // x bigger than 1
                {
                    for (int x = 0; x < objectToSpawn.X; x++)
                        neighbours.Add(spawnPosition + MainAlgorithms.Direction2D.FourDirectionsList[1] * x);
                }
                else if (objectToSpawn.X <= 1 && objectToSpawn.Y > 1) // y bigger than 1
                {
                    for (int y = 0; y < objectToSpawn.Y; y++)
                        neighbours.Add(spawnPosition + MainAlgorithms.Direction2D.FourDirectionsList[2] * y);
                }
                else if (objectToSpawn.X <= 1 && objectToSpawn.Y <= 1) // x & y equals 1
                {
                    if (CheckIfNearWall(spawnPosition, availablePositions) is true)
                    {
                        SpawnInteriorObjectInPlace(objectToSpawn, spawnPosition, availablePositions, neighbours, parent);
                        break;
                    }
                }

                //if (availablePositions.ContainsAll(neighbours))
                //{
                //    if (CheckIfNearWall(spawnPosition, availablePositions) is true || CheckIfNearWall(neighbours[neighbours.Count - 1], availablePositions) is true)
                //    {
                //        SpawnInteriorObjectInPlace(objectToSpawn, spawnPosition, availablePositions, neighbours, parent);
                //        break;
                //    }
                //}

                //if (neighbours.All(neighbour => availablePositions.Contains(neighbour)))
                //{
                //    if (CheckIfNearWall(spawnPosition, availablePositions) || CheckIfNearWall(neighbours[neighbours.Count - 1], availablePositions))
                //    {
                //        SpawnInteriorObjectInPlace(objectToSpawn, spawnPosition, availablePositions, neighbours, parent);
                //        break;
                //    }
                //}

                if (neighbours.Count > 0)
                {
                    if (neighbours.All(neighbour => availablePositions.Contains(neighbour)))
                    {
                        if (CheckIfNearWall(spawnPosition, availablePositions) || CheckIfNearWall(neighbours[neighbours.Count - 1], availablePositions))
                        {
                            SpawnInteriorObjectInPlace(objectToSpawn, spawnPosition, availablePositions, neighbours, parent);
                            break;
                        }
                    }
                }

                limiter -= 1;
            }
        }
    }

    public static void SpawnEnemy(EnemyToSpawn enemyToSpawn, List<Vector2Int> availablePositions, Transform parent)
    {
        var quantity = Random.Range(enemyToSpawn.MinQuantity, enemyToSpawn.MaxQuantity);

        for (int i = 0; i < quantity; i++)
        {
            var limiter = 10;

            while (limiter >= 0)
            {
                var randomSpawnPosition = availablePositions[Random.Range(0, availablePositions.Count)];

                // створення ворога і видалення його позицію зі списку доступних позицій
                if (availablePositions.Contains(randomSpawnPosition))
                {
                    Instantiate(enemyToSpawn.ObjectPrefab, randomSpawnPosition + enemyToSpawn.positionOffset, Quaternion.Euler(new Vector3(0,0,0)), parent);
                    availablePositions.Remove(randomSpawnPosition);

                    break;
                }

                limiter -= 1;
            }
        }
    }
    #endregion

    #region HELPER METHODS
    private static void SpawnInteriorObjectInPlace(InteriorObject objectToSpawn, Vector2Int spawnPosition, ICollection<Vector2Int> availablePositions, IReadOnlyCollection<Vector2Int> neighbouringPositions, Transform parent)
    {
        Instantiate(objectToSpawn.ObjectPrefab, spawnPosition + objectToSpawn.PositionOffset, Quaternion.identity, parent);

        // видалити зайняті позиції зі списку доступних позицій
        availablePositions.Remove(spawnPosition);

        if (neighbouringPositions.Count > 0)
        {
            foreach (var pos in neighbouringPositions)
            {
                availablePositions.Remove(pos);
            }
        }
    }

    private static bool CheckIfNearWall(Vector2Int position, ICollection<Vector2Int> availablePositions)
    {
        var counter = 0;

        foreach (var dir in MainAlgorithms.Direction2D.EightDirectionsList)
        {
            if (!availablePositions.Contains(position + dir))
            {
                counter += 1;
            }
            else
            {
                counter -= 1;

                if (counter <= 0)
                    counter = 0;
            }
        }

        return counter >= 3;
    }
    #endregion

}