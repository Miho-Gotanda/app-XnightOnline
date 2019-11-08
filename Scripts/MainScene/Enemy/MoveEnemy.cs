
using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.UI;
using System;

public class MoveEnemy : MonoBehaviourPun, IPunObservable
{
    #region Fields
    public Action enemyCollisionSet;
    //状態を管理する列挙型
    public enum EnemyState:byte
    {
        Walk=0,
        Wait,
        Chase,
        Atack,
        Freeze,
        Damage,
        Shouto,
        Dead
    };
    //敵の状態
    private EnemyState state;
    //追いかけるプレイヤーの位置情報
    private Transform playerTransform;
    private CharacterController enemyController;
    private Animator animator;
    //　目的地
    private Vector3 destination;
    //　歩くスピード
    [SerializeField]
    private float walkSpeed = 1.0f;
    //　速度
    private Vector3 velocity;
    //　移動方向
    private Vector3 direction;
    //　到着フラグ
    private bool arrived;
    //　SetPositionスクリプト
    private SetPosition setPosition;
    //　待ち時間
    [SerializeField]
    private float waitTime = 5f;
    //　経過時間
    private float elapsedTime;
    //攻撃した後のフリーズ時間
    [SerializeField]
    private float freezeTime = 10f;
    //回転速度
    [SerializeField]
    private float rotateSpeed = 5f;
    public bool damaging = false;
    private int randomDamage;
    private ParticleSystem particle;
    [SerializeField]
    private Transform destenationPos=default;
    private SoundManager soundManager;

    #endregion

    void Start()
    {
        soundManager = SoundManager.Instance;
        animator = GetComponent<Animator>();
        setPosition = GetComponent<SetPosition>();
        destination = setPosition.GetDestination();
        velocity = Vector3.zero;
        arrived = false;
        elapsedTime = 3f;
        SetState(EnemyState.Walk);
        particle = GetComponentInChildren<ParticleSystem>();
    }

    #region MonoBehaviour

    void Update()
    {
        //見回りまたは追いかける状態
        if (state == EnemyState.Walk || state == EnemyState.Chase)
        {
                //追いかける状態であれば目的地を設定
                if (state == EnemyState.Chase)
                {
                    if (playerTransform != null)
                        destination = playerTransform.position;

                }

                velocity = Vector3.zero;
                animator.SetBool("RunBool", true);
                direction = (destination - transform.position).normalized;
                //滑らかに移動方向に回転
                if (destination.y != transform.position.y)
                {
                    destination = new Vector3(destination.x, transform.position.y, destination.z);
                }

                Quaternion targetRotate = Quaternion.LookRotation(destination - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotate, rotateSpeed * Time.deltaTime);
                transform.position = transform.position + transform.forward * walkSpeed * Time.deltaTime;
        
            if (state == EnemyState.Walk)
            {
                //　目的地に到着したかどうかの判定
                if (Vector3.Distance(destenationPos.position, destination) < 3f)
                {
                    SetState(EnemyState.Wait);
                    animator.SetBool("RunBool", false);
                }
            }
            else if (state == EnemyState.Chase)
            {
                //攻撃する距離だったら攻撃
                if (Vector3.Distance(destenationPos.position, destination) <= 3f)
                {
                    if (randomDamage == 3)
                    {
                        photonView.RPC("ChangeState", RpcTarget.All, (byte)EnemyState.Shouto);
                    }
                    else
                    {
                        photonView.RPC("ChangeState", RpcTarget.All, (byte)EnemyState.Atack);
                    }

                }
                else if (Vector3.Distance(destenationPos.position, destination) >= 20)
                {
                    SetState(MoveEnemy.EnemyState.Wait);
                }
            }

        }
        //　到着していたら
        else if (state == EnemyState.Wait)
        {
            elapsedTime += Time.deltaTime;

            //　待ち時間を越えたら次の目的地を設定
            if (elapsedTime > waitTime)
            {
                SetState(EnemyState.Walk);
            }
        }
        //攻撃後のフリーズ
        else if (state == EnemyState.Freeze)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= freezeTime)
            {
                SetState(EnemyState.Walk);
            }
        }
    }

    #endregion

    #region Methods

    public void SetState(EnemyState tempState, Transform targetobj = null)
    {
        
        if (tempState == EnemyState.Walk)
        {
            arrived = false;
            animator.SetBool("RunBool", true);
            elapsedTime = 0f;
            state = tempState;
            destination = setPosition.GetDestination();
        }
        else if (tempState ==EnemyState.Chase)
        {
            photonView.RPC("RandDamageChange", RpcTarget.All);
            state = tempState;
            arrived = false;
            //追いかける対象セット
            playerTransform = targetobj;
        }
        else if (tempState == EnemyState.Wait)
        {
            elapsedTime = 0f;
            state = tempState;
            arrived = true;
            velocity = Vector3.zero;
            animator.SetBool("RunBool", false);
        }
        else if (tempState == EnemyState.Atack)
        {
            randomDamage = 0;
            state = tempState;
            velocity = Vector3.zero;
            animator.SetBool("RunBool", false);
            animator.SetBool("AtkBool", true);
        }
        else if (tempState == EnemyState.Freeze)
        {
            damaging = false;
            elapsedTime = 0f;
            state = tempState;
            velocity = Vector3.zero;
            animator.SetBool("RunBool", false);
            animator.SetBool("AtkBool", false);
        }
        else if (tempState == EnemyState.Shouto)
        {
            state = tempState;
            animator.SetBool("RunBool", false);
            animator.SetBool("AtkBool", false);
            animator.SetTrigger("ShoutTrigger");
            StartCoroutine(MonsterSE());
            particle.Play();
            randomDamage = 0;
        }
        else if (tempState == EnemyState.Dead)
        {
            state = tempState;
            enemyCollisionSet();
            soundManager.PlayBGM("CreaBGM");
            animator.SetBool("RunBool", false);
            animator.SetBool("AtkBool", false);
            animator.SetTrigger("DeadTrigger");
            soundManager.PlaySE("MonsterDieSE", false, 0.5f);
            StartCoroutine(EnemyDead());
        }
    }
    //敵キャラクターの状態取得メソッド
    public EnemyState GetState()
    {
        return state;
    }

    [PunRPC]
    private void RandDamageChange()
    {
        randomDamage = UnityEngine.Random.Range(1, 4);
    }

    [PunRPC]
    public void ChangeState(byte state)
    {
        EnemyState enemyState = (EnemyState)state;
        SetState(enemyState);
    }

    public void EnemyTakeDamage()
    {
        if (damaging == false)
            SetState(EnemyState.Damage);
    }
    public void EnemyDamageEnd()
    {
        SetState(EnemyState.Atack);
    }
    public void ShoutEnd()
    {
        SetState(EnemyState.Walk);
    }

    //ダメージを受けたときの処理
  [PunRPC]
    public void EnemyDamage(int damage)
    {
            LocalVariables.enemyHealth -= damage;
            if (LocalVariables.enemyHealth <= 0 && GetState() != MoveEnemy.EnemyState.Dead)
            {
                SetState(MoveEnemy.EnemyState.Dead);
            }   
    }

    public IEnumerator EnemyDead()
    {
        yield return new WaitForSeconds(7);
        photonView.RPC("ClearSceneChange", RpcTarget.All);
    }

    private IEnumerator MonsterSE()
    {
        yield return new WaitForSeconds(1);
        soundManager.PlaySE("MonsterSE2", false, 0.9f);

    }

    //Enemy死亡時ClearScene呼び出し
    [PunRPC]
    private void ClearSceneChange()
    {
        soundManager.StopFullSE();
        PhotonNetwork.LoadLevel("ClearScene");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(destination);
            stream.SendNext(state);
            stream.SendNext(randomDamage);
        }
        else
        {
            destination = (Vector3)stream.ReceiveNext();
            state = (EnemyState)stream.ReceiveNext();
            randomDamage = (int)stream.ReceiveNext();
        }
    }

   

    #endregion


}


    