using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public GameObject itemButtonPrefab; // Префаб кнопки предмета
    public Transform inventoryPanel;    // Панель, в которой будут элементы инвентаря
    public Button dropButton;           // Кнопка "Выкинуть"

    private Dictionary<Item, GameObject> uiItems = new Dictionary<Item, GameObject>();
    private Inventory inventory;        // Ссылка на инвентарь
    private Item selectedItem;          // Выбранный предмет

    private void Start()
    {
        if (dropButton != null)
        {
            dropButton.onClick.AddListener(DropSelectedItem);
            dropButton.interactable = false; // По умолчанию выключаем кнопку
        }
    }

    public void InitializeUI(Inventory inventory)
    {
        this.inventory = inventory;
        RefreshUI(inventory);
    }

    public void RefreshUI(Inventory inventory)
    {
        ClearUI();
        foreach (var item in inventory.items)
        {
            AddItemToUI(item);
        }
    }

    public void AddItemToUI(Item item)
    {
        if (uiItems.ContainsKey(item)) return; // Предотвращаем дубликаты

        GameObject newItemButton = Instantiate(itemButtonPrefab, inventoryPanel);
        InventoryItemUI itemUI = newItemButton.GetComponent<InventoryItemUI>();

        if (itemUI != null)
        {
            itemUI.SetupItem(item);
            uiItems[item] = newItemButton;

            // Добавляем обработчик клика для выбора предмета
            Button button = newItemButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectItem(item));
            }
        }
    }

    public void RemoveItemFromUI(Item item)
    {
        if (uiItems.TryGetValue(item, out GameObject itemObject))
        {
            Destroy(itemObject);
            uiItems.Remove(item);
        }
    }

    private void ClearUI()
    {
        foreach (var itemObject in uiItems.Values)
        {
            Destroy(itemObject);
        }
        uiItems.Clear();
    }

    private void SelectItem(Item item)
    {
        selectedItem = item;
        dropButton.interactable = true; // Включаем кнопку "Выкинуть"
    }

    private void DropSelectedItem()
    {
        if (selectedItem != null && inventory != null)
        {
            // Получаем позицию для выбрасывания (например, перед игроком)
            Vector3 dropPosition = GetDropPosition();

            // Выбрасываем предмет в мир
            inventory.DropItem(selectedItem, dropPosition);

            // Сбрасываем выбор
            selectedItem = null;
            dropButton.interactable = false;
        }
    }

    private Vector3 GetDropPosition()
    {
        // Можно улучшить, например, определить точку перед игроком
        return Camera.main.transform.position + Camera.main.transform.forward * 2f;
    }
}
