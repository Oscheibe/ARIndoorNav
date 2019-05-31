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

    private readonly List<AugmentedImage> _images = new List<AugmentedImage>();
    private readonly Dictionary<int, Anchor> _anchors = new Dictionary<int, Anchor>();
    private float updateSpeed = 3f;
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
        UpdateARScene();
        
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
            Vector3 arImageCenter = image.CenterPose.position;
            
            // Searches the Scene for the image target to align position.
            // Immediatly returns if no image target could be found. Place a corresponding unity object in scene if that happens
            Vector3 unityImageCenter;
            var unityImage = GameObject.Find(image.Name);
            if(unityImage == null)
            {
                Debug.Log("Augmented Image with the name: "+image.Name+" could not be found.");
                return;
            }
            else
            {
                unityImageCenter = unityImage.transform.position;
            }
            
            Debug.Log("Processing DOG");
            if (image.TrackingState == TrackingState.Tracking)
            {
                // Use TrackingMethod introduced in SDK v1.9.0
                Debug.Log("DOG found!");
                debugText.text = "DOG found!";
                
                
                var targetPoint =  image.CenterPose.position - GameObject.Find(image.Name).transform.position;
                AlignScenePose(targetPoint);
            }
            else if (image.TrackingState == TrackingState.Stopped || image.TrackingState == TrackingState.Paused)
            {
                // TODO: Decide on logic
            }
        }
    }

/*  Aligns a real-world marker's position within Unity with the detected position by ARCore.
    A database will be used to keep track of all the markers position.
 */
    private void AlignScenePose(Vector3 targetPoint)
    {
       if(_arScene.transform.position != targetPoint)
       {
           meshIsBacked = false;
           _arScene.transform.position = Vector3.Lerp(_arScene.transform.position, targetPoint, Time.deltaTime * updateSpeed);
       }
       else
       {
           BackeMesh();
           meshIsBacked = true;
       }
    }

    private void BackeMesh()
    {
        throw new NotImplementedException();
    }
}
