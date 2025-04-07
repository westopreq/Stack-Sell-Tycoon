using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;      // Название предмета
    public int itemID;           // Уникальный ID предмета
    public Texture2D itemIcon;   // Иконка предмета
    public float itemPrice;      // Цена предмета
    public GameObject itemPrefab; // Префаб для восстановления в мир
}
