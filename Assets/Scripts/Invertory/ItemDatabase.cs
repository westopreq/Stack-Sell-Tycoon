using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    // Список всех префабов, который будет заполняться через инспектор
    public GameObject[] itemPrefabs;

    // Метод для получения префаба по названию предмета
    public GameObject GetItemPrefab(string itemName)
    {
        foreach (var prefab in itemPrefabs)
        {
            if (prefab.name == itemName)
            {
                return prefab;
            }
        }
        return null; // Если не найдено
    }
}
