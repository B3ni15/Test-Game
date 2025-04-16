using UnityEngine;
using Steamworks;

public class SteamManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        if (!SteamAPI.Init())
        {
            Debug.LogError("SteamAPI initialization failed!");
            return;
        }
        
        Debug.Log("SteamAPI initialized successfully!");
    }

    private void Update()
    {
        SteamAPI.RunCallbacks();
    }

    private void OnDestroy()
    {
        SteamAPI.Shutdown();
    }
}