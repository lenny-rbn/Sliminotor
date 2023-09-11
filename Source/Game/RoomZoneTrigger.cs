using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomZoneTrigger : MonoBehaviour
{
    public bool isPlayerIn = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.name == "Player")
            isPlayerIn = true;
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.name == "Player")
            isPlayerIn = false;
    }

    
}
