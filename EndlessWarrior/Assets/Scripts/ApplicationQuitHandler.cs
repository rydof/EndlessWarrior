using UnityEngine;
using System.Diagnostics;

public class ApplicationQuitHandler : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        #if !UNITY_EDITOR
            Process.GetCurrentProcess().Kill();
        #endif
    }
}
