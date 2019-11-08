using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadScene : MonoBehaviour
{
    public void Button()
    {
        SceneManager.LoadScene("LoginScene");
    }
}
