using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllScriptTests : MonoBehaviour
{
    // Model
    public ARPositionTracking _ARPositionTracking;
    public MarkerDatabase _MarkerDatabase;
    public MarkerDetection _MarkerDetection;
    public Navigation _Navigation;
    public PoseEstimation _PoseEstimation;
    public RoomDatabase _RoomDatabase;
    public TrackingErrorHandling _TrackingErrorHandling;

    public HelperFunctions _HelperFunctions;
    
    // Presenter
    public ModelPresenter _ModelPresenter;
    public NavigationPresenter _NavigationPresenter;
    public RoomPresenter _RoomPresenter;
    public UserMessagesPresenter _UserMessagesPresenter;

    // View
    public ARVisuals _ARVisuals;
    public MapUI _MapUI;
    public RecommendedUI _RecommendedUI;
    public SearchUI _SearchUI;
    public TargetDestinationUI _TargetDestinationUI;
    public UserMessageUI _UserMessageUI;
    
    private string testResult = "(none)\n";
    
    // Start is called before the first frame update
    void Start()
    {
        TestARPositionTracking();


        Debug.Log("Following ccripts failed the test: " + testResult);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TestARPositionTracking()
    {
        // Can only be tested in runtime
    }

    private void TestMarkerDatabase()
    {
        
    }

}
