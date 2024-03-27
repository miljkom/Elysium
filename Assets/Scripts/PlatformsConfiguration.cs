using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlatformsConfiguration")]
public class PlatformsConfiguration : ScriptableObject
{
    [System.Serializable]
    public class SpawnPlatform
    {
        public Platform Prefab;
        [Range(0, 5)] public float SpawnPositionRange;
        [Range(0, 100)] public float SpawnProbability;
        [HideInInspector] public double _weight;
    }
    public List<SpawnPlatform> BasicPlatforms = new ();
    [Space(5f)] public GameObject ZonePlatform;
}