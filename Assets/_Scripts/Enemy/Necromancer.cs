using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necromancer : RangedEnemyController
{
    public GameObject skeletonPrefab;
    public float spawnDelay;
    public float spawnOffset;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(SpawnSkeleton());
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        GetEnemies();
    }

    IEnumerator SpawnSkeleton()
    {
        while (true)
        {
            Vector3 spawnPositionLeft = transform.position - transform.right * spawnOffset;
            Vector3 spawnPositionRight = transform.position + transform.right * spawnOffset;
            Instantiate(skeletonPrefab, spawnPositionLeft, Quaternion.identity);
            Instantiate(skeletonPrefab, spawnPositionRight, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
