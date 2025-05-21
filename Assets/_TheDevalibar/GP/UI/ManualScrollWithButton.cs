using System.Collections;
using UnityEngine;
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


    void Start()
    {
        if (!_scrollRect)
        {
            _scrollRect = GetComponent<ScrollRect>();
        }
    }
    void OnEnable()
    {
        _upOrRightButtonHoldTracker = _upOrRightButton?.gameObject.AddComponent<ButtonHoldTracker>();
        if (_upOrRightButtonHoldTracker)
        {
            _upOrRightButtonHoldTracker.OnPointerStateUpdated += HandleScrollStateChanged;
        }
           
        _downOrLeftButtonHoldTracker = _downOrLeftButton?.gameObject.AddComponent<ButtonHoldTracker>();
        if (_downOrLeftButtonHoldTracker)
        {
            _downOrLeftButtonHoldTracker.OnPointerStateUpdated += HandleScrollStateChanged;
        }
    }

    void OnDisable()
    {
        if (_upOrRightButtonHoldTracker)
        {
            _upOrRightButtonHoldTracker.OnPointerStateUpdated -= HandleScrollStateChanged;
        }

        if (_downOrLeftButtonHoldTracker)
        { 
            _downOrLeftButtonHoldTracker.OnPointerStateUpdated -= HandleScrollStateChanged;
        }
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

   

    // Update is called once per frame
    void Update()
    {
        
    }
}
