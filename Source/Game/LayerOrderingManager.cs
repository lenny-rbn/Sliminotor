using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerOrderingManager : MonoBehaviour
{
    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var item in LayerOrdering.entities.ToArray())
        {
            if(Vector2.Distance(player.position, item.transform.position) > 16f)
                item.enabled = false;
            else
                item.enabled = true;
        }    
    }
}
