using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARVisuals_HapticFeedback : MonoBehaviour, IARVisuals
{
    public NavigationPresenter _NavigationPresenter;
    public GameObject _StartStopButton;
    public GameObject _WhiteScreen;

    float closenessBreakpoint1 = 0.81f; // 0.9 x 0.9
    float closenessBreakpoint2 = 0.25f; // 0.5 x 0.5
    float closenessBreakpoint3 = 0.04f; // 0.2 x 0.2

    private float nextActionTime = 0.0f;
    private float period = 0.1f;

    private bool isVibrating = false;
    private bool stopAll = false;
    private bool startedOnce = false;

    void Update()
    {
        if (stopAll || startedOnce == false)
            return;

        PulseVibrate();
    }

    /**
     * This function is called each update cycle.
     * It will alternate between vibrating the phone and pausing for "period" seconds
     */
    private void PulseVibrate()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;

            if (isVibrating)
            {
                Vibration.Cancel();
                HideWhiteScreen();
                isVibrating = false;
            }
            else
            {
                Handheld.Vibrate();
                ShowWhiteScreen();
                isVibrating = true;
            }
        }
    }

    private void ShowWhiteScreen()
    {
        _WhiteScreen.SetActive(true);
    }

    private void HideWhiteScreen()
    {
        _WhiteScreen.SetActive(false);
    }

    public void ClearARDisplay()
    {
        StopVibration();
        _StartStopButton.SetActive(false);
        HideWhiteScreen();
    }

    /**
     * Called by NavigationPresenter each update cycle after a destination has been chosen
     * After calculating the position of the next set relative towards where the user is pointing
     * their device, a vibration pattern will be generated to guide them
     */
    public void SendNavigationInformation(NavigationInformation navigationInformation)
    {
        _StartStopButton.SetActive(true);
        startedOnce = true;
        Vector2 screen = navigationInformation.Vector3ToScreenPos();
        var tmpx = screen.x;
        var tmpy = screen.y;

        float middleX = HelperFunctions.BetweenZeroAndOne(tmpx, Screen.width);
        float middleY = HelperFunctions.BetweenZeroAndOne(tmpy, Screen.height);

        float closeness = middleX * middleY;

        if (closeness >= closenessBreakpoint1)
        {

            //Vibrate(pattern1);
            period = 0.1f;
        }
        else if (closeness >= closenessBreakpoint2)
        {

            //Vibrate(pattern2);
            period = 0.2f;
        }
        else if (closeness >= closenessBreakpoint3)
        {

            //Vibrate(pattern3);
            period = 0.3f;
        }
        else
        {

            //NoVibrate();
            period = 0.4f;
            //Vibrate(pattern4);
        }


        //Debug.Log("Closeness: " + closeness);

    }

    public void StartStop()
    {
        if (stopAll)
        {
            StartVibration();
        }
        else
        {
            StopVibration();
        }
    }

    private void StopVibration()
    {
        Vibration.Cancel();
        isVibrating = false;
        stopAll = true;
    }

    private void StartVibration()
    {
        stopAll = false;
    }



}
