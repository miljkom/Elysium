using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private float parallaxFactor;
    
    private float maxPoint;
    private float minPoint;

    public void SetBoundaryPoints(List<GameObject> layerList)
    {
        var yLastInList = layerList.Last().transform.position.y;
        var yFirstInList = layerList.First().transform.position.y;
        maxPoint = yLastInList + (yLastInList - yFirstInList) / (layerList.Count - 1);
        minPoint = yFirstInList;
    }

    public void Move(float delta)
    {
        Vector3 newPos = transform.position;
        newPos.y -= delta * parallaxFactor;
        
        transform.position = newPos;
    }

    public void ChangeObjectsTransorm(List<GameObject> layerList, float delta)
    {
        maxPoint -= delta;
        minPoint -= delta;
        
        var roundedMaxPoint = Mathf.Round(maxPoint);
        var roundedMinPoint = Mathf.Round(minPoint);
        
        if (layerList.Last().transform.position.y > maxPoint)
        {
            //get it out of list
            var currentObj = layerList[layerList.Count - 1];
            layerList.RemoveAt(layerList.Count - 1);
            //change position
            currentObj.SetActive(false);
            currentObj.transform.position = new Vector3(
                currentObj.transform.localPosition.x,
                Mathf.Round(roundedMinPoint),
                0);
            //put it back to list
            layerList.Insert(0, currentObj);
            currentObj.SetActive(true);
            Debug.Log(currentObj.name + " = " + currentObj.transform.position.y);
        }

        if (layerList.First().transform.position.y < minPoint)
        {
            //get it out of list
            var currentObj = layerList[0];
            layerList.RemoveAt(0);
            //change position
            currentObj.SetActive(false);
            currentObj.transform.position = new Vector3(
                currentObj.transform.localPosition.x,
                Mathf.Round(roundedMaxPoint),
                0);
            //put it back to list
            layerList.Add(currentObj);
            currentObj.SetActive(true);
            Debug.Log(currentObj.name + " = " + currentObj.transform.position.y);
        }
    }
}