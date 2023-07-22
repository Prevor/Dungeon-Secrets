using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

#if Editor
using UnityEditor;
[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGeneratorEditor : Editor
{
    private DungeonGenerator dungeonGenerator;
    private SerializedProperty useRandomWalk;

    private void OnEnable()
    {
        dungeonGenerator = (DungeonGenerator)target;
        useRandomWalk = serializedObject.FindProperty("useRandomWalk");
    }

    public override void OnInspectorGUI()
    {
        if (dungeonGenerator == null) return;

        serializedObject.Update();
        EditorGUILayout.BeginVertical();

        if (!Application.isPlaying)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("tilemapPaint"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("roomSystem"));
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Dungeon Size", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dungeonWidth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dungeonHeight"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Room parameters", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("minRoomWidth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("minRoomHeight"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("roomOffset"));
        //  EditorGUILayout.PropertyField(serializedObject.FindProperty("wideCoridor"));

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(useRandomWalk);
        if (EditorGUILayout.BeginFadeGroup(useRandomWalk.boolValue ? 1 : 0))
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("randomWalkData"));
        }
        EditorGUILayout.EndFadeGroup();

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate"))
            dungeonGenerator.GenerateDungeon();

        if (GUILayout.Button("Clear"))
            dungeonGenerator.ClearDungeon();

        // apply modified properties and repaint
        EditorGUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }
}
#endif



/// <summary>
/// �������� ������, �� ������� �� ��������� ���������.
/// </summary>
[SelectionBase]
public class DungeonGenerator : MonoBehaviour
{
    [Serializable]
    public struct RandomWalkData
    {
        [Tooltip("Number of algorithm iterations")]
        [SerializeField, Range(1, 100)] internal int iterations;

        [Tooltip("Number of steps algorithm performs during each iteration")]
        [SerializeField, Range(1, 50)] internal int steps;

        [Tooltip("Should the position be random during each iteration? \n YES : more tight passes \n NO : more island shape")]
        [SerializeField] internal bool startRandomlyEachIteration;
    }

    #region ���� � ���������
    [Tooltip("Tilemap Generator script reference")]
    [SerializeField] private TilemapPaint tilemapPaint;

    [Tooltip("Room System script reference")]
    [SerializeField] private RoomSystem roomSystem;

    [SerializeField] private int dungeonWidth, dungeonHeight = 20;
    [SerializeField] private int minRoomWidth, minRoomHeight = 4;
    [SerializeField, Range(1, 5)] private int roomOffset = 1;

    [Tooltip("Use random walk algorithm? YES / NO")]
    [SerializeField] private bool useRandomWalk = false;
    [SerializeField] private RandomWalkData randomWalkData;

    [Tooltip("Use wide coridor? YES / NO")]
    [SerializeField] private static bool wideCoridor = true;
    #endregion

    private readonly Vector2Int startPosition = Vector2Int.zero;
    public Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary = new Dictionary<Vector2Int, HashSet<Vector2Int>>();

    private void Awake() => GenerateDungeon();

    /// <summary>
    /// ������� ���� ���������
    /// </summary>
    public void GenerateDungeon()
    {
        ClearDungeon();
        CreateRooms();
    }

    /// <summary>
    /// ����� ���������
    /// </summary>
    public void ClearDungeon()
    {
        tilemapPaint.ClearTilemap();
        roomSystem.ClearContent();
        roomsDictionary.Clear();
    }

    /// <summary>
    /// ������� ������
    /// </summary>
    public void CreateRooms()
    {
        var roomsList = MainAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition,
            new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        // �����, �� ���������� �������� ��� ��������� ������
        var floor = useRandomWalk
            ? CreateRandomRooms(roomsList)
            : CreateRectangularRooms(roomsList);

        // ���������
        if (floor.Count > 0)
        {
            var roomCenterPositionsList = roomsList.Select(room => (Vector2Int)Vector3Int.RoundToInt(room.center)).ToList();

            var corridors = ConnectRooms(roomCenterPositionsList);

            floor.UnionWith(corridors);

            // ����� ������ �� ���� ������ ��������� ������
            tilemapPaint.GenerateFloorTiles(floor);
            WallPaint.GenerateWalls(floor, tilemapPaint);
            tilemapPaint.GenerateFloorDecorationTiles(floor);

        }
        else
        {
            throw new Exception("Room generation error!");
        }

        roomSystem.CreateContent(roomsDictionary);
    }

    /// <summary>
    /// ������� ������ ���������� �����
    /// </summary>
    /// <param name="roomsList"> ������ ����� </param>
    /// <returns> ������ hash set </returns>
    private HashSet<Vector2Int> CreateRectangularRooms(IEnumerable<BoundsInt> roomsList)
    {
        var floor = new HashSet<Vector2Int>();

        foreach (var roomBounds in roomsList)
        {
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloorWithOffset = new HashSet<Vector2Int>(); //  ��������� ������ � ��������

            for (int column = roomOffset; column < roomBounds.size.x - roomOffset; column++)
            {
                for (int row = roomOffset; row < roomBounds.size.y - roomOffset; row++)
                {
                    var position = (Vector2Int)roomBounds.min + new Vector2Int(column, row);
                    floor.Add(position);
                    roomFloorWithOffset.Add(position); // ��������� ������ �� ������ �� ������������� �������
                }
            }

            roomsDictionary.Add(roomCenter, roomFloorWithOffset); // ���������� �������� ������ � ������� ��������
        }

        return floor;
    }

    /// <summary>
    /// ������� ������ ������� �����
    /// </summary>
    /// <param name="roomsList"> ������ ����� </param>
    /// <returns> ������ hash set </returns>
    private HashSet<Vector2Int> CreateRandomRooms(IEnumerable<BoundsInt> roomsList)
    {
        var floor = new HashSet<Vector2Int>();

        foreach (var roomBounds in roomsList)
        {
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = GetRandomWalk(randomWalkData, roomCenter);
            var roomFloorWithOffset = new HashSet<Vector2Int>(); //  ��������� ������ � ��������

            // ������������ �����
            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + roomOffset) && position.x <= (roomBounds.xMax - roomOffset) &&
                    position.y >= (roomBounds.yMin - roomOffset) && position.y <= (roomBounds.yMax - roomOffset))
                {
                    floor.Add(position);
                    roomFloorWithOffset.Add(position); // ��������� ������ �� ������ �� ������������� �������
                }
            }

            roomsDictionary.Add(roomCenter, roomFloorWithOffset); // ���������� �������� ������ � ������� ��������
        }

        return floor;
    }

    /// <summary>
    /// ������� ������� �� ����� �������
    /// </summary>
    /// <param name="currentRoomCenter"> ��������� ������� �������� </param>
    /// <param name="destination"> ������ ��������� �������� </param>
    /// <returns> ���� �����, � ���� ���������� ������� </returns>
    private static IEnumerable<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        var position = currentRoomCenter;
        var corridor = new HashSet<Vector2Int> { position };

        while (position.y != destination.y)
        {
            if (destination.y > position.y) // go up
                position += Vector2Int.up;
            else if (destination.y < position.y) // go down
                position += Vector2Int.down;

            corridor.Add(position);
        }

        while (position.x != destination.x)
        {
            if (destination.x > position.x) // go right
                position += Vector2Int.right;
            else if (destination.x < position.x) // go left
                position += Vector2Int.left;

            corridor.Add(position);
        }

        if (wideCoridor == true)
        {
            var newCorridor = IncreaseCorridorBrush3by3(corridor);
            return newCorridor;
        }
        else
            return corridor;
    }

    public static HashSet<Vector2Int> IncreaseCorridorBrush3by3(HashSet<Vector2Int> corridor)
    {
        HashSet<Vector2Int> newCorridor = new HashSet<Vector2Int>();

        foreach (Vector2Int position in corridor)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    newCorridor.Add(position + new Vector2Int(x, y));
                }
            }
        }

        return newCorridor;
    }

    /// <summary>
    /// �'���� ������, ��������� ��������
    /// </summary>
    /// <param name="roomCenterPositionsList"> ������ ����������� ������� ������ </param>
    /// <returns> �������� hash set </returns>
    private static IEnumerable<Vector2Int> ConnectRooms(IList<Vector2Int> roomCenterPositionsList)
    {
        var corridors = new HashSet<Vector2Int>();

        var currentRoomCenter = roomCenterPositionsList[Random.Range(0, roomCenterPositionsList.Count)]; // ���������� �������� ����� ����� �� ��������� �����

        roomCenterPositionsList.Remove(currentRoomCenter);

        while (roomCenterPositionsList.Count > 0)
        {
            var closestPoint = FindClosestPoint(currentRoomCenter, roomCenterPositionsList);
            roomCenterPositionsList.Remove(closestPoint);
            var newCorridor = CreateCorridor(currentRoomCenter, closestPoint);

            currentRoomCenter = closestPoint;
            corridors.UnionWith(newCorridor);
        }

        return corridors;
    }

    /// <summary>
    /// ��������� ��������� ����� �� ������ �����
    /// </summary>
    /// <param name="currentRoomCenter"> ��������� ����� </param>
    /// <param name="roomCenterPositionsList"> ������ ����������� ������� ������ </param>
    /// <returns>  �������� Vector2Int, ��� ��������� ���������� ������ </returns>
    private static Vector2Int FindClosestPoint(Vector2Int currentRoomCenter, IEnumerable<Vector2Int> roomCenterPositionsList)
    {
        var closestPoint = Vector2Int.zero;
        var distance = float.MaxValue;

        foreach (var position in roomCenterPositionsList)
        {
            var currentDistance = Vector2.Distance(position, currentRoomCenter);

            if (currentDistance < distance)
            {
                distance = currentDistance;
                closestPoint = position;
            }
        }

        return closestPoint;
    }

    /// <summary>
    /// ������ �������� ��������� ������
    /// </summary>
    /// <param name="simpleRandomWalkInputData"> ������������ ����� ��������� ������ </param>
    /// <param name="position"> ��������� ������� </param>
    /// <returns> ���� �����, �� �������� �� ��������� ������ ��� ����� </returns>
    private static IEnumerable<Vector2Int> GetRandomWalk(RandomWalkData randomWalkInputData, Vector2Int position)
    {
        var currentPosition = position;
        var floorPositions = new HashSet<Vector2Int>();

        for (int i = 0; i < randomWalkInputData.iterations; i++)
        {
            var path = MainAlgorithms.RandomWalk(currentPosition, randomWalkInputData.steps);
            floorPositions.UnionWith(path);

            if (randomWalkInputData.startRandomlyEachIteration)
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count)); // ���������� ������� ������� �� ��������� ������� �� �����
        }

        return floorPositions;
    }

}