using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CreatureInventoryUI : MonoBehaviour
{
    public GameObject itemButtonPrefab; // Префаб кнопки предмета
    public Transform inventoryPanel;    // Панель, в которой будут элементы инвентаря
    public Button dropButton;           // Кнопка "Выкинуть"

    private Dictionary<Item, GameObject> uiItems = new Dictionary<Item, GameObject>();
    private CreatureInventory creatureInventory;  // Ссылка на инвентарь существа
    private Item selectedItem;          // Выбранный предмет

    // Публичная ссылка на текущий инвентарь, который установлен
    public CreatureInventory CurrentInventory { get; private set; }

    private void Start()
    {
        if (dropButton != null)
        {
            dropButton.onClick.AddListener(DropSelectedItem);
            dropButton.interactable = false; // По умолчанию выключаем кнопку
            Debug.Log("Start: Drop button initialized and set to inactive.");
        }
    }

    // Инициализация UI с передачей инвентаря существа
    public void InitializeUI(CreatureInventory creatureInventory)
    {
        this.creatureInventory = creatureInventory;
        CurrentInventory = creatureInventory; // Обновляем ссылку на текущий инвентарь
        Debug.Log("InitializeUI: Creature inventory initialized.");
        RefreshUI(creatureInventory);  // Обновляем UI с текущими предметами
    }

    // Обновление UI инвентаря существа
    public void RefreshUI(CreatureInventory creatureInventory)
    {
        ClearUI(); // Очищаем UI перед добавлением новых предметов
        Debug.Log("RefreshUI: UI cleared.");
        foreach (var item in creatureInventory.GetInventory())  // Получаем список предметов из инвентаря существа
        {
            AddItemToUI(item); // Добавляем каждый предмет в UI
            Debug.Log($"RefreshUI: Item {item.itemName} added to UI.");
        }
    }

    // Добавление предмета в UI
    public void AddItemToUI(Item item)
    {
        if (uiItems.ContainsKey(item)) 
        {
            Debug.Log($"AddItemToUI: Item {item.itemName} already exists in UI.");
            return; // Предотвращаем дубликаты
        }

        GameObject newItemButton = Instantiate(itemButtonPrefab, inventoryPanel);  // Создаем новую кнопку
        InventoryItemUI itemUI = newItemButton.GetComponent<InventoryItemUI>();

        if (itemUI != null)
        {
            itemUI.SetupItem(item);  // Настроить элемент UI для этого предмета
            uiItems[item] = newItemButton;  // Сохраняем кнопку в словарь
            Debug.Log($"AddItemToUI: Item {item.itemName} button added to UI.");

            // Добавляем обработчик клика для выбора предмета
            Button button = newItemButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectItem(item));  // При клике на кнопку выбираем предмет
                Debug.Log($"AddItemToUI: Button click listener added for item {item.itemName}.");
            }
        }
    }

    // Удаление предмета из UI
    public void RemoveItemFromUI(Item item)
    {
        if (uiItems.TryGetValue(item, out GameObject itemObject))
        {
            Destroy(itemObject);  // Удаляем объект из UI
            uiItems.Remove(item);  // Удаляем предмет из словаря
            Debug.Log($"RemoveItemFromUI: Item {item.itemName} removed from UI.");
        }
    }

    // Очистка UI
    private void ClearUI()
    {
        foreach (var itemObject in uiItems.Values)
        {
            Destroy(itemObject);  // Удаляем все UI элементы
            Debug.Log("ClearUI: UI element destroyed.");
        }
        uiItems.Clear();  // Очищаем словарь
        Debug.Log("ClearUI: UI cleared.");
    }

    // Выбор предмета
    private void SelectItem(Item item)
    {
        selectedItem = item;
        dropButton.interactable = true; // Включаем кнопку "Выкинуть"
        Debug.Log($"SelectItem: Item {item.itemName} selected.");
    }

    // Метод для выбрасывания предмета
    private void DropSelectedItem()
    {
        if (selectedItem != null && creatureInventory != null)
        {
            // Получаем позицию для выбрасывания (например, перед игроком)
            Vector3 dropPosition = GetDropPosition();
            Debug.Log($"DropSelectedItem: Dropping item {selectedItem.itemName} at position {dropPosition}.");

            // Выбрасываем предмет в мир
            creatureInventory.RemoveItem(selectedItem);

            // Сбрасываем выбор
            selectedItem = null;
            dropButton.interactable = false;
            Debug.Log("DropSelectedItem: Item dropped and selection cleared.");
        }
        else
        {
            Debug.Log("DropSelectedItem: No item selected or creature inventory is null.");
        }
    }

    // Получение позиции для выбрасывания предмета
    private Vector3 GetDropPosition()
    {
        // Можно улучшить, например, определить точку перед игроком
        Vector3 position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
        Debug.Log($"GetDropPosition: Drop position calculated as {position}.");
        return position;
    }

    // Установка инвентаря
    public void SetSelectedInventory(CreatureInventory inventory)
    {
        this.creatureInventory = inventory;
        CurrentInventory = inventory;  // Обновляем публичную ссылку на инвентарь
        Debug.Log($"SetSelectedInventory: Selected inventory set to {inventory.gameObject.name}");
    }
}
