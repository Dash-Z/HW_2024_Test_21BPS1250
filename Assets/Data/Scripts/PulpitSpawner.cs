using UnityEngine;
using System.Collections;

public class RandomPlatformSpawner : MonoBehaviour
{
    [System.Serializable]
    public class PulpitData
    {
        public float min_pulpit_destroy_time;
        public float max_pulpit_destroy_time;
        public float pulpit_spawn_time;
    }

    [System.Serializable]
    public class GameData
    {
        public PulpitData pulpit_data;
    }

    public GameObject Pulpits;
    public float platformSize;
    public float minDespawnTime;
    public float maxDespawnTime;
    public float platformSpawnInterval;

    private int platformCount = 0; // Counter to keep track of active platforms

    void Start()
    {
        LoadConfiguration();

        // Start the spawning process with the first platform
        Vector3 initialPosition = Vector3.zero;
        SpawnPlatform(initialPosition);

        StartCoroutine(SpawnPlatformsRepeatedly());
    }

    void LoadConfiguration()
    {
        TextAsset json = Resources.Load<TextAsset>("doofus_diary");

        if(json!=null)
        {
            GameData gameData = JsonUtility.FromJson<GameData>(json.text);
            minDespawnTime = gameData.pulpit_data.min_pulpit_destroy_time;
            maxDespawnTime = gameData.pulpit_data.max_pulpit_destroy_time;
            platformSpawnInterval = gameData.pulpit_data.pulpit_spawn_time;
        }
        else
        {
            Debug.LogError("Config file not found.");
        }
    }

    void SpawnPlatform(Vector3 position)
    {
        if (platformCount >= 2)
        {
            return;
        }

        //First Platform Spawnnnnnnnnnnnnnnn
        GameObject newPlatform = Instantiate(Pulpits, position, Quaternion.identity);
        platformCount++;
        float randomDespawnTime = Random.Range(minDespawnTime, maxDespawnTime);
        StartCoroutine(HandlePlatformLifecycle(newPlatform, randomDespawnTime)); //PlatformLifeCycle
    }

    IEnumerator HandlePlatformLifecycle(GameObject platform, float lifetime)
    {
        yield return new WaitForSeconds(lifetime / 2);
        if (platform != null)
        {
            if (platformCount == 1)
            {
                Vector3 nextPosition = GetRandomAdjacentPosition(platform.transform.position);
                SpawnPlatform(nextPosition);
            }
        }

        yield return new WaitForSeconds(lifetime / 2);
        if (platform != null)
        {
            Destroy(platform);
            platformCount--;
        }
    }

    IEnumerator SpawnPlatformsRepeatedly()
    {
        while (true)
        {
            yield return new WaitForSeconds(platformSpawnInterval);
            if (platformCount < 2)
            {
                GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
                if (platforms.Length > 0)
                {
                    Vector3 lastPlatformPosition = platforms[platforms.Length - 1].transform.position;
                    Vector3 nextPosition = GetRandomAdjacentPosition(lastPlatformPosition);
                    SpawnPlatform(nextPosition);
                }
            }
        }
    }

    Vector3 GetRandomAdjacentPosition(Vector3 basePosition)
    {
        Vector3[] directions = new Vector3[]
        {
            new Vector3(platformSize, 0, 0),  // Right
            new Vector3(-platformSize, 0, 0), // Left
            new Vector3(0, 0, platformSize),  // Front
            new Vector3(0, 0, -platformSize)  // Back
        };

        Vector3 randomDirection = directions[Random.Range(0, directions.Length)];

        return basePosition + randomDirection;
    }
}
