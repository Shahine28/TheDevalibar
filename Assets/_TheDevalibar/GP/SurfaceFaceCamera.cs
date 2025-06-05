using UnityEngine;

public class SurfaceFaceCamera : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    void Start()
    {
        if (!_mainCamera)
        {
            _mainCamera = Camera.main;
        }
    }
    
    void LateUpdate()
    {
        if (!_mainCamera) return;
        Vector3 cameraPosition = _mainCamera.transform.position;
        
        cameraPosition.y = _mainCamera.transform.position.y;
        transform.LookAt(cameraPosition);
        transform.Rotate(0f, 180f, 0f);
    }
}
