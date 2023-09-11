using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    GameObject target;
    Transform player;
    Rigidbody2D rb;

    [SerializeField] float smooth = 1;
    [SerializeField] float maxSmooth = 3;
    [SerializeField] float minSmooth = 1;

    Vector2 velocity = new Vector2();

    private void Start()
    {
        target = GameObject.Find("Player");
        rb = target.GetComponent<Rigidbody2D>();
        player = target.transform;
    }

    private void Update()
    {
        transform.position = Vector2.SmoothDamp(transform.position,
            new Vector2(player.position.x + rb.velocity.x, player.position.y + rb.velocity.y),
            ref velocity,
            smooth / Mathf.Clamp(rb.velocity.magnitude, minSmooth, maxSmooth));
    }
}