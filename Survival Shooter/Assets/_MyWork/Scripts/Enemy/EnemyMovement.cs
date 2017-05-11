using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EnemyMovement : NetworkBehaviour
{
    EnemyHealth enemyHealth;
    UnityEngine.AI.NavMeshAgent nav;
    Vector3 destination;
    Vector3 temp;
    float distance;
    float tempdistance;


    void Awake()
    {
        enemyHealth = GetComponent <EnemyHealth> ();
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    [Server]
    void Update()
    {
        if (enemyHealth.currentHealth > 0 && GameOverManager.gameBegin == true && PlayerMovement.players.Count != 0)
        {
            destination = PlayerMovement.players[0].transform.position;
            temp = PlayerMovement.players[0].transform.position - transform.position;
            distance = Vector3.Dot(temp, temp);
            for (int i = 1; i < PlayerMovement.players.Count; i++)
            {
                temp = PlayerMovement.players[i].transform.position - transform.position;
                tempdistance = Vector3.Dot(temp, temp);
                if (tempdistance < distance)
                    destination = PlayerMovement.players[i].transform.position;
            }
            nav.SetDestination(destination);
        }
        // Otherwise...
        else
        {
            // ... disable the nav mesh agent.
            nav.enabled = false;
        }
    }
}
