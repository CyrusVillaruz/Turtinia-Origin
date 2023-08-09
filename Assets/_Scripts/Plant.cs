using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Plant : MonoBehaviour
{
    [SerializeField] private PlantBar plantHealthBar;
    [SerializeField] private PlantBar plantGrowthBar;
    [SerializeField] private Sprite fullPlantSprite;
    [SerializeField] private BoxCollider2D startCollider;
    [SerializeField] private BossController boss;
    [SerializeField] private float maxNecromancerSpawnRate;

    private PlayerController player;
    private float growth = 0f;
    private float timeSinceLastSpawn = 0f;
    private float currentNecromancerSpawnRate;
    private float currentHealth;

    private GrassSpread grassSpread;

    private bool isInteracting = false;
    private bool isAttacked = false;

    public List<GameObject> enemySpawnPoints = new List<GameObject>();
    public List<GameObject> enemies = new List<GameObject>();

    public GameObject necromancer;
    public GameObject playerUI;
    public GameObject plantUI;
    public GameObject bossUI;
    public GameObject floatingText;
    public GameObject plantGrowSpawnPoint;
    public BossBar bossBar;

    public TextMeshProUGUI startText;

    public float maxHealth;
    public float growthRate;
    public float spawnRate;
    public bool canGrow = true;
    public bool canSpawnEnemies = false;
    public bool isTargetted = true;
    public bool hasBossSpawned = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        plantHealthBar.SetMaxHealth(maxHealth);
        plantGrowthBar.SetMaxGrowth(maxHealth);
        plantGrowthBar.SetGrowth(growth);

        grassSpread = GetComponent<GrassSpread>();

        currentNecromancerSpawnRate = maxNecromancerSpawnRate;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && startText.gameObject.activeSelf && !isInteracting)
        {
            plantUI.SetActive(true);
            playerUI.SetActive(true);
            isInteracting = true;
            startCollider.enabled = false;
            canSpawnEnemies = true;
        }

        if (canSpawnEnemies)
        {
            if (currentHealth > 0 && growth < maxHealth && canSpawnEnemies)
            {
                if (canGrow)
                {
                    growth += growthRate * Time.deltaTime;
                    plantGrowthBar.SetGrowth(growth);
                }

                SpawnEnemies();
                SpawnBoss();

            }
        }

        GrowPlant();
    }

    private void SpawnBoss()
    {
        if (growth >= maxHealth * 0.8f && !hasBossSpawned)
        {
            bossUI.SetActive(true);
            canSpawnEnemies = false;
            canGrow = false;
            hasBossSpawned = true;

            GameObject spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Count)];
         
            bossBar.SetMaxBossHealth(boss.maxHealth);
            bossBar.SetBossHealth(boss.currentHealth);
            Instantiate(boss, spawnPoint.transform.position, Quaternion.identity);
            boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossController>();
        }
    }

    private void SpawnEnemies()
    {
        timeSinceLastSpawn += Time.deltaTime;
        currentNecromancerSpawnRate -= Time.deltaTime;

        if (timeSinceLastSpawn >= spawnRate)
        {
            GameObject spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Count)];
            GameObject enemyPrefab = enemies[Random.Range(0, enemies.Count)];
            Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);

            timeSinceLastSpawn = 0f;
        }

        if (currentNecromancerSpawnRate <= 0)
        {
            GameObject spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Count)];
            Instantiate(necromancer, spawnPoint.transform.position, Quaternion.identity);

            currentNecromancerSpawnRate = maxNecromancerSpawnRate;
        }
    }

    private float delay = 3f;
    private bool allowPlayerMovement = false;
    
    private void GrowPlant()
    {
        if (growth >= maxHealth && !allowPlayerMovement)
        {
            canSpawnEnemies = false;

            GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject enemy in enemyObjects)
            {
                Destroy(enemy);
            }

            delay -= Time.deltaTime;
            player.transform.position = plantGrowSpawnPoint.transform.position;

            if (delay <= 0)
            {
                allowPlayerMovement = true;
                GetComponent<SpriteRenderer>().sprite = fullPlantSprite;
                grassSpread.enabled = true;
            }
            playerUI.SetActive(false);
            plantUI.SetActive(false);

            foreach (Transform child in player.transform)
            {
                child.gameObject.SetActive(false);
            }

            StopAllCoroutines();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            startText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            startText.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss")) && isTargetted)
        {
            MeleeEnemyController meleeEnemy = collision.gameObject.GetComponent<MeleeEnemyController>();
            RangedEnemyController rangedEnemy = collision.gameObject.GetComponent<RangedEnemyController>();
            EnemyProjectile enemyProjectile = collision.gameObject.GetComponent<EnemyProjectile>();

            isAttacked = true;

            if (meleeEnemy != null) StartCoroutine(DamagePlant(meleeEnemy.damage));
            else if (rangedEnemy != null) StartCoroutine(DamagePlant(rangedEnemy.damage));
            else if (enemyProjectile != null) StartCoroutine(DamagePlant(enemyProjectile.damage));
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss")) && isTargetted)
        {
            MeleeEnemyController meleeEnemy = collision.gameObject.GetComponent<MeleeEnemyController>();
            RangedEnemyController rangedEnemy = collision.gameObject.GetComponent<RangedEnemyController>();
            EnemyProjectile enemyProjectile = collision.gameObject.GetComponent<EnemyProjectile>();

            isAttacked = false;

            if (meleeEnemy != null) StopCoroutine(DamagePlant(meleeEnemy.damage));
            else if (rangedEnemy != null)  StopCoroutine(DamagePlant(rangedEnemy.damage));
            else if (enemyProjectile != null) StopCoroutine(DamagePlant(enemyProjectile.damage));
        }
    }

    private IEnumerator DamagePlant(float damage)
    {
        while (isAttacked && isTargetted)
        {
            TakeDamage(damage);
            yield return new WaitForSeconds(1f);
        }
    }

    public void TakeDamage(float damage)
    {
        RectTransform textTransform = Instantiate(floatingText).GetComponent<RectTransform>();
        textTransform.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        Canvas textCanvas = GameObject.FindGameObjectWithTag("FloatingTextCanvas").GetComponent<Canvas>();
        textTransform.SetParent(textCanvas.transform);

        TextMeshProUGUI damageText = textTransform.GetComponentInChildren<TextMeshProUGUI>();
        damageText.text = damage.ToString();

        currentHealth -= damage;
        plantHealthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
