using UnityEngine;
using System.Collections.Generic;

public class CreatureInventory : MonoBehaviour
{
    [Header("Инвентарь существа")]
    public List<Item> creatureItems = new List<Item>(); // Инвентарь существа
    public int maxInventorySize = 10; // Максимальный размер инвентаря

    public CreatureInventoryUI inventoryUI; // UI инвентаря
    private Shelf shelf; // Ссылка на полку, если есть

    private Item selectedItem; // Выбранный предмет

    private void Start()
    {
        // Ищем UI инвентаря один раз при старте
        inventoryUI = FindObjectOfType<CreatureInventoryUI>();

        // Проверяем, есть ли на этом объекте полка
        shelf = GetComponent<Shelf>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Правая кнопка мыши (Mouse 2)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                if (inventoryUI != null)
                {
                    inventoryUI.InitializeUI(this);
                    inventoryUI.SetSelectedInventory(this);
                    inventoryUI.RefreshUI(this);
                }
            }
        }
    }

    // Выбор предмета
    public void SelectItem(Item item)
    {
        selectedItem = item;
    }

    public void AddItem(Item item)
    {
        if (creatureItems.Count >= maxInventorySize)
        {
            return;
        }

        if (!creatureItems.Contains(item))
        {
            creatureItems.Add(item);

            // Обновляем UI
            inventoryUI?.RefreshUI(this);

            // Если есть полка, обновляем её
            if (shelf != null)
            {
                shelf.RefreshShelf();
            }
        }
    }

    public bool HasItem(Item item)
    {
        return creatureItems.Contains(item);
    }

    public void RemoveItem(Item item)
    {
        if (creatureItems.Remove(item))
        {
            // Обновляем UI
            inventoryUI?.RefreshUI(this);

            // Если есть полка, обновляем её
            if (shelf != null)
            {
                shelf.RefreshShelf();
            }
        }
    }

    public List<Item> GetInventory()
    {
        return creatureItems;
    }

    // Метод для случайного выбора предмета для покупателя
    public Item GetRandomItem()
    {
        if (creatureItems.Count > 0)
        {
            int randomIndex = Random.Range(0, creatureItems.Count);
            return creatureItems[randomIndex];
        }
        else
        {
            return null;
        }
    }
}
