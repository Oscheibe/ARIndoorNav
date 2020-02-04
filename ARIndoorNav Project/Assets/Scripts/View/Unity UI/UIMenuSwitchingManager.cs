using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIMenuSwitchingManager : MonoBehaviour
{
    public RectTransform _mainMenu,
                        _settingsMenu,
                        _navigationMenu,
                        _destinationHeader,
                        _markerDetectionMenu,
                        _systemInformationHeader,
                        _confirmScreen;

    private float animationSpeed = 0.25f; // in seconds

    private Vector2 mainInViewPos = new Vector2(0, -560);
    private Vector2 mainOutOfViewPos = new Vector2(0, -2000);

    private Vector2 settingsInViewPos = new Vector2(0, -560);
    private Vector2 settingsOutOfViewPos = new Vector2(0, -2000);

    private Vector2 navigationInViewPos = new Vector2(0, -560);
    private Vector2 navigationOutOfViewPos = new Vector2(0, -2000);

    private Vector2 destinationHeaderInViewPos = new Vector2(0, 1070 - 50);
    private Vector2 destinationHeaderOutOfViewPos = new Vector2(0, 2000);

    private Vector2 markerDetectionMenuInViewPos = new Vector2(0, 0);
    private Vector2 markerDetectionMenuOutOfViewPos = new Vector2(0, 2000);

    private Vector2 systemInformationInViewPos; // 200px under destinationHeader
    private Vector2 systemInformationOutOfViewPos = new Vector2(0, 2000);

    private Vector2 confirmScreenInViewPos = new Vector2(0, 0);
    private Vector2 confirmScreenOutOfViewPos = new Vector2(0, -2500);

    // Start is called before the first frame update
    void Start()
    {
        systemInformationInViewPos = destinationHeaderInViewPos - new Vector2(0, 200);
        OpenMarkerDetectionMenu();
    }

    private void CloseAllMenus()
    {
        CloseMainMenu();
        CloseSettingsMenu();
        CloseNavigationMenu();
        CloseDestinationHeader();
        CloseMarkerDetectionMenu();
        CloseConfirmScreen();
    }

    public void OpenMainMenu()
    {
        CloseAllMenus();
        OpenDestinationHeader();
        _mainMenu.DOAnchorPos(mainInViewPos, animationSpeed);
    }

    private void CloseMainMenu()
    {
        _mainMenu.DOAnchorPos(mainOutOfViewPos, animationSpeed);
    }
    public void OpenSettingsMenu()
    {
        CloseAllMenus();
        _settingsMenu.DOAnchorPos(settingsInViewPos, animationSpeed);
    }

    private void CloseSettingsMenu()
    {
        _settingsMenu.DOAnchorPos(settingsOutOfViewPos, animationSpeed);
    }

    public void OpenNavigationMenu()
    {
        CloseAllMenus();
        OpenDestinationHeader();
        _navigationMenu.DOAnchorPos(navigationInViewPos, animationSpeed);
    }

    private void CloseNavigationMenu()
    {
        _navigationMenu.DOAnchorPos(navigationOutOfViewPos, animationSpeed);
    }

    private void OpenDestinationHeader()
    {
        CloseAllMenus();
        _destinationHeader.DOAnchorPos(destinationHeaderInViewPos, animationSpeed);
    }

    private void CloseDestinationHeader()
    {
        _destinationHeader.DOAnchorPos(destinationHeaderOutOfViewPos, animationSpeed);
    }

    public void OpenMarkerDetectionMenu()
    {
        CloseAllMenus();
        _markerDetectionMenu.DOAnchorPos(markerDetectionMenuInViewPos, animationSpeed);
    }

    private void CloseMarkerDetectionMenu()
    {
        _markerDetectionMenu.DOAnchorPos(markerDetectionMenuOutOfViewPos, animationSpeed);
    }

    public void OpenSystemInformationHeader()
    {
        // Does not close all menus when called
        _systemInformationHeader.DOAnchorPos(systemInformationInViewPos, animationSpeed);
    }

    public void CloseSystemInformationHeader()
    {
        _systemInformationHeader.DOAnchorPos(systemInformationOutOfViewPos, animationSpeed);
    }

    public void OpenConfirmScreen()
    {
        CloseAllMenus();
        _confirmScreen.DOAnchorPos(confirmScreenInViewPos, animationSpeed);
    }

    private void CloseConfirmScreen()
    {
        _confirmScreen.DOAnchorPos(confirmScreenOutOfViewPos, animationSpeed);
    }
}
