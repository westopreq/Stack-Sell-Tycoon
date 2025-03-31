using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;        // Скорость движения камеры
    public float zoomSpeed = 5f;         // Скорость приближения/удаления
    public float rotationSpeed = 100f;   // Скорость вращения
    public float fixedHeight = 10f;      // Фиксированная высота камеры
    public bool canChangeHeight = false; // Галочка, разрешающая изменение высоты

    private Vector3 targetPosition;
    private float currentHeight;

    void Start()
    {
        // Изначальная позиция камеры
        targetPosition = transform.position;
        currentHeight = transform.position.y;  // Начальная высота
    }

    void Update()
    {
        // Получаем направление движения относительно камеры
        Vector3 forward = transform.forward;  // Направление камеры по оси Z
        Vector3 right = transform.right;      // Направление камеры по оси X

        // Движение камеры по осям X и Z (WASD)
        // Движение по оси Z (вперёд и назад)
        if (Input.GetKey(KeyCode.W))
        {
            targetPosition += forward * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            targetPosition -= forward * moveSpeed * Time.deltaTime;
        }

        // Движение по оси X (влево и вправо)
        if (Input.GetKey(KeyCode.A))
        {
            targetPosition -= right * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            targetPosition += right * moveSpeed * Time.deltaTime;
        }

        // Изменение высоты, если галочка включена
        if (canChangeHeight)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                currentHeight += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed; // Меняем высоту камеры
            }
        }
        else
        {
            currentHeight = fixedHeight; // Устанавливаем фиксированную высоту, если галочка выключена
        }

        // Обновляем высоту камеры
        targetPosition.y = currentHeight;

        // Перемещаем камеру в новое положение
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10);

        // Вращение камеры (правая кнопка мыши)
        if (Input.GetMouseButton(1)) // Правая кнопка мыши
        {
            float rotateX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotateX, Space.World);
        }
    }

    // Функция для изменения состояния галочки через UI
    public void ToggleHeightChange(bool state)
    {
        canChangeHeight = state;
    }
}
