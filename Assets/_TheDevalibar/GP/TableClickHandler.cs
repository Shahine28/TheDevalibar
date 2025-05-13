using UnityEngine;
using UnityEngine.EventSystems;

public class TableClickHandler : MonoBehaviour
{
    [SerializeField] private LayerMask _clickableLayer; // Filtre les objets cliquables (ex: décor, table, etc.)
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private CameraZoomToTarget _cameraZoom; // référence au script qui gère le reset

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Ignore les clics sur l'UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _clickableLayer))
            {
                // Vérifie si l'objet cliqué a un composant Table
                Table clickedTable = hit.collider.GetComponentInParent<Table>();
                if (clickedTable == null)
                {
                    _cameraZoom.ResetCamera(); // Appelle la fonction de retour caméra
                }
            }
        }
    }
}

