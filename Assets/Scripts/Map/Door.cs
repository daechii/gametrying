using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;

public class Door : MonoBehaviour

{
    public Vector2Int direction;
    public Transform exitPoint;
    private RoomManager roomManager;
    private void Start()
    {
        roomManager = Object.FindFirstObjectByType<RoomManager>();
        Debug.Log($"Дверь запустилась");

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Нам нужно сказать RoomManager, что пора менять комнату
            // Передаем направление, в котором идет игрок
            roomManager.MovePlayerToRoom(direction);
            Debug.Log($"Триггер сработал");

        }
    }
}
