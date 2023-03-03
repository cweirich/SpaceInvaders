using UnityEngine;

public class Player : MonoBehaviour
{       
    public float speed = 1.5f;
    public float limit = 3f;
    public float missileSpeed = 3f;
    public float cooldownLimit = 1f;

    public GameObject missilePrefab;
    public GameObject explosionPrefab;

    private bool fired = false;
    private float cooldown;
    private static Player instance;

    public static Player Instance => instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Update is called once per frame
    private void Update()
    {
        float posX = transform.position.x;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, 0);

        if (posX > limit || posX < -limit)
        {
            if (posX > 0)
                transform.position = new Vector2(limit, transform.position.y);
            else
                transform.position = new Vector2(-limit, transform.position.y);

            rb.velocity = Vector2.zero;
        }

        cooldown -= Time.deltaTime;
        if (Input.GetAxis("Fire1") == 1f)
        {
            if (cooldown <= 0 && !fired)
            {
                fired = true;
                cooldown = cooldownLimit;

                GameObject missile = Instantiate(missilePrefab);

                missile.transform.SetParent(transform.parent);
                missile.transform.position = transform.position;
                missile.GetComponent<Rigidbody2D>().velocity = new Vector2(0, missileSpeed);
                Destroy(missile, 1f);
            }
        }
        else
            fired = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyMissile") || collision.CompareTag("Enemy"))
        {
            GameObject explosion = Instantiate(explosionPrefab);
            explosion.transform.SetParent(transform.parent);
            explosion.transform.position = transform.position;

            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }

    public void ResetCooldown()
    {
        cooldown = 0;
    }
}
