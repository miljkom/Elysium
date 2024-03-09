using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private ParallaxCamera parallaxCamera;
    
    private List<ParallaxLayer> parallaxLayers = new ();
 
    void Start()
    {
        if (parallaxCamera == null)
            parallaxCamera = Camera.main.GetComponent<ParallaxCamera>();
 
        SetLayers();
        
        if (parallaxCamera != null)
            parallaxCamera.onCameraTranslate += Move;
    }
 
    void SetLayers()
    {
        parallaxLayers.Clear();
 
        for (int i = 0; i < transform.childCount; i++)
        {
            ParallaxLayer layer = transform.GetChild(i).GetComponent<ParallaxLayer>();

            layer.CalculateTextureUnitSize();
            
            if (layer != null)
            {
                parallaxLayers.Add(layer);
            }
        }
    }
    private void Move(float delta)
    {
        foreach (var layer in parallaxLayers)
        {
            layer.Move(delta);
            layer.ChangeObjectsTransorm(parallaxCamera, delta);
        }
    }

    private void HideObject(GameObject parallaxObject)
    {
        parallaxObject.SetActive(false);
    }
}