using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;


public class RebindMenu : MonoBehaviour
{
    [SerializedDictionary("Rebind Menu Type", "GameObject"), SerializeField]
    private SerializedDictionary<RebindMenuType, GameObject> _rebindMenus = new SerializedDictionary<RebindMenuType,  GameObject>();
    
    public void ResetRebindMenu()
    {
        ShowRebindMenuByType(RebindMenuType.MainMenu);
    }
    
    public void ShowKeyboardRebindMenu()
    {
        ShowRebindMenuByType(RebindMenuType.Keyboard);
    }
    public void ShowMouseRebindMenu()
    {
        ShowRebindMenuByType(RebindMenuType.Mouse);
    }
    public void ShowGamepadRebindMenu()
    {
        ShowRebindMenuByType(RebindMenuType.Gamepad);
    }
    public void ShowRebindMenuByType(RebindMenuType rebindMenuType)
    {
        _rebindMenus[rebindMenuType]?.gameObject.SetActive(true);
        List<RebindMenuType> rebindMenuTypes = _rebindMenus.Keys.ToList();
        for (int i = 0; i < rebindMenuTypes.Count; i++)
        {
            if (rebindMenuTypes[i] != rebindMenuType)
            {
                _rebindMenus[rebindMenuTypes[i]]?.gameObject.SetActive(false);
            }
        }
    }

    
    public void OnEnable()
    {
        ResetRebindMenu();
    }
}

[Serializable]
public enum RebindMenuType
{
    MainMenu,
    Keyboard,
    Mouse,
    Gamepad,
}
