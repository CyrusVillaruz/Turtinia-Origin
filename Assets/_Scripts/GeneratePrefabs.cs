using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePrefabs : MonoBehaviour
{
    public GameObject prefab;
    public float spawnDistance;
    public int numberOfObjects;

    void Start()
    {
        Vector3 centerPosition = transform.position;

        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * spawnDistance;
            Vector3 position = centerPosition + randomOffset;

            Instantiate(prefab, position, Quaternion.identity);
        }
    }

}
