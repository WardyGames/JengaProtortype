using UnityEngine;

public class CameraMouse : MonoBehaviour
{
    public Transform target;
    public Camera sceneCamera;

    [Range(5f, 15f)] [Tooltip("How sensitive the mouse drag to camera rotation")]
    public float mouseRotateSpeed = 5f;

    [Range(10f, 50f)] [Tooltip("How sensitive the touch drag to camera rotation")]
    public float touchRotateSpeed = 10f;

    [Tooltip("Smaller positive value means smoother rotation, 1 means no smooth apply")]
    public float slerpSmoothValue = 0.3f;

    [Tooltip("How long the smoothDamp of the mouse scroll takes")]
    public float scrollSmoothTime = 0.12f;

    public float editorFOVSensitivity = 5f;

    public float touchFOVSensitivity = 5f;

    //Can we rotate camera, which means we are not blocking the view
    bool _canRotate = true;
    Vector2 _swipeDirection; //swipe delta vector2
    Vector2 _touch1OldPos;
    Vector2 _touch2OldPos;
    Vector2 _touch1CurrentPos;
    Vector2 _touch2CurrentPos;
    Quaternion _currentRot; // store the quaternion after the slerp operation
    Quaternion _targetRot;

    Touch _touch;

    //Mouse rotation related
    float _rotX; // around x

    float _rotY; // around y

    //Mouse Scroll
    float _cameraFieldOfView;
    float _cameraFOVDamp; //Damped value
    float _fovChangeVelocity = 0;

    float _distanceBetweenCameraAndTarget;

    //Clamp Value
    float _minXRotAngle = -85; //min angle around x axis
    float _maxXRotAngle = 85; // max angle around x axis
    float _minCameraFieldOfView = 6;
    float _maxCameraFieldOfView = 30;
    Vector3 _dir;

    void Awake()
    {
        GetCameraReference();
    }

    // Start is called before the first frame update
    void Start()
    {
        _distanceBetweenCameraAndTarget = Vector3.Distance(sceneCamera.transform.position, target.position);
        _dir = new Vector3(0, 0,
            _distanceBetweenCameraAndTarget); //assign value to the distance between the maincamera and the target
        sceneCamera.transform.position = target.position + _dir; //Initialize camera position
        _cameraFOVDamp = sceneCamera.fieldOfView;
        _cameraFieldOfView = sceneCamera.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_canRotate)
        {
            return;
        }

        if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            EditorCameraInput();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            FrontView();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            TopView();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LeftView();
        }
    }

    void LateUpdate()
    {
        RotateCamera();
        SetCameraFOV();
    }

    public void GetCameraReference()
    {
        if (sceneCamera == null)
        {
            sceneCamera = Camera.main;
        }
    }

    //May be the problem with Euler angles
    public void TopView()
    {
        _rotX = -85;
        _rotY = 0;
    }

    public void LeftView()
    {
        _rotY = 90;
        _rotX = 0;
    }

    public void FrontView()
    {
        _rotX = 0;
        _rotY = 0;
    }

    void EditorCameraInput()
    {
        //Camera Rotation
        if (Input.GetMouseButton(0))
        {
            _rotX += Input.GetAxis("Mouse Y") * mouseRotateSpeed; // around X
            _rotY += Input.GetAxis("Mouse X") * mouseRotateSpeed;
            if (_rotX < _minXRotAngle)
            {
                _rotX = _minXRotAngle;
            }
            else if (_rotX > _maxXRotAngle)
            {
                _rotX = _maxXRotAngle;
            }
        }

        //Camera Field Of View
        if (Input.mouseScrollDelta.magnitude > 0)
        {
            _cameraFieldOfView += Input.mouseScrollDelta.y * editorFOVSensitivity * -1; //-1 make FOV change natual
        }
    }

    void RotateCamera()
    {
        if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Vector3 tempV = new Vector3(_rotX, _rotY, 0);
            _targetRot = Quaternion.Euler(tempV); //We are setting the rotation around X, Y, Z axis respectively
        }
        else
        {
            _targetRot = Quaternion.Euler(-_swipeDirection.y, _swipeDirection.x, 0);
        }

        //Rotate Camera
        _currentRot =
            Quaternion.Slerp(_currentRot, _targetRot,
                Time.smoothDeltaTime * slerpSmoothValue *
                50); //let cameraRot value gradually reach newQ which corresponds to our touch
        //Multiplying a quaternion by a Vector3 is essentially to apply the rotation to the Vector3
        //This case it's like rotate a stick the length of the distance between the camera and the target and then look at the target to rotate the camera.
        sceneCamera.transform.position = target.position + _currentRot * _dir;
        sceneCamera.transform.LookAt(target.position);
    }

    void SetCameraFOV()
    {
        //Set Camera Field Of View
        //Clamp Camera FOV value
        if (_cameraFieldOfView <= _minCameraFieldOfView)
        {
            _cameraFieldOfView = _minCameraFieldOfView;
        }
        else if (_cameraFieldOfView >= _maxCameraFieldOfView)
        {
            _cameraFieldOfView = _maxCameraFieldOfView;
        }

        _cameraFOVDamp = Mathf.SmoothDamp(_cameraFOVDamp, _cameraFieldOfView, ref _fovChangeVelocity, scrollSmoothTime);
        sceneCamera.fieldOfView = _cameraFOVDamp;
    }
}