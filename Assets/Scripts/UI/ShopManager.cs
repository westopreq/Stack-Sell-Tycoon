using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject itemButtonPrefab;  // Префаб кнопки для покупки
    public Transform itemsContainer;     // Контейнер для кнопок

    private void Start()
    {
        // Загружаем все предметы из базы данных
        LoadItemsFromDatabase();
    }

    private void LoadItemsFromDatabase()
    {
        ItemDatabase itemDatabase = ItemDatabase.Instance;

        // Для каждого предмета в базе данных создаем кнопку
        foreach (GameObject itemPrefab in itemDatabase.itemPrefabs)
        {
            CreateItemButton(itemPrefab);
        }
    }

    private void CreateItemButton(GameObject itemPrefab)
    {
        // Получаем компонент DraggableItem из префаба
        DraggableItem item = itemPrefab.GetComponent<DraggableItem>();

        // Создаем кнопку для предмета
        GameObject buttonObj = Instantiate(itemButtonPrefab, itemsContainer);
        ItemButton itemButton = buttonObj.GetComponent<ItemButton>();
        itemButton.SetupButton(item); // Настройка кнопки с данными из DraggableItem
    }
}
