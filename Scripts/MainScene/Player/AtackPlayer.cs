using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AtackPlayer : MonoBehaviourPun
{
    private PlayerMove playerMove;
    private PlayerMove.PlayerState state;
    private PhotonView _photonView;
    private AnimationController animationController;

    private void Start()
    {
        playerMove = GetComponentInParent<PlayerMove>();
        _photonView = GetComponentInParent<PhotonView>();
        animationController = GetComponentInParent<AnimationController>();
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
                moveEnemy.photonView.RPC("EnemyDamage",RpcTarget.All,5);
                if (_photonView.IsMine)
                {
                    if (LocalVariables.deathblow <= 100&&animationController.DeathBlowGage==false)
                        LocalVariables.deathblow += 5;
                    else if (LocalVariables.deathblow >= 100)
                        playerMove.DeathBlowLight();
                }
            }

        }
    }
}
