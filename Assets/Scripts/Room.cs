using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] GameObject TopDoor;
    [SerializeField] GameObject BottomDoor;
    [SerializeField] GameObject RightDoor;
    [SerializeField] GameObject LeftDoor;

    [SerializeField] Door topDoorScript;
    [SerializeField] Door bottomDoorScript;
    [SerializeField] Door leftDoorScript;
    [SerializeField] Door rightDoorScript;

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
    public Transform GetExitPoint(Vector2Int dir)
    {
        if (dir == Vector2Int.up) return topDoorScript.exitPoint;
        if (dir == Vector2Int.down) return bottomDoorScript.exitPoint;
        if (dir == Vector2Int.left) return leftDoorScript.exitPoint;
        if (dir == Vector2Int.right) return rightDoorScript.exitPoint;
        return null;
    }
}