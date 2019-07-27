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
    public Text debugTextHough;
    public Text debugTextTrackingState;
    public GameObject floor;
    public Material invisibleMaterial;
    public Image EdgeDetectionBackgroundImage;

    private NavigationController navigationController;
    private PoseController poseController;
    private readonly List<DetectedPlane> _detectedPlanes = new List<DetectedPlane>();
    private readonly List<AugmentedImage> _detectedImages = new List<AugmentedImage>();
    private byte[] m_EdgeDetectionResultImage = null;
    private Texture2D m_EdgeDetectionBackgroundTexture = null;
    private WebCamTexture cameraTest;
    private int trackingCount = 0;

    // QuitOnConnectionErrors checks the state of the ARCore Session.
    void Start()
    {
        QuitOnConnectionErrors();
        navigationController = GetComponent<NavigationController>();
        poseController = GetComponent<PoseController>();
        // Make everything invisible!
        floor.GetComponent<Renderer>().material = invisibleMaterial;

        cameraTest = new WebCamTexture();
        cameraTest.deviceName = WebCamTexture.devices[0].name;
    }

    void Update()
    {
        ProcessTouches();
        DrawSobelEdges();

        // If tracking failed, no calculations can be made.
        // !!! Any code below this point relies on sucessful tracking !!!
        if (ProcessTracking() == false)
        {
            debugTextTrackingState.text = "Tracking state: Tracking failed " + ++trackingCount;
            return;
        }
        else
        {
            trackingCount = 0;
        }
        
        debugTextTrackingState.text = "Tracking state: \nTracking Images: " + _detectedImages.Count + " Detected Planes: " + _detectedPlanes.Count;
        if(_detectedImages.Count > 0)
        {
            debugTextTrackingState.text += " (Name:" + _detectedImages[0].Name+  ") ";
        }

        // Align the real world data with the virtual one
        if (poseController != null)
        {
            
            var hasUpdated = poseController.UpdateARScene(_detectedImages);
            if (hasUpdated)
            {
                navigationController.BakeMesh();
            }
        }

        // TODO: Decide on logic when to rebake the mesh after UpdateARScene
        // Function: navigationController.BakeMesh();

        // Draw navigation elements into the scene
        if (navigationController != null)
        {
            navigationController.DrawNextPathCorner(_detectedPlanes);
        }

    }

    public void UpdateDestination(string plateNumber)
    {
        Debug.Log("Destination changed to: " + plateNumber);
        debugText.text = "Destination: " + plateNumber;
        navigationController.ChangeDestination(plateNumber);
    }

    /** A debugging function that detects and draws edges to an given texture
    
     */
    private void DrawSobelEdges()
    {
        if (Application.isEditor)
        {
            return;
        }

        
        using (var image = Frame.CameraImage.AcquireCameraImageBytes())
        {
            if (!image.IsAvailable)
            {
                debugTextHough.text = "Camera: " + cameraTest.name + "  image not available! Size: " + cameraTest.GetPixels().Length;
                //debugText.text = "Image not available";
                return;
            }
            IntPtr inputImage = image.Y;
            int width = image.Width;
            int height = image.Height;
            int rowStride = image.YRowStride;
            DisplayUvCoords m_CameraImageToDisplayUvTransformation = Frame.CameraImage.ImageDisplayUvs;
            m_EdgeDetectionResultImage = new byte[width * height];

            int[,] hougAccoumulator = SobelEdgeDetector.Sobel(m_EdgeDetectionResultImage, inputImage, width, height, rowStride);
            if (hougAccoumulator != null)
            {
                debugTextHough.text = "Hough size: " + hougAccoumulator.GetLength(0) + ", " + hougAccoumulator.GetLength(1);
                
                /* Debugging purposes only!
                foreach (var line in hougAccoumulator)
                {
                    if (line > 0)
                    {
                        debugTextHough.text += line + ", ";
                    }
                }
                */
            }
            else
            {
                debugTextHough.text = "Hough not found :(";
            }
        }
        // Detect edges within the image.
        //if (SobelEdgeDetector.Sobel(m_EdgeDetectionResultImage, inputImage, width, height, rowStride))
        //if (GoogleARCore.Examples.ComputerVision.EdgeDetector.Detect(m_EdgeDetectionResultImage, inputImage, width, height, rowStride))
        /*
        if (SobelEdgeDetector.Sobel(m_EdgeDetectionResultImage, inputImage, width, height, rowStride))
        {
            // Update the rendering texture with the edge image.     
            
            m_EdgeDetectionBackgroundTexture.LoadRawTextureData(m_EdgeDetectionResultImage);
            m_EdgeDetectionBackgroundTexture.Apply();
            EdgeDetectionBackgroundImage.material.SetTexture(
                "_ImageTex", m_EdgeDetectionBackgroundTexture);

            const string TOP_LEFT_RIGHT = "_UvTopLeftRight";
            const string BOTTOM_LEFT_RIGHT = "_UvBottomLeftRight";
            EdgeDetectionBackgroundImage.material.SetVector(TOP_LEFT_RIGHT, new Vector4(
                m_CameraImageToDisplayUvTransformation.TopLeft.x,
                m_CameraImageToDisplayUvTransformation.TopLeft.y,
                m_CameraImageToDisplayUvTransformation.TopRight.x,
                m_CameraImageToDisplayUvTransformation.TopRight.y));
            EdgeDetectionBackgroundImage.material.SetVector(BOTTOM_LEFT_RIGHT, new Vector4(
                m_CameraImageToDisplayUvTransformation.BottomLeft.x,
                m_CameraImageToDisplayUvTransformation.BottomLeft.y,
                m_CameraImageToDisplayUvTransformation.BottomRight.x,
                m_CameraImageToDisplayUvTransformation.BottomRight.y));
                
            //Debug.Log(m_EdgeDetectionResultImage);
            //debugText.text = m_EdgeDetectionResultImage.ToString();
            

        } 
        */
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
            debugTextTrackingState.text = "Tracking state: Tracking failed. Reason is SessionStatus not Tracking!";
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
