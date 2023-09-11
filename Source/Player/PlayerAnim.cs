using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    Animator animator;
    Player player;


    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
    }


    void Update()
    {
        
    }
}
