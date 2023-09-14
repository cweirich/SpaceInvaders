using UnityEngine;

public class Enemy : MonoBehaviour
{
    public delegate void EnemyHandler(int points);
    public event EnemyHandler OnEnemyHit;
    public int value = 10;

    public GameObject explosionPrefab;
    private Player player;
    
    // Start is called before the first frame update
    public void Start()
    {
        player = Player.Instance;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerMissile"))
        {
            GameObject explosion = Instantiate(explosionPrefab);
            explosion.transform.SetParent(transform.parent.parent);
            explosion.transform.position = transform.position;

            Destroy(gameObject);
            Destroy(collision.gameObject);
            OnEnemyHit?.Invoke(value);

            player.ResetCooldown();
        }
    }
}
