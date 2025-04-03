using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RoomBuilder : MonoBehaviour
{
    public GameObject roomPrefab; // Первая комната с дверью
    public GameObject previewPrefab; // Превью и будущие комнаты без дверей

    public float wallRemovalThreshold = 0.1f; // Публичная переменная для порога удаления стен
    public float xLimit = -0.4752f; // Публичная переменная для ограничения по оси X

    private Vector3 floorSize;
    private GameObject previewInstance;
    private Vector3 lastPreviewPosition;
    private float roomBaseHeight;
    private bool isFirstRoom = true;

    private List<GameObject> builtRooms = new List<GameObject>(); // Храним все комнаты
    private GameObject selectedWall = null; // Стена, на которую наведена мышь

    private void Start()
    {
        floorSize = previewPrefab.GetComponentInChildren<Renderer>().bounds.size;
        roomBaseHeight = previewPrefab.transform.position.y;
    }

    public void BuildRoom(Vector3 position)
    {
        position.y = roomBaseHeight;

        if (isFirstRoom)
        {
            GameObject firstRoom = Instantiate(roomPrefab, position, Quaternion.identity);
            builtRooms.Add(firstRoom);
            isFirstRoom = false;
        }
        else
        {
            GameObject newRoom = Instantiate(previewPrefab, position, Quaternion.identity);
            builtRooms.Add(newRoom);
            RemoveConnectingWalls(newRoom);
        }
    }

    private void Update()
    {
        Vector3? buildableZone = GetMouseBuildPosition();

        if (buildableZone.HasValue)
        {
            Vector3 fixedPosition = GetSnappedPosition(buildableZone.Value);

            // Проверка на допустимость позиции по оси X
            if (fixedPosition.x < xLimit)
            {
                // Если позиция по оси X находится внутри запрещенной области, то не показываем превью
                if (previewInstance != null)
                {
                    Destroy(previewInstance);
                    previewInstance = null;
                }
                return;
            }

            // Проверка, не пересекает ли место существующие полы
            if (IsPositionOnExistingFloor(fixedPosition))
            {
                if (previewInstance != null)
                {
                    Destroy(previewInstance);
                    previewInstance = null;
                }
                return;
            }

            if (previewInstance == null)
            {
                previewInstance = Instantiate(previewPrefab, fixedPosition, Quaternion.identity);
                DisableColliders(previewInstance);
            }
            else
            {
                previewInstance.transform.position = fixedPosition;
            }

            lastPreviewPosition = previewInstance.transform.position;
        }
        else if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
        }

        if (Input.GetMouseButtonDown(0) && buildableZone.HasValue)
        {
            BuildRoom(lastPreviewPosition);
        }
    }

    private void DisableColliders(GameObject obj)
    {
        foreach (var col in obj.GetComponentsInChildren<Collider>())
            col.enabled = false;
    }

    private Vector3? GetMouseBuildPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                selectedWall = hit.collider.gameObject;  // Запоминаем выбранную стену

                GameObject hitRoom = FindParentRoom(hit.collider.gameObject);
                if (hitRoom == null) return null;

                Vector3 roomCenter = hitRoom.transform.position;
                Vector3 buildDirection = (hit.point - roomCenter).normalized;
                buildDirection.y = 0; // Убираем смещение по высоте

                // Рассчитываем направление для строительства, игнорируя точку на стене
                Vector3 relativeDirection = hit.point - roomCenter;

                // Выбираем, по какой оси будет строиться новая комната (X или Z)
                Vector3 buildPosition = hitRoom.transform.position;

                if (Mathf.Abs(relativeDirection.x) > Mathf.Abs(relativeDirection.z))
                {
                    // Строим вдоль оси X
                    buildPosition.x += Mathf.Sign(relativeDirection.x) * floorSize.x;
                }
                else
                {
                    // Строим вдоль оси Z
                    buildPosition.z += Mathf.Sign(relativeDirection.z) * floorSize.z;
                }

                buildPosition.y = roomBaseHeight;

                return buildPosition;
            }
        }

        return null;
    }

    private GameObject FindParentRoom(GameObject wall)
    {
        foreach (var room in builtRooms)
        {
            if (wall.transform.IsChildOf(room.transform))
                return room;
        }
        return null;
    }

    private void RemoveConnectingWalls(GameObject newRoom)
    {
        List<GameObject> wallsToRemove = new List<GameObject>();

        // Проходим по всем построенным комнатам (включая первую)
        foreach (var existingRoom in builtRooms)
        {
            if (existingRoom == newRoom) continue; // Пропускаем текущую новую комнату

            // Проходим по всем стенам новой комнаты
            foreach (Transform newWall in newRoom.transform)
            {
                if (!newWall.CompareTag("Wall")) continue; // Проверяем только стены

                // Проходим по всем стенам уже построенных комнат
                foreach (Transform existingWall in existingRoom.transform)
                {
                    if (!existingWall.CompareTag("Wall")) continue; // Проверяем только стены

                    // Сравниваем расстояние между центрами стен
                    float distance = Vector3.Distance(newWall.position, existingWall.position);
                    if (distance < wallRemovalThreshold)
                    {
                        wallsToRemove.Add(newWall.gameObject);  // Добавляем стену для удаления
                        wallsToRemove.Add(existingWall.gameObject);  // Добавляем стену для удаления
                    }
                }
            }
        }

        // Удаляем найденные стены
        foreach (var wall in wallsToRemove)
        {
            if (wall != null) // Проверяем, что стена все еще существует
            {
                // Задержка, чтобы дать время на выполнение других операций
                StartCoroutine(DestroyAfterFrame(wall)); 
            }
        }
    }
    
    // Корутин для задержки удаления
    private IEnumerator DestroyAfterFrame(GameObject wall)
    {
        yield return null; // Даем время на завершение всех операций в этом кадре
        Destroy(wall); // Удаляем объект после кадра
    }

    private bool IsPositionOnExistingFloor(Vector3 position)
    {
        // Проверяем, не находится ли позиция на уже существующем поле (поле каждой комнаты)
        foreach (var room in builtRooms)
        {
            Vector3 roomFloorPosition = room.transform.position;
            float roomFloorHeight = roomFloorPosition.y;

            // Проверяем, совпадает ли Y координата (высота) и позиция по X и Z
            if (Mathf.Abs(position.y - roomFloorHeight) < 0.1f &&
                Mathf.Abs(position.x - roomFloorPosition.x) < floorSize.x &&
                Mathf.Abs(position.z - roomFloorPosition.z) < floorSize.z)
            {
                return true; // Позиция пересекается с полом другой комнаты
            }
        }

        return false; // Позиция не пересекается с полами
    }

    private Vector3 GetSnappedPosition(Vector3 targetPosition)
    {
        float xSnap = Mathf.Round(targetPosition.x / floorSize.x) * floorSize.x;
        float zSnap = Mathf.Round(targetPosition.z / floorSize.z) * floorSize.z;
        return new Vector3(xSnap, roomBaseHeight, zSnap);
    }
}
