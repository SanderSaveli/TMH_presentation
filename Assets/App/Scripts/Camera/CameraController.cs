using UnityEngine;

public class CameraController : MonoBehaviour, ICameraController
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _rotationSpeed = 100.0f, _zoomSpeed = 2.0f;
    [SerializeField] private float _minDistance = 5.0f, _maxDistance = 20.0f;
    [SerializeField] private float _smoothingSpeed = 5.0f;

    private Vector3 _currentEulerAngles;
    private Vector3 _targetEulerAngles;
    private float _distanceToTarget;
    private float _targetDistance;

    private bool _isRotatingMouse = false;
    private bool _isRotatingTouch = false;
    private Vector2 _lastTouchPosition;
    private float _initialPinchDistance;

    private void Start()
    {
        if (_target == null) 
        {
            Debug.LogError("Target not assigned for CameraController!");
            return;
        }

        _distanceToTarget = Vector3.Distance(transform.position, _target.position);
        _targetDistance = _distanceToTarget;
        _currentEulerAngles = transform.eulerAngles;
        _targetEulerAngles = _currentEulerAngles;
    }

    private void LateUpdate()
    {
        if (Input.touchCount > 0)
        {
            if (Input.touchCount == 1)
            {
                _isRotatingMouse = false;
                HandleTouchRotation();
            }
            else if (Input.touchCount == 2)
            {
                _isRotatingMouse = false;
                HandlePinchZoom();
            }
        }
        else
        {
            HandleMouseRotation();
            HandleMouseZoom();
        }

        SmoothCameraMovement();
    }

    private void HandleTouchRotation()
    {
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            _isRotatingTouch = true;
            _lastTouchPosition = touch.position;
        }

        if (touch.phase == TouchPhase.Moved && _isRotatingTouch)
        {
            Vector2 delta = touch.position - _lastTouchPosition;

            float horizontal = delta.x * _rotationSpeed * Time.deltaTime / Screen.width;
            float vertical = -delta.y * _rotationSpeed * Time.deltaTime / Screen.height;

            _targetEulerAngles.x += vertical;
            _targetEulerAngles.y += horizontal;

            _lastTouchPosition = touch.position;
        }

        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            _isRotatingTouch = false;
        }
    }

    private void HandlePinchZoom()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        if (touch2.phase == TouchPhase.Began)
        {
            _initialPinchDistance = Vector2.Distance(touch1.position, touch2.position);
            return;
        }

        if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
        {
            float currentDistance = Vector2.Distance(touch1.position, touch2.position);
            float pinchDelta = currentDistance - _initialPinchDistance;

            _targetDistance -= pinchDelta * _zoomSpeed * Time.deltaTime / Screen.width;
            _targetDistance = Mathf.Clamp(_targetDistance, _minDistance, _maxDistance);

            _initialPinchDistance = currentDistance;
        }
    }

    private void HandleMouseRotation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isRotatingMouse = true;
            _isRotatingTouch = false;
        }

        if (Input.GetMouseButtonUp(0))
            _isRotatingMouse = false;

        if (_isRotatingMouse)
        {
            float horizontal = Input.GetAxis("Mouse X") * _rotationSpeed * Time.deltaTime;
            float vertical = -Input.GetAxis("Mouse Y") * _rotationSpeed * Time.deltaTime;

            _targetEulerAngles.x += vertical;
            _targetEulerAngles.y += horizontal;
        }
    }

    private void HandleMouseZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            _targetDistance -= scroll * _zoomSpeed;
            _targetDistance = Mathf.Clamp(_targetDistance, _minDistance, _maxDistance);
        }
    }

    private void SmoothCameraMovement()
    {
        _distanceToTarget = Mathf.Lerp(_distanceToTarget, _targetDistance, Time.deltaTime * _smoothingSpeed);

        _currentEulerAngles = Vector3.Lerp(_currentEulerAngles, _targetEulerAngles, Time.deltaTime * _smoothingSpeed);

        Quaternion rotation = Quaternion.Euler(_currentEulerAngles);
        transform.position = _target.position - rotation * Vector3.forward * _distanceToTarget;
        transform.LookAt(_target);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        _distanceToTarget = Vector3.Distance(transform.position, _target.position);
        _targetDistance = _distanceToTarget;
        _currentEulerAngles = transform.eulerAngles;
        _targetEulerAngles = _currentEulerAngles;
    }
}