using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviourPunCallbacks
{
    #region Fields

    //プレイヤーの状態管理列挙型
    public enum PlayerState
    {
        Normal,
        Run,
        Damage,
        Atack,
        Dead,
        Role
    }
    //状態管理変数
    private PlayerState playerState;
    private Vector3 moveForward;
    //左右入力
    private float inputHorizontal;
    private float inputVertical;
    private Rigidbody rigid;
    //歩くスピード
    [SerializeField]
    private float moveSpeed = 3f;
    private Animator animator;
    public bool atacking=false;
    public bool damaging = false;
    //攻撃を受けたときのダメージ値
    [SerializeField]
    private int damageValue = 5;
    private CameraWork cameraWork;
    private NetworkManager networkManager;
    //毒状態判定
    public bool poisn = false;
    private float poisntimer = 20f;
    private GameObject poisnGas;
    private ParticleSystem particle;
    private float timer = 0;
    //必殺技
    [SerializeField] private GameObject deathblowLight=default;
    public bool DeathBlowTrigger { get; set; } = false;
    [SerializeField] private GameObject deathBlowText=default;
    //回避中フラグ
    public bool RoleTrigger { get; set; } = false;
    private PhotonView view;
    private GameManager gameManager;
    private SoundManager soundManager;
    private bool deading = false;
    #endregion
    private void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        view = GetComponent<PhotonView>();
        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        cameraWork = this.gameObject.GetComponent<CameraWork>();
        poisnGas = transform.Find("PoisonGas").gameObject;
        particle = poisnGas.GetComponent<ParticleSystem>();
        if (cameraWork != null)
        {
            if (photonView.IsMine)
            {
                cameraWork.OnStartFollowing();
                deading = false;
            }
        }
        else
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
        }
        PlayerSetState(PlayerState.Normal);
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
                inputHorizontal = Input.GetAxis("Horizontal");
                inputVertical = Input.GetAxis("Vertical");
        }
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            //プレイヤーが走る状態に遷移しているときの処理
            if (playerState == PlayerState.Run)
            {
                //カメラの方向からx-z平面の単位ベクトルを取得
                Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
                //方向キーの入力値とカメラの向きから、移動方向を決定
                moveForward = cameraForward * inputVertical + Camera.main.transform.right * inputHorizontal;

                //移動方向にスピードをかける。
                rigid.velocity =moveForward * moveSpeed + new Vector3(0, rigid.velocity.y, 0);


                //キャラクターの向きを進行方向に向かせる
                if (moveForward != Vector3.zero)
                {
                    Quaternion rotation = Quaternion.LookRotation(moveForward);
                    transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, 0.1f);
                }
                //移動キーが押されていなかったらNormal状態に遷移
                if (inputHorizontal == 0 && inputVertical == 0)
                {
                    PlayerSetState(PlayerState.Normal);
                }
            }
            if (playerState == PlayerState.Atack)
                rigid.velocity = new Vector3(0, 0, 0);
            //攻撃状態でなくNormal状態の場合
            if (atacking == false&&playerState==PlayerState.Normal)
            {
               //移動キーが押されていたら
             if (inputHorizontal != 0 || inputVertical != 0)
                {
                    PlayerSetState(PlayerState.Run);
                }
            }
            if (playerState == PlayerState.Role)
            {
                rigid.velocity = moveForward * 10f;
            }
            if (playerState == PlayerState.Dead)
            {
                cameraWork.OnStartFollowing();
            }
            if (poisn)
            {
                timer+=Time.deltaTime;
                if(playerState!=PlayerState.Damage)
                    LocalVariables.currentHP-=0.5f*Time.deltaTime;
                if (timer >=poisntimer)
                {
                    particle.Stop();
                    timer = 0;
                    poisn = false;
                }
            }            
        }
    }

    #region Method

    //プレイヤーの状態を遷移するメソッド
    public void PlayerSetState(PlayerState playerstate)
    {
        switch (playerstate)
        {
            case PlayerState.Run:
                atacking = false;
                animator.SetBool("RunBool", true);
                playerState = playerstate;
                soundManager.PlaySE("WalkSe",true,0.05f);
                break;
            case PlayerState.Normal:
                animator.SetBool("RunBool", false);
                RoleTrigger = false;
                atacking = false;
                damaging = false;
                playerState = playerstate;
                soundManager.SeStop("WalkSe");
                break;
            case PlayerState.Atack:
                atacking = true;
                animator.SetBool("RunBool", false);
                playerState = playerstate;
                break;
            case PlayerState.Damage:
                atacking = false;
                damaging = true;
                LocalVariables.currentHP -= 5;
                playerState = playerstate;
                soundManager.PlaySE("HitSE", false, 0.4f);
                animator.SetBool("RunBool", false);
                animator.SetTrigger("DamageTrigger");
                break;
            case PlayerState.Dead:
                if (deading == false)
                {
                    playerState = playerstate;
                    soundManager.StopFullSE();
                    gameManager.LifeDelete(view); //ハートの削除、LifeCounter減算
                    animator.SetBool("DieBool", true);
                    StartCoroutine(DeadCorutine()); //ReStart呼び出しor観戦モード切替
                    deading = true;
                }
                break;
            case PlayerState.Role:
                RoleTrigger = true;
                playerState = playerstate;
                soundManager.SeStop("PlayerAtkSE2");
                soundManager.SeStop("WalkSe");
                animator.SetTrigger("RoleTrigger");
                break;
        }
    }

    //プレイヤーの状態を取得するメソッド
    public PlayerState GetPlayerState()
    {
        return playerState;
    }
    
    //攻撃のアニメーションが終わったら呼び出される
    public void AtackEnd()
    {
        atacking = false;
        PlayerSetState(PlayerState.Normal);
        soundManager.SeStop("PlayerAtkSE");
        soundManager.SeStop("PlayerAtkSE2");
    }

    //攻撃アニメーションが始まったら呼び出し
    public void AtackStart()
    {
        atacking = true;
        PlayerSetState(PlayerState.Atack);
        soundManager.PlaySE("PlayerAtkSE",false, 0.2f);
    }

    public void CombAtackStart()
    {
        atacking = true;
        PlayerSetState(PlayerState.Atack);
        soundManager.PlaySE("PlayerAtkSE2", false, 0.2f);
    }
    public void PlayerStateEnd()
    {
        PlayerSetState(PlayerState.Normal);
    }

    private IEnumerator DeadCorutine()
    {
        yield return new WaitForSeconds(4);
        if (gameManager.lifeCounter >= -1)
            networkManager.Restart();
        else
            gameManager.OneDeath = true;
        PhotonNetwork.Destroy(gameObject);

    }

    public void DeathBlowLight()
    {
        deathblowLight.SetActive(true);
        deathBlowText.SetActive(true);
        DeathBlowTrigger = true;
    }

    public void DeathBlowLightStop()
    {
        deathblowLight.SetActive(false);
    }

    public void DeathBlowStart()
    {
        DeathBlowTrigger = false;
        deathBlowText.SetActive(false);
    }


    #endregion
}
