using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target; // �����, ������ ������� ����� ��������� ������
    public float distance = 5.0f; // ���������� �� �����
    public float rotationSpeed = 2.0f; // �������� ��������
    public float minVerticalAngle = 10.0f; // ����������� ���� ������� ������
    public float maxVerticalAngle = 80.0f; // ������������ ���� ������� ������

    private float currentVerticalAngle;
    private float currentHorizontalAngle;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("�� ������� ����� �������� ��� ������.");
            return;
        }

        // ������������� ��������� ����
        Vector3 direction = transform.position - target.position;
        currentVerticalAngle = Mathf.Clamp(Vector3.Angle(Vector3.up, direction), minVerticalAngle, maxVerticalAngle);
        currentHorizontalAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        // ��������� ������� ������ ����� ��� ������
        UpdateCameraPosition();
    }

    void Update()
    {
        if (target == null) return;

        // ���������, ������������ �� ����� ������ ����
        if (Input.GetMouseButton(0)) // 0 - ��� ����� ������ ����
        {
            // �������� �������� ����������� ����
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = -Input.GetAxis("Mouse Y") * rotationSpeed;

            // �������� ���� � ����������� �� ����������� ����
            currentHorizontalAngle += mouseX;
            currentVerticalAngle = Mathf.Clamp(currentVerticalAngle + mouseY, minVerticalAngle, maxVerticalAngle);

            // ��������� ������� ������
            UpdateCameraPosition();
        }
    }

    // ����� ��� ���������� ������� ������ �� ������ ������� �����
    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0);
        Vector3 offset = rotation * Vector3.back * distance;

        // ��������� ������� � ����������� ������
        transform.position = target.position + offset;
        transform.LookAt(target);
    }
}