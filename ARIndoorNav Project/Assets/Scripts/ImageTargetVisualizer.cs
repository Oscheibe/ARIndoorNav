using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;

public class ImageTargetVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject _imageVisual;
    private readonly List<AugmentedImage> _images = new List<AugmentedImage>();
    private readonly Dictionary<int, GameObject> _visualizers = new Dictionary<int, GameObject>();
    public Text debugText;

    void Update()
    {   
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }
        
        Session.GetTrackables(_images, TrackableQueryFilter.Updated);
        VisualizeTrackables();
    }

    /*  Adds or removes a visualizer on each updated trackabe within the session
     */
    private void VisualizeTrackables()
    {
        foreach(var image in _images)
        {
            if(image.TrackingState == TrackingState.Tracking)
            {
                debugText.text = "Adding anchor at: " + image.CenterPose.ToString();
                AddVisualizer(image);
            } else if(image.TrackingState == TrackingState.Stopped || image.TrackingState == TrackingState.Paused)
            {
                debugText.text = "Removed Visualizer";
                RemoveVisualizer(image);
            }
        }
    }

    /*  Creates an anchor at the AugmentedImage's position that is attached to it and instantiates a visualizer on it. 
        The visualizer is then added to a Dictionary for deletion in the future.    
     */
    private void AddVisualizer(AugmentedImage image)
    {
        var anchor = image.CreateAnchor(image.CenterPose);
        var visualizer = Instantiate(_imageVisual, anchor.transform);
        visualizer.transform.localScale = new Vector3(image.ExtentX, image.ExtentZ, 1);
        _visualizers.Add(image.DatabaseIndex, visualizer);
    }

    /*  Destroy the visualizer assoziated with the image and deletes the Dictionary entry.
        TODO: Manage anchors
    */
    private void RemoveVisualizer(AugmentedImage image)
    {
        GameObject visualizer;
        _visualizers.TryGetValue(image.DatabaseIndex, out visualizer);
        _visualizers.Remove(image.DatabaseIndex);
        if(visualizer != null) 
        {
            Destroy(visualizer);
        }
    }
}
