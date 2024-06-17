using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public LayerMask groundLayer;

    void Start()
    {
        Vector3 spawnPosition;
        int attempts = 0;
        int maxAttempts = 10;

        do
        {
            spawnPosition = GetRandomPositionWithinArea();
            attempts++;
        } while (!IsValidSpawnPosition(spawnPosition) && attempts < maxAttempts);

        if (attempts < maxAttempts)
        {
            Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Error");
        }
    }

    private Vector3 GetRandomPositionWithinArea()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        Vector3 center = boxCollider.center;
        Vector3 size = boxCollider.size;

        float x = center.x + Random.Range(-size.x / 2, size.x / 2);
        float y = center.y + Random.Range(-size.y / 2, size.y / 2);
        float z = center.z + Random.Range(-size.z / 2, size.z / 2);

        Vector3 randomPosition = transform.TransformPoint(new Vector3(x, y + 5, z));

        return randomPosition;
    }

    private bool IsValidSpawnPosition(Vector3 position)
    {
        float checkRadius = 0.5f;
        Collider[] colliders = Physics.OverlapSphere(position, checkRadius, groundLayer);

        return colliders.Length == 0;
    }
}

