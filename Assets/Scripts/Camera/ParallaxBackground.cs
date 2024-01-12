using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private ParallaxCamera parallaxCamera;
    
    private Dictionary<ParallaxLayer, List<GameObject>> parallaxLayers = new ();
 
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
            
            List<GameObject> layerList = new ();
            for (int j = 0; j < layer.transform.childCount ; j++)
            {
                layerList.Add(layer.transform.GetChild(j).gameObject);
            }

            // Sorting by Y value
            layerList = layerList.OrderBy(obj => obj.transform.position.y).ToList();
            layer.SetBoundaryPoints(layerList);
            if (layer != null)
            {
                parallaxLayers.Add(layer, layerList);
            }
        }
    }
    private void Move(float delta)
    {
        foreach (var layer in parallaxLayers)
        {
            layer.Key.Move(delta);
            layer.Key.ChangeObjectsTransorm(layer.Value, delta);
        }
    }

    private void HideObject(GameObject parallaxObject)
    {
        parallaxObject.SetActive(false);
    }
}