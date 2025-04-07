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
        }
    }

    // Инициализация UI с передачей инвентаря существа
    public void InitializeUI(CreatureInventory creatureInventory)
    {
        this.creatureInventory = creatureInventory;
        CurrentInventory = creatureInventory; // Обновляем ссылку на текущий инвентарь
        RefreshUI(creatureInventory);  // Обновляем UI с текущими предметами
    }

    // Обновление UI инвентаря существа
    public void RefreshUI(CreatureInventory creatureInventory)
    {
        ClearUI(); // Очищаем UI перед добавлением новых предметов
        foreach (var item in creatureInventory.GetInventory())  // Получаем список предметов из инвентаря существа
        {
            AddItemToUI(item); // Добавляем каждый предмет в UI
        }
    }

    // Добавление предмета в UI
    public void AddItemToUI(Item item)
    {
        if (uiItems.ContainsKey(item)) 
        {
            return; // Предотвращаем дубликаты
        }

        GameObject newItemButton = Instantiate(itemButtonPrefab, inventoryPanel);  // Создаем новую кнопку
        InventoryItemUI itemUI = newItemButton.GetComponent<InventoryItemUI>();

        if (itemUI != null)
        {
            itemUI.SetupItem(item);  // Настроить элемент UI для этого предмета
            uiItems[item] = newItemButton;  // Сохраняем кнопку в словарь

            // Добавляем обработчик клика для выбора предмета
            Button button = newItemButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectItem(item));  // При клике на кнопку выбираем предмет
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
        }
    }

    // Очистка UI
    private void ClearUI()
    {
        foreach (var itemObject in uiItems.Values)
        {
            Destroy(itemObject);  // Удаляем все UI элементы
        }
        uiItems.Clear();  // Очищаем словарь
    }

    // Выбор предмета
    private void SelectItem(Item item)
    {
        selectedItem = item;
        dropButton.interactable = true; // Включаем кнопку "Выкинуть"
    }

    // Метод для выбрасывания предмета
    private void DropSelectedItem()
    {
        if (selectedItem != null && creatureInventory != null)
        {
            // Получаем позицию для выбрасывания (например, перед игроком)
            Vector3 dropPosition = GetDropPosition();

            // Выбрасываем предмет в мир
            creatureInventory.RemoveItem(selectedItem);

            // Сбрасываем выбор
            selectedItem = null;
            dropButton.interactable = false;
        }
    }

    // Получение позиции для выбрасывания предмета
    private Vector3 GetDropPosition()
    {
        // Можно улучшить, например, определить точку перед игроком
        Vector3 position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
        return position;
    }

    // Установка инвентаря
    public void SetSelectedInventory(CreatureInventory inventory)
    {
        this.creatureInventory = inventory;
        CurrentInventory = inventory;  // Обновляем публичную ссылку на инвентарь
    }
}
