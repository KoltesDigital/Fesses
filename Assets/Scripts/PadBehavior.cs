using UnityEngine;
using System.Collections;

public class PadBehavior : MonoBehaviour {

    public string ButtonName;

    public Transform bodyTransform;
    public Transform emitterTransform;

    public Transform SpriteContainer;
    public GameObject SpriteTemplate;

    public AudioClip[] audioClips;
    private int audioClipIndex = 0;

    private float angle;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown(ButtonName))
        {
            AudioSource source = GetComponent<AudioSource>();
            source.PlayOneShot(audioClips[TunnelBehavior.instance.themeIndex * 2 + audioClipIndex]);
            audioClipIndex = (audioClipIndex + 1) % 2;

            GetComponentInChildren<Animator>().SetTrigger("Slap");
            
            GameObject spriteObject = Instantiate(SpriteTemplate, emitterTransform.position, Quaternion.identity) as GameObject;
            spriteObject.transform.SetParent(SpriteContainer, false);
            spriteObject.GetComponent<SpriteRenderer>().sprite = TunnelBehavior.instance.Sprites[Random.Range(0, TunnelBehavior.instance.Sprites.Length)];
            SpriteBehavior spriteBehavior = spriteObject.GetComponent<SpriteBehavior>();
            Vector2 dispersion = Random.insideUnitCircle * 5f;
            spriteBehavior.Advance = new Vector3(dispersion.x, dispersion.y, 4f);
            spriteBehavior.Lifetime = 3f;
            spriteBehavior.FromPlayer = true;

            
        }

        angle *= Mathf.Exp(-5f * Time.deltaTime);

        Vector3 eulerAngles = bodyTransform.localEulerAngles;
        eulerAngles.x = angle;
        bodyTransform.localEulerAngles = eulerAngles;
	}
}
