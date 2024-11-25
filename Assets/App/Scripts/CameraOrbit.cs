using System.Runtime.InteropServices.WindowsRuntime;
using ModestTree;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    /*
    Поля, к которым не нужен доступ из других классов, 
    но для которых необходима возможность редактирования
    из Inspector помечены как [SerializeField] private
    для поддержания инкапсуляции.
    */
    [SerializeField] private Transform _target;
    [SerializeField] private float _distance = 5.0f, _rotationSpeed = 2.0f, _minVerticalAngle = 10.0f, _maxVerticalAngle = 80.0f;

    private float _currentVerticalAngle, _currentHorizontalAngle;
    private Vector2 _touchStartPosition, _touchDirection;

    /*
    Названия осей мыши вынесены в константы 
    для избежания "магических" значений
    и упрощенного редактирования 
    */
    private const string MOUSEXAXIS = "Mouse X", MOUSEYAXIS = "Mouse Y", CENTERTAG = "Center";
    
    private void Start()
    {
        /*
        Если _target == null, то происходит поиск первого объекта 
        с тегом "Center" на сцене.
        */
        if (_target == null)
            _target = !GameObject.FindGameObjectsWithTag(CENTERTAG).IsEmpty() ? GameObject.FindGameObjectsWithTag(CENTERTAG)[0].transform : null;

        Vector3 direction = transform.position - _target.position;

        _currentVerticalAngle = Mathf.Clamp(Vector3.Angle(Vector3.up, direction), _minVerticalAngle, _maxVerticalAngle);
        _currentHorizontalAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }

    /*
    Изменение позиции объектов, за которыми следит камера 
    может происходить в Update, поэтому используем LateUpdate.
    */

    private void LateUpdate()
    {
        if (_target == null) 
            return;

        Debug.Log("Touch supported status: " + Input.touchSupported.ToString());
        
        /*
        Возможно свойство Input.touchSupported всегда будет равняться false 
        на некоторых устройствах под управлением Windows с touch-экранами.
        Поэтому следующая строка закомментирована
        */

        //if(Input.touchSupported && Input.touchCount > 0) 
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    Debug.Log("New touch started");
                    
                    _touchStartPosition = touch.position;
                    break;

                case TouchPhase.Moved:
                    _touchDirection = (touch.position - _touchStartPosition).normalized;

                    _currentHorizontalAngle += _touchDirection.x * _rotationSpeed;
                    _currentVerticalAngle = Mathf.Clamp(_currentVerticalAngle + (-_touchDirection.y * _rotationSpeed), _minVerticalAngle, _maxVerticalAngle);
                    break;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis(MOUSEXAXIS) * _rotationSpeed;
            float mouseY = -Input.GetAxis(MOUSEYAXIS) * _rotationSpeed;

            _currentHorizontalAngle += mouseX;
            _currentVerticalAngle = Mathf.Clamp(_currentVerticalAngle + mouseY, _minVerticalAngle, _maxVerticalAngle);
        }

        UpdateCameraTransform(_currentVerticalAngle, _currentHorizontalAngle);
    }

    private void UpdateCameraTransform(float currentVerticalAngle, float currentHorizontalAngle)
    {
        Quaternion rotation = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0);
        Vector3 offset = rotation * Vector3.back * _distance;

        transform.position = _target.position + offset;
        transform.LookAt(_target);
    }
}
