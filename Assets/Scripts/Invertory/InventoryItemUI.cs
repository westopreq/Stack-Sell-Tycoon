using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventoryItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TMP_Text itemNameText;   // Название предмета
    public TMP_Text itemPriceText;  // Цена предмета
    public Image itemImage;         // Иконка предмета

    private Item itemData;          // Данные предмета
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent; // Оригинальный родитель объекта
    private bool isDragged = false;   // Флаг, чтобы понять, перетаскивается ли объект
    private Canvas canvas;

    private Inventory inventory;     // Ссылка на инвентарь
    private RectTransform inventoryRectTransform; // Ссылка на RectTransform панели инвентаря

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // Проверяем и добавляем CanvasGroup, если его нет
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        originalParent = transform.parent;
        canvas = GetComponentInParent<Canvas>(); // Получаем ссылку на Canvas
        inventory = FindObjectOfType<Inventory>(); // Получаем ссылку на инвентарь

        // Получаем RectTransform панели инвентаря
        inventoryRectTransform = inventory.inventoryUI.GetComponent<RectTransform>();
    }

    // Метод для обновления UI элемента
    public void SetupItem(Item item)
    {
        itemData = item; // Сохраняем ссылку на предмет
    
        // Лог, чтобы убедиться, что префаб присутствует
        if (itemData.itemPrefab != null)
        {
            Debug.Log("Префаб предмета: " + itemData.itemPrefab.name);
        }
        else
        {
            Debug.LogError("Префаб не найден для предмета: " + item.itemName);
        }
    
        itemNameText.text = item.itemName;
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


    // Метод для получения данных предмета (нужно для выбрасывания)
    public Item GetItem()
    {
        return itemData;
    }

    // Начало перетаскивания
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragged = true;
        canvasGroup.alpha = 0.6f; // Делаем объект полупрозрачным
        canvasGroup.blocksRaycasts = false; // Отключаем блокировку лучей, чтобы элемент не мешал кликам
        transform.SetParent(canvas.transform); // Переносим элемент на верхний уровень канваса
    }

    // Перетаскивание
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragged)
        {
            rectTransform.position = Input.mousePosition; // Следуем за курсором
        }
    }

    // Завершение перетаскивания
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f; // Восстанавливаем прозрачность
        canvasGroup.blocksRaycasts = true; // Возвращаем блокировку лучей

        // Получаем координаты мыши в локальных координатах инвентаря
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventoryRectTransform, Input.mousePosition, canvas.worldCamera, out localMousePosition);

        // Проверяем, находится ли курсор внутри RectTransform инвентаря
        if (inventoryRectTransform.rect.Contains(localMousePosition))
        {
            // Если в пределах инвентаря, возвращаем карточку в инвентарь
            ReturnItemToInventory();
            Debug.Log("Предмет отпущен внутри инвентаря.");
        }
        else
        {
            // Если не в пределах инвентаря, создаем предмет в мире
            ThrowItem();
            Debug.Log("Предмет отпущен вне инвентаря и преобразован в игровой объект.");
        }
    }

    // Метод для выбрасывания предмета в мир
    private void ThrowItem()
    {
        if (itemData != null && itemData.itemPrefab != null)
        {
            // Логируем информацию о префабе перед созданием
            Debug.Log("Префаб найден: " + itemData.itemPrefab.name);
    
            // Получаем мировую позицию мыши
            Vector3 spawnPosition = GetMouseWorldPosition();
    
            if (spawnPosition != Vector3.zero)
            {
                // Создаем объект в мире в позиции курсора
                GameObject newItem = Instantiate(itemData.itemPrefab, spawnPosition, Quaternion.identity);
    
                // Устанавливаем высоту предмета над землей (примерно 0.5 метра)
                newItem.transform.position = new Vector3(spawnPosition.x, 0.5f, spawnPosition.z);
    
                // Удаляем предмет из инвентаря
                inventory.RemoveItem(itemData);
    
                // Удаляем карточку UI
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("Ошибка: Не удалось получить корректную позицию мыши в мире.");
            }
        }
        else
        {
            // Логируем, если префаб пустой
            Debug.LogError("Префаб не найден для предмета: " + itemData.itemName);
        }
    }



    // Метод для возврата предмета в инвентарь
    private void ReturnItemToInventory()
    {
        // Восстанавливаем позицию предмета в UI
        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero;

        // Добавляем предмет обратно в инвентарь
        inventory.AddItem(itemData);

        // Обновляем UI инвентаря
        inventory.inventoryUI.RefreshUI(inventory);
    }

    // Получение позиции мыши в мире
    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point; // Возвращаем точку столкновения
        }
        return Vector3.zero;
    }
}
