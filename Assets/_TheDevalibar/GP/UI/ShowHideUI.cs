using System;
using System.Collections.Generic;
using MyUtilities;
using UnityEngine;

public class ShowHideUI : MonoBehaviour
{
    [SerializeField] private List<RectTransform> _UIElementsToHide = new List<RectTransform>();
    [SerializeField] private List<MoveUI> _UIElementsToMove = new List<MoveUI>();
    private CameraZoomToTarget _cameraZoomToTarget;
    private CameraMovementAndZoomControl _cameraMovementAndZoomControl;

    public event Action OnUIShowed;
    public event Action OnUIHided;

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    void Start()
    {
        _cameraZoomToTarget = ServiceLocator.Get<CameraZoomToTarget>();
        _cameraMovementAndZoomControl = ServiceLocator.Get<CameraMovementAndZoomControl>();
    }
    public void HideUI()
    {
        _cameraMovementAndZoomControl.CanZoom = true;
        HideSlidingUI();
        foreach (RectTransform transform in _UIElementsToHide)
        {
            transform.gameObject.SetActive(false);
        }
        OnUIHided?.Invoke();
    }

    public void HideSlidingUI()
    {
        foreach (MoveUI moveUI in _UIElementsToMove)
        {
            moveUI.LaunchMoveUI(true);
        }
    }
    
    public void ShowSlidingUI()
    {
        foreach (MoveUI moveUI in _UIElementsToMove)
        {
            moveUI.LaunchMoveUI();
        }
    }

    public void ShowUI()
    {
        _cameraMovementAndZoomControl.CanZoom = false;
        _cameraZoomToTarget.ResetCamera();
        TabsManager tabsManager = ServiceLocator.Get<TabsManager>();
        if (tabsManager != null)
        {
            tabsManager.SelectFirstTabs();
        }
        ShowSlidingUI();
        foreach (RectTransform transform in _UIElementsToHide)
        {
            transform.gameObject.SetActive(true);
        }
        OnUIShowed?.Invoke();
    }
}   

