using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animation : MonoBehaviour
{

 private Animator anim;
    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>(); // Get Animator component
    }

    public void TriggerDeath()
    {
        if (!isDead)
        {
            
            isDead = true;
            anim.SetTrigger("Dead"); // Play death animation

        }
    }



}
