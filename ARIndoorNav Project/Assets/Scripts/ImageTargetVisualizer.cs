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

    private void VisualizeTrackables()
    {
        foreach(var image in _images)
        {
            if(image.TrackingState == TrackingState.Tracking)
            {
                AddVisualizer(image);
            } else if(image.TrackingState == TrackingState.Stopped || image.TrackingState == TrackingState.Paused)
            {
                RemoveVisualizer(image);
            }
        }
    }

    private void AddVisualizer(AugmentedImage image)
    {
        var anchor = image.CreateAnchor(image.CenterPose);
        var visualizer = Instantiate(_imageVisual, anchor.transform);
        _visualizers.Add(image.DatabaseIndex, visualizer);
    }

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
