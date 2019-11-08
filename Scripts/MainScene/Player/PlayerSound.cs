using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound: MonoBehaviour
{
    [SerializeField]
    private AudioSource bgmAudioSource;
    [SerializeField]
    private AudioSource seAudioSource;
    [SerializeField]
    private AudioClip walkBGM;
    [SerializeField]
    private AudioClip chaseBGM;
    private PlayerMove playerMove;
    
    // Start is called before the first frame update
    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
    

    }
}
