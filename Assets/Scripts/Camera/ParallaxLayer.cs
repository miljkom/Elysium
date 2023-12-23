using System;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private float parallaxFactor;

    public Action<ParallaxObject> hideObject;
    
    private List<ParallaxObject> _parallaxObjects = new();
    
    // private void Start()
    // {
    //     for (int i = 0; i < transform.childCount; i++)
    //     {
    //         var parallaxObject = transform.GetChild(i).GetComponent<ParallaxObject>();
    //         _parallaxObjects.Add(parallaxObject);
    //         parallaxObject.hideObject += HideObject;
    //     }
    // }

    public void Move(float delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.y -= delta * parallaxFactor;
 
        transform.localPosition = newPos;
    }

    public void HideObject(ParallaxObject parallaxObject)
    {
        hideObject?.Invoke(parallaxObject);
    }
}