using UnityEngine;
using UnityEngine.Networking;

public class EnemyManager : NetworkBehaviour
{
    public GameObject enemyPrefab;
    public float spawnTime = 3f;
    public Transform[] spawnPoints;

    GameObject enemy;
    int index = 0;

    [ServerCallback]
    void Start()
    {
        InvokeRepeating("Spawn", spawnTime, spawnTime);
    }


    void Spawn()
    {
        /*if (GameOverManager.gameBegin == true && PlayerMovement.players.Count == 0)
        {
            return;
        }*/

        if (GameOverManager.gameBegin == false || index > 2)
            return;

        int spawnPointIndex = Random.Range(0, spawnPoints.Length);

        enemy = (GameObject)Instantiate(enemyPrefab, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
        NetworkServer.Spawn(enemy);
        index++;
    }
}
