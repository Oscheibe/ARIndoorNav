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
    private bool meshNeedsBaking = false;

    /*  Each tracked image represents a marker in the real world. To match the position of the marker within the ARCore space
        the unity space changes its position and rotation accordingly.
        The center of the marker is used and matched with an database that has the position of it within unity. 
        After the ARScene has been moved, the NavMesh has to rebaked
        Returns true if the scene has updated and the NavMesh needs to be baked
    */
    public bool UpdateARScene(List<AugmentedImage> images)
    {
        if (testMode)
        {
            return TestAlignScenePose();
        }

        debugText.text = "Tracked images: " + images.Count;
        foreach (var image in images)
        {
            if (image.TrackingState == TrackingState.Tracking)
            {
                if (image.TrackingMethod == AugmentedImageTrackingMethod.FullTracking)
                {
                    return AlignScenePose(image);
                }
            }
            else if (image.TrackingState == TrackingState.Stopped || image.TrackingState == TrackingState.Paused)
            {
                // TODO: Decide on logic
            }
        }
        // If no images are tracked, the mesh does not need to be baked
        return false;
    }

    /*  Aligns a real-world marker's position with the corresponding virtual Unity marker.
        The real-world marker is an AugmentedImage detected by ARCore.
        The virtual (target) marker are GameObjects within Unity with unique names to identify them.
        Returns true if the scene has updated and the NavMesh needs to be baked
     */
    private bool AlignScenePose(AugmentedImage image)
    {

        // Searches the Scene for the image target to align position.
        // Immediatly returns if no image target could be found. Place a corresponding unity object in scene if that happens
        var targetPlate = GameObject.Find(image.Name);
        if (targetPlate == null)
        {
            Debug.Log("Augmented Image with the name: " + image.Name + " could not be found.");
            return false;
        }

        // Rotate first because the rotation changes the position of the object.
        // Rotate over the X axis by 90° to account for AugmentedImage detection angles (Might change later, hopefully not)
        var targetRotation = _arScene.transform.rotation * (Quaternion.Inverse(targetPlate.transform.rotation) * image.CenterPose.rotation);
        targetRotation = targetRotation * Quaternion.Euler(90, 0, 0);
        if (_arScene.transform.rotation != targetRotation) // Math.Abs(targetPlate.transform.rotation.eulerAngles.y - image.CenterPose.rotation.eulerAngles.y) >= 0.1
        {
            meshNeedsBaking = true;
            _arScene.transform.rotation = targetRotation;
        }
        else
        {
            meshNeedsBaking = false;
        }
        // Calculate the new position of the _arScene center, based on relative distance between unity and ARCore plates
        // Calculation needs to be done after changing the rotation.
        var targetPosition = (image.CenterPose.position - targetPlate.transform.position) + _arScene.transform.position;
        if (targetPlate.transform.position != image.CenterPose.position)
        {
            meshNeedsBaking = true;
            _arScene.transform.position = targetPosition;
        }
        else
        {
            // After the ARScene has been moved, the NavMesh has to be rebaked in order to recalculate the navigation path.
            meshNeedsBaking = false;
        }
        // Return true if the mesh needs baking 
        return meshNeedsBaking;
    }

    /* This is only for testing purposes. Do not use!
     */
    private bool TestAlignScenePose()
    {
        var targetPlate = GameObject.Find("3.219");
        var image = GameObject.Find("Debug Virtual AR Image");
        if (targetPlate == null)
        {
            Debug.Log("Target Plate with the name: " + "Dog Plate" + " could not be found.");
            return false;
        }
        var targetRotation = _arScene.transform.rotation * (Quaternion.Inverse(targetPlate.transform.rotation) * image.transform.rotation);
        targetRotation = targetRotation * Quaternion.Euler(90, 0, 0);
        if (_arScene.transform.rotation != targetRotation) // Math.Abs(targetPlate.transform.rotation.eulerAngles.y - image.CenterPose.rotation.eulerAngles.y) >= 0.1
        {
            meshNeedsBaking = true;
            _arScene.transform.rotation = targetRotation;
        }
        else
        {
            meshNeedsBaking = false;
        }
        // Calculate the new position of the _arScene center, based on relative distance between unity and ARCore plates
        // Calculation needs to be done after changing the rotation.
        var targetPosition = (image.transform.position - targetPlate.transform.position) + _arScene.transform.position;
        if (targetPlate.transform.position != image.transform.position)
        {
            meshNeedsBaking = true;
            _arScene.transform.position = targetPosition;
        }
        else
        {
            // After the ARScene has been moved, the NavMesh has to be rebaked in order to recalculate the navigation path.
            meshNeedsBaking = false;
        }
        // Return true if the mesh needs baking 
        return meshNeedsBaking;












        /*
        var targetPosition = (image.transform.position - targetPlate.transform.position) + _arScene.transform.position;
        var targetRotation = _arScene.transform.rotation * (Quaternion.Inverse(targetPlate.transform.rotation) * image.transform.rotation);
        targetRotation = targetRotation * Quaternion.Euler(90, 0, 0);
        if (targetPlate.transform.position != image.transform.position || Math.Abs(targetPlate.transform.rotation.eulerAngles.y - image.transform.rotation.eulerAngles.y) >= 0.1)
        {
            meshNeedsBaking = true;
            //_arScene.transform.rotation = targetRotation;
            _arScene.transform.rotation = Quaternion.Lerp(_arScene.transform.rotation, targetRotation, Time.deltaTime * updateSpeed);
            _arScene.transform.position = Vector3.Lerp(_arScene.transform.position, targetPosition, Time.deltaTime * updateSpeed);
            //_arScene.transform.RotateAround(targetPlate.transform.position, new Vector3(1,1,1), Time.deltaTime * updateSpeed);
            //_arScene.transform.RotateAround(targetPlate.transform.position, new Vector3(0, 1, 0),Time.deltaTime * ((float)100/360) * (targetPlate.transform.rotation.eulerAngles.y - targetRotation.eulerAngles.y) );
            //_arScene.transform.RotateAround(targetPlate.transform.position, new Vector3(0, 0, 1),Time.deltaTime * ((float)100/360) * (targetPlate.transform.rotation.eulerAngles.z - targetRotation.eulerAngles.z) );
        }
        else
        {
            meshNeedsBaking = false;
        }
        return meshNeedsBaking;
        */
    }
}
