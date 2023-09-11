using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHitBox : MonoBehaviour
{
    public string opponentTag;

    public int damage;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == opponentTag) 
            collision.GetComponent<HealthBar>().TakeDamage(damage);
    }
}