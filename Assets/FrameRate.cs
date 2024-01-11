using UnityEngine;

public class FrameRate : MonoBehaviour
{
#if UNITY_ANDROID // fali nam za IOS 
    private void Awake()
    {
        Application.targetFrameRate = 60;

    }
#endif
}
