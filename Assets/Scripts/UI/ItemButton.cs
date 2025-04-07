using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    [Header("UI элементы для кнопки")]
    public TextMeshProUGUI itemNameText; // Название предмета
    public TextMeshProUGUI itemPriceText; // Цена предмета
    public Image itemIconImage; // Иконка предмета
    public Button buyButton; // Кнопка покупки

    // Настроить кнопку для каждого предмета
    public void SetupButton(DraggableItem item)
    {
        itemNameText.text = item.itemName;
        itemPriceText.text = "$" + item.itemPrice.ToString();
        itemIconImage.sprite = Sprite.Create(item.itemIcon, new Rect(0, 0, item.itemIcon.width, item.itemIcon.height), new Vector2(0.5f, 0.5f));

        // Назначаем обработчик для кнопки "Купить"
        buyButton.onClick.RemoveAllListeners(); // Убираем старые слушатели на случай многократных вызовов
        buyButton.onClick.AddListener(() => BuyItem(item));  // При нажатии покупаем предмет
    }

    private void BuyItem(DraggableItem draggableItem)
    {
        // Создаем новый Item из DraggableItem
        Item item = new Item
        {
            itemName = draggableItem.itemName,
            itemPrice = draggableItem.itemPrice,
            itemIcon = draggableItem.itemIcon,
            itemPrefab = draggableItem.gameObject // Ссылаемся на сам объект, который может быть префабом
        };
    
        // Находим инвентарь на сцене
        Inventory inventory = FindObjectOfType<Inventory>();
        if (inventory != null)
        {
            // Проверка, есть ли у игрока достаточно средств через GameStats
            if (GameStats.Instance.money >= item.itemPrice)
            {
                GameStats.Instance.SpendMoney((int)item.itemPrice); // Списание средств
                inventory.AddItem(item);  // Добавление предмета в инвентарь
                Debug.Log($"Предмет {item.itemName} куплен!");
            }
            else
            {
                Debug.LogWarning("Недостаточно средств для покупки");
            }
        }
        else
        {
            Debug.LogError("Инвентарь не найден на сцене!");
        }
    }

}
