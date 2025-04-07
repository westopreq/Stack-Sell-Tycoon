using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public InventoryType inventoryType; // Тип инвентаря
    public List<Item> items = new List<Item>(); // Список предметов в инвентаре
    public int maxItems = 10; // Максимальное количество предметов

    public InventoryUI inventoryUI; // Ссылка на UI инвентаря

    private void Start()
    {
        if (inventoryUI != null)
        {
            inventoryUI.InitializeUI(this); // Инициализируем UI при старте
        }
    }

    // Метод для добавления предмета в инвентарь
   public void AddItem(Item item)
   {
       if (items.Count < maxItems)
       {
           items.Add(item);
           Debug.Log($"{item.itemName} добавлен в инвентарь {gameObject.name}, Префаб: {item.itemPrefab}");
   
           if (inventoryUI != null)
           {
               inventoryUI.AddItemToUI(item); // Добавляем предмет в UI
           }
       }
       else
       {
           Debug.Log("Инвентарь полон!");
       }
   }
   
   public bool HasItem(Item item)
   {
       return items.Contains(item);
   }




    // Метод для удаления предмета
    public void RemoveItem(Item item)
    {
        if (items.Remove(item))
        {
            Debug.Log($"{item.itemName} удален из инвентаря {gameObject.name}");

            if (inventoryUI != null)
            {
                inventoryUI.RemoveItemFromUI(item); // Удаляем из UI
            }
        }
    }

    // Новый метод для выбрасывания предмета в мир
    public void DropItem(Item item, Vector3 dropPosition)
    {
        if (items.Contains(item))
        {
            RemoveItem(item); // Удаляем предмет из инвентаря

            if (item.itemPrefab != null)
            {
                // Создаём объект в мире по сохранённому префабу
                GameObject droppedItem = Instantiate(item.itemPrefab, dropPosition, Quaternion.identity);
                Debug.Log($"{item.itemName} выброшен в мир!");
            }
            else
            {
                Debug.LogWarning($"Префаб для {item.itemName} не задан!");
            }
        }
    }
}
