using System;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private ParallaxCamera parallaxCamera;
    [SerializeField] private ParallaxObject maxPoint;
    [SerializeField] private ParallaxObject minPoint;
    
    private List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();
 
    void Start()
    {
        if (parallaxCamera == null)
            parallaxCamera = Camera.main.GetComponent<ParallaxCamera>();
 
        if (parallaxCamera != null)
            parallaxCamera.onCameraTranslate += Move;
 
        SetLayers();
        maxPoint.hideObject += HideObject;
        minPoint.hideObject += HideObject;
    }
 
    void SetLayers()
    {
        parallaxLayers.Clear();
 
        for (int i = 0; i < transform.childCount; i++)
        {
            ParallaxLayer layer = transform.GetChild(i).GetComponent<ParallaxLayer>();
 
            if (layer != null)
            {
                parallaxLayers.Add(layer);
            }

            // layer.hideObject += HideObject;
        }
    }
 
    void Move(float delta)
    {
        foreach (ParallaxLayer layer in parallaxLayers)
        {
            layer.Move(delta);
        }
    }

    private void HideObject(GameObject parallaxObject)
    {
        parallaxObject.SetActive(false);
    }
}