using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private float enemyShootingTimer;
    private float enemyMissileSpeed;
    private float enemyMovementTimer;
    private float enemyMovementDirection = 1f;
    private float enemyMovementInterval;
    private Enemy[] enemies;
    private int initialEnemyCount;

    // Start is called before the first frame update
    void Start()
    {
        enemies = GetComponentsInChildren<Enemy>();
        initialEnemyCount = enemies.Length;

        ResetEnemyShootingInterval();
        ResetEnemyMovementInterval();
    }

    // Update is called once per frame
    void Update()
    {
        enemies = GetComponentsInChildren<Enemy>();
        EnemyShooting();
        EnemyMovement();
    }

    private void EnemyMovement()
    {
        enemyMovementTimer -= Time.deltaTime;
        if (enemyMovementTimer <= 0)
        {
            ResetEnemyMovementInterval();

            if (enemies.Length == 0)
                return;
            
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
}
