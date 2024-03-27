using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms;

public class PlatformObjectPool : MonoBehaviour
{
    [SerializeField] private PlatformsConfiguration platformConfig;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform container;
    [SerializeField] private Transform topBounds;
    [SerializeField] private Transform bottomBounds;
    [SerializeField] private int platformPoolSize;
    [SerializeField] private float despawnDistance;
    [SerializeField] private float spawnInterval;
    [SerializeField] private float despawnTimer = 3f;
    [SerializeField] private float lastSpawnPlatformPositionY;
    
    private List<Platform> platformPool = new ();
    private List<GameObject> bigPlatformPool = new ();
    private bool init = false;
    private double accumulatedWeights;
    private System.Random random = new System.Random();
    
    private void Start()
    {
        playerTransform ??= Camera.main.transform;
        lastSpawnPlatformPositionY = bottomBounds.position.y;

        CalculateWeights();
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
            var spawningPlatform = platformConfig.BasicPlatforms[GetRandomPlatformIndex()];
            var platform = Instantiate(spawningPlatform.Prefab, container);
            platform.SetPlatformConfig(spawningPlatform);
            platform.gameObject.SetActive(false);
            platformPool.Add(platform);
            
            Debug.Log("Spawned " + spawningPlatform.Prefab.name);
        }

        int initialActivePlatforms = 10;
        lastSpawnPlatformPositionY = -15f;
        PlayerStats.platformSpawned = initialActivePlatforms;
        for (int i = 0; i < initialActivePlatforms; i++)
        {
            platformPool[i].gameObject.SetActive(true);
            // float randomX = Random.Range(0.2f, 0.8f);
            // float spawnX = Random.Range(-4f,4f);
            // if(randomX > 0.5f)
            //     spawnX = Random.Range(-2.2f, 2.2f); // Adjust as needed
            // else
            // {
            //     spawnX = Random.Range(-3.6f, 3.6f);
            // }
            float spawnX = Random.Range(
                -platformPool[i].platformConfiguration.SpawnPositionRange,
                platformPool[i].platformConfiguration.SpawnPositionRange);
            float spawnY = lastSpawnPlatformPositionY + 2.5f; // Adjust as needed
            // float fixedY = platformPool[i].transform.localScale.y;
            // float fixedZ = platformPool[i].transform.localScale.z;

            platformPool[i].transform.position = new Vector3(spawnX, spawnY, 0f);
            // platformPool[i].transform.localScale = new Vector3(randomX, fixedY, fixedZ);
            lastSpawnPlatformPositionY = spawnY;
        }

        GameObject bigPlatform = Instantiate(platformConfig.ZonePlatform, container);
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
        Platform platform = GetPooledPlatform();
        if (platform != null)
        {
            // float spawnX = Random.Range(-2.2f, 2.2f); // Adjust as needed
            float spawnY = lastSpawnPlatformPositionY + 2f; // Adjust as needed
            // float randomX = Random.Range(0.2f, 1f);
            // var localScale = platform.transform.localScale;
            // float fixedY = localScale.y;
            // float fixedZ = localScale.z;

            float spawnX = Random.Range(
                -platform.platformConfiguration.SpawnPositionRange,
                platform.platformConfiguration.SpawnPositionRange);
            platform.transform.position = new Vector3(spawnX, spawnY, 0f);
            //platform.transform.localScale = new Vector3(randomX, fixedY, fixedZ);
            lastSpawnPlatformPositionY = spawnY;
            platform.gameObject.SetActive(true);
            PlayerStats.platformSpawned++;
        }
    }

    private Platform GetPooledPlatform()
    {
        return platformPool.FirstOrDefault(platform => !platform.gameObject.activeInHierarchy);
    }


    private void DespawnPlatforms()
    {
        foreach (var platform in platformPool.Where(platform => platform.gameObject.activeInHierarchy && playerTransform.position.y - platform.transform.position.y > despawnDistance))
        {
            StartCoroutine(DeactivatePlatformDelayed(platform));
        }

        foreach (var bigPlatform in bigPlatformPool.Where(platform => platform.activeInHierarchy && playerTransform.position.y - platform.transform.position.y > despawnDistance))
        {
            StartCoroutine(DeactivateBigPlatformDelayed(bigPlatform));
        }
    }

    private IEnumerator DeactivatePlatformDelayed(Platform platform)
    {
        //yield return new WaitForSeconds(despawnTimer); ovo ne radi
        platform.gameObject.SetActive(false);
        yield return null;
    }
    
    private IEnumerator DeactivateBigPlatformDelayed(GameObject platform)
    {
        //yield return new WaitForSeconds(despawnTimer); ovo ne radi
        platform.SetActive(false);
        yield return null;
    }

    private int GetRandomPlatformIndex()
    {
        var r = random.NextDouble() * accumulatedWeights;
        for (int i = 0; i < platformConfig.BasicPlatforms.Count; i++)
        {
            if (platformConfig.BasicPlatforms[i]._weight >= r)
                return i;
        }

        return 0;
    }
    
    private void CalculateWeights()
    {
        accumulatedWeights = 0;
        foreach (var platform in platformConfig.BasicPlatforms)
        {
            accumulatedWeights += platform.SpawnProbability;
            platform._weight = accumulatedWeights;
        }
    }
}
