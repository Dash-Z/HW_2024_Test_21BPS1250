using UnityEngine;
using System.Collections;

public class RandomPlatformSpawner : MonoBehaviour
{
    public GameObject Pulpits;    // Assign the flat platform prefab in the Inspector
    public float platformSize = 18.0f;    // The size of the platform (used for spacing)
    public float minDespawnTime = 3.0f;  // Minimum time before the platform despawns
    public float maxDespawnTime = 5.0f;  // Maximum time before the platform despawns

    void Start()
    {
        // Start the spawning process with the first platform
        Vector3 initialPosition = Vector3.zero;  // Initial spawn at world origin
        SpawnPlatform(initialPosition);
    }

    void SpawnPlatform(Vector3 position)
    {
        // Instantiate the platform at the given position and start its lifecycle
        GameObject newPlatform = Instantiate(Pulpits, position, Quaternion.identity);

        // Determine the random time for despawn
        float randomDespawnTime = Random.Range(minDespawnTime, maxDespawnTime);

        // Start the lifecycle coroutine for this platform
        StartCoroutine(HandlePlatformLifecycle(newPlatform, randomDespawnTime));
    }

    IEnumerator HandlePlatformLifecycle(GameObject platform, float lifetime)
    {
        // Wait for half of the lifetime
        yield return new WaitForSeconds(lifetime / 2);

        // If the platform still exists, spawn the next one
        if (platform != null)
        {
            // Calculate the next platform's position
            Vector3 nextPosition = GetRandomAdjacentPosition(platform.transform.position);

            // Spawn the next platform
            SpawnPlatform(nextPosition);
        }

        // Wait for the remaining half of the lifetime
        yield return new WaitForSeconds(lifetime / 2);

        // Destroy the current platform after its full lifetime has elapsed
        if (platform != null)
        {
            Destroy(platform);
        }
    }

    Vector3 GetRandomAdjacentPosition(Vector3 basePosition)
    {
        // Define possible directions (right, left, front, back) based on edge alignment
        Vector3[] directions = new Vector3[]
        {
            new Vector3(platformSize, 0, 0),  // Right
            new Vector3(-platformSize, 0, 0), // Left
            new Vector3(0, 0, platformSize),  // Front
            new Vector3(0, 0, -platformSize)  // Back
        };

        // Choose a random direction from the array
        Vector3 randomDirection = directions[Random.Range(0, directions.Length)];

        // Return the new position by adding the chosen direction to the base position
        return basePosition + randomDirection;
    }
}
