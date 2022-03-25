using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Player player;
    public GameObject explosionPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        player = Player.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerMissile"))
        {
            GameObject explosion = Instantiate(explosionPrefab);
            explosion.transform.SetParent(transform.parent.parent);
            explosion.transform.position = transform.position;

            Destroy(gameObject);
            Destroy(collision.gameObject);

            player.ResetCooldown();
        }
    }
}
