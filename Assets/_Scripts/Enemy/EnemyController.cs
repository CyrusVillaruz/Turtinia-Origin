using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    [SerializeField] protected Transform player;
    [SerializeField] protected Transform plant;
    [SerializeField] protected SpriteRenderer sr;
    [SerializeField] protected Rigidbody2D rb;

    protected Plant plantRef;
    protected Animator anim;

    public GameObject floatingText;

    public float maxHealth;
    public float currentHealth;
    public float knockbackForce;
    public float damage;
    public float movementSpeed;
    public float iframeDuration;
    public float separationRadius;
    protected bool canDamage = true;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        plant = GameObject.FindGameObjectWithTag("Plant").transform;
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        plantRef = GameObject.FindGameObjectWithTag("Plant").GetComponent<Plant>();
    }

    protected virtual void FixedUpdate()
    {
        if (!player.gameObject.GetComponent<PlayerController>().isInvincible)
        {
            Vector2 toPlayer = player.position - transform.position;
            toPlayer.Normalize();

            plantRef.isTargetted = false;
            Move(toPlayer, GetEnemies());
        }
        else
        {
            Vector2 toPlant = plant.position - transform.position;
            toPlant.Normalize();

            plantRef.isTargetted = true;
            Move(toPlant, GetEnemies());
        }
    }

    protected virtual List<Transform> GetEnemies()
    {
        List<Transform> enemies = new List<Transform>();

        foreach (GameObject enemyObj in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemies.Add(enemyObj.transform);
        }

        return enemies;
    }

    protected virtual void Move(Vector2 toTarget, List<Transform> enemies)
    {
        Vector2 separationForce = Vector2.zero;
        bool shouldFlip = false;

        foreach (Transform enemy in enemies)
        {
            if (enemy != transform)
            {
                Vector2 toEnemy = transform.position - enemy.position;
                float distance = toEnemy.magnitude;
                if (distance < separationRadius)
                {
                    float strength = Mathf.Clamp01((separationRadius - distance) / separationRadius);
                    separationForce += toEnemy.normalized * strength;
                }
            }
            if (!plantRef.isTargetted)
            {
                Vector2 toPlant = transform.position - plant.position;
                float plantDistance = toPlant.magnitude;
                if (plantDistance < separationRadius)
                {
                    float newSeparationRadius = separationRadius * 2f;
                    float plantStrength = Mathf.Clamp01((newSeparationRadius - plantDistance) / newSeparationRadius);
                    Vector2 perpendicular = Vector2.Perpendicular(toPlant.normalized);
                    Vector2 plantForce = perpendicular * plantStrength * movementSpeed * Time.deltaTime * (1.0f - plantDistance / newSeparationRadius);
                    rb.AddForce(plantForce);
                }
            }
        }

        rb.AddForce(separationForce * movementSpeed * Time.deltaTime);

        if (toTarget.x > 0 && sr.flipX) shouldFlip = true;
        else if (toTarget.x < 0 && !sr.flipX) shouldFlip = true;

        if (shouldFlip) sr.flipX = !sr.flipX;

        rb.AddForce(toTarget * movementSpeed * Time.deltaTime);
        anim.SetBool("run", toTarget != Vector2.zero);
    }

    public virtual void TakeDamage(float damage, Vector2 knockbackDirection)
    {
        if (!canDamage) return;

        RectTransform textTransform = Instantiate(floatingText).GetComponent<RectTransform>();
        textTransform.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        Canvas textCanvas = GameObject.FindGameObjectWithTag("FloatingTextCanvas").GetComponent<Canvas>();
        textTransform.SetParent(textCanvas.transform);

        TextMeshProUGUI damageText = textTransform.GetComponentInChildren<TextMeshProUGUI>();
        damageText.text = damage.ToString();

        currentHealth -= damage;
        if (currentHealth <= 0) DestroyEnemy();

        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        StartCoroutine(InvincibilityCountdown());
    }

    protected IEnumerator InvincibilityCountdown()
    {
        canDamage = false;
        for (float i = 0f; i < iframeDuration; i += Time.deltaTime)
        {
            sr.enabled = !sr.enabled;
            yield return null;
        }

        sr.enabled = true;
        canDamage = true;
    }

    protected virtual void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
