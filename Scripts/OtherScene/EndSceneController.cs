using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class EndSceneController
{
    private Button continue_Button = null;
    private Button quit_Button = null;

    public EndSceneController(Button continue_Button,Button quit_Button)
    {
        this.continue_Button = continue_Button;
        this.quit_Button = quit_Button;
    }

    public void ButtonSet()
    {
        if (continue_Button != null)
            continue_Button.onClick.AddListener(() => SceneManager.LoadScene("LoginScene"));

        //実行環境判定
        Action buttonEvent = () =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
           UnityEngine.Application.Quit();
#endif
        };

        if (quit_Button != null)
            quit_Button.onClick.AddListener(() => buttonEvent());

    }
}
