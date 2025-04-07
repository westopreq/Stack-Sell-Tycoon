using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventoryItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TMP_Text itemNameText;
    public TMP_Text itemPriceText;
    public Image itemImage;

    private Item itemData;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Transform parentAfterDrag; // Родитель после перетаскивания
    private bool isDragged = false;
    private Canvas canvas;

    private Inventory inventory;
    private RectTransform inventoryRectTransform;
    private CreatureInventory creatureInventory;
    private RectTransform creatureInventoryRectTransform;

    private CreatureInventoryUI creatureInventoryUI; // Ссылка на UI инвентаря существа

   private void Awake()
   {
       rectTransform = GetComponent<RectTransform>();
       canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
   
       originalParent = transform.parent;
       parentAfterDrag = originalParent;
       canvas = GetComponentInParent<Canvas>();
   
       inventory = FindObjectOfType<Inventory>();
       if (inventory == null)
       {
           Debug.LogError("Inventory не найден в сцене!");
       }
       else if (inventory.inventoryUI == null)
       {
           Debug.LogError("Inventory UI отсутствует!");
       }
       else
       {
           inventoryRectTransform = inventory.inventoryUI.GetComponent<RectTransform>();
       }
   
      creatureInventory = FindObjectOfType<CreatureInventory>();
      if (creatureInventory != null)
      {
          if (creatureInventory.inventoryUI == null)
          {
              Debug.LogWarning("CreatureInventoryUI отсутствует на " + creatureInventory.name + ". Но продолжим выполнение.");
          }
          else
          {
              creatureInventoryRectTransform = creatureInventory.inventoryUI.GetComponent<RectTransform>();
              creatureInventoryUI = creatureInventory.inventoryUI.GetComponent<CreatureInventoryUI>();
          }
      }
      else
      {
          Debug.LogWarning("CreatureInventory не найден в сцене. Инвентарь существа не будет доступен.");
      }

   }


    public void SetupItem(Item item)
    {
        itemData = item;
        itemNameText.text = item.itemName;  // Используем itemName вместо name
        itemPriceText.text = $"${item.itemPrice}";

        if (item.itemIcon != null)
        {
            itemImage.sprite = Sprite.Create(
                item.itemIcon,
                new Rect(0, 0, item.itemIcon.width, item.itemIcon.height),
                new Vector2(0.5f, 0.5f)
            );
        }
    }

    public Item GetItem() => itemData;

   public void OnBeginDrag(PointerEventData eventData)
   {
       isDragged = true;
       canvasGroup.alpha = 0.6f;
       canvasGroup.blocksRaycasts = false;
       transform.SetParent(canvas.transform);
       
       // Проверяем, если creatureInventoryUI не пустой, то извлекаем инвентарь существа
       if (creatureInventoryUI != null)
       {
           creatureInventory = creatureInventoryUI.CurrentInventory;
       }
       else
       {
           Debug.LogWarning("CreatureInventoryUI не найден. Не будет установлен инвентарь существа.");
       }
   }



    public void OnDrag(PointerEventData eventData)
    {
        if (isDragged)
        {
            rectTransform.position = Input.mousePosition;
        }
    }

    // В методе OnEndDrag
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    
        Vector2 localMousePosition;
    
        // Проверяем основной инвентарь игрока
        bool isInsidePlayerInventory = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            inventoryRectTransform, Input.mousePosition, canvas.worldCamera, out localMousePosition) &&
            inventoryRectTransform.rect.Contains(localMousePosition);
    
        // Проверяем инвентарь существа
        bool isInsideCreatureInventory = false;
        if (creatureInventoryRectTransform != null)
        {
            isInsideCreatureInventory = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                creatureInventoryRectTransform, Input.mousePosition, canvas.worldCamera, out localMousePosition) &&
                creatureInventoryRectTransform.rect.Contains(localMousePosition);
        }
    
        // Если предмет в пределах инвентаря существа
        if (isInsideCreatureInventory)
        {
            if (creatureInventory != null)
            {
                if (creatureInventory.creatureItems.Count >= creatureInventory.maxInventorySize)
                {
                    Debug.LogWarning("Инвентарь существа заполнен! Предмет возвращается в инвентарь игрока.");
                    MoveItemToInventory(inventory);
                    transform.SetParent(inventory.inventoryUI.transform); // Перемещаем карточку в UI игрока
                    transform.localPosition = Vector3.zero; // Сбрасываем позицию
                }
                else
                {
                    MoveItemToCreatureInventory(creatureInventory);
                    Debug.Log("Предмет перемещён в инвентарь существа.");
                }
            }
            else
            {
                Debug.LogError("Нет выбранного инвентаря существа.");
            }
        }


        // Если предмет в пределах инвентаря игрока
        else if (isInsidePlayerInventory)
        {
            MoveItemToInventory(inventory);
            Debug.Log("Предмет перемещён в инвентарь игрока.");
        }
        // Если предмет вне инвентаря, выбрасываем его в мир
        else
        {
            ThrowItem();
            Debug.Log("Предмет выброшен в мир.");
        }
    }


    private void MoveItemToInventory(Inventory newInventory)
    {
        Debug.Log($"Moving {itemData.itemName} to {newInventory.name}");

        // Проверяем, действительно ли предмет уже есть в новом инвентаре
        if (newInventory.HasItem(itemData))
        {
            Debug.Log($"Item {itemData.itemName} already exists in {newInventory.name}, skipping move.");
            return;
        }

        bool removed = false;

        // Удаление из старого инвентаря
        if (creatureInventory.HasItem(itemData))
        {
            Debug.Log($"Removing {itemData.itemName} from Creature Inventory");
            creatureInventory.RemoveItem(itemData);
            removed = true;
        }
        else if (inventory.HasItem(itemData))
        {
            Debug.Log($"Removing {itemData.itemName} from Player Inventory");
            inventory.RemoveItem(itemData);
            removed = true;
        }

        if (!removed)
        {
            Debug.LogError($"ERROR: {itemData.itemName} не найден в инвентарях!");
            return;
        }

        // Добавление в новый инвентарь
        newInventory.AddItem(itemData);
        Debug.Log($"Added {itemData.itemName} to {newInventory.name}");

        // Перемещение карточки UI в новый инвентарь
        transform.SetParent(newInventory.inventoryUI.transform);
        transform.localPosition = Vector3.zero;

        // Обновление UI
        inventory.inventoryUI.RefreshUI(inventory);
        newInventory.inventoryUI.RefreshUI(newInventory);

        Debug.Log("Moved to new inventory and UI refreshed");
    }

    private void MoveItemToCreatureInventory(CreatureInventory newInventory)
    {
        Debug.Log($"Moving {itemData.itemName} to {newInventory.name}");

        // Проверяем, действительно ли предмет уже есть в новом инвентаре
        if (newInventory.HasItem(itemData))
        {
            Debug.Log($"Item {itemData.itemName} already exists in {newInventory.name}, skipping move.");
            return;
        }

        bool removed = false;

        // Удаление из старого инвентаря
        if (inventory.HasItem(itemData))
        {
            Debug.Log($"Removing {itemData.itemName} from Player Inventory");
            inventory.RemoveItem(itemData);
            removed = true;
        }
        else if (creatureInventory.HasItem(itemData))
        {
            Debug.Log($"Removing {itemData.itemName} from Creature Inventory");
            creatureInventory.RemoveItem(itemData);
            removed = true;
        }

        if (!removed)
        {
            Debug.LogError($"ERROR: {itemData.itemName} не найден в инвентарях!");
            return;
        }

        // Добавление в новый инвентарь существа
        newInventory.AddItem(itemData);
        Debug.Log($"Added {itemData.itemName} to {newInventory.name}");

        // Перемещение карточки UI в новый инвентарь
        transform.SetParent(newInventory.inventoryUI.transform);
        transform.localPosition = Vector3.zero;

        // Обновление UI
        inventory.inventoryUI.RefreshUI(inventory);
        newInventory.inventoryUI.RefreshUI(newInventory);

        Debug.Log("Moved to creature's inventory and UI refreshed");
    }

    private void ThrowItem()
    {
        if (itemData != null && itemData.itemPrefab != null)
        {
            Vector3 spawnPosition = GetMouseWorldPosition();
            if (spawnPosition != Vector3.zero)
            {
                GameObject newItem = Instantiate(itemData.itemPrefab, spawnPosition, Quaternion.identity);
                newItem.transform.position = new Vector3(spawnPosition.x, 0.5f, spawnPosition.z);

                inventory.RemoveItem(itemData);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("Не удалось определить позицию предмета.");
            }
        }
        else
        {
            Debug.LogError("У предмета отсутствует префаб.");
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}
