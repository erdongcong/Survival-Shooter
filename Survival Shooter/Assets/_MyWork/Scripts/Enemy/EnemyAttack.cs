using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EnemyAttack : NetworkBehaviour
{
    public float timeBetweenAttacks = 0.5f;
    public int attackDamage = 10;

    NetworkAnimator anim;
    PlayerHealth playerHealth;
    EnemyHealth enemyHealth;
    bool playerInRange;
    float timer;


    void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        anim = GetComponent<NetworkAnimator>();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInRange = true;
            playerHealth = other.GetComponent<PlayerHealth>();
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInRange = false;
            playerHealth = null;
        }
    }

    [Server]
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeBetweenAttacks && playerInRange && enemyHealth.currentHealth > 0)
        {
            timer = 0f;
            Attack();
        }

        if (GameOverManager.gameBegin == true && PlayerMovement.players.Count == 0)
        {
            anim.SetTrigger("PlayerDead");
        }
    }


    void Attack()
    {

        if (playerHealth.currentHealth > 0)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }
}
