using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] GameObject RoomPrefab;
    [SerializeField] GameObject PlayerPrefab;

    [SerializeField] private int MaxRooms = 7;
    [SerializeField] private int MinRooms = 3;

    int RoomWidth = 20;
    int RoomHeight = 12;

    [SerializeField] int GridSizeX = 10;
    [SerializeField] int GridSizeY = 10;

    private List<GameObject> RoomObjects = new List<GameObject>();
    private Queue<Vector2Int> RoomQueue = new Queue<Vector2Int>();
    private int[,] RoomGrid;
    private int RoomCount;
    private bool GenerationComplete = false;

    private Vector2Int StartRoomIndex;
    private Vector2Int currentRoomIndex; 
    private Room currentRoomScript;

    private void Start()
    {
        if (RoomPrefab == null)
        {
            //Debug.LogError("RoomPrefab íå íàçíà÷åí â èíñïåêòîðå!");
            return;
        }

        RoomGrid = new int[GridSizeX, GridSizeY];
        RoomQueue = new Queue<Vector2Int>();

        StartRoomIndex = new Vector2Int(GridSizeX / 2, GridSizeY / 2);

        StarRoomGenerationFromRoom(StartRoomIndex);
    }

    private void Update()
    {
        if (RoomCount > 0 && RoomCount < MaxRooms && !GenerationComplete)
        {
            if (RoomQueue.Count == 0)
            {
                GenerationComplete = true;
                SpawnPlayer();
                Debug.Log($"Generation complete (Queue empty), {RoomCount} rooms created");
                return;
            }

            Vector2Int roomIndex = RoomQueue.Dequeue();
            int GridX = roomIndex.x;
            int GridY = roomIndex.y;

            TryGenerateRoom(new Vector2Int(GridX - 1, GridY));
            TryGenerateRoom(new Vector2Int(GridX + 1, GridY));
            TryGenerateRoom(new Vector2Int(GridX, GridY + 1));
            TryGenerateRoom(new Vector2Int(GridX, GridY - 1));
        }
        else if (!GenerationComplete)
        {
            //Debug.Log($"Generation complete, {RoomCount} rooms created");
            GenerationComplete = true;
            currentRoomIndex = StartRoomIndex;
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        if (PlayerPrefab == null)
        {
            //Debug.LogWarning("PlayerPrefab íå íàçíà÷åí! Èãðîê íå ñîçäàí.");
            return;
        }

        Vector3 spawnPosition = GetPositionFromGridIndex(StartRoomIndex);

        GameObject playerInstance = Instantiate(PlayerPrefab, spawnPosition, Quaternion.identity);
        playerInstance.name = "Player";

        //Debug.Log($"Èãðîê ñîçäàí â êîìíàòå: {StartRoomIndex}, Ïîçèöèÿ: {spawnPosition}");
    }

    private void StarRoomGenerationFromRoom(Vector2Int RoomIndex)
    {
        RoomQueue.Enqueue(RoomIndex);
        int x = RoomIndex.x;
        int y = RoomIndex.y;
        RoomGrid[x, y] = 1;
        RoomCount++;

        var InitialRoom = Instantiate(RoomPrefab, GetPositionFromGridIndex(RoomIndex), Quaternion.identity);
        InitialRoom.name = $"Room-{RoomCount}";

        Room roomScript = InitialRoom.GetComponent<Room>();
        if (roomScript != null)
            roomScript.RoomIndex = RoomIndex;

        RoomObjects.Add(InitialRoom);
    }

    private bool TryGenerateRoom(Vector2Int RoomIndex)
    {
        int x = RoomIndex.x;
        int y = RoomIndex.y;

        if (x < 0 || x >= GridSizeX || y < 0 || y >= GridSizeY)
            return false;

        if (RoomCount >= MaxRooms)
            return false;

        if (RoomGrid[x, y] != 0)
            return false;

        if (Random.value < 0.5f && RoomIndex != Vector2Int.zero)
            return false;

        if (CountAdjecentRooms(RoomIndex) > 1)
            return false;

        RoomQueue.Enqueue(RoomIndex);
        RoomGrid[x, y] = 1;
        RoomCount++;

        var NewRoom = Instantiate(RoomPrefab, GetPositionFromGridIndex(RoomIndex), Quaternion.identity);
        NewRoom.name = $"Room-{RoomCount}";

        Room newRoomScript = NewRoom.GetComponent<Room>();
        if (newRoomScript != null)
            newRoomScript.RoomIndex = RoomIndex;

        RoomObjects.Add(NewRoom);

        OpenDoor(NewRoom, x, y);

        return true;
    }

    void OpenDoor(GameObject Room, int x, int y)
    {
        Room NewRoomScript = Room.GetComponent<Room>();
        if (NewRoomScript == null) return;

        Room LeftScript = GetRoomScriptAt(new Vector2Int(x - 1, y));
        Room RightScript = GetRoomScriptAt(new Vector2Int(x + 1, y));
        Room TopScript = GetRoomScriptAt(new Vector2Int(x, y + 1));
        Room BottomScript = GetRoomScriptAt(new Vector2Int(x, y - 1));

        if (x > 0 && RoomGrid[x - 1, y] != 0 && LeftScript != null)
        {
            NewRoomScript.OpenDoor(Vector2Int.left);
            LeftScript.OpenDoor(Vector2Int.right);
        }
        if (x < GridSizeX - 1 && RoomGrid[x + 1, y] != 0 && RightScript != null)
        {
            NewRoomScript.OpenDoor(Vector2Int.right);
            RightScript.OpenDoor(Vector2Int.left);
        }
        if (y > 0 && RoomGrid[x, y - 1] != 0 && BottomScript != null)
        {
            NewRoomScript.OpenDoor(Vector2Int.down);
            BottomScript.OpenDoor(Vector2Int.up);
        }
        if (y < GridSizeY - 1 && RoomGrid[x, y + 1] != 0 && TopScript != null)
        {
            NewRoomScript.OpenDoor(Vector2Int.up);
            TopScript.OpenDoor(Vector2Int.down);
        }
    }

    Room GetRoomScriptAt(Vector2Int Index)
    {
        GameObject RoomObject = RoomObjects.Find(r => {
            Room rComp = r.GetComponent<Room>();
            return rComp != null && rComp.RoomIndex == Index;
        });

        if (RoomObject != null)
            return RoomObject.GetComponent<Room>();
        return null;
    }

    private int CountAdjecentRooms(Vector2Int RoomIndex)
    {
        int x = RoomIndex.x;
        int y = RoomIndex.y;
        int count = 0;

        if (x > 0 && RoomGrid[x - 1, y] != 0) count++;
        if (x < GridSizeX - 1 && RoomGrid[x + 1, y] != 0) count++;
        if (y > 0 && RoomGrid[x, y - 1] != 0) count++;
        if (y < GridSizeY - 1 && RoomGrid[x, y + 1] != 0) count++;

        return count;
    }

    private Vector3 GetPositionFromGridIndex(Vector2Int GridIndex)
    {
        int GridX = GridIndex.x;
        int GridY = GridIndex.y;
        return new Vector3(RoomWidth * (GridX - GridSizeX / 2), RoomHeight * (GridY - GridSizeY / 2), 0);
    }

    private void OnDrawGizmos()
    {
        Color gizmoColor = new Color(0, 1, 1, 0.5f);
        Gizmos.color = gizmoColor;

        for (int x = 0; x < GridSizeX; x++)
        {
            for (int y = 0; y < GridSizeY; y++)
            {
                Vector3 position = GetPositionFromGridIndex(new Vector2Int(x, y));
                Gizmos.DrawWireCube(position, new Vector3(RoomWidth, RoomHeight, 1));
            }
        }
    }
    public void MovePlayerToRoom(Vector2Int direction)
    {
        Vector2Int nextRoomIndex = currentRoomIndex + direction;

        // Ищем скрипт комнаты, в которую переходим
        Room nextRoom = GetRoomScriptAt(nextRoomIndex);

        if (nextRoom != null)
        {
            currentRoomIndex = nextRoomIndex;

            // Находим игрока
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            // Находим дверь в НОВОЙ комнате, которая ведет В ОБРАТНУЮ сторону
            // Если мы шли ВВЕРХ (0,1), то в новой комнате нам нужна НИЖНЯЯ дверь (0,-1)
            Vector2Int oppositeDirection = direction * -1;

            // Получаем точку выхода из противоположной двери новой комнаты
            Transform spawnPoint = nextRoom.GetExitPoint(oppositeDirection);

            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.position;

                // Если у тебя готова камера, тут же говорим ей:
                // Camera.main.GetComponent<CameraController>().MoveToRoom(nextRoom.transform.position);
            }
        }
        Camera.main.transform.position = new Vector3(nextRoom.transform.position.x, nextRoom.transform.position.y, -10f);
    }
}