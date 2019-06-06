using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;
using System;
using System.Drawing;

public class SceneController : MonoBehaviour
{
    public Camera firstPersonCamera;
    public Text debugText;

    private NavigationController navigationController;
    private PoseController poseController;
    private readonly List<DetectedPlane> _detectedPlanes = new List<DetectedPlane>();
    private readonly List<AugmentedImage> _detectedImages = new List<AugmentedImage>();

    // QuitOnConnectionErrors checks the state of the ARCore Session.
    void Start()
    {
        QuitOnConnectionErrors();
        navigationController = GetComponent<NavigationController>();
        poseController = GetComponent<PoseController>();
    }


    void Update()
    {
        ProcessTouches();

        // If tracking failed, no calculations can be made.
        // Any code below this point relies on a sucessful tracking.
        if (ProcessTracking() == false)
        {
            return;
        }

        // Align the real world data with the virtual one
        if (poseController != null)
        {
            poseController.UpdateARScene(_detectedImages);
        }
        
        // TODO: Decide on logic when to rebake the mesh after UpdateARScene
        // Function: navigationController.BakeMesh();

        // Draw navigation elements into the scene
        if (navigationController != null)
        {
            navigationController.DrawNextPathCorner(_detectedPlanes);
        }

    }

    /* Most of tracking related processes require the ground floor as a reference point. 
     * This method determines the biggest upwards facing plane as the ground floor.
     * It also updates the static variable floorPlane.
     * Returns true if succesfull
     * Returns false if tracking failed
     */
    private bool ProcessTracking()
    {
        /* ARCore needs to capture and process enough information to start tracking the user's movements in the real world. 
         * Once ARCore is tracking, the Frame object is used to interact with ARCore.
         *
         * Source: https://codelabs.developers.google.com/codelabs/arcore-intro/index.html?index=..%2F..io2018#2 
         */
        if (Session.Status != SessionStatus.Tracking)
        {
            //Resets the screen timeout after tracking is lost.
            int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            return false;
        }
        //Adjusting screen timeout so that the screen stays on when tracking
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // Override _detectedPlanes with all tracked Planes
        Session.GetTrackables<DetectedPlane>(_detectedPlanes, TrackableQueryFilter.All);

        // Override _detectedImages with all tracked Augmented Images
        Session.GetTrackables(_detectedImages, TrackableQueryFilter.Updated);
        return true;
    }


    /* To process the touches, we get a single touch and raycast it using the ARCore session to check if the user tapped on a plane. 
     * If so, we'll use that one to display the rest of the objects.
     * 
     * 
     * Source: https://codelabs.developers.google.com/codelabs/arcore-intro/index.html?index=..%2F..io2018#6
     */
    void ProcessTouches()
    {
        Touch touch;

        // Only a single touch that didn't move, was stationary or ended gets processed
        if (Input.touchCount != 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        Frame.Pose.rotation.SetLookRotation(new Vector3(0, 0, 0));

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;
        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {

            // Custom.method(hit.trackable as DetectedPlane);
        }
    }


    /* This method checks the state of the ARCore Session to make sure ARCore is working in our app:
     * 
     * 1: Is the permission to use the camera granted? ARCore uses the camera to sense the real world. 
     * The user is prompted to grant this permission the first time the application is run. 
     * This check is done by ARCore automatically.
     * 
     * 2: Can the ARCore library connect to the ARCore Services? ARCore relies on AR Services which runs on the device in a separate process.
     * 
     * Source: https://codelabs.developers.google.com/codelabs/arcore-intro/index.html?index=..%2F..io2018#2 
     */
    void QuitOnConnectionErrors()
    {
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            StartCoroutine(ToastAndExit(
                "Camera permission is needed to run this application.", 5));
        }
        else if (Session.Status.IsError())
        {
            StartCoroutine(ToastAndExit(
                "ARCore encountered a problem connecting. Please restart the app.", 5));
        }
    }

    /// <summary>Coroutine to display an error then exit.</summary>
    /// Source: https://codelabs.developers.google.com/codelabs/arcore-intro/index.html?index=..%2F..io2018#0
    public static IEnumerator ToastAndExit(string message, int seconds)
    {
        _ShowAndroidToastMessage(message);
        yield return new WaitForSeconds(seconds);
        Application.Quit();
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    /// <param name="length">Toast message time length.</param>
    public static void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass =
                new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject =
                    toastClass.CallStatic<AndroidJavaObject>(
                        "makeText", unityActivity,
                        message, 0);
                toastObject.Call("show");
            }));
        }
    }

}
