using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class CanvasTabletRescale : MonoBehaviour
{
    private CanvasScaler canvasScaler =>  GetComponent<CanvasScaler>();

    private void Awake()
    {
        Resize((float)Screen.width / (float)Screen.height);
    }

    private void Resize(float testValue)
    {
        canvasScaler.matchWidthOrHeight=testValue<=0.5625f? 0 : 1;
    }

    public void ReInit()
    {
        Awake();
    }
}
