using UnityEngine;
using System.Collections;

public class WordBehavior : MonoBehaviour
{
    public Sprite[] sprites;
    private int spriteIndex = 0;
    private float countdown = 0f;
    private float lifetime = 2f;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        countdown -= Time.deltaTime;
        if (countdown <= 0f)
        {
            countdown = 0.2f;
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprites[spriteIndex];
            spriteIndex = (spriteIndex + 1) % sprites.Length;
        }
    }
}
