using UnityEngine;
using System.Collections;

public class SpriteBehavior : MonoBehaviour {
    public Vector3 Advance;
    public float Lifetime = 0f;
    public bool FromPlayer = false;
    
	// Use this for initialization
	void Start () {
        transform.Rotate(Vector3.forward, Random.value * 360);
	}
	
	// Update is called once per frame
	void Update () {
        Lifetime -= Time.deltaTime;
	    if (Lifetime <= 0)
        {
            DestroyObject(gameObject);
            return;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color color = spriteRenderer.color;
        color.a = Mathf.Min(Lifetime * 5f, 1f);
        spriteRenderer.color = color;

        transform.Translate(Advance * Time.deltaTime);
    }
}
