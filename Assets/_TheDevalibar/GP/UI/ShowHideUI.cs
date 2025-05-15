using System;
using System.Collections.Generic;
using MyUtilities;
using UnityEngine;

public class ShowHideUI : MonoBehaviour
{
    [SerializeField] private List<RectTransform> _UIElementsToHide = new List<RectTransform>();

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    public void HideUI()
    {
        foreach (RectTransform transform in _UIElementsToHide)
        {
            transform.gameObject.SetActive(false);
        }
    }

    public void ShowUI()
    {
        foreach (RectTransform transform in _UIElementsToHide)
        {
            transform.gameObject.SetActive(true);
        }
    }
}   

