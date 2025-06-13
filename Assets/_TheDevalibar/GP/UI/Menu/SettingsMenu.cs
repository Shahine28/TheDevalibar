using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;


public class SettingsMenu : MonoBehaviour
{
    [SerializedDictionary("Menu Type", "GameObject"), SerializeField]
    private SerializedDictionary<MenuType, GameObject> _settingsMenus = new SerializedDictionary<MenuType,  GameObject>();
    
    
    public void ShowSettingsMenuByType(MenuType rebindMenuType)
    {
        _settingsMenus[rebindMenuType]?.gameObject.SetActive(true);
        List<MenuType> rebindMenuTypes = _settingsMenus.Keys.ToList();
        for (int i = 0; i < rebindMenuTypes.Count; i++)
        {
            if (rebindMenuTypes[i] != rebindMenuType)
            {
                _settingsMenus[rebindMenuTypes[i]]?.gameObject.SetActive(false);
            }
        }
    }

    public void ShowAudioMenu()
    {
        ShowSettingsMenuByType(MenuType.Audio);
    }
    
    public void ShowRebindMenu()
    {
        ShowSettingsMenuByType(MenuType.Rebind);
        _settingsMenus[MenuType.Rebind]?.GetComponent<RebindMenu>()?.ResetRebindMenu();
    }
    
    public void ShowTextMenu()
    {
        ShowSettingsMenuByType(MenuType.Text);
    }

    private void OnEnable()
    {
        ShowAudioMenu();
    }
}

[Serializable]
public enum MenuType
{
    Audio,
    Rebind,
    Text,
}