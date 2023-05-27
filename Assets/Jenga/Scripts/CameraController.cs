using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    
    [SerializeField] Transform cameraOrbit;
    [SerializeField] Transform target;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cameraOrbit.position = target.position;
    }

    void Update()
    {
        if (target == null) return;
        
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0);

        transform.LookAt(target.position);
    }
    
    public void GoToNewStackTower(Transform newTarget)
    {
        Vector3 newPosition = cameraOrbit.position;
        newPosition.x = newTarget.position.x;
        cameraOrbit.position = newPosition;
        target = newTarget;
    }
}