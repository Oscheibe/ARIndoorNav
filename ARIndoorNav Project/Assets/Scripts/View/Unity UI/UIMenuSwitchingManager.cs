using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIMenuSwitchingManager : MonoBehaviour
{
    public RectTransform _mainMenu, _settingsMenu, _navigationMenu, _destinationHeader;

    private float animationSpeed = 0.25f; // in seconds

    private Vector2 mainInViewPos = new Vector2(0, -560);
    private Vector2 mainOutOfViewPos = new Vector2(0, -2000);

    private Vector2 settingsInViewPos = new Vector2(0, -560);
    private Vector2 settingsOutOfViewPos = new Vector2(0, -2000);

    private Vector2 navigationInViewPos = new Vector2(0, -560);
    private Vector2 navigationOutOfViewPos = new Vector2(0, -2000);

    private Vector2 destinationHeaderInViewPos = new Vector2(0, 1070);
    private Vector2 destinationHeaderOutOfViewPos = new Vector2(0, 2000);

    // Start is called before the first frame update
    void Start()
    {
        OpenNavigationMenu();
        OpenDestinationHeader();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenMainMenu()
    {
        _mainMenu.DOAnchorPos(mainInViewPos, animationSpeed);
    }

    public void CloseMainMenu()
    {
        _mainMenu.DOAnchorPos(mainOutOfViewPos, animationSpeed);
    }
    public void OpenSettingsMenu()
    {
        _settingsMenu.DOAnchorPos(settingsInViewPos, animationSpeed);
    }

    public void CloseSettingsMenu()
    {
        _settingsMenu.DOAnchorPos(settingsOutOfViewPos, animationSpeed);
    }

    public void OpenNavigationMenu()
    {
        _navigationMenu.DOAnchorPos(navigationInViewPos, animationSpeed);
    }

    public void CloseNavigationMenu()
    {
        _navigationMenu.DOAnchorPos(navigationOutOfViewPos, animationSpeed);
    }

    public void OpenDestinationHeader()
    {
        _destinationHeader.DOAnchorPos(destinationHeaderInViewPos, animationSpeed);
    }

    public void CloseDestinationHeader()
    {
        _destinationHeader.DOAnchorPos(destinationHeaderOutOfViewPos, animationSpeed);
    }
}
