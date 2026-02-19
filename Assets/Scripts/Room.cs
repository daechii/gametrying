using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] GameObject TopDoor;
    [SerializeField] GameObject BottomDoor;
    [SerializeField] GameObject RightDoor;
    [SerializeField] GameObject LeftDoor;
    public Vector2Int RoomIndex { get; set; }

    public void OpenDoor(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
        {
            TopDoor.SetActive(true);
        }
        else if (direction == Vector2Int.down)
        {
            BottomDoor.SetActive(true);
        }
        else if (direction == Vector2Int.left)
        {
            LeftDoor.SetActive(true);
        }
        else if (direction == Vector2Int.right)
        {
            RightDoor.SetActive(true);
        }
    }
}