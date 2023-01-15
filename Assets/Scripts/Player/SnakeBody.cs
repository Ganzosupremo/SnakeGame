using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class SnakeBody : MonoBehaviour
{
    public Sprite[] bodySprites;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (bodySprites.Length != 0)
        {
            // Set a random sprite for diferent parts of the snake body
            int randomSprite = Random.Range(0, bodySprites.Length);
            spriteRenderer.sprite = bodySprites[randomSprite];
        }
    }

}
