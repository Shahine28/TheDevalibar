using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyUtilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ManualScrollWithButton : MonoBehaviour
{

    [Header("Buttons")] 
    [SerializeField] private Button _upOrRightButton;
    private ButtonHoldTracker _upOrRightButtonHoldTracker;
    [SerializeField, ReadOnly] private bool _isButtonUpOrRightHold;
    [SerializeField] private Button _downOrLeftButton;
    private ButtonHoldTracker _downOrLeftButtonHoldTracker;
    [SerializeField, ReadOnly] private bool _isButtonDownOrLeftHold;
    
    [Header("Scroll View")]
    [SerializeField] private ScrollRect _scrollRect;

    [SerializeField] private bool _scrollHorizontally;
    private Coroutine _scrollCoroutine;
    [SerializeField] private float _scrollSpeed = 0.5f;

    [SerializeField] private Transform _scrollListContainer;
    private GameObject _lastSelectedGameObject;


    void Start()
    {
        if (!_scrollRect)
        {
            _scrollRect = GetComponent<ScrollRect>();
        }
    }
    void OnEnable()
    {
        _upOrRightButton.onClick.AddListener(GoUpOrRight);
        _downOrLeftButton.onClick.AddListener(GoDownOrLeft);
        
        // _upOrRightButtonHoldTracker = _upOrRightButton?.gameObject.AddComponent<ButtonHoldTracker>();
        // if (_upOrRightButtonHoldTracker)
        // {
        //     _upOrRightButtonHoldTracker.OnPointerStateUpdated += HandleScrollStateChanged;
        // }
        //    
        // _downOrLeftButtonHoldTracker = _downOrLeftButton?.gameObject.AddComponent<ButtonHoldTracker>();
        // if (_downOrLeftButtonHoldTracker)
        // {
        //     _downOrLeftButtonHoldTracker.OnPointerStateUpdated += HandleScrollStateChanged;
        // }
    }

    void OnDisable()
    {
        _upOrRightButton.onClick.RemoveListener(GoUpOrRight);
        _downOrLeftButton.onClick.RemoveListener(GoDownOrLeft);
        
        // if (_upOrRightButtonHoldTracker)
        // {
        //     _upOrRightButtonHoldTracker.OnPointerStateUpdated -= HandleScrollStateChanged;
        // }
        //
        // if (_downOrLeftButtonHoldTracker)
        // { 
        //     _downOrLeftButtonHoldTracker.OnPointerStateUpdated -= HandleScrollStateChanged;
        // }
    }
    
    public void GoUpOrRight()
    {
        if (_lastSelectedGameObject == null || _lastSelectedGameObject.transform.parent != _scrollListContainer)
        {
            SelectFirstChild();
            return;
        }

        Selectable current = _lastSelectedGameObject.GetComponent<Selectable>();
        if (current == null) return;

        Selectable next = _scrollHorizontally ? current.FindSelectableOnRight() : current.FindSelectableOnUp();;

        if (next != null)
        {
            if (next.transform.IsChildOf(_scrollListContainer.transform))
            {
                EventSystem.current.SetSelectedGameObject(next.gameObject);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(_lastSelectedGameObject.gameObject);
            }
        }
        
    }

    public void GoDownOrLeft()
    {
        if (_lastSelectedGameObject == null || _lastSelectedGameObject.transform.parent != _scrollListContainer)
        {
            SelectFirstChild();
            return;
        }

        Selectable current = _lastSelectedGameObject.GetComponent<Selectable>();
        if (current == null) return;

        Selectable next = _scrollHorizontally ? current.FindSelectableOnLeft() : current.FindSelectableOnDown();

        if (next != null)
        {
            if (next.transform.IsChildOf(_scrollListContainer.transform))
            {
                EventSystem.current.SetSelectedGameObject(next.gameObject);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(_lastSelectedGameObject.gameObject);
            }
        }
    }

    public void SelectFirstChild()
    {
        Transform firstActiveChild = _scrollListContainer.transform
            .Cast<Transform>()
            .FirstOrDefault(child => child.gameObject.activeInHierarchy &&
                                     child.GetComponent<Selectable>() != null);

        if (firstActiveChild != null)
        {
            EventSystem.current.SetSelectedGameObject(firstActiveChild.gameObject);
        }
        else
        {
            TabsManager tabsManager = ServiceLocator.Get<TabsManager>();
            if (tabsManager != null)
            {
                tabsManager.SelectFirstTabs();
            }
        }
    }

    public void SelectFirstChildDelayed()
    {
        StartCoroutine(SelectFirstChildCoroutine());
    }

    IEnumerator SelectFirstChildCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        SelectFirstChild();
    }

    public void SetNewSelectedGameObject()
    {
        _lastSelectedGameObject = EventSystem.current.currentSelectedGameObject;
    }
    public void SetNewSelectedGameObject(GameObject newSelectedGameObject)
    {
        _lastSelectedGameObject = newSelectedGameObject;
    }

    public void ResetSelectedGameObject()
    {
        _lastSelectedGameObject = null;
    }
    
    
    private void HandleScrollStateChanged()
    {
        _isButtonUpOrRightHold = _upOrRightButtonHoldTracker?.IsHolding ?? false;
        _isButtonDownOrLeftHold = _downOrLeftButtonHoldTracker?.IsHolding ?? false;
        
        
        if ((_isButtonUpOrRightHold || _isButtonDownOrLeftHold) && _scrollCoroutine == null)
        {
            _scrollCoroutine = StartCoroutine(ScrollContinuously());
        }
    }
    
    private IEnumerator ScrollContinuously()
    {
        while (_isButtonUpOrRightHold || _isButtonDownOrLeftHold)
        {
            float direction = 0f;

            if (_isButtonUpOrRightHold)
                direction = 1f;
            else if (_isButtonDownOrLeftHold)
                direction = -1f;

            if (_scrollHorizontally)
            {
                _scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(
                    _scrollRect.horizontalNormalizedPosition + direction * _scrollSpeed * Time.deltaTime);
            }
            else
            {
                _scrollRect.verticalNormalizedPosition = Mathf.Clamp01(
                    _scrollRect.verticalNormalizedPosition + direction * _scrollSpeed * Time.deltaTime);
            }

            yield return null;
        }

        _scrollCoroutine = null; // fin de scroll
    }
}
