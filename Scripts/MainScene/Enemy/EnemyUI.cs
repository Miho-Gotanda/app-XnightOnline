using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    //EnemySlider
    public  Slider enemySlider;
    private int Health;
    
    void Awake()
    {
        enemySlider =GetComponent<Slider>();
    }

    private void Start()
    {

        if (enemySlider != null)
        {
            Health = LocalVariables.enemyHealth;
            enemySlider.value = this.Health;
        }
    }

    void Update()
    {
            Health = LocalVariables.enemyHealth;
            enemySlider.value = this.Health;
        
    }

}
