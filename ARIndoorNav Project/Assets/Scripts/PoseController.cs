using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using System;
using UnityEngine.UI;

public class PoseController : MonoBehaviour
{
    public GameObject _arScene;
    public Text debugText;
    public bool testMode = false;

    private readonly List<AugmentedImage> _images = new List<AugmentedImage>();
    private readonly Dictionary<int, Anchor> _anchors = new Dictionary<int, Anchor>();
    private float updateSpeed = 4f;
    private bool meshIsBacked = true;

    void Start()
    {
        Debug.Log("Starting Pose Controller");
    }

    /*  Quit when ARCore is not tracking. 
        Update _images list with all tracked AugementedImages within the ARCore frame.
        Change the ARScene to match up with the position of the tracked images.
    */
    void Update()
    {
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }
        Session.GetTrackables<AugmentedImage>(_images, TrackableQueryFilter.Updated);

        if (testMode)
        {
            TestAlignScenePose();
        }
        else
        {
            UpdateARScene();
        }


    }

    /*  Each tracked image represents a marker in the real world. To match the position of the marker within the ARCore space
        the unity space changes its position and rotation accordingly.
        The center of the marker is used and matched with an database that has the position of it within unity. 
        After the ARScene has been moved, the NavMesh has to rebaked
    */
    private void UpdateARScene()
    {
        foreach (var image in _images)
        {
            if (image.TrackingState == TrackingState.Tracking)
            {
                // Use TrackingMethod introduced in SDK v1.9.0
                Debug.Log("DOG found!");
                debugText.text = "DOG found!";

                AlignScenePose(image);
            }
            else if (image.TrackingState == TrackingState.Stopped || image.TrackingState == TrackingState.Paused)
            {
                // TODO: Decide on logic
            }
        }
    }

    /*  Aligns a real-world marker's position with the corresponding virtual Unity marker.
        The real-world marker is an AugmentedImage detected by ARCore.
        The virtual (target) marker are GameObjects within Unity with unique names to identify them.
     */
    private void AlignScenePose(AugmentedImage image)
    {
        // Searches the Scene for the image target to align position.
        // Immediatly returns if no image target could be found. Place a corresponding unity object in scene if that happens
        var targetMarker = GameObject.Find(image.Name);
        if (targetMarker == null)
        {
            Debug.Log("Augmented Image with the name: " + image.Name + " could not be found.");
            return;
        }
        // Calculate the target position and rotation that is used to move the ARScene GameObject
        var targetPoint = (image.CenterPose.position - targetMarker.transform.position) + _arScene.transform.position;
        var targetRotation = Quaternion.Inverse(image.CenterPose.rotation) * targetMarker.transform.rotation;

        if (targetMarker.transform.position != image.CenterPose.position || targetMarker.transform.rotation != image.CenterPose.rotation)
        {
            // Align the virtual marker with the real-world marker by rotating and moving all GameObjects within Unity
            meshIsBacked = false;
            _arScene.transform.position = Vector3.Lerp(_arScene.transform.position, targetPoint, Time.deltaTime * updateSpeed);
            _arScene.transform.RotateAround(targetMarker.transform.position, new Vector3(1, 0, 0), targetRotation.x);
            _arScene.transform.RotateAround(targetMarker.transform.position, new Vector3(0, 1, 0), targetRotation.y);
            _arScene.transform.RotateAround(targetMarker.transform.position, new Vector3(0, 0, 1), targetRotation.z);
        }
        else
        {
            // After the ARScene has been moved, the NavMesh has to be rebaked in order to recalculate the navigation path.
            BackeMesh();
            meshIsBacked = true;
        }
    }

    /* This is only for testing purposes. Do not use!
     */
    private void TestAlignScenePose()
    {
        var targetMarker = GameObject.Find("Dog Marker");
        var arMarker = GameObject.Find("Dog Target");
        if (targetMarker == null)
        {
            Debug.Log("Augmented Image with the name: " + "Dog Marker" + " could not be found.");
            return;
        }
        var targetPoint = (arMarker.transform.position - targetMarker.transform.position) + _arScene.transform.position;
        var targetRotation = Quaternion.Inverse(arMarker.transform.rotation) * targetMarker.transform.rotation;
        if (targetMarker.transform.position != arMarker.transform.position || targetMarker.transform.rotation != arMarker.transform.rotation)
        {
            meshIsBacked = false;
            _arScene.transform.position = Vector3.Lerp(_arScene.transform.position, targetPoint, Time.deltaTime * updateSpeed);
            _arScene.transform.RotateAround(targetMarker.transform.position, new Vector3(1, 0, 0), targetRotation.x);
            _arScene.transform.RotateAround(targetMarker.transform.position, new Vector3(0, 1, 0), targetRotation.y);
            _arScene.transform.RotateAround(targetMarker.transform.position, new Vector3(0, 0, 1), targetRotation.z);
        }
        else
        {
            //BackeMesh();
            meshIsBacked = true;
        }
    }

    private void BackeMesh()
    {
        throw new NotImplementedException();
    }
}
