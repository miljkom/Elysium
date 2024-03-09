using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private float parallaxFactor;
    
    private float textureUnitSizeY;
    
    public void CalculateTextureUnitSize()
    {
        var sprite = GetComponent<SpriteRenderer>().sprite;
        textureUnitSizeY = sprite.texture.height / sprite.pixelsPerUnit;
    }

    public void Move(float delta)
    {
        Vector3 newPos = transform.position;
        newPos.y -= delta * parallaxFactor;
        
        transform.position = newPos;
    }

    public void ChangeObjectsTransorm(ParallaxCamera parallaxCamera, float delta)
    {
        if (Mathf.Abs(parallaxCamera.transform.position.y - transform.position.y) >= textureUnitSizeY)
            transform.position = new Vector3(transform.position.x, parallaxCamera.transform.position.y + delta);
    }
}