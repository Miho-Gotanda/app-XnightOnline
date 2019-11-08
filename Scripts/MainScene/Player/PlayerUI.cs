using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerUI : MonoBehaviourPunCallbacks, IPunObservable
{

    //プレイヤー名前設定用Text
    public Text PlayerNameText;

    //プレイヤーのHP用Slider
    public  Slider PlayerHPSlider;

    //プレイヤーの必殺技ゲージ
    [SerializeField] private Slider PlayerDeathblowSlider=default;

    private PlayerMove _target;

    //プレイヤーのチャット用Text
    //public Text PlayerChatText;

    private float PlayerHP;
    private float Deathblow;


    void Awake()
    {
        PlayerHPSlider = GetComponent<Slider>();
        _target=GetComponentInParent<PlayerMove>();
    }
    private void Start()
    {
        if (PlayerNameText != null)
        {
            PlayerNameText.text = _target.photonView.Owner.NickName;
        }
        if (PlayerHPSlider != null)
        {
            PlayerHPSlider.value =LocalVariables.currentHP;
        }
        if (PlayerDeathblowSlider != null)
            PlayerDeathblowSlider.value = LocalVariables.deathblow;
    }
    public void Update()
    {
        if (photonView.IsMine)
        {
            if(LocalVariables.currentHP<=100)
                LocalVariables.currentHP += 0.01f*Time.deltaTime;
            PlayerHP = LocalVariables.currentHP;
            PlayerHPSlider.value = this.PlayerHP;

            Deathblow = LocalVariables.deathblow;
            PlayerDeathblowSlider.value = this.Deathblow;
            if (this.PlayerHP <= 0)
            {
                if(_target.GetPlayerState()!=PlayerMove.PlayerState.Dead)
                _target.PlayerSetState(PlayerMove.PlayerState.Dead);
            }
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(PlayerHP);
            stream.SendNext(Deathblow);
        }
        else
        {
            PlayerHP = (float)stream.ReceiveNext();
            Deathblow = (float)stream.ReceiveNext();
            PlayerHPSlider.value = PlayerHP;
            PlayerDeathblowSlider.value = Deathblow;
        }
    }
}
