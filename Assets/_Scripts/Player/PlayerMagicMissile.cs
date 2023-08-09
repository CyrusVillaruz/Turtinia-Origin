using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagicMissile : MonoBehaviour
{
    public Animator animator;
    public LayerMask enemyLayerMask;

    public float speed;
    public float lifeTime;
    public float distance;
    public float damage;
    public float manaCost;
    public float aoeRadius;

    private bool hasCollided = false;
    private bool isBoss = false;

    private void Start()
    {
        Invoke("DestroyProjectile", lifeTime);
    }

    private void Update()
    {
        if (!hasCollided)
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        MeleeEnemyController meleeEnemy = collision.gameObject.GetComponent<MeleeEnemyController>();
        RangedEnemyController rangedEnemy = collision.gameObject.GetComponent<RangedEnemyController>();
        Necromancer necromancer = collision.gameObject.GetComponent<Necromancer>();

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (meleeEnemy != null)
            {
                ApplyDamageToEnemies(meleeEnemy.transform.position);
                meleeEnemy.TakeDamage(damage, Vector2.zero);
            }
            else if (rangedEnemy != null)
            {
                ApplyDamageToEnemies(rangedEnemy.transform.position);
                rangedEnemy.TakeDamage(damage, Vector2.zero);
            }
            else if (necromancer != null)
            {
                ApplyDamageToEnemies(necromancer.transform.position);
                necromancer.TakeDamage(damage, Vector2.zero);
            }

            hasCollided = true;
            PlayDestroyAnimation();
        }
        else if (collision.gameObject.CompareTag("Boss"))
        {
            isBoss = true;
            BossController boss = collision.gameObject.GetComponent<BossController>();
            if (boss != null) boss.TakeDamage(damage, Vector2.zero);

            hasCollided = true;
            PlayDestroyAnimation();
        }
    }

    private void ApplyDamageToEnemies(Vector2 centerPoint)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(centerPoint, aoeRadius, enemyLayerMask);

        foreach (Collider2D hitCollider in hitColliders)
        {
            MeleeEnemyController meleeEnemy = hitCollider.GetComponent<MeleeEnemyController>();
            RangedEnemyController rangedEnemy = hitCollider.GetComponent<RangedEnemyController>();
            Necromancer necromancer = hitCollider.GetComponent<Necromancer>();

            if (meleeEnemy != null && !isBoss)
            {
                Vector2 knockbackDirection = meleeEnemy.transform.position - transform.position;
                knockbackDirection.Normalize();
                meleeEnemy.TakeDamage(damage, knockbackDirection);
            }
            else if (rangedEnemy != null && !isBoss)
            {
                Vector2 knockbackDirection = rangedEnemy.transform.position - transform.position;
                knockbackDirection.Normalize();
                rangedEnemy.TakeDamage(damage, knockbackDirection);
            }
            else if (necromancer != null && !isBoss)
            {
                Vector2 knockbackDirection = necromancer.transform.position - transform.position;
                knockbackDirection.Normalize();
                necromancer.TakeDamage(damage, knockbackDirection);
            }
        }
    }

    private void PlayDestroyAnimation()
    {
        if (animator != null)
        {
            if (hasCollided)
            {
                animator.SetTrigger("Hit");
                AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
                Invoke("DestroyProjectile", clipInfo[0].clip.length);
            }
            else
            {
                animator.SetTrigger("Destroy");
                animator.CrossFade("Hit", 0f);
                Invoke("DestroyProjectile", 0);
            }
        }
        else
        {
            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
