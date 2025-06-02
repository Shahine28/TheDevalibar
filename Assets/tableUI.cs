using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
public class tableUI : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Camera _cam;

    
    void Update()
    {
        transform.position = _cam.WorldToScreenPoint(_target.position);
    }
}
