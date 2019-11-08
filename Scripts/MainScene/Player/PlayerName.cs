using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerName : MonoBehaviour
{

    public InputField nameText;
    // Start is called before the first frame update
    void Start()
    {
        nameText = nameText.GetComponent<InputField>();
    }

    private void Update()
    {
        if (nameText.text == "")
        {
            PhotonNetwork.NickName = "DefaultPlayer";
        }
    }

    public void InputText()
    {
        PhotonNetwork.NickName = nameText.text;
    }
}
