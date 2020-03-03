using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class COMPoseTracking : MonoBehaviour
{
    public Transform _ARCoreOriginTransform;
    public Transform _ARCoreFPSTransform;

    // Start is called before the first frame update
    void Start()
    {
        QuitOnConnectionErrors();

    }

    // Update is called once per frame
    void Update()
    {
        // Maybe FeaturePoint drift
    }

    /**
     * Returns the origin position of ARCore
     * The origin is a point in space relative to the ARCore first person camera
     */
    public Vector3 GetOriginPosition()
    {
        return _ARCoreOriginTransform.position;
    }

    /**
     * Sets the ARCore origin position to the new value
     * Called when PoseEstimation calculated a new pose
     */
    public void SetOriginPosition(Vector3 newPosition)
    {
        _ARCoreOriginTransform.position = newPosition;
    }

    /**
    * Returns the origin rotation of ARCore
    * The origin is a point in space relative to the ARCore first person camera
    */
    public Quaternion GetOriginRotation()
    {
        return _ARCoreOriginTransform.rotation;
    }

    /**
     * Sets the ARCore origin rotation to the new value
     * Called when PoseEstimation calculated a new pose
     */
    public void SetOriginRotation(Quaternion newRotation)
    {
        _ARCoreOriginTransform.rotation = newRotation;
    }

    /**
     * Rotates the origin around a point
     * Used when the user manually adjusts the rotation 
     */
    public void RotateAroundOrigin(float rotationDegree)
    {
        _ARCoreOriginTransform.transform.RotateAround(_ARCoreFPSTransform.transform.position, Vector3.up, rotationDegree);
    }

    /**
     * Returns the total position of the ARCore first person camera
     * The origin pose is included in this
     */
    public Vector3 GetUnityPosition()
    {
        return _ARCoreFPSTransform.position;
    }

    /**
     * Returns the total rotation of the ARCore first person camera
     * The origin pose is included in this
     */
    public Quaternion GetUnityRotation()
    {
        return _ARCoreFPSTransform.rotation;
    }


    /**
    ARCore needs to capture and process enough information to start tracking the user's movements in the real world.
    Once ARCore is tracking, the Frame object is used to interact with ARCore.
    Source: https://codelabs.developers.google.com/codelabs/arcore-intro/index.html?index=..%2F..io2018#2 

    Returns true if ARCore is currently tracking
    */
    private bool IsInTrackingState()
    {
        // Case: ARCore is not tracking
        if (Session.Status != SessionStatus.Tracking)
        {
            //Resets the screen timeout after tracking is lost.
            int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            return false;
        }
        // Case: ARCore is tracking
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        return true;
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
