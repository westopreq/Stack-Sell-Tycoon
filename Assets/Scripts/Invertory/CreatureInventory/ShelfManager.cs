using UnityEngine;
using System.Collections.Generic;

public class Shelf : MonoBehaviour
{
    public List<Transform> shelfSlots = new List<Transform>(); // Точки для размещения предметов
    public CreatureInventory creatureInventory; // Инвентарь полки

    private Dictionary<Item, GameObject> itemInstances = new Dictionary<Item, GameObject>();

    private void Start()
    {
        RefreshShelf();
    }

    // Обновляет содержимое полки, синхронизируя с инвентарём
    public void RefreshShelf()
    {
        Debug.Log("Обновление полки...");

        // Получаем текущие предметы в инвентаре
        List<Item> inventoryItems = creatureInventory.GetInventory();

        // Удаляем предметы, которых больше нет в инвентаре
        List<Item> itemsToRemove = new List<Item>();
        foreach (var item in itemInstances.Keys)
        {
            if (!inventoryItems.Contains(item))
            {
                itemsToRemove.Add(item);
            }
        }

        foreach (var item in itemsToRemove)
        {
            RemoveItemFromShelf(item);
        }

        // Добавляем новые предметы
        foreach (Item item in inventoryItems)
        {
            if (!itemInstances.ContainsKey(item))
            {
                AddItemToShelf(item);
            }
        }
    }

    // Добавляет предмет на свободное место
    public void AddItemToShelf(Item item)
    {
        if (itemInstances.ContainsKey(item)) return; // Если уже добавлено, пропускаем

        Transform freeSlot = GetFreeSlot();
        if (freeSlot == null)
        {
            Debug.LogWarning("Нет свободных мест на полке!");
            return;
        }

        GameObject itemObject = Instantiate(item.itemPrefab, freeSlot.position, Quaternion.identity, freeSlot);
        itemInstances[item] = itemObject;

        Debug.Log($"Предмет {item.itemName} добавлен на полку.");
    }

    // Удаляет предмет с полки
    public void RemoveItemFromShelf(Item item)
    {
        if (itemInstances.TryGetValue(item, out GameObject itemObject))
        {
            Destroy(itemObject);
            itemInstances.Remove(item);
            Debug.Log($"Предмет {item.itemName} удалён с полки.");
        }
    }

    // Возвращает первую свободную точку для размещения предмета
    private Transform GetFreeSlot()
    {
        foreach (Transform slot in shelfSlots)
        {
            if (slot.childCount == 0) return slot;
        }
        return null;
    }
}
