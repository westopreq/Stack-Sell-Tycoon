using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 lastMousePosition; // Последняя позиция мыши
    private float originalY; // Фиксированная высота
    private Collider objCollider;
    private float dragStartTime; // Время начала перетаскивания
    private bool canMove = false; // Флаг, разрешающий перемещение

    private Vector3 objectOffset; // Смещение объекта относительно мыши

    public float rotationSpeed = 100f; // Увеличена скорость вращения объекта (было 15)
    private static Transform selectedObject = null; // Выбранный объект для вращения
    private static Rigidbody selectedRigidbody = null; // Rigidbody выбранного объекта

    public float moveSpeed = 10f; // Скорость перемещения объекта

    void Start()
    {
        objCollider = GetComponent<Collider>();
        originalY = transform.position.y; // Фиксируем высоту объекта
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Raycast для выбора объекта
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Проверяем, есть ли скрипт MoveObject на выбранном объекте
                MoveObject moveScript = hit.transform.GetComponent<MoveObject>();
                if (moveScript != null) // Если скрипт найден
                {
                    if (hit.transform != selectedObject)
                    {
                        selectedObject = hit.transform; // Устанавливаем выбранный объект
                        selectedRigidbody = selectedObject.GetComponent<Rigidbody>(); // Получаем Rigidbody выбранного объекта

                        if (selectedRigidbody != null)
                        {
                            selectedRigidbody.isKinematic = false; // Включаем Rigidbody
                        }

                        dragStartTime = Time.time; // Сохраняем время начала перетаскивания
                        StartCoroutine(WaitForMovePermission()); // Запускаем корутину для задержки
                        isDragging = true; // Разрешаем перетаскивание объекта
                        lastMousePosition = GetMouseWorldPosition(); // Запоминаем начальную позицию мыши
                        objectOffset = selectedObject.position - lastMousePosition; // Рассчитываем смещение объекта
                    }
                }
                else
                {
                    // Если скрипт отсутствует, то ничего не делаем
                    selectedObject = null;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (selectedRigidbody != null)
            {
                selectedRigidbody.isKinematic = true; // Выключаем Rigidbody при отпускании
            }
            isDragging = false;
            selectedObject = null; // Сбрасываем выбранный объект после отпускания
            canMove = false; // Сбрасываем возможность перемещения
        }
    }

    void FixedUpdate()
    {
        // Перемещение выбранного объекта, если прошло достаточно времени
        if (isDragging && selectedObject != null && canMove)
        {
            Vector3 newMousePosition = GetMouseWorldPosition();
            Vector3 mouseDelta = newMousePosition - lastMousePosition; // Получаем смещение мыши

            if (mouseDelta.magnitude > 0.001f) // Двигаем объект только если мышь двигается
            {
                // Плавно перемещаем объект с учетом смещения (без телепортации в координаты мыши)
                Vector3 targetPosition = newMousePosition + objectOffset; // Целевая позиция с учетом смещения
                targetPosition.y = originalY; // Фиксируем высоту

                // Плавно перемещаем объект к целевой позиции через Rigidbody
                selectedRigidbody.MovePosition(Vector3.Lerp(selectedObject.position, targetPosition, moveSpeed * Time.deltaTime));

                lastMousePosition = newMousePosition; // Обновляем последнюю позицию мыши
            }
        }

        // Вращение только выбранного объекта с помощью клавиш E и Q
        if (selectedObject != null)
        {
            if (Input.GetKey(KeyCode.E))
            {
                selectedObject.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World); // Поворот по оси Y
            }
            if (Input.GetKey(KeyCode.Q))
            {
                selectedObject.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime, Space.World); // Поворот в противоположную сторону
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Plane plane = new Plane(Vector3.up, new Vector3(0, originalY, 0));
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return transform.position;
    }

    private IEnumerator WaitForMovePermission()
    {
        // Задержка в 0.25 секунды перед разрешением перемещения
        yield return new WaitForSeconds(0.25f);
        canMove = true; // Разрешаем перемещение объекта
    }
}
