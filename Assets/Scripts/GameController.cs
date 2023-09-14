using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public float enemyShootingInterval = 3f;
    public float enemyMaxMissileSpeed = 2f;
    public float enemyMinMissileSpeed = 1f;
    public float enemyMovementHorizontalSpeed = 0.2f;
    public float enemyMovementVerticalSpeed = 0.6f;
    public float enemyMaxMovementInterval = 1f;
    public float enemyMinMovementInterval = 0.1f;
    public float horizontalLimit = 3f;

    public GameObject enemyMissilePrefab;
    public GameObject enemyContainer;

    public Text levelText;
    public Text scoreText;
    public Text gameOverText;
    public Text levelUpText;

    public AudioSource boingSfx;
    public AudioSource deathSfx;
    public AudioSource levelUpSfx;

    private float enemyShootingTimer;
    private float enemyMissileSpeed;
    private float enemyMovementTimer;
    private float enemyMovementDirection = 1f;
    private float enemyMovementInterval;
    private Player player;
    private Enemy[] enemies;
    private int initialEnemyCount;
    private readonly float restartTimer = 3f;

    private static int level = 1;
    private static int score = 0;

    // Start is called before the first frame update
    public void Start()
    {
        player = GetComponentInChildren<Player>();
        player.OnPlayerHit += GameOver;
        enemies = GetComponentsInChildren<Enemy>();
        foreach(var enemy in enemies)
        {
            enemy.OnEnemyHit += OnEnemyHit;
            var sr = enemy.GetComponent<SpriteRenderer>();
            Color yellow = new Color(1.0f, 0.961f, 0);
            if (sr.color == Color.white)
                enemy.value = 30 * level;
            else if (sr.color.ToString() == yellow.ToString())
                enemy.value = 20 * level;
            else
                enemy.value = 10 * level;
        }
        initialEnemyCount = enemies.Length;

        levelText.text = level.ToString();
        scoreText.text = score.ToString();
        gameOverText.gameObject.SetActive(false);
        levelUpText.gameObject.SetActive(false);

        ResetEnemyShootingInterval();
        ResetEnemyMovementInterval();
    }

    // Update is called once per frame
    public void Update()
    {
        enemies = GetComponentsInChildren<Enemy>();
        EnemyShooting();
        EnemyMovement();
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void EnemyMovement()
    {
        enemyMovementTimer -= Time.deltaTime;
        if (enemyMovementTimer <= 0)
        {
            ResetEnemyMovementInterval();

            if (enemies.Length == 0)
                return;

            if (!boingSfx.isPlaying)
            {
                boingSfx.pitch = ManagePitch(boingSfx.pitch);
                boingSfx.Play();
            }

            float posX = enemyContainer.transform.position.x;
            float posY = enemyContainer.transform.position.y;

            enemyContainer.transform.position = new Vector2(posX + enemyMovementHorizontalSpeed * enemyMovementDirection, posY);

            if (enemyMovementDirection > 0)
            {
                float rightmostEnemyPos = 0;

                foreach (Enemy enemy in enemies)
                {
                    float enemyX = enemy.transform.position.x;
                    if (enemyX > rightmostEnemyPos)
                        rightmostEnemyPos = enemyX;
                }

                if (rightmostEnemyPos >= horizontalLimit)
                    DownAndReverse(posX, posY);
            }
            else
            {
                float leftmostEnemyPos = horizontalLimit;

                foreach (Enemy enemy in enemies)
                {
                    float enemyX = enemy.transform.position.x;
                    if (enemyX < leftmostEnemyPos)
                        leftmostEnemyPos = enemyX;
                }

                if (leftmostEnemyPos <= -horizontalLimit)
                    DownAndReverse(posX, posY);
            }
        }
    }

    private float ManagePitch(float currentPitch)
    {
        if (Mathf.Approximately(currentPitch, 0.7f))
            return 1;

        return currentPitch - 0.1f;
    }

    private void DownAndReverse(float posX, float posY)
    {
        enemyMovementDirection *= -1;
        enemyContainer.transform.position = new Vector2(posX, posY - enemyMovementHorizontalSpeed);
    }

    private void EnemyShooting()
    {
        enemyShootingTimer -= Time.deltaTime;
        if (enemyShootingTimer <= 0)
        {
            ResetEnemyShootingInterval();

            if (enemies.Length == 0)
                return;

            Enemy shootingEnemy = enemies[Random.Range(0, enemies.Length - 1)];
            GameObject enemyMissile = Instantiate(enemyMissilePrefab);

            enemyMissile.transform.SetParent(transform);
            enemyMissile.transform.position = shootingEnemy.transform.position;
            enemyMissile.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -enemyMissileSpeed);
            Destroy(enemyMissile, 4f);
        }
    }

    private void ResetEnemyShootingInterval()
    {
        int currentEnemyCount = enemies.Length;
        float difficulty = 1f - (float)currentEnemyCount / (float)initialEnemyCount;

        enemyMissileSpeed = enemyMinMissileSpeed + (enemyMaxMissileSpeed - enemyMinMissileSpeed) * difficulty;
        enemyShootingTimer = enemyShootingInterval;
    }

    private void ResetEnemyMovementInterval()
    {
        int currentEnemyCount = enemies.Length;
        float difficulty = 1f - (float)currentEnemyCount / (float)initialEnemyCount;

        enemyMovementInterval = enemyMaxMovementInterval - (enemyMaxMovementInterval - enemyMinMovementInterval) * difficulty;
        enemyMovementTimer = enemyMovementInterval;
    }

    private void OnEnemyHit(int points)
    {
        AddToScore(points);

        if (enemies.Length == 1)
            LevelUp();
    }

    private void GameOver()
    {
        gameOverText.gameObject.SetActive(true);
        deathSfx.Play();
        StartCoroutine(Restart());
        score = 0;
        level = 1;
    }

    private void LevelUp()
    {
        levelUpText.gameObject.SetActive(true);
        levelUpSfx.Play();
        StartCoroutine(Restart());
        level++;
    }

    private IEnumerator Restart()
    {
        yield return new WaitForSeconds(restartTimer);

        SceneManager.LoadScene("Game");
    }

    private void AddToScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();
    }
}
