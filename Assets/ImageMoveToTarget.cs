using UnityEngine;

 [ExecuteAlways]
public class ImageMoveToTarget : MonoBehaviour
{

    public Canvas _canvas;
    public Camera _camera;
    public GameObject _target;
    public Vector3 _offset;

    private void Update()
    {
        var pos = _camera.WorldToScreenPoint(_target.transform.position);
        transform.position = pos + _offset;
    }


}
