using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// This script is responsible for placing tiles on tilemap.
/// Add this component to Your Grid object.
/// </summary>
[RequireComponent(typeof(Grid))]
public class TilemapPaint : MonoBehaviour
{
    #region INSPECTOR FIELDS
    [Header("Tilemaps")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap wallTilemapCollider;
    [SerializeField] private Tilemap floorDecorationsTilemap;

    [SerializeField] public TileBase floorTile, wallTop, wallSideRight, wallSiderLeft, wallBottom, wallFull,
    wallInnerCornerDownLeft, wallInnerCornerDownRight, wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft;

    [SerializeField] private List<TileBase> floorDecorationsTiles = new List<TileBase>();
    #endregion

    /// <summary>
    ///  Створює плитку для підлоги
    /// </summary>
    /// <param name="floorPositions"> позиції для створення тайлів підлги</param>
    public void GenerateFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        ClearTilemap(); // clear tilemap to prevent overwriting

        foreach (var position in floorPositions)
        {
            PaintSingleTile(floorTilemap, floorTile, position);
        }
    }

    ///// <summary>
    ///// Generates floor decoration tiles (bones and vines on floor)
    ///// </summary>
    ///// <param name="floorPositions"> positions considered for spawning floor decoration tiles on them </param>
    public void GenerateFloorDecorationTiles(IEnumerable<Vector2Int> floorPositions)
    {
        foreach (var position in floorPositions)
        {
            if (UnityEngine.Random.value >= 0.95f)
                PaintSingleTile(floorDecorationsTilemap, floorDecorationsTiles[UnityEngine.Random.Range(0, floorDecorationsTiles.Count)], position);
        }
    }

    /// <summary>
    /// Generates single tile
    /// </summary>
    /// <param name="tilemap"> tilemap, on which tile will be spawned </param>
    /// <param name="tile"> tile to generate </param>
    /// <param name="position"> position to spawn tile </param>
    private static void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    /// <summary>
    /// Clears all tilemaps
    /// </summary>
    public void ClearTilemap()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        floorDecorationsTilemap.ClearAllTiles();
    }

    internal void PaintSingleBasicWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if (WallTypesHelper.wallTop.Contains(typeAsInt))
        {
            tile = wallTop;
        }
        else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
        {
            tile = wallSideRight;
        }
        else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
        {
            tile = wallSiderLeft;
        }
        else if (WallTypesHelper.wallBottm.Contains(typeAsInt))
        {
            tile = wallBottom;
        }
        else if (WallTypesHelper.wallFull.Contains(typeAsInt))
        {
            tile = wallFull;
        }

        if (tile != null)
            PaintSingleTile(wallTilemap, tile, position);
    }

    internal void PaintSingleCornerWall(Vector2Int position, string binaryType)
    {
        int typeASInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeASInt))
        {
            tile = wallInnerCornerDownLeft;
        }
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeASInt))
        {
            tile = wallInnerCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeASInt))
        {
            tile = wallDiagonalCornerDownLeft;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeASInt))
        {
            tile = wallDiagonalCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeASInt))
        {
            tile = wallDiagonalCornerUpRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeASInt))
        {
            tile = wallDiagonalCornerUpLeft;
        }
        else if (WallTypesHelper.wallFullEightDirections.Contains(typeASInt))
        {
            tile = wallFull;
        }
        else if (WallTypesHelper.wallBottmEightDirections.Contains(typeASInt))
        {
            tile = wallBottom;
        }

        if (tile != null)
            PaintSingleTile(wallTilemap, tile, position);
    }
}