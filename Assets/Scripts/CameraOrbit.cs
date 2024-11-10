using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target; // Точка, вокруг которой будет вращаться камера
    public float distance = 5.0f; // Расстояние до точки
    public float rotationSpeed = 2.0f; // Скорость вращения
    public float minVerticalAngle = 10.0f; // Минимальный угол наклона камеры
    public float maxVerticalAngle = 80.0f; // Максимальный угол наклона камеры

    private float currentVerticalAngle;
    private float currentHorizontalAngle;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Не указана точка вращения для камеры.");
            return;
        }

        // Устанавливаем начальные углы
        Vector3 direction = transform.position - target.position;
        currentVerticalAngle = Mathf.Clamp(Vector3.Angle(Vector3.up, direction), minVerticalAngle, maxVerticalAngle);
        currentHorizontalAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        // Обновляем позицию камеры сразу при старте
        UpdateCameraPosition();
    }

    void Update()
    {
        if (target == null) return;

        // Проверяем, удерживается ли левая кнопка мыши
        if (Input.GetMouseButton(0)) // 0 - это левая кнопка мыши
        {
            // Получаем значения перемещения мыши
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = -Input.GetAxis("Mouse Y") * rotationSpeed;

            // Изменяем углы в зависимости от перемещения мыши
            currentHorizontalAngle += mouseX;
            currentVerticalAngle = Mathf.Clamp(currentVerticalAngle + mouseY, minVerticalAngle, maxVerticalAngle);

            // Обновляем позицию камеры
            UpdateCameraPosition();
        }
    }

    // Метод для обновления позиции камеры на основе текущих углов
    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0);
        Vector3 offset = rotation * Vector3.back * distance;

        // Обновляем позицию и направление камеры
        transform.position = target.position + offset;
        transform.LookAt(target);
    }
}
