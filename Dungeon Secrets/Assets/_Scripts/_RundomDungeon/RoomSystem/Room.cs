using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Room : MonoBehaviour
{
    #region ПОЗИЦІОНУВАННЯ МАТЕРІАЛІВ ТА КОНТЕНТУ В КІМНАТІ
    protected Vector2Int roomCenterPosition;
    private HashSet<Vector2Int> roomFloorsPositions;
    protected List<Vector2Int> availableFloorsPositions;

    [SerializeField] protected RoomData roomData;
    #endregion

    /// <summary>
    /// Метод ініціалізації, який перевизначає конкретне приміщення
    /// </summary>
    /// <param name="centerPosition"> розташування центру кімнати </param>
    /// <param name="floorsPositions"> набір положень підлог для поточної кімнати </param>
    public virtual void Create(Vector2Int centerPosition, HashSet<Vector2Int> floorsPositions) { }

    /// <summary>
    /// Загальний метод ініціалізації кімнати
    /// Встановлює центр кімнати та її положення на підлозі
    /// </summary>
    /// <param name="centerPosition"> положення центру кімнати </param>
    /// <param name="floorsPositions"> набір положень підлог для поточної кімнати </param>
    protected void Init(Vector2Int centerPosition, HashSet<Vector2Int> floorsPositions)
    {
        transform.position = (Vector2)centerPosition;

        roomCenterPosition = centerPosition;
        roomFloorsPositions = floorsPositions;

        availableFloorsPositions = roomFloorsPositions.ToList();
        availableFloorsPositions.Remove(centerPosition);

        //// створення пішохідних доріжкок від центру кімнати в чотирьох напрямках
        //var walkablePaths = new List<Vector2Int>();
        //walkablePaths.AddRange(MainAlgorithms.NStepWalk(roomCenterPosition, 10, MainAlgorithms.Direction2D.FourDirectionsList[0]));
        //walkablePaths.AddRange(MainAlgorithms.NStepWalk(roomCenterPosition, 10, MainAlgorithms.Direction2D.FourDirectionsList[1]));
        //walkablePaths.AddRange(MainAlgorithms.NStepWalk(roomCenterPosition, 10, MainAlgorithms.Direction2D.FourDirectionsList[2]));
        //walkablePaths.AddRange(MainAlgorithms.NStepWalk(roomCenterPosition, 10, MainAlgorithms.Direction2D.FourDirectionsList[3]));

        //foreach (var position in walkablePaths)
        //{
        //    availableFloorsPositions.Remove(position);
        //}
    }

    #region РОЗТАШУВАННЯ ПРЕДМЕТІВ ІНТЕР'ЄРУ
    protected void SpawnInteriorObjectNearWall(InteriorObject interiorObject) =>
        InteriorGenerator.SpawnInteriorObjectNearWall(interiorObject, availableFloorsPositions, gameObject.transform);

    protected void SpawnInteriorObjectRandomly(InteriorObject interiorObject) =>
        InteriorGenerator.SpawnInteriorObjectRandomly(interiorObject, availableFloorsPositions, gameObject.transform);
    #endregion

}