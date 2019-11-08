using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtackEnemy : MonoBehaviour
{
    private MoveEnemy moveEnemy;

    private void Start()
    {
        moveEnemy = GetComponentInParent<MoveEnemy>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        { 
            collision.gameObject.GetComponent<AnimationController>().TakeDamage();
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<AnimationController>().TakePoisun();
        }
    }
}
