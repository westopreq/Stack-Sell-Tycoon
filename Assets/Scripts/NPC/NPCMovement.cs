using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCController : MonoBehaviour
{
    public Transform entryPoint; // Точка перед дверью для входа
    public Transform insidePoint; // Точка внутри помещения после двери
    public List<Transform> shelfPoints = new List<Transform>(); // Список точек полок
    public Transform terminalPoint;  // Точка для терминала (кассы)

    private Animator animator; // Ссылка на аниматор
    private bool insideStore = false; // Проверка, зашел ли NPC внутрь магазина

    private float waitTime = 2f; // Время ожидания, если путь занят
    private float stopDistance = 1f; // Расстояние для остановки перед полкой или терминалом

    private float moneyToPay = 0f; // Сколько должен заплатить NPC
    private int itemsBought = 0; // Количество купленных товаров

    private void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(MoveThroughWaypoints()); // Начинаем движение NPC сразу
    }

    private IEnumerator MoveThroughWaypoints()
    {
        // Сначала NPC идет к entryPoint (перед дверью)
        yield return MoveToPoint(entryPoint.position);

        // После того как дошли, NPC идет к insidePoint (внутри магазина)
        yield return MoveToPoint(insidePoint.position);

        // После того как NPC зашел в магазин, начинаем искать полки и терминал
        if (!insideStore)
        {
            FindTerminalAndShelves();
            insideStore = true;
        }

        // После того как прошел через дверь, начинаем искать полки и терминал
        yield return StartCoroutine(InteractWithShelfAndTerminal());

        // После взаимодействия NPC идет обратно
        yield return MoveToPoint(insidePoint.position); // Возвращаемся к точке внутри
        yield return MoveToPoint(entryPoint.position); // И возвращаемся к точке перед дверью

        // Удаляем NPC после завершения маршрута
        Destroy(gameObject); // Удаляем объект NPC
    }

    private void FindTerminalAndShelves()
    {
        // Ищем терминал по тегу
        GameObject terminal = GameObject.FindGameObjectWithTag("Terminal");
        if (terminal != null)
        {
            terminalPoint = terminal.transform;
            Debug.Log("Терминал найден: " + terminal.name);
        }
        else
        {
            Debug.LogWarning("Терминал не найден.");
        }

        // Находим все полки по тегу
        GameObject[] shelves = GameObject.FindGameObjectsWithTag("Shelf");
        if (shelves.Length > 0)
        {
            shelfPoints.Clear();
            foreach (GameObject shelf in shelves)
            {
                shelfPoints.Add(shelf.transform);
                Debug.Log("Полка найдена: " + shelf.name);
            }
        }
        else
        {
            Debug.LogWarning("Полки не найдены.");
        }
    }

    private IEnumerator MoveToPoint(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > stopDistance)
        {
            animator.SetBool("Run", true);

            Vector3 direction = targetPosition - transform.position;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime);

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 2f * Time.deltaTime);

            yield return null;
        }

        animator.SetBool("Run", false);
    }

    private IEnumerator InteractWithShelfAndTerminal()
    {
        foreach (var shelfPoint in shelfPoints)
        {
            // Двигаемся к полке
            yield return MoveToPoint(shelfPoint.position);

            // Разворачиваемся к полке
            Vector3 directionToShelf = shelfPoint.position - transform.position;
            directionToShelf.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(directionToShelf);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime);

            // Имитируем взаимодействие с полкой
            Debug.Log($"NPC взаимодействует с полкой в {shelfPoint.position}");

            // Время взаимодействия с полкой
            yield return new WaitForSeconds(1f);

            // Работаем с инвентарем полки
            InteractWithShelfInventory(shelfPoint);
        }

        if (terminalPoint != null)
        {
            yield return MoveToPoint(terminalPoint.position);

            Debug.Log($"NPC взаимодействует с терминалом в {terminalPoint.position}");

            yield return new WaitForSeconds(2f);

            PayAtTerminal();
        }
    }

    private void InteractWithShelfInventory(Transform shelf)
    {
        CreatureInventory shelfInventory = shelf.GetComponent<CreatureInventory>();
        if (shelfInventory != null && shelfInventory.GetInventory().Count > 0)
        {
            Item selectedItem = shelfInventory.GetRandomItem();
            if (selectedItem != null)
            {
                moneyToPay += selectedItem.itemPrice;
                itemsBought++;

                Debug.Log($"NPC добавил товар {selectedItem.itemName} за {selectedItem.itemPrice}. Общая сумма: {moneyToPay}");

                shelfInventory.RemoveItem(selectedItem);
            }
        }
        else
        {
            Debug.Log("Полка пуста или нет инвентаря.");
        }
    }

    private void PayAtTerminal()
    {
        // Рассчитываем цену с учетом 25% наценки
        float priceWithMarkup = moneyToPay * 1.25f;
    
        // Округляем до целого числа
        int finalPrice = Mathf.FloorToInt(priceWithMarkup);
    
        Debug.Log($"NPC оплачивает {finalPrice} на кассе.");
    
        // Добавляем деньги в систему
        GameStats.Instance.AddMoney(finalPrice);
    
        // Начисляем опыт за каждый купленный товар
        for (int i = 0; i < itemsBought; i++)
        {
            GameStats.Instance.AddExperience(1);
        }
    
        Debug.Log($"Оплата успешна. Текущие деньги: {GameStats.Instance.money}");
    
        // Сброс значений после оплаты
        moneyToPay = 0f;
        itemsBought = 0;
    }

}
