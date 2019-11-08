using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeathBlowAtack : MonoBehaviourPun
{
    private PlayerMove playerMove;
    private PlayerMove.PlayerState state;

    private void Start()
    {
        playerMove = GetComponentInParent<PlayerMove>();
    }
    private void Update()
    {
        state = playerMove.GetPlayerState();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            MoveEnemy moveEnemy = other.GetComponentInParent<MoveEnemy>();
            if (state == PlayerMove.PlayerState.Atack)
            {
                moveEnemy.photonView.RPC("EnemyDamage", RpcTarget.All,5);
            }

        }
    }
}
