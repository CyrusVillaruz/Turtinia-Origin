using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Vector2 movementInput;
    [HideInInspector] public float acceleration;

    [SerializeField] private Animator swordAnim;
    [SerializeField] private Transform sword;
    [SerializeField] private Transform safetySpawnPoint, gameSpawnPoint;
    [SerializeField] private TextMeshProUGUI respawnText;
    [SerializeField] private PlayerWand wand;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;

    public PlayerBars healthBar, staminaBar, manaBar;
    public GameObject floatingText;

    private int idleStaminaIncrement;

    public float movementSpeed;
    public float maxHealth, maxStamina, maxMana;
    public float currentHealth, currentStamina, currentMana;
    public float knockbackForce;
    public float invincibilityTime;

    public float iframeDuration;
    public bool isInvincible = false;
    public bool canDamage = true;

    public int swordDamage;
    public int staminaIncrement, staminaIncrementMultiplier, manaIncrement, healthIncrement;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        safetySpawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;

        acceleration = movementSpeed;

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);

        currentMana = maxMana;
        manaBar.SetMaxMana(maxMana);

        idleStaminaIncrement = staminaIncrement * staminaIncrementMultiplier;
    }

    public void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    public void OnFire()
    {
        wand.isFiring = true;
        swordAnim.SetTrigger("meleeAttack");
    }

    private void Update()
    {
        if (movementInput.magnitude == 0f) staminaIncrement = idleStaminaIncrement;
        else staminaIncrement = idleStaminaIncrement / staminaIncrementMultiplier;

        if (currentHealth > maxHealth) currentHealth = maxHealth;
        else currentHealth += healthIncrement * Time.deltaTime;
        healthBar.SetHealth(currentHealth);

        if (currentStamina > maxStamina) currentStamina = maxStamina;
        else currentStamina += staminaIncrement * Time.deltaTime;
        staminaBar.SetStamina(currentStamina);

        if (currentMana > maxStamina) currentMana = maxMana;
        else currentMana += manaIncrement * Time.deltaTime;
        manaBar.SetStamina(currentMana);
    }

    private void FixedUpdate()
    {
        rb.velocity += movementInput * acceleration * Time.fixedDeltaTime;

        FaceMouse();
        
        if (movementInput.magnitude > 0f) anim.SetBool("run", true);
        else anim.SetBool("run", false);
    }
    private void FaceMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.x -= objectPos.x;
        if (mousePos.x > 0)
        {
            sr.flipX = false;
            sword.localScale = Vector2.one;
        }
        else if (mousePos.x < 0)
        {
            sr.flipX = true;
            sword.localScale = new Vector2(-1, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            Rigidbody2D enemyRb = collision.GetComponent<Rigidbody2D>();
            Vector2 knockbackDirection = enemyRb.transform.position - transform.position;
            knockbackDirection.Normalize();

            if (collision.GetComponent<EnemyController>())
            {
                collision.GetComponent<EnemyController>().TakeDamage(swordDamage, knockbackDirection);
            }
            else if (collision.GetComponent<RangedEnemyController>())
            {
                collision.GetComponent<RangedEnemyController>().TakeDamage(swordDamage, knockbackDirection);
            }
        }
    }

    public void TakeDamage(float damage, Vector2 knockbackDirection)
    {
        if (!canDamage) return;

        RectTransform textTransform = Instantiate(floatingText).GetComponent<RectTransform>();
        textTransform.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        Canvas textCanvas = GameObject.FindGameObjectWithTag("FloatingTextCanvas").GetComponent<Canvas>();
        textTransform.SetParent(textCanvas.transform);

        TextMeshProUGUI damageText = textTransform.GetComponentInChildren<TextMeshProUGUI>();
        damageText.text = damage.ToString();

        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        if (currentHealth <= 0)
        {
            Respawn();
        }

        StartCoroutine(InvincibilityCountdown());
    }

    private IEnumerator InvincibilityCountdown()
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

    public void ConsumeStamina(float stamina)
    {
        currentStamina -= stamina;
        staminaBar.SetStamina(currentStamina);
    }

    public void ConsumeMana(float mana)
    {
        currentMana -= mana;
        manaBar.SetMana(currentMana);
    }

    public void Respawn()
    {
        GetComponent<PlayerInput>().enabled = false;
        isInvincible = true;

        StartCoroutine(RespawnCountdown());

        currentHealth = maxHealth;
        transform.position = safetySpawnPoint.position;
        healthBar.SetHealth(currentHealth);
    }
    private IEnumerator RespawnCountdown()
    {
        respawnText.gameObject.SetActive(true);
        GetComponent<PlayerInput>().enabled = false;
        transform.position = safetySpawnPoint.position;

        for (int i = (int) invincibilityTime; i >= 0; i--)
        {
            respawnText.text = "Respawning in " + i + "...";
            yield return new WaitForSeconds(1f);
        }
        respawnText.gameObject.SetActive(false);
        isInvincible = false;
        GetComponent<PlayerInput>().enabled = true;
        transform.position = gameSpawnPoint.position;
    }
}
