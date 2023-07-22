using System.Collections.Generic;
using UnityEngine;
using static MainAlgorithms;

public class WallPaint : MonoBehaviour
{
    /// <summary>
    /// ������� ����
    /// </summary>
    /// <param name="floorPositions"> ���� �������� ������ </param>
    /// <param name="tilemapPaint"> �����, ��� ���� ����������� ��� ��������� �������� ������ </param>
    public static void GenerateWalls(HashSet<Vector2Int> floorPositions, TilemapPaint tilemapPaint)
    {
        var basicWallPositions = FindWallsInDirections(floorPositions, Direction2D.FourDirectionsList);

        var cornerWallPositions = FindWallsInDirections(floorPositions, Direction2D.diagonalDirectionsList); // ���������� ��������� ��� ������� ���

        CreateBasicWall(tilemapPaint, basicWallPositions, floorPositions);
        CreateCornerWalls(tilemapPaint, cornerWallPositions, floorPositions);

    }

    /// <summary>
    /// ��������� ���� � �������� ������ ��������
    /// </summary>
    /// <param name="floorPositions"> ���� �������� ������ </param>
    /// <param name="directionsList"> ������ �������� </param>
    /// <returns> ���� �������� �� ��� </returns>
    private static HashSet<Vector2Int> FindWallsInDirections(ICollection<Vector2Int> floorPositions, IReadOnlyCollection<Vector2Int> directionsList)
    {
        var wallPositions = new HashSet<Vector2Int>();

        foreach (var position in floorPositions)
        {
            foreach (var direction in directionsList)
            {
                var neighbourPosition = position + direction;

                if (!floorPositions.Contains(neighbourPosition)) //  ���� � ������ ������� ������ ���� ������� �������, �� ������, �� �� ������� � �����
                    wallPositions.Add(neighbourPosition);
            }
        }

        return wallPositions;
    }

    private static void CreateBasicWall(TilemapPaint tilemapPaint, HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in basicWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Direction2D.FourDirectionsList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                {
                    neighboursBinaryType += "1";
                }
                else
                {
                    neighboursBinaryType += "0";
                }
            }
            tilemapPaint.PaintSingleBasicWall(position, neighboursBinaryType);
        }
    }

    private static void CreateCornerWalls(TilemapPaint tilemapPaint, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in cornerWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Direction2D.EightDirectionsList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                {
                    neighboursBinaryType += "1";
                }
                else
                {
                    neighboursBinaryType += "0";
                }
            }
            tilemapPaint.PaintSingleCornerWall(position, neighboursBinaryType);
        }
    }
}
