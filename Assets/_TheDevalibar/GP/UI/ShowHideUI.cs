using System;
using System.Collections.Generic;
using MyUtilities;
using UnityEngine;

public class ShowHideUI : MonoBehaviour
{
    [SerializeField] private List<RectTransform> _UIElementsToHide = new List<RectTransform>();
    [SerializeField] private List<MoveUI> _UIElementsToMove = new List<MoveUI>();

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    public void HideUI()
    {
        HideSlidingUI();
        foreach (RectTransform transform in _UIElementsToHide)
        {
            transform.gameObject.SetActive(false);
        }
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
        ShowSlidingUI();
        foreach (RectTransform transform in _UIElementsToHide)
        {
            transform.gameObject.SetActive(true);
        }
    }
}   

