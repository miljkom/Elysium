using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public PlatformsConfiguration.SpawnPlatform platformConfiguration { get; private set; }
    public void SetPlatformConfig(PlatformsConfiguration.SpawnPlatform configuration) => platformConfiguration = configuration;
}
