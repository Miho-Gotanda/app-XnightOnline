using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyAnimationEvent : MonoBehaviourPun
{
    private MoveEnemy moveEnemy;
    [SerializeField]
    private GameObject atackCollisionL=default;
    [SerializeField]
    private GameObject atackCollisionR=default;

    // Start is called before the first frame update
    void Start()
    {
        moveEnemy = GetComponent<MoveEnemy>();
        moveEnemy.enemyCollisionSet=() =>
        {
            if (atackCollisionL.activeSelf)
                atackCollisionL.SetActive(false);
            if (atackCollisionR.activeSelf)
                atackCollisionR.SetActive(false);
        };
    }

    public void StateEnd()
    {
        photonView.RPC("ChangeState", RpcTarget.All, (byte)MoveEnemy.EnemyState.Freeze);
    }
    public void AtackCollision()
    {
        atackCollisionR.SetActive(true);
        atackCollisionL.SetActive(true);
    }

    public void CollisionOff()
    {
        atackCollisionR.SetActive(false);
        atackCollisionL.SetActive(false);
    }
}
