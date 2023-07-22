using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Room : MonoBehaviour
{
    #region ����ֲ�������� ����в�˲� �� �������� � ʲ���Ҳ
    protected Vector2Int roomCenterPosition;
    private HashSet<Vector2Int> roomFloorsPositions;
    protected List<Vector2Int> availableFloorsPositions;

    [SerializeField] protected RoomData roomData;
    #endregion

    /// <summary>
    /// ����� �����������, ���� ����������� ��������� ���������
    /// </summary>
    /// <param name="centerPosition"> ������������ ������ ������ </param>
    /// <param name="floorsPositions"> ���� �������� ����� ��� ������� ������ </param>
    public virtual void Create(Vector2Int centerPosition, HashSet<Vector2Int> floorsPositions) { }

    /// <summary>
    /// ��������� ����� ����������� ������
    /// ���������� ����� ������ �� �� ��������� �� �����
    /// </summary>
    /// <param name="centerPosition"> ��������� ������ ������ </param>
    /// <param name="floorsPositions"> ���� �������� ����� ��� ������� ������ </param>
    protected void Init(Vector2Int centerPosition, HashSet<Vector2Int> floorsPositions)
    {
        transform.position = (Vector2)centerPosition;

        roomCenterPosition = centerPosition;
        roomFloorsPositions = floorsPositions;

        availableFloorsPositions = roomFloorsPositions.ToList();
        availableFloorsPositions.Remove(centerPosition);

        //// ��������� ��������� ������� �� ������ ������ � �������� ���������
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

    #region ������������ ������Ҳ� �����'���
    protected void SpawnInteriorObjectNearWall(InteriorObject interiorObject) =>
        InteriorGenerator.SpawnInteriorObjectNearWall(interiorObject, availableFloorsPositions, gameObject.transform);

    protected void SpawnInteriorObjectRandomly(InteriorObject interiorObject) =>
        InteriorGenerator.SpawnInteriorObjectRandomly(interiorObject, availableFloorsPositions, gameObject.transform);
    #endregion

}