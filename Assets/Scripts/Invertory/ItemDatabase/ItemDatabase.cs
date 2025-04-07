using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    public GameObject[] itemPrefabs;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public GameObject GetItemPrefab(string itemName)
    {
        foreach (var prefab in itemPrefabs)
        {
            if (prefab.name == itemName)
                return prefab;
        }
        return null;
    }

    public Item GetRandomItem()
    {
        if (itemPrefabs.Length == 0)
            return null;

        var prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
        return prefab.GetComponent<Item>(); // Предполагается, что префаб имеет компонент Item
    }
}
