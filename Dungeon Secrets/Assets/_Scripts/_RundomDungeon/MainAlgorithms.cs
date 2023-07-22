using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Основні алгоритми, що використовуються для генерації підземель
/// </summary>
public static class MainAlgorithms
{
    /// <summary>
    /// Створює шлях, випадковим чином проходячи n кроків від початкової позиції у випадковому напрямку кожного разу
    /// </summary>
    /// <param name="startPosition"> стартова позиція </param>
    /// <param name="steps"> алгоритм приймає кількість кроків, для обчислення шляху, під час однієї ітерації </param>
    /// <returns> множина точок, з яких складається шлях </returns>
    public static IEnumerable<Vector2Int> RandomWalk(Vector2Int startPosition, int steps)
    {
        var path = new HashSet<Vector2Int> { startPosition };
        var previousPosition = startPosition;

        for (int i = 0; i < steps; i++)
        {
            var newPosition = previousPosition + Direction2D.GetRandomSimpleDirection();
            previousPosition = newPosition;
            path.Add(newPosition);
        }
        return path;
    }

    /// <summary>
    /// Створює шлях шляхом проходження n-кроків від початкового положення у вибраному напрямку
    /// </summary>
    /// <param name="startPosition"> стартова позиція </param>
    /// <param name="steps">  алгоритм приймає кількість кроків, для обчислення шляху, під час однієї ітерації  </param>
    /// <param name="direction"> напрямок руху </param>
    /// <returns> множина точок, з яких складається шлях </returns>
    //public static IEnumerable<Vector2Int> NStepWalk(Vector2Int startPosition, int steps, Vector2Int direction)
    //{
    //    var path = new HashSet<Vector2Int> { startPosition };
    //    var previousPosition = startPosition;

    //    for (int i = 0; i < steps; i++)
    //    {
    //        var newPosition = previousPosition + direction;
    //        path.Add(newPosition);
    //        previousPosition = newPosition;
    //    }

    //    return path;
    //}

    /// <summary>
    /// Розбиває наданий простір (зону підземелля) на менші кімнати
    /// </summary>
    /// <param name="spaceToSplit"> межі підземелля </param>
    /// <param name="minWidth"> мінімальна ширина приміщення (x) </param>
    /// <param name="minHeight"> мінімальна висота приміщення (y) </param>
    /// <returns> список кімнат, створених в результаті розділення </returns>
    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        var roomsQueue = new Queue<BoundsInt>();
        var roomsList = new List<BoundsInt>();

        roomsQueue.Enqueue(spaceToSplit);

        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();

            // відкидає кімнати, які занадто малі для генерації номерів
            if (room.size.x >= minWidth && room.size.y >= minHeight)
            {
                if (Random.value < 0.5f) // спочатку горизонтальне розділення
                {
                    if (room.size.y >= minHeight * 2) SplitHorizontally(roomsQueue, room);
                    else if (room.size.x >= minWidth * 2) SplitVertically(roomsQueue, room);
                    else if (room.size.x >= minWidth && room.size.y >= minHeight) roomsList.Add(room);
                }
                else // спочатку вертикальне розділення
                {
                    if (room.size.x >= minWidth * 2) SplitVertically(roomsQueue, room);
                    else if (room.size.y >= minHeight * 2) SplitHorizontally(roomsQueue, room);
                    else if (room.size.x >= minWidth && room.size.y >= minHeight) roomsList.Add(room);
                }
            }
        }

        return roomsList;
    }

    /// <summary>
    /// Розділяє кімнату по вертикалі на дві менші кімнати
    /// </summary>
    /// <param name="roomsQueue"> черга кімнат, що використовуються для алгоритму розбиття </param>
    /// <param name="room"> кімната для поділу </param>
    private static void SplitVertically(Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x);

        var room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        var room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z), new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));

        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    /// <summary>
    /// Розділяє кімнату горизонталі на дві менші кімнати
    /// </summary>
    /// <param name="roomsQueue"> черга кімнат, що використовуються для алгоритму розбиття </param>
    /// <param name="room"> кімната для поділу </param>
    private static void SplitHorizontally(Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y);

        var room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        var room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z), new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));

        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    /// <summary>
    /// Допоміжний клас, який зберігає можливі напрямки для алгоритму випадкової ходьби
    /// </summary>
    public static class Direction2D
    {
        // список простих перпендикулярних напрямків
        public static readonly List<Vector2Int> FourDirectionsList = new List<Vector2Int>
        {
            new Vector2Int(0,1), // top
            new Vector2Int(1,0), // right
            new Vector2Int(0,-1), // bottom
            new Vector2Int(-1,0) // left
        };

        // список діагональних напрямків
        public static List<Vector2Int> diagonalDirectionsList = new List<Vector2Int>
    {
         new Vector2Int(1,1), //UP-RIGHT
         new Vector2Int(1,-1), //RIGHT-DOWN
         new Vector2Int(-1, -1), // DOWN-LEFT
         new Vector2Int(-1, 1) //LEFT-UP
    };

        // список усіх напрямків, включаючи діагоналі
        public static readonly List<Vector2Int> EightDirectionsList = new List<Vector2Int>
        {
            new Vector2Int(0,1), // top
            new Vector2Int(1,1), // top-right
            new Vector2Int(1,0), // right
            new Vector2Int(1,-1), // bottom-right
            new Vector2Int(0,-1), // bottom
            new Vector2Int(-1,-1), // bottom-left
            new Vector2Int(-1,0), // left
            new Vector2Int(-1,1), // top-left
        };

        public static readonly List<Vector3> Rotations = new List<Vector3>
        {
            new Vector3(0,0,0),
            new Vector3(0,0,-90),
            new Vector3(0,0,-180),
            new Vector3(0,0,-270)
        };

        /// <summary>
        /// Gets random direction (top / right / bottom / left)
        /// </summary>
        /// <returns> normalized vector that indicates the direction </returns>
        public static Vector2Int GetRandomSimpleDirection() => FourDirectionsList[Random.Range(0, FourDirectionsList.Count)];

    }
}
