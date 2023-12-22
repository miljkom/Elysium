using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform container;
    [SerializeField] private Transform topBounds;
    [SerializeField] private Transform bottomBounds;
    [SerializeField] private int platformPoolSize = 30;
    [SerializeField] private float despawnDistance = 30f;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float despawnTimer = 3f;
    [SerializeField] private float lastSpawnPlatformPositionY;
    
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

        int initialActivePlatforms = 10;
        for (int i = 0; i < initialActivePlatforms; i++)
        {
            platformPool[i].SetActive(true);
            float spawnX = Random.Range(-4f, 4f); // Adjust as needed
            float spawnY = lastSpawnPlatformPositionY + 2.5f; // Adjust as needed

            platformPool[i].transform.position = new Vector3(spawnX, spawnY, 0f);
            lastSpawnPlatformPositionY = spawnY;
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
        DespawnPlatforms();
    }

    private void DespawnPlatforms()
    {
        foreach (GameObject platform in platformPool)
        {
            if (platform.activeInHierarchy && playerTransform.position.y - platform.transform.position.y > despawnDistance)
            {
                StartCoroutine(DeactivatePlatformDelayed(platform));
            }
        }
    }

    private IEnumerator DeactivatePlatformDelayed(GameObject platform)
    {
        //animation
        yield return new WaitForSeconds(despawnTimer);
        platform.SetActive(false);
    }
}
