using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Android.Net.Wifi;

public class WiFiTestingScript : MonoBehaviour
{



/**
    Source: https://stackoverflow.com/questions/39566989/how-to-get-bssid-of-wifi-im-connecting-to-in-unity-c
 */
    public static string getBSSID()
    {
        string tempBSSID = "";

        try
        {
            using (var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (var wifiManager = activity.Call<AndroidJavaObject>("getSystemService", "wifi"))
                {
                    tempBSSID = wifiManager.Call<AndroidJavaObject>("getConnectionInfo").Call<string>("getBSSID");
                }
            }
        }
        catch(AndroidJavaException e)
        {
            
        }
        
        return tempBSSID;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
