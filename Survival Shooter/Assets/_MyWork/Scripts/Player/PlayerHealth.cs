using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;


public class PlayerHealth : NetworkBehaviour
{
    public int startingHealth = 100;
    [SyncVar(hook ="OnHealthChange")]
    public int currentHealth;
    public AudioClip deathClip;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);

    Slider healthSlider;
    Image damageImage;
    NetworkAnimator anim;
    AudioSource playerAudio;
    PlayerMovement playerMovement;
    PlayerShooting playerShooting;
    bool isDead;
    bool damaged;


    void Awake()
    {
        anim = GetComponent<NetworkAnimator>();
        playerAudio = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponentInChildren <PlayerShooting> ();
        healthSlider = GameObject.Find("Canvas/HealthUI/HealthSlider").GetComponent<Slider>();
        damageImage = GameObject.Find("Canvas/DamageImage").GetComponent<Image>();
        currentHealth = startingHealth;
    }


    void Update()
    {
        if (damaged)
        {
            damageImage.color = flashColour;
        }
        else
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;
    }

    [Server]
    public void TakeDamage(int amount)
    {
        RpcIsDamage();
        isDead = false;
        currentHealth -= amount;
        if (currentHealth <= 0 && !isDead)
        {
            playerMovement.enabled = false;
            playerShooting.enabled = false;
            RpcDeath();
        }
    }

    [ClientRpc]
    void RpcIsDamage()
    {
        if (isLocalPlayer)
        {
            damaged = true;
            playerAudio.Play();
        }
    }
    [ClientRpc]
    void RpcDeath()
    {
        isDead = true;
        playerShooting.CmdDisableEffects();
        anim.SetTrigger("Die");
        if (isLocalPlayer)
        {
            playerAudio.clip = deathClip;
            playerAudio.Play();
            playerMovement.enabled = false;
            playerShooting.enabled = false;
        }    
    }

    void OnHealthChange(int currentHealth)
    {
        if (isLocalPlayer)
            healthSlider.value = currentHealth;
    }

    public void RestartLevel()
    {
        return;
    }
}
