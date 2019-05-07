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
    
    static DetectedPlane floorPlane;

    private GameObject arrowInstance;
    private List<GameObject> objectList = new List<GameObject>();
    private NavigationController navigation;
    private readonly List<DetectedPlane> m_NewPlanes = new List<DetectedPlane>();

    // QuitOnConnectionErrors checks the state of the ARCore Session.
    void Start()
    {
        QuitOnConnectionErrors();
        navigation = GetComponent<NavigationController>();
    }

    
    void Update()
    {
        ProcessTracking();
        ProcessTouches();

        float floorHeight;
        if(floorPlane == null){
            floorHeight = -1f; //TODO: Change behaviour from debugging state to dynamic
        }
        else 
        {
            floorHeight = floorPlane.CenterPose.position.y;
        }
        //navigation.DrawPathLine(floorHeight);
        navigation.DrawNextPathCorner(floorHeight);

        debugText.text = Frame.Pose.position.ToString();

    }

    /* Most of tracking related processes require the ground floor as a reference point. 
     * This method determines the biggest upwards facing plane as the ground floor.
     * It also updates the static variable floorPlane.
     */
    private void ProcessTracking()
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
            return;
        }
        //Adjusting screen timeout so that the screen stays on when tracking
        Screen.sleepTimeout = SleepTimeout.NeverSleep;



        // Iterate over all planes found in the AR frame and determine the floor plane.
        Session.GetTrackables<DetectedPlane>(m_NewPlanes, TrackableQueryFilter.All);
        for (int i = 0; i < m_NewPlanes.Count; i++)
        {
            if(m_NewPlanes[i].PlaneType == DetectedPlaneType.HorizontalUpwardFacing)
            {
                // In case a smaller area, like a table, has been detected in the view,
                // the bigger plane is usually the floor area
                if (GroundPlaneArea(m_NewPlanes[i]) >= GroundPlaneArea(floorPlane))
                {
                    floorPlane = m_NewPlanes[i];
                }
            }
        }


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

        Frame.Pose.rotation.SetLookRotation(new Vector3(0,0,0));

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;
        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            
            // Custom.method(hit.trackable as DetectedPlane);
        }

    }


    /* Returns the area of a flat, upward facing, DetectedPlane. 
     * 
     * Source: https://answers.unity.com/questions/684909/how-to-calculate-the-surface-area-of-a-irregular-p.html
     */
    private float GroundPlaneArea(DetectedPlane groundPlane)
    {

        if(groundPlane == null)
        {
            return 0;
        }
        else if(groundPlane.PlaneType != DetectedPlaneType.HorizontalUpwardFacing)
        {
            throw new ArgumentException("The DetectedPlane type is not HorizontalUpwardFacing");
        }

        List<Vector3> list = new List<Vector3>();
        groundPlane.GetBoundaryPolygon(list);

        float area = 0;
        int i = 0;
        for (; i < list.Count; i++)
        {
            if (i != list.Count - 1)
            {
                float mulA = list[i].x * list[i + 1].z;
                float mulB = list[i + 1].x * list[i].z;
                area = area + (mulA - mulB);
            }
            else
            {
                float mulA = list[i].x * list[0].z;
                float mulB = list[0].x * list[i].z;
                area = area + (mulA - mulB);
            }
        }
        area *= 0.5f;
        return Mathf.Abs(area);
    }

    private void SpawnObjects(GameObject spawnObject, Vector3[] positions)
    {
        GameObject newObject;
        if(objectList.Count > 0)
        {
            foreach (GameObject obj in objectList)
            {
                DestroyImmediate(obj);
            }
            objectList.Clear();
        }

        foreach (Vector3 pos in positions){
            newObject = Instantiate(spawnObject, pos, Quaternion.identity, transform);
            objectList.Add(spawnObject);
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
