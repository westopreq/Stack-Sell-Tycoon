using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    private HashSet<Vector3> roomPositions = new HashSet<Vector3>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void RegisterRoom(Vector3 position)
    {
        roomPositions.Add(position);
    }

    public bool IsRoomAtPosition(Vector3 position)
    {
        return roomPositions.Contains(position);
    }

    public Vector3 GetNearestRoom(Vector3 position)
    {
        float minDistance = float.MaxValue;
        Vector3 nearestRoom = Vector3.zero;

        foreach (var roomPos in roomPositions)
        {
            float distance = Vector3.Distance(position, roomPos);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestRoom = roomPos;
            }
        }

        return nearestRoom;
    }
}
