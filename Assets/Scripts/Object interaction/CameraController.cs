using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 100f;
    public float zoomSpeed = 5f; // Скорость изменения высоты

    private Vector3 targetPosition;
    private bool isRightMouseHeld = false; // Флаг для ПКМ

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        // ПКМ нажата → вращаем камеру
        if (Input.GetMouseButton(1))
        {
            isRightMouseHeld = true;
            float rotateX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotateX, Space.World);
        }
        else
        {
            isRightMouseHeld = false;
        }

        // Движение камеры по осям (WASD) по горизонтали
        Vector3 forward = transform.forward; // Направление вперед по оси Z камеры
        Vector3 right = transform.right;     // Направление вправо по оси X камеры

        // Нормализуем векторы, чтобы скорость была одинаковой в обоих направлениях
        forward.y = 0f; // Игнорируем движение по оси Y
        right.y = 0f;   // Игнорируем движение по оси Y
        forward.Normalize();
        right.Normalize();

        // Двигаем камеру только по осям X и Z, игнорируя ось Y (высоту)
        Vector3 newTargetPosition = targetPosition; // Копируем текущую позицию
        if (Input.GetKey(KeyCode.W)) newTargetPosition += forward * moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S)) newTargetPosition -= forward * moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A)) newTargetPosition -= right * moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D)) newTargetPosition += right * moveSpeed * Time.deltaTime;

        // Сохраняем текущую высоту (игнорируем изменение по Y)
        newTargetPosition.y = targetPosition.y;

        // Применяем новую позицию
        targetPosition = newTargetPosition;

        // Изменение высоты через колесико мыши (если не нажата ПКМ)
        if (!isRightMouseHeld) // Только если не нажата ПКМ
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                targetPosition.y += scroll * zoomSpeed; // Изменение высоты
            }
        }

        // Перемещаем камеру
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10);
    }
}
