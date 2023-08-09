using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RangedEnemyController : EnemyController
{
    private float timeBetweenShots;

    public GameObject projectile;

    public float stoppingDistance;
    public float retreatDistance;
    public float startTimeBetweenShots;

    protected override void Start()
    {
        base.Start();
        timeBetweenShots = startTimeBetweenShots;
    }

    protected override void FixedUpdate()
    {
        if (!player.gameObject.GetComponent<PlayerController>().isInvincible)
        {
            Vector2 toPlayer = player.position - transform.position;
            toPlayer.Normalize();

            if (Vector2.Distance(transform.position, player.position) > stoppingDistance) Move(toPlayer, GetEnemies());
            else if (Vector2.Distance(transform.position, player.position) < stoppingDistance && Vector2.Distance(transform.position, player.position) > retreatDistance) Move(Vector2.zero, GetEnemies());
            else if (Vector2.Distance(transform.position, player.position) < retreatDistance) Move(-toPlayer, GetEnemies());
        }
        else
        {
            Vector2 toPlant = plant.position - transform.position;
            toPlant.Normalize();

            if (Vector2.Distance(transform.position, plant.position) > stoppingDistance) Move(toPlant, GetEnemies());
            else if (Vector2.Distance(transform.position, plant.position) < stoppingDistance && Vector2.Distance(transform.position, plant.position) > retreatDistance) Move(Vector2.zero, GetEnemies());
        }

        if (!sr.flipX) projectile.transform.localScale = new Vector3(1, -1, 1);
        else projectile.transform.localScale = new Vector3(1, 1, 1);

        if (timeBetweenShots <= 0)
        {
            Instantiate(projectile, transform.position, Quaternion.identity);
            timeBetweenShots = startTimeBetweenShots;
        }
        else
        {
            timeBetweenShots -= Time.deltaTime;
        }
    }
}
