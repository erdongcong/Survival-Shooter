using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerShooting : NetworkBehaviour
{
    public int damagePerShot = 20;
    public float timeBetweenBullets = 0.15f;
    public float range = 100f;
    public Transform shootPosition;


    float timer;
    Ray shootRay = new Ray();
    RaycastHit shootHit;
    int shootableMask;
    ParticleSystem gunParticles;
    LineRenderer gunLine;
    AudioSource gunAudio;
    Light gunLight;
    float effectsDisplayTime = 0.2f;


    void Awake()
    {
        shootableMask = LayerMask.GetMask("Shootable");
        gunParticles = GetComponentInChildren<ParticleSystem>();
        gunLine = GetComponentInChildren<LineRenderer>();
        gunAudio = shootPosition.GetComponentInChildren<AudioSource>();
        gunLight = GetComponentInChildren<Light>();
    }


    void Update()
    {
        if (!isLocalPlayer)
            return;

        timer += Time.deltaTime;

#if !MOBILE_INPUT
        if (Input.GetButton("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0)
        {
            timer = 0f;
            CmdShoot();
        }
#else
        if ((CrossPlatformInputManager.GetAxisRaw("Mouse X") != 0 || CrossPlatformInputManager.GetAxisRaw("Mouse Y") != 0) && timer >= timeBetweenBullets)
        {
            timer = 0f;
            CmdShoot();
        }
#endif

        if (timer >= timeBetweenBullets * effectsDisplayTime)
        {
            CmdDisableEffects();
        }
    }

    [Command]
    public void CmdDisableEffects()
    {
        RpcDisableEffects();
    }
    [ClientRpc]
    void RpcDisableEffects()
    {
        gunLine.enabled = false;
        gunLight.enabled = false;
    }

    [Command]
    void CmdShoot()
    {
        shootRay.origin = shootPosition.position;
        shootRay.direction = transform.forward;
        Vector3 hitPoint;

        if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
        {
            EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                int enemyScore = enemyHealth.TakeDamage(damagePerShot, shootHit.point);
                if (enemyScore > 0)
                    RpcAddScore(enemyScore);
            }
            gunLine.SetPosition(1, shootHit.point);
            hitPoint = shootHit.point;
        }
        else
        {
            hitPoint = shootRay.origin + shootRay.direction * range;
        }
        RpcShowEffects(shootPosition.position, hitPoint);
    }

    [ClientRpc]
    void RpcShowEffects(Vector3 origin, Vector3 hitpoint)
    {
        gunAudio.Play();
        gunLight.enabled = true;
        gunParticles.Stop();
        gunParticles.Play();
        gunLine.enabled = true;
        gunLine.SetPosition(0, origin);
        gunLine.SetPosition(1, hitpoint);
    }

    [ClientRpc]
    void RpcAddScore(int enemyScore)
    {
        if (isLocalPlayer)
            ScoreManager.score += enemyScore;
    }
}
