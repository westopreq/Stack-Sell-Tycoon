using UnityEngine;

public class GameManager : MonoBehaviour
{
    public RoomBuilder roomBuilder;

    void Start()
    {
        // Строим комнату, начиная с координат (0, 0, 0)
        roomBuilder.BuildRoom(new Vector3(0, 0, 0));
    }
}