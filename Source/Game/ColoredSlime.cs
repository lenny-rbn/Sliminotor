using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredSlime : MonoBehaviour
{
    void Start()
    {
        int value = Random.Range(1, 5);

        switch (value)
        {
            case 1:
                gameObject.tag = "Mine";
                gameObject.GetComponent<SpriteRenderer>().color = new Vector4(0.2f, 1, 0.2f, 1);
                break;
            case 2:
                gameObject.tag = "Sword";
                gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1, 0.35f, 0, 1);
                break;
            case 3:
                gameObject.tag = "Tornado";
                gameObject.GetComponent<SpriteRenderer>().color = new Vector4(0.2f, 1, 1, 1);
                break;
            case 4:
                gameObject.tag = "Fireball";
                gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                break;
        }
    }

    void Update()
    {
        
    }
}
