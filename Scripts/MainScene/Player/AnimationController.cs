using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using UnityEngine.UI;

public class AnimationController : MonoBehaviourPunCallbacks
{
    private PlayerMove playerMove;
    private Animator animator;
    private float inputHorizontal;
    private float inputVertical;
    //毒状態
    [SerializeField] private GameObject poisnObj;
    private ParticleSystem particle;
    private bool combonation = false;
    //必殺技状態
    [SerializeField] private GameObject deathBlowObj;
    private ParticleSystem deathBlowParticle;
    public bool DeathBlowGage { get; set; } = false;
    [SerializeField]private GameObject deathBlowCollider;
    public bool HitTrigger { get; set; } = false;
    private SoundManager soundManager;


    void Start()
    {
        soundManager = SoundManager.Instance;
        playerMove = GetComponent<PlayerMove>();
        animator = GetComponent<Animator>();
        particle = poisnObj.GetComponent<ParticleSystem>();
        deathBlowParticle = deathBlowObj.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
              //攻撃01のアニメーション
            if (Input.GetKeyDown(KeyCode.T))
            {
                playerMove.PlayerSetState(PlayerMove.PlayerState.Atack);
                animator.SetBool("AtkBool", true);
            }
            //必殺技アニメーション（Enter）
            if (Input.GetKeyDown(KeyCode.Return) && playerMove.DeathBlowTrigger)
            {
                photonView.RPC("DeathBlowPlay", RpcTarget.All);
                soundManager.PlaySE("DeathBlowSE", false, 0.1f);
                soundManager.PlaySE("DeathBlowLoopSE", true, 0.4f);
            }

            //必殺技が始まったらゲージを減少
            if (DeathBlowGage && LocalVariables.deathblow >= 0)
            {
                LocalVariables.deathblow -= 5.5f * Time.deltaTime;
            }

            else if (DeathBlowGage && LocalVariables.deathblow <= 0)
            {
                photonView.RPC("DeathBlowStop", RpcTarget.All);
                soundManager.SeStop("DeathBlowSE");
                soundManager.SeStop("DeathBlowLoopSE");
            }

            if (!HitTrigger)
            {
                //回避アニメーション
                if (Input.GetKeyDown(KeyCode.Space) && !playerMove.RoleTrigger)
                    playerMove.PlayerSetState(PlayerMove.PlayerState.Role);
            }
        }
     }

    public void TakeDamage()
    {
        if (playerMove.damaging == false)
        {
            if (photonView.IsMine)
            {
                if (playerMove.GetPlayerState() != PlayerMove.PlayerState.Role&&playerMove.GetPlayerState()!=PlayerMove.PlayerState.Dead)
                {
                    HitTrigger = true;
                    playerMove.PlayerSetState(PlayerMove.PlayerState.Damage);
                }
            }
        } 
    }

    public void TakePoisun()
    {
        if (playerMove.poisn == false)
        {
            if (photonView.IsMine)
            {
                if (playerMove.GetPlayerState() != PlayerMove.PlayerState.Role)
                {
                    particle.Play();
                    playerMove.poisn = true;
                }
            }
        }
    }

    //必殺技開始
    [PunRPC]
    private void DeathBlowPlay()
    {
        deathBlowParticle.Play();
        deathBlowCollider.SetActive(true);
        playerMove.DeathBlowStart();
        DeathBlowGage = true;
    }

    //必殺技終了
    [PunRPC]
    private void DeathBlowStop()
    {
        deathBlowParticle.Stop();
        deathBlowCollider.SetActive(false);
        playerMove.DeathBlowLightStop();
        DeathBlowGage = false;
    }

    public void HitEnd()
    {
        HitTrigger = false;
        playerMove.PlayerSetState(PlayerMove.PlayerState.Normal);
    }
   

}
