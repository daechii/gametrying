using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Проход (триггер Door) — только если в графе есть сосед")]
    [SerializeField] GameObject TopDoor;
    [SerializeField] GameObject BottomDoor;
    [SerializeField] GameObject RightDoor;
    [SerializeField] GameObject LeftDoor;

    [Header("Закрытый вид — только на тех же сторонах, где есть сосед (бой / замок)")]
    [Tooltip("Не на всех 4 сторонах: без соседа объект выключен. С соседом — показывается вместо прохода, пока SetExitsLocked(true).")]
    [SerializeField] GameObject TopDoorClosed;
    [SerializeField] GameObject BottomDoorClosed;
    [SerializeField] GameObject RightDoorClosed;
    [SerializeField] GameObject LeftDoorClosed;

    [SerializeField] Door topDoorScript;
    [SerializeField] Door bottomDoorScript;
    [SerializeField] Door leftDoorScript;
    [SerializeField] Door rightDoorScript;

    private bool _connLeft;
    private bool _connRight;
    private bool _connDown;
    private bool _connUp;
    private bool _exitsLocked;

    public Vector2Int RoomIndex { get; set; }
    public bool IsStartRoom { get; private set; }

    public void MarkAsStartRoom() => IsStartRoom = true;

    private void Awake()
    {
        _connLeft = _connRight = _connDown = _connUp = false;
        _exitsLocked = false;
        HideAllDoorObjects();
    }

    private void HideAllDoorObjects()
    {
        SetRootActive(TopDoor, false);
        SetRootActive(BottomDoor, false);
        SetRootActive(LeftDoor, false);
        SetRootActive(RightDoor, false);
        SetRootActive(TopDoorClosed, false);
        SetRootActive(BottomDoorClosed, false);
        SetRootActive(LeftDoorClosed, false);
        SetRootActive(RightDoorClosed, false);
    }

    private static void SetRootActive(GameObject root, bool active)
    {
        if (root != null)
            root.SetActive(active);
    }

    /// <summary>Замок на выходах только там, где реально есть соседняя комната.</summary>
    public void SetExitsLocked(bool locked)
    {
        _exitsLocked = locked;
        RefreshAllExits();
    }

    /// <summary>Включает проходы только со сторон, где в сетке есть сосед. Закрытый вид — только на этих же сторонах.</summary>
    public void SetDoorsFromConnections(bool openLeft, bool openRight, bool openDown, bool openUp)
    {
        _connLeft = openLeft;
        _connRight = openRight;
        _connDown = openDown;
        _connUp = openUp;
        RefreshAllExits();
    }

    private void RefreshAllExits()
    {
        ApplyExitSide(_connLeft, LeftDoor, LeftDoorClosed);
        ApplyExitSide(_connRight, RightDoor, RightDoorClosed);
        ApplyExitSide(_connDown, BottomDoor, BottomDoorClosed);
        ApplyExitSide(_connUp, TopDoor, TopDoorClosed);
    }

    /// <summary>Нет соседа — оба объекта выкл. Есть сосед — проход или закрытый вид при SetExitsLocked(true).</summary>
    private void ApplyExitSide(bool hasNeighbor, GameObject passage, GameObject closedOverlay)
    {
        if (!hasNeighbor)
        {
            SetRootActive(passage, false);
            SetRootActive(closedOverlay, false);
            return;
        }

        if (closedOverlay != null)
        {
            if (_exitsLocked)
            {
                SetRootActive(passage, false);
                SetRootActive(closedOverlay, true);
            }
            else
            {
                SetRootActive(passage, true);
                SetRootActive(closedOverlay, false);
            }
        }
        else
            SetRootActive(passage, !_exitsLocked);
    }

    public Transform GetExitPoint(Vector2Int dir)
    {
        if (dir == Vector2Int.up) return topDoorScript != null ? topDoorScript.exitPoint : null;
        if (dir == Vector2Int.down) return bottomDoorScript != null ? bottomDoorScript.exitPoint : null;
        if (dir == Vector2Int.left) return leftDoorScript != null ? leftDoorScript.exitPoint : null;
        if (dir == Vector2Int.right) return rightDoorScript != null ? rightDoorScript.exitPoint : null;
        return null;
    }

    public void OnPlayerEnter()
    {
        if (IsStartRoom)
            return;

        if (TryGetComponent<EnemySpawnPoint>(out EnemySpawnPoint battleManager))
            battleManager.StartBattle();
    }
}
