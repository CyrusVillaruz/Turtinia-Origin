using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MeleeEnemyController : EnemyController
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.collider.GetComponent<Rigidbody2D>();
            Vector2 knockbackDirection = playerRb.transform.position - transform.position;
            knockbackDirection.Normalize();

            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (player.canDamage) player.TakeDamage(damage, knockbackDirection);
        }
    }
}
