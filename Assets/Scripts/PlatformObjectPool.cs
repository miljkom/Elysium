using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private GameObject bigPlatformPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform container;
    [SerializeField] private Transform topBounds;
    [SerializeField] private Transform bottomBounds;
    [SerializeField] private int platformPoolSize;
    [SerializeField] private float despawnDistance;
    [SerializeField] private float spawnInterval;
    [SerializeField] private float despawnTimer = 3f;
    [SerializeField] private float lastSpawnPlatformPositionY;
    
    private List<GameObject> platformPool = new List<GameObject>();
    private List<GameObject> bigPlatformPool = new List<GameObject>();
    private bool init = false;

    private void Start()
    {
        playerTransform ??= Camera.main.transform;
        lastSpawnPlatformPositionY = bottomBounds.position.y;

        InitializePool();
    }
    private void Update()
    {
        if (init)
        {
            SpawnPlatforms();
            DespawnPlatforms();
        }
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
        lastSpawnPlatformPositionY = -15f;
        PlayerStats.platformSpawned = initialActivePlatforms;
        for (int i = 0; i < initialActivePlatforms; i++)
        {
            platformPool[i].SetActive(true);
            float randomX = Random.Range(0.2f, 0.8f);
            float spawnX = Random.Range(-4f,4f);
            if(randomX > 0.5f)
                spawnX = Random.Range(-2.2f, 2.2f); // Adjust as needed
            else
            {
                spawnX = Random.Range(-3.6f, 3.6f);
            }
            float spawnY = lastSpawnPlatformPositionY + 2.5f; // Adjust as needed
            float fixedY = platformPool[i].transform.localScale.y;
            float fixedZ = platformPool[i].transform.localScale.z;

            platformPool[i].transform.position = new Vector3(spawnX, spawnY, 0f);
            platformPool[i].transform.localScale = new Vector3(randomX, fixedY, fixedZ);
            lastSpawnPlatformPositionY = spawnY;
        }

        GameObject bigPlatform = Instantiate(bigPlatformPrefab, container);
        bigPlatform.SetActive(false);
        bigPlatformPool.Add(bigPlatform);

        init = true;
    }

    private void SpawnPlatforms()
    {
        float distanceSinceLastSpawn = topBounds.position.y - lastSpawnPlatformPositionY;

        if (!(distanceSinceLastSpawn >= spawnInterval)) return;
        if (PlayerStats.platformSpawned % 50 == 0)
        {
            SpawnBigPlatform();
        }
        else
        {
            SpawnPlatform();
        }
    }

    private void SpawnBigPlatform()
    {
        GameObject platform = bigPlatformPool[0];
        if (platform != null)
        { 
            float spawnY = lastSpawnPlatformPositionY + 2f; // Adjust as needed
            platform.transform.position = new Vector3(0, spawnY, 0f);
            lastSpawnPlatformPositionY = spawnY;
            platform.SetActive(true);
            PlayerStats.platformSpawned++;
        }
    }

    private void SpawnPlatform()
    {
        GameObject platform = GetPooledPlatform();
        if (platform != null)
        {
            float spawnX = Random.Range(-2.2f, 2.2f); // Adjust as needed
            float spawnY = lastSpawnPlatformPositionY + 2f; // Adjust as needed
            float randomX = Random.Range(0.2f, 1f);
            var localScale = platform.transform.localScale;
            float fixedY = localScale.y;
            float fixedZ = localScale.z;

            platform.transform.position = new Vector3(spawnX, spawnY, 0f);
            platform.transform.localScale = new Vector3(randomX, fixedY, fixedZ);
            lastSpawnPlatformPositionY = spawnY;
            platform.SetActive(true);
            PlayerStats.platformSpawned++;
        }
    }

    private GameObject GetPooledPlatform()
    {
        return platformPool.FirstOrDefault(platform => !platform.activeInHierarchy);
    }


    private void DespawnPlatforms()
    {
        foreach (var platform in platformPool.Where(platform => platform.activeInHierarchy && playerTransform.position.y - platform.transform.position.y > despawnDistance))
        {
            StartCoroutine(DeactivatePlatformDelayed(platform));
        }

        foreach (var bigPlatform in bigPlatformPool.Where(platform => platform.activeInHierarchy && playerTransform.position.y - platform.transform.position.y > despawnDistance))
        {
            StartCoroutine(DeactivatePlatformDelayed(bigPlatform));
        }
    }

    private IEnumerator DeactivatePlatformDelayed(GameObject platform)
    {
        //animation miljko #todo
        //yield return new WaitForSeconds(despawnTimer); ovo ne radi
        platform.SetActive(false);
        yield return null;
    }
}
