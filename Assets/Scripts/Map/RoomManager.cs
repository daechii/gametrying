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
                if (RoomCount < MinRooms)
                {
                    RegrowFromExistingRooms();
                    return;
                }

                FinishGeneration($"Generation complete (Queue empty), {RoomCount} rooms created");
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
            FinishGeneration($"Generation complete, {RoomCount} rooms created");
        }
    }

    private void FinishGeneration(string logMessage)
    {
        GenerationComplete = true;
        currentRoomIndex = StartRoomIndex;
        SyncAllRoomDoorsToGrid();
        SpawnPlayer();
        Debug.Log(logMessage);
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

        if (PlayerStats._playerStats != null)
            PlayerStats._playerStats.BindPlayer(playerInstance);

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
        {
            roomScript.RoomIndex = RoomIndex;
            if (RoomIndex == StartRoomIndex)
                roomScript.MarkAsStartRoom();
        }

        if (RoomIndex == StartRoomIndex)
            DisableCombatInRoom(InitialRoom);

        RoomObjects.Add(InitialRoom);
    }

    static void DisableCombatInRoom(GameObject roomObject)
    {
        if (roomObject.TryGetComponent(out EnemySpawnPoint spawnPoint))
            spawnPoint.enabled = false;
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

        if (Random.value < 0.5f && RoomIndex != StartRoomIndex)
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

        return true;
    }

    private void RegrowFromExistingRooms()
    {
        foreach (GameObject roomObject in RoomObjects)
        {
            Room roomScript = roomObject.GetComponent<Room>();
            if (roomScript != null)
            {
                RoomQueue.Enqueue(roomScript.RoomIndex);
            }
        }
    }

    private void SyncAllRoomDoorsToGrid()
    {
        foreach (GameObject go in RoomObjects)
        {
            Room room = go.GetComponent<Room>();
            if (room == null)
                continue;

            Vector2Int idx = room.RoomIndex;
            int x = idx.x;
            int y = idx.y;

            bool openLeft = x > 0 && RoomGrid[x - 1, y] != 0;
            bool openRight = x < GridSizeX - 1 && RoomGrid[x + 1, y] != 0;
            bool openDown = y > 0 && RoomGrid[x, y - 1] != 0;
            bool openUp = y < GridSizeY - 1 && RoomGrid[x, y + 1] != 0;

            room.SetDoorsFromConnections(openLeft, openRight, openDown, openUp);
        }
    }

    private bool HasRoomAt(Vector2Int index)
    {
        if (index.x < 0 || index.x >= GridSizeX || index.y < 0 || index.y >= GridSizeY)
            return false;
        return RoomGrid[index.x, index.y] != 0;
    }

    private bool IsRoomBlockingExits(Vector2Int roomIndex)
    {
        Room room = GetRoomScriptAt(roomIndex);
        if (room == null)
            return false;
        if (room.TryGetComponent(out EnemySpawnPoint battle))
            return battle.AreExitsBlocked;
        return false;
    }

    /// <summary>Вызывается с двери: проверки как в Isaac (текущая комната, не в бою, есть сосед).</summary>
    public bool TryRequestRoomTransition(Room fromRoom, Vector2Int direction)
    {
        if (fromRoom == null)
            return false;
        if (fromRoom.RoomIndex != currentRoomIndex)
            return false;
        if (IsRoomBlockingExits(currentRoomIndex))
            return false;

        Vector2Int nextIndex = currentRoomIndex + direction;
        if (!HasRoomAt(nextIndex))
            return false;

        MovePlayerToRoom(direction);
        return true;
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

        for (int x = 0; x < GridSizeX; x++)
        {
            for (int y = 0; y < GridSizeY; y++)
            {
                Vector3 position = GetPositionFromGridIndex(new Vector2Int(x, y));
                Gizmos.DrawWireCube(position, new Vector3(RoomWidth, RoomHeight, 1));
            }
        }
    }
    private void MovePlayerToRoom(Vector2Int direction)
    {
        Vector2Int nextRoomIndex = currentRoomIndex + direction;
        Room nextRoom = GetRoomScriptAt(nextRoomIndex);

        if (nextRoom == null)
            return;

        currentRoomIndex = nextRoomIndex;
        currentRoomScript = nextRoom;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector2Int oppositeDirection = direction * -1;
        Transform spawnPoint = nextRoom.GetExitPoint(oppositeDirection);

        if (spawnPoint != null && player != null)
        {
            player.transform.position = spawnPoint.position;
        }

        nextRoom.OnPlayerEnter();

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(nextRoom.transform.position.x, nextRoom.transform.position.y, -10f);
        }
    }
}