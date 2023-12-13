using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformObjectPool : MonoBehaviour
{
    public GameObject platformPrefab;
    public int platformPoolSize = 30;
    public float despawnDistance = 30f;
    public float spawnInterval = 2f;

    public Transform playerTransform;
    public Transform container;
    public Transform topBounds;
    public Transform bottomBounds;
    
    private float lastSpawnPlatformPositionY;
    
    private List<GameObject> platformPool = new List<GameObject>();

    private void Start()
    {
        playerTransform ??= Camera.main.transform;
        lastSpawnPlatformPositionY = bottomBounds.position.y;

        InitializePool();
        StartCoroutine(SpawnPlatforms());
    }

    private void InitializePool()
    {
        for (int i = 0; i < platformPoolSize; i++)
        {
            GameObject platform = Instantiate(platformPrefab, container);
            platform.SetActive(false);
            platformPool.Add(platform);
        }
    }

    private IEnumerator SpawnPlatforms()
    {
        while (true)
        {
            float distanceSinceLastSpawn = topBounds.position.y - lastSpawnPlatformPositionY;

            if (distanceSinceLastSpawn >= spawnInterval)
            {
                SpawnPlatform();
            }

            yield return null;
        }
    }
    
    private void SpawnPlatform()
    {
        GameObject platform = GetPooledPlatform();
        if (platform != null)
        {
            float spawnX = Random.Range(-4f, 4f); // Adjust as needed
            float spawnY = lastSpawnPlatformPositionY + 2f; // Adjust as needed

            platform.transform.position = new Vector3(spawnX, spawnY, 0f);
            lastSpawnPlatformPositionY = spawnY;
            platform.SetActive(true);
        }
    }

    private GameObject GetPooledPlatform()
    {
        foreach (GameObject platform in platformPool)
        {
            if (!platform.activeInHierarchy)
            {
                return platform;
            }
        }

        return null;
    }

    private void Update()
    {
        //DespawnPlatforms();
    }

    private void DespawnPlatforms()
    {
        foreach (GameObject platform in platformPool)
        {
            if (platform.activeInHierarchy && playerTransform.position.y - platform.transform.position.y > despawnDistance)
            {
                platform.SetActive(false);
            }
        }
    }
}
