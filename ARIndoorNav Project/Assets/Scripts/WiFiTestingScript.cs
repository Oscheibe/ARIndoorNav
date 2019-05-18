using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class WiFiTestingScript : MonoBehaviour
{


    public Text debugText;

    private string result;

    
    // Start is called before the first frame update
    void Start()
    {
        
        getLocationPermission();
        
        result = getBSSID();
        Debug.Log(result);
        debugText.text = result;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void getLocationPermission()
    {
        #if PLATFORM_ANDROID
        if(!Permission.HasUserAuthorizedPermission("android.permission.ACCESS_WIFI_STATE"))
        {
            Permission.RequestUserPermission("android.permission.ACCESS_WIFI_STATE");
            
        }  
        #endif
    }

    /**
    Source: https://stackoverflow.com/questions/39566989/how-to-get-bssid-of-wifi-im-connecting-to-in-unity-c
    */
    private string getBSSID()
    {
        string tempBSSID = "";

        AndroidJavaClass javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
        AndroidJavaObject activity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject wifiManager = activity.Call<AndroidJavaObject>("getSystemService", "wifi");
        AndroidJavaObject wifiReceiver = activity.Call<AndroidJavaObject>("new BroadcastReceiver()");

        tempBSSID = wifiManager.Call<AndroidJavaObject>("getConnectionInfo").Call<string>("getSSID");
        
        return tempBSSID;
    }
}
