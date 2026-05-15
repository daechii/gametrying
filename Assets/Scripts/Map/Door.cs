using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector2Int direction;
    public Transform exitPoint;

    private RoomManager roomManager;
    private Room room;

    private void Awake()
    {
        room = GetComponentInParent<Room>();
        roomManager = Object.FindFirstObjectByType<RoomManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (room == null)
            room = GetComponentInParent<Room>();
        if (roomManager == null)
            roomManager = Object.FindFirstObjectByType<RoomManager>();
        if (roomManager == null || room == null)
            return;

        roomManager.TryRequestRoomTransition(room, direction);
    }
}
