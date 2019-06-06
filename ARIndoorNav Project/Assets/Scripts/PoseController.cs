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

    private float updateSpeed = 4f;
    private bool meshIsBaked = true;

    /*  Each tracked image represents a marker in the real world. To match the position of the marker within the ARCore space
        the unity space changes its position and rotation accordingly.
        The center of the marker is used and matched with an database that has the position of it within unity. 
        After the ARScene has been moved, the NavMesh has to rebaked
    */
    public void UpdateARScene(List<AugmentedImage> _images)
    {
        if (testMode)
        {
            TestAlignScenePose();
            return;
        }

        foreach (var image in _images)
        {
            if (image.TrackingState == TrackingState.Tracking)
            {
                if (image.TrackingMethod == AugmentedImageTrackingMethod.FullTracking)
                {
                    AlignScenePose(image);
                }
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
        var targetPlate = GameObject.Find(image.Name);
        if (targetPlate == null)
        {
            Debug.Log("Augmented Image with the name: " + image.Name + " could not be found.");
            return;
        }
        // Calculate the target position and rotation that is used to move the ARScene GameObject
        var targetPosition = (image.CenterPose.position - targetPlate.transform.position) + _arScene.transform.position;
        var targetRotation = _arScene.transform.rotation * (Quaternion.Inverse(targetPlate.transform.rotation) * image.CenterPose.rotation);
        // Rotate over the X axis by 90° to account for AugmentedImage detection
        targetRotation = targetRotation * Quaternion.Euler(90, 0, 0);

        if (targetPlate.transform.position != image.CenterPose.position || targetPlate.transform.rotation != image.CenterPose.rotation)
        {
            // Align the virtual marker with the real-world marker by rotating and moving all GameObjects within Unity
            meshIsBaked = false;
            _arScene.transform.rotation = Quaternion.Lerp(_arScene.transform.rotation, targetRotation, Time.deltaTime * updateSpeed);
            _arScene.transform.position = Vector3.Lerp(_arScene.transform.position, targetPosition, Time.deltaTime * updateSpeed);
        }
        else
        {
            // After the ARScene has been moved, the NavMesh has to be rebaked in order to recalculate the navigation path.
            BackeMesh();
            meshIsBaked = true;
        }
    }

    /* This is only for testing purposes. Do not use!
     */
    private void TestAlignScenePose()
    {
        var targetPlate = GameObject.Find("Dog Plate");
        var image = GameObject.Find("ARDog Marker");
        if (targetPlate == null)
        {
            Debug.Log("Target Plate with the name: " + "Dog Plate" + " could not be found.");
            return;
        }
        var targetPosition = (image.transform.position - targetPlate.transform.position) + _arScene.transform.position;
        var targetRotation = _arScene.transform.rotation * (Quaternion.Inverse(targetPlate.transform.rotation) * image.transform.rotation);
        targetRotation = targetRotation * Quaternion.Euler(90, 0, 0);

        if (targetPlate.transform.position != image.transform.position || targetPlate.transform.rotation != image.transform.rotation)
        {
            meshIsBaked = false;
            //_arScene.transform.rotation = targetRotation;
            _arScene.transform.rotation = Quaternion.Lerp(_arScene.transform.rotation, targetRotation, Time.deltaTime * updateSpeed);
            _arScene.transform.position = Vector3.Lerp(_arScene.transform.position, targetPosition, Time.deltaTime * updateSpeed);
            //_arScene.transform.RotateAround(targetPlate.transform.position, new Vector3(1, 0, 0), targetRotation.x);
            //_arScene.transform.RotateAround(targetPlate.transform.position, new Vector3(0, 1, 0), targetRotation.y);
            //_arScene.transform.RotateAround(targetPlate.transform.position, new Vector3(0, 0, 1), targetRotation.z);

        }
        else
        {
            //BackeMesh();
            meshIsBaked = true;
        }
    }

    private void BackeMesh()
    {
        throw new NotImplementedException();
    }
}
