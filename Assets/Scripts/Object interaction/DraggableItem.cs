using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.3f; // Максимальное время между кликами для дабл-клика

    public string itemName; // Название предмета, которое будем вводить через инспектор
    public float itemPrice; // Цена предмета, которую будем вводить через инспектор
    public Texture2D itemIcon; // Иконка, назначаемая через инспектор

    private void OnMouseDown()
    {
        if (Time.time - lastClickTime <= doubleClickThreshold)
        {
            Inventory inventory = FindObjectOfType<Inventory>(); // Ищем инвентарь
            ItemDatabase itemDatabase = FindObjectOfType<ItemDatabase>(); // Получаем ссылку на ItemDatabase

            if (inventory != null && itemDatabase != null && inventory.inventoryType == InventoryType.Player)
            {
                // Получаем префаб из базы данных по имени
                GameObject itemPrefab = itemDatabase.GetItemPrefab(itemName);

                if (itemPrefab != null)
                {
                    Item newItem = new Item
                    {
                        itemName = itemName,
                        itemID = gameObject.GetInstanceID(),
                        itemIcon = itemIcon,
                        itemPrice = itemPrice,
                        itemPrefab = itemPrefab // Присваиваем найденный префаб
                    };

                    inventory.AddItem(newItem); // Добавляем предмет в инвентарь
                    Destroy(gameObject); // Удаляем предмет с сцены
                }
                else
                {
                    Debug.LogWarning($"Префаб для {itemName} не найден в ItemDatabase.");
                }
            }
        }
        else
        {
            lastClickTime = Time.time;
        }
    }
}
