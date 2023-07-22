using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ���������, �� ���������������� ��� ��������� ��������
/// </summary>
public static class MainAlgorithms
{
    /// <summary>
    /// ������� ����, ���������� ����� ��������� n ����� �� ��������� ������� � ����������� �������� ������� ����
    /// </summary>
    /// <param name="startPosition"> �������� ������� </param>
    /// <param name="steps"> �������� ������ ������� �����, ��� ���������� �����, �� ��� ���� �������� </param>
    /// <returns> ������� �����, � ���� ���������� ���� </returns>
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
    /// ������� ���� ������ ����������� n-����� �� ����������� ��������� � ��������� ��������
    /// </summary>
    /// <param name="startPosition"> �������� ������� </param>
    /// <param name="steps">  �������� ������ ������� �����, ��� ���������� �����, �� ��� ���� ��������  </param>
    /// <param name="direction"> �������� ���� </param>
    /// <returns> ������� �����, � ���� ���������� ���� </returns>
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
    /// ������� ������� ������ (���� ���������) �� ����� ������
    /// </summary>
    /// <param name="spaceToSplit"> ��� ��������� </param>
    /// <param name="minWidth"> �������� ������ ��������� (x) </param>
    /// <param name="minHeight"> �������� ������ ��������� (y) </param>
    /// <returns> ������ �����, ��������� � ��������� ��������� </returns>
    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        var roomsQueue = new Queue<BoundsInt>();
        var roomsList = new List<BoundsInt>();

        roomsQueue.Enqueue(spaceToSplit);

        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();

            // ������ ������, �� ������� ��� ��� ��������� ������
            if (room.size.x >= minWidth && room.size.y >= minHeight)
            {
                if (Random.value < 0.5f) // �������� ������������� ���������
                {
                    if (room.size.y >= minHeight * 2) SplitHorizontally(roomsQueue, room);
                    else if (room.size.x >= minWidth * 2) SplitVertically(roomsQueue, room);
                    else if (room.size.x >= minWidth && room.size.y >= minHeight) roomsList.Add(room);
                }
                else // �������� ����������� ���������
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
    /// ������� ������ �� �������� �� �� ����� ������
    /// </summary>
    /// <param name="roomsQueue"> ����� �����, �� ���������������� ��� ��������� �������� </param>
    /// <param name="room"> ������ ��� ����� </param>
    private static void SplitVertically(Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x);

        var room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        var room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z), new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));

        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    /// <summary>
    /// ������� ������ ���������� �� �� ����� ������
    /// </summary>
    /// <param name="roomsQueue"> ����� �����, �� ���������������� ��� ��������� �������� </param>
    /// <param name="room"> ������ ��� ����� </param>
    private static void SplitHorizontally(Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y);

        var room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        var room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z), new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));

        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    /// <summary>
    /// ��������� ����, ���� ������ ������ �������� ��� ��������� ��������� ������
    /// </summary>
    public static class Direction2D
    {
        // ������ ������� ���������������� ��������
        public static readonly List<Vector2Int> FourDirectionsList = new List<Vector2Int>
        {
            new Vector2Int(0,1), // top
            new Vector2Int(1,0), // right
            new Vector2Int(0,-1), // bottom
            new Vector2Int(-1,0) // left
        };

        // ������ ����������� ��������
        public static List<Vector2Int> diagonalDirectionsList = new List<Vector2Int>
    {
         new Vector2Int(1,1), //UP-RIGHT
         new Vector2Int(1,-1), //RIGHT-DOWN
         new Vector2Int(-1, -1), // DOWN-LEFT
         new Vector2Int(-1, 1) //LEFT-UP
    };

        // ������ ��� ��������, ��������� �������
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
