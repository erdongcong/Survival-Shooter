using UnityEngine;
using UnityEngine.Networking;

public class EnemyHealth : NetworkBehaviour
{
    public int startingHealth = 100;
    [SyncVar]
    public int currentHealth;
    public float sinkSpeed = 2.5f;
    public int scoreValue = 10;
    public AudioClip deathClip;


    NetworkAnimator anim;
    AudioSource enemyAudio;
    ParticleSystem hitParticles;
    CapsuleCollider capsuleCollider;
    bool isDead;
    bool isSinking;


    void Awake()
    {
        anim = GetComponent<NetworkAnimator>();
        enemyAudio = GetComponent<AudioSource>();
        hitParticles = GetComponentInChildren<ParticleSystem>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        currentHealth = startingHealth;
    }


    void Update()
    {
        if (isSinking)
        {
            transform.Translate(-Vector3.up * sinkSpeed * Time.deltaTime);
        }
    }

    [Server]
    public int TakeDamage(int amount, Vector3 hitPoint)
    {
        RpcPlayEnemyEffects(hitPoint);
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            RpcDeath();
            return scoreValue;
        }
        return 0;
    }

    [ClientRpc]
    void RpcPlayEnemyEffects(Vector3 hitPoint)
    {
        enemyAudio.Play();
        hitParticles.transform.position = hitPoint;
        hitParticles.Play();
    }
    [ClientRpc]
    void RpcDeath()
    {
        isDead = true;
        anim.SetTrigger("Dead");
        capsuleCollider.isTrigger = true;

        enemyAudio.clip = deathClip;
        enemyAudio.Play();
    }


    public void StartSinking()
    {
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        isSinking = true;
        //ScoreManager.score += scoreValue;
        CmdDestroy(gameObject, 2f);
    }

    [Command]
    void CmdDestroy(GameObject gameObject, float delay)
    {
        Destroy(gameObject, delay);
    }
}
