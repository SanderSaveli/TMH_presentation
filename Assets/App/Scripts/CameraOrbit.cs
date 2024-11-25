using UnityEngine;
using UnityEngine.EventSystems;
using ModestTree;

public class CameraOrbit : MonoBehaviour
{
    /*
    Поля, к которым не нужен доступ из других классов, 
    но для которых необходима возможность редактирования
    из Inspector помечены как [SerializeField] private
    для поддержания инкапсуляции.
    */
    [SerializeField] private Transform _target;
    [SerializeField] private float _rotationSpeed = 2f, _zoomSpeed = 2f, 
    _minVerticalAngle = 10f, _maxVerticalAngle = 80f, 
    _minZoomDistance = 5f, _maxZoomDistance = 20f;

    private float _currentVerticalAngle, _currentHorizontalAngle, _distanceToTarget, _currentRotationSpeed, _currentZoomSpeed;
    private Vector2 _touchStartPosition;
    private bool _isFingerContinuesToMove = false;

    /*
    Названия осей мыши вынесены в константы 
    для избежания "магических" значений
    и упрощенного редактирования 
    */
    private const string MOUSEXAXIS = "Mouse X", MOUSEYAXIS = "Mouse Y", MOUSEWHEELAXIS = "Mouse ScrollWheel", CENTERTAG = "Center";
    private const float PINCHTOZOOMMULTIPLIER = 0.0005f, INTERPOLATIONSPEED = 50f, TOUCHMOVEMENTMARGIN = 3f;
    
    private void Start()
    {
        /*
        Если _target == null, то происходит поиск первого объекта 
        с тегом "Center" на сцене.
        */
        if (_target == null)
            _target = !GameObject.FindGameObjectsWithTag(CENTERTAG).IsEmpty() ? GameObject.FindGameObjectsWithTag(CENTERTAG)[0].transform : null;

        _distanceToTarget = _minZoomDistance;

        Vector3 direction = transform.position - _target.position;

        _currentVerticalAngle = Mathf.Clamp(Vector3.Angle(Vector3.up, direction), _minVerticalAngle, _maxVerticalAngle);
        _currentHorizontalAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }

    private void Update() 
    {
        if (_target == null) 
            return;

        ProcessInput();
    }

    /*
    Изменение позиции объектов, за которыми следит камера 
    может происходить в Update, поэтому используем LateUpdate.
    */
    private void LateUpdate()
    {
        if (_target == null) 
            return;
        
        UpdateCameraTransform(_currentVerticalAngle, _currentHorizontalAngle, _distanceToTarget);
    }

    private void ProcessInput() 
    {
        //Вращение камеры с помощью пальца
        if(Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    Debug.Log("New touch started");
                    
                    _touchStartPosition = touch.position;
                    break;

                case TouchPhase.Moved:
                    _currentRotationSpeed = touch.deltaPosition.magnitude > TOUCHMOVEMENTMARGIN ? _rotationSpeed : 0f;

                    Vector2 _touchDirection = (touch.position - _touchStartPosition).normalized;

                    _currentHorizontalAngle = Mathf.Lerp(_currentHorizontalAngle, _currentHorizontalAngle + _touchDirection.x * _currentRotationSpeed, Time.deltaTime * INTERPOLATIONSPEED);
                    _currentVerticalAngle = Mathf.Lerp(_currentVerticalAngle, _currentVerticalAngle + (-_touchDirection.y * _currentRotationSpeed), Time.deltaTime * INTERPOLATIONSPEED);
                    _currentVerticalAngle = Mathf.Clamp(_currentVerticalAngle, _minVerticalAngle, _maxVerticalAngle);
                    break;

                case TouchPhase.Stationary:
                    _touchStartPosition = touch.position;
                    break;
            }
        }
        
        //Приближение камеры с помощью щипка пальцами
        if(Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0), touch2 = Input.GetTouch(1);
            Vector2 touch1Position = touch1.position, touch2Position = touch2.position;
            bool isTouch1Stationary = false, isTouch2Stationary = false;

            switch (touch1.phase)
            {
                case TouchPhase.Moved:
                    touch1Position = touch1.position;
                    break;

                case TouchPhase.Stationary:
                    isTouch1Stationary = true;
                    break;
            }

            switch (touch2.phase)
            {
                case TouchPhase.Moved:
                    touch2Position = touch2.position;
                    break;

                case TouchPhase.Stationary:
                    isTouch2Stationary = true;
                    break;
            }

            float currentDistanceBetweenTouches = (touch1Position - touch2Position).magnitude;

            bool isDistanceGrowed = ((touch1Position + touch1.deltaPosition) - (touch2Position + touch2.deltaPosition)).magnitude >= currentDistanceBetweenTouches;
            _isFingerContinuesToMove = !(isTouch1Stationary & isTouch2Stationary);
            bool isInsideMargin = touch1.deltaPosition.magnitude <= TOUCHMOVEMENTMARGIN || touch2.deltaPosition.magnitude <= TOUCHMOVEMENTMARGIN;

            _currentZoomSpeed = _isFingerContinuesToMove && !isInsideMargin ? _zoomSpeed : 0f;

            _distanceToTarget = Mathf.Lerp(_distanceToTarget, _distanceToTarget + currentDistanceBetweenTouches * PINCHTOZOOMMULTIPLIER * _currentZoomSpeed * (isDistanceGrowed ? -1 : 1), Time.deltaTime * INTERPOLATIONSPEED);
            _distanceToTarget = Mathf.Clamp(_distanceToTarget, _minZoomDistance, _maxZoomDistance);
        }
        
        //Вращение камеры с помощью мыши при зажатой левой клавише
        if (Input.GetMouseButton(0))
        {
            float mouseXValue = Input.GetAxis(MOUSEXAXIS) * _rotationSpeed;
            float mouseYValue = -Input.GetAxis(MOUSEYAXIS) * _rotationSpeed;

            _currentHorizontalAngle = Mathf.Lerp(_currentHorizontalAngle, _currentHorizontalAngle + mouseXValue, Time.deltaTime * INTERPOLATIONSPEED);
            _currentVerticalAngle = Mathf.Lerp(_currentVerticalAngle, _currentVerticalAngle + mouseYValue, Time.deltaTime * INTERPOLATIONSPEED);
            _currentVerticalAngle = Mathf.Clamp(_currentVerticalAngle, _minVerticalAngle, _maxVerticalAngle);
        }

        //Приближение камеры с помощью колеса мыши
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            float wheelValue = Input.GetAxis(MOUSEWHEELAXIS);

            _currentZoomSpeed = wheelValue != 0f ? _zoomSpeed : 0f;

            _distanceToTarget = Mathf.Lerp(_distanceToTarget, _distanceToTarget - wheelValue * _currentZoomSpeed, Time.deltaTime * INTERPOLATIONSPEED);
            _distanceToTarget = Mathf.Clamp(_distanceToTarget, _minZoomDistance, _maxZoomDistance);
        }
    }

    private void UpdateCameraTransform(float currentVerticalAngle, float currentHorizontalAngle, float distanceToTarget)
    {
        Quaternion rotation = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0);
        Vector3 offset = rotation * Vector3.back * distanceToTarget;

        transform.position = _target.position + offset;
        transform.LookAt(_target);
    }
}
