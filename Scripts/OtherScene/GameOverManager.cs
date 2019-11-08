using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameOverManager : MonoBehaviour
{
    [SerializeField]
    private Button continue_Button;
    [SerializeField]
    private Button quit_Button;
    private EndSceneController endSceneController;

    private void Awake()
    {
        endSceneController = new EndSceneController(continue_Button, quit_Button);
    }
    // Start is called before the first frame update
    void Start()
    {
        endSceneController.ButtonSet();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
