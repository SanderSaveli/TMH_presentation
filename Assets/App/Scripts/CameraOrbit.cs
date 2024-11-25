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
    private Vector2 _touchStartPosition, _swipeDirection;
    private bool _isFingerContinuesToMove = false;

    /*
    Названия осей мыши вынесены в константы 
    для избежания "магических" значений
    и упрощенного редактирования 
    */
    private const string MOUSEXAXIS = "Mouse X", MOUSEYAXIS = "Mouse Y", MOUSEWHEELAXIS = "Mouse ScrollWheel", CENTERTAG = "Center";
    private const float PINCHTOZOOMMULTIPLIER = 0.0005f, INTERPOLATIONSPEED = 10f, TOUCHMOVEMENTMARGIN = 3f, ANGLETHRESHOLD = 0.1f;
    
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
                    _touchStartPosition = touch.position;
                    break;

                case TouchPhase.Moved:
                    _swipeDirection = (touch.position - _touchStartPosition).normalized;
                    break;

                case TouchPhase.Stationary:
                    _touchStartPosition = touch.position;
                    break;
            }

            _currentRotationSpeed = touch.deltaPosition.magnitude > TOUCHMOVEMENTMARGIN ? _rotationSpeed : 0f;

            _currentHorizontalAngle += _swipeDirection.x * _currentRotationSpeed;
            _currentVerticalAngle += -_swipeDirection.y * _currentRotationSpeed;
            _currentVerticalAngle = Mathf.Clamp(_currentVerticalAngle, _minVerticalAngle, _maxVerticalAngle);
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

            _distanceToTarget += currentDistanceBetweenTouches * PINCHTOZOOMMULTIPLIER * _currentZoomSpeed * (isDistanceGrowed ? -1 : 1);
            _distanceToTarget = Mathf.Clamp(_distanceToTarget, _minZoomDistance, _maxZoomDistance);
        }
        
        //Вращение камеры с помощью мыши при зажатой левой клавише
        if (Input.GetMouseButton(0))
        {
            float mouseXValue = Input.GetAxis(MOUSEXAXIS) * _rotationSpeed;
            float mouseYValue = -Input.GetAxis(MOUSEYAXIS) * _rotationSpeed;

            _currentHorizontalAngle += mouseXValue;
            _currentVerticalAngle += mouseYValue;
            _currentVerticalAngle = Mathf.Clamp(_currentVerticalAngle, _minVerticalAngle, _maxVerticalAngle);
        }

        //Приближение камеры с помощью колеса мыши
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            float wheelValue = Input.GetAxis(MOUSEWHEELAXIS);

            if(wheelValue != 0f)
            {
                _distanceToTarget -= wheelValue * _zoomSpeed;
                _distanceToTarget = Mathf.Clamp(_distanceToTarget, _minZoomDistance, _maxZoomDistance);
            }
        }
    }

    private void UpdateCameraTransform(float currentVerticalAngle, float currentHorizontalAngle, float distanceToTarget)
    {
        Quaternion rotation = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0);
        Vector3 offset = rotation * Vector3.back * distanceToTarget;

        transform.position = Vector3.Lerp(transform.position, _target.position + offset, Time.deltaTime * INTERPOLATIONSPEED);
        SmoothLookAtTarget(transform, _target.position, INTERPOLATIONSPEED, ANGLETHRESHOLD);
    }

    private void SmoothLookAtTarget(Transform rotatingObject, Vector3 targetPosition, float speed, float angleThreshold)
    {
        Vector3 direction = targetPosition - rotatingObject.position;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            if (Quaternion.Angle(rotatingObject.rotation, targetRotation) > angleThreshold)
                rotatingObject.rotation = Quaternion.Slerp(rotatingObject.rotation, targetRotation, Time.deltaTime * speed);
        }
    }
}
