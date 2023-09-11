using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerOrdering : MonoBehaviour
{
    public static List<LayerOrdering> entities = new List<LayerOrdering>();

    SpriteRenderer spriteRenderer;

    [SerializeField] GameObject clippingPoint;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!clippingPoint)
            clippingPoint = gameObject;
        entities.Add(this);
    }

    private void Update()
    {
        entities.Sort((p1,  p2) => p1.clippingPoint.transform.position.y < p2.clippingPoint.transform.position.y ? 1 : -1);


        int layer = 10;
        foreach (var entity in entities)
        {
            entity.spriteRenderer.sortingOrder = layer;
            layer++;
        }
    }

    private void OnDestroy()
    {
        entities.Remove(this);
    }

    private void OnDisable()
    {
        entities.Remove(this);
    }

    private void OnEnable()
    {
        entities.Add(this);
    }
}
