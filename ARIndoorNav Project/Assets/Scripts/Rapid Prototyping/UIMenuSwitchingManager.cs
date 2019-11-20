using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIMenuSwitchingManager : MonoBehaviour
{
    public RectTransform mainMenu, settingsMenu;

    private float animationSpeed = 0.25f; // in seconds
    
    // Start is called before the first frame update
    void Start()
    {
        mainMenu.DOAnchorPos(new Vector2(0, -535), animationSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenSettingsMenu()
    {
        mainMenu.DOAnchorPos(new Vector2(-1200, -535), animationSpeed);
        settingsMenu.DOAnchorPos(new Vector2(0, -535), animationSpeed);
    }

    public void CloseSettingsMenu()
    {
        mainMenu.DOAnchorPos(new Vector2(0, -535), animationSpeed);
        settingsMenu.DOAnchorPos(new Vector2(1200, -535), animationSpeed);
    }
}
