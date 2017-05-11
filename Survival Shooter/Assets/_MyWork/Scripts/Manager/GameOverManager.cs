using UnityEngine;
using UnityEngine.Networking;

public class GameOverManager : NetworkBehaviour
{
    public static bool gameBegin;

    NetworkAnimator anim;


    void Awake()
    {
        anim = GetComponent<NetworkAnimator>();
    }

    [ServerCallback]
    void Update()
    {
        if (PlayerMovement.players.Count > 1)
            gameBegin = true;
        if (gameBegin == true && PlayerMovement.players.Count == 0)
            anim.SetTrigger("GameOver");
    }
}
