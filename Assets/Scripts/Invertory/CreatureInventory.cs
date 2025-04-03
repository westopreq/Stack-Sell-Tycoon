using UnityEngine;
using System.Collections.Generic;

public class CreatureInventory : MonoBehaviour
{
    [Header("Инвентарь существа")]
    public List<Item> creatureItems = new List<Item>(); // Инвентарь конкретного существа

    public CreatureInventoryUI inventoryUI;  // Сделаем это поле публичным

    private Item selectedItem;  // Для отслеживания выбранного предмета

    private void Start()
    {
        // Ищем CreatureInventoryUI один раз при старте
        inventoryUI = FindObjectOfType<CreatureInventoryUI>();
        Debug.Log("Инвентарь существа и UI инвентаря инициализированы.");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Правая кнопка мыши (Mouse 2)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Преобразуем экранные координаты в мировые
            RaycastHit hit;
    
            // Отправляем луч и проверяем столкновение
            if (Physics.Raycast(ray, out hit))
            {
                // Проверяем, попал ли луч в наше существо
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log("Мышь 2 нажата на: " + gameObject.name);
    
                    // Инициализируем инвентарь существа в UI
                    if (inventoryUI != null)
                    {
                        inventoryUI.InitializeUI(this); // Передаем текущий инвентарь существа в UI
                        inventoryUI.SetSelectedInventory(this); // Устанавливаем текущий инвентарь для дальнейшего использования
                        inventoryUI.RefreshUI(this); // Обновляем отображение инвентаря
                        
                        // Логируем выбранный предмет
                        if (selectedItem != null)
                        {
                            Debug.Log($"Инвентарь существа отображен в UI. Выбранный предмет: {selectedItem.itemName}");
                        }
                        else
                        {
                            Debug.Log("Инвентарь существа отображен в UI. Выбранный предмет: нет.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("UI инвентаря не найдено!");
                    }
                }
            }
        }
    }

    // Метод для выбора предмета
    public void SelectItem(Item item)
    {
        selectedItem = item;
        Debug.Log($"Предмет {item.itemName} выбран.");
    }

    public void AddItem(Item item)
    {
        if (!creatureItems.Contains(item))
        {
            creatureItems.Add(item);
            Debug.Log($"{item.itemName} добавлен в инвентарь {gameObject.name}");

            // Обновляем UI инвентаря существа
            if (inventoryUI != null)
            {
                inventoryUI.RefreshUI(this); // Перерисовываем инвентарь
                Debug.Log("UI инвентаря обновлен.");
            }
            else
            {
                Debug.LogWarning("UI инвентаря не найдено при добавлении предмета!");
            }
        }
        else
        {
            Debug.Log($"{item.itemName} уже есть в инвентаре {gameObject.name}");
        }
    }

    public bool HasItem(Item item)
    {
        bool hasItem = creatureItems.Contains(item);
        Debug.Log($"Проверка наличия {item.itemName} в инвентаре: {hasItem}");
        return hasItem;
    }

    public void RemoveItem(Item item)
    {
        if (creatureItems.Remove(item))
        {
            Debug.Log($"{item.itemName} удален из инвентаря {gameObject.name}");

            // Обновляем UI инвентаря существа
            if (inventoryUI != null)
            {
                inventoryUI.RefreshUI(this); // Перерисовываем инвентарь
                Debug.Log("UI инвентаря обновлен после удаления.");
            }
            else
            {
                Debug.LogWarning("UI инвентаря не найдено при удалении предмета!");
            }
        }
        else
        {
            Debug.LogWarning($"{item.itemName} не найден в инвентаре для удаления.");
        }
    }

    public List<Item> GetInventory()
    {
        Debug.Log($"Запрос инвентаря существа. Количество предметов: {creatureItems.Count}");
        return creatureItems;
    }
}
