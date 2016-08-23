using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TunnelBehavior : MonoBehaviour {
    public static TunnelBehavior instance;

    public float MinUOffsetPerSecond = 0f;
    public float MaxUOffsetPerSecond = 0f;
    public float MinVOffsetPerSecond = 0f;
    public float MaxVOffsetPerSecond = 0f;
    public float MinDisplacementAnglePerSecond = 0f;
    public float MaxDisplacementAnglePerSecond = 0f;
    public float MinTime = 0f;
    public float MaxTime = 0f;
    public float Constant = 0f;
    public float DisplacementRadius = 0f;

    public Material SpriteMaterial;
    public Material TunnelMaterial;

    public float MinSpriteCountdown = 0f;
    public float MaxSpriteCountdown = 0f;
    public Vector3 SpriteAdvance;
    public float MaxSpriteRadius = 0f;
    public Transform SpriteContainer;
    public GameObject SpriteTemplate;
    public Sprite[] Sprites;
    private float spriteCountdown = 0f;

    public GameObject[] WordTemplates;
    private int wordIndex = 0;

    public Texture[] BgTextures;
    private int bgIndex = 0;
    private float bgCountdown = 0f;

    public AudioSource themeSource;
    public AudioClip[] themeClips;
    public int themeIndex = 0;

    public AudioClip wordClip;

    private ChangingFloat uOffsetPerSecond = new ChangingFloat();
    private ChangingFloat vOffsetPerSecond = new ChangingFloat();
    private ChangingFloat displacementAnglePerSecond = new ChangingFloat();

    private Vector4 uvOffset = new Vector4();
    private float displacementAngle = 0f;

    void updateChangingFloatConstants()
    {
        uOffsetPerSecond.MinValue = MinUOffsetPerSecond;
        uOffsetPerSecond.MaxValue = MaxUOffsetPerSecond;
        uOffsetPerSecond.MinTime = MinTime;
        uOffsetPerSecond.MaxTime = MaxTime;
        uOffsetPerSecond.Constant = Constant;

        vOffsetPerSecond.MinValue = MinVOffsetPerSecond;
        vOffsetPerSecond.MaxValue = MaxVOffsetPerSecond;
        vOffsetPerSecond.MinTime = MinTime;
        vOffsetPerSecond.MaxTime = MaxTime;
        vOffsetPerSecond.Constant = Constant;

        displacementAnglePerSecond.MinValue = MinDisplacementAnglePerSecond;
        displacementAnglePerSecond.MaxValue = MaxDisplacementAnglePerSecond;
        displacementAnglePerSecond.MinTime = MinTime;
        displacementAnglePerSecond.MaxTime = MaxTime;
        displacementAnglePerSecond.Constant = Constant;
    }

    // Use this for initialization
    void Start ()
    {
        instance = this;

        updateChangingFloatConstants();
    }
	
    float Repeat(float value, float period)
    {
        if (value >= period)
            value -= period;
        if (value < period)
            value += period;
        return value;
    }

	// Update is called once per frame
	void Update ()
    {
        updateChangingFloatConstants();
        uOffsetPerSecond.Update();
        vOffsetPerSecond.Update();
        displacementAnglePerSecond.Update();

        uvOffset.x = Repeat(uvOffset.x + uOffsetPerSecond.Value * Time.deltaTime, 1f);
        uvOffset.y = Repeat(uvOffset.y + vOffsetPerSecond.Value * Time.deltaTime, 1f);
        TunnelMaterial.SetVector("_UVOffset", uvOffset);
        
        displacementAngle = Repeat(displacementAngle + displacementAnglePerSecond.Value * Time.deltaTime, Mathf.PI * 2);
        Vector4 displacement = new Vector4();
        displacement.x = Mathf.Cos(displacementAngle) * DisplacementRadius;
        displacement.y = Mathf.Sin(displacementAngle) * DisplacementRadius;
        SpriteMaterial.SetVector("_Displacement", displacement);
        TunnelMaterial.SetVector("_Displacement", displacement);

        spriteCountdown -= Time.deltaTime;
        if (spriteCountdown <= 0)
        {
            spriteCountdown = Random.Range(MinSpriteCountdown, MaxSpriteCountdown);
            Vector2 position = Random.insideUnitCircle * MaxSpriteRadius;
            GameObject spriteObject = Instantiate(SpriteTemplate, new Vector3(position.x, position.y, 50f), Quaternion.identity) as GameObject;
            spriteObject.transform.SetParent(SpriteContainer, false);
            spriteObject.GetComponent<SpriteRenderer>().sprite = Sprites[Random.Range(0, Sprites.Length)];
            SpriteBehavior spriteBehavior = spriteObject.GetComponent<SpriteBehavior>();
            spriteBehavior.Advance = SpriteAdvance;
            spriteBehavior.Lifetime = 4.5f;
        }

        foreach (Transform sprite in SpriteContainer.transform)
        {
            if (sprite.GetComponent<SpriteBehavior>().FromPlayer)
                continue;

            foreach (Transform spritePlayer in SpriteContainer.transform)
            {
                if (!spritePlayer.GetComponent<SpriteBehavior>().FromPlayer)
                    continue;

                Vector3 offset = sprite.localPosition - spritePlayer.localPosition;
                float distance = offset.magnitude;
                if (distance < 2f)
                {
                    Instantiate(WordTemplates[wordIndex], spritePlayer.localPosition + offset / 2f, Quaternion.identity);
                    wordIndex = (wordIndex + 1) % WordTemplates.Length;

                    Destroy(sprite.gameObject);
                    Destroy(spritePlayer.gameObject);

                    GetComponent<AudioSource>().PlayOneShot(wordClip);
                }
            }
        }

        bgCountdown -= Time.deltaTime;
        if (bgCountdown <= 0)
        {
            bgCountdown = 120f;
            GetComponent<MeshRenderer>().sharedMaterial.mainTexture = BgTextures[bgIndex];
            bgIndex = (bgIndex + 1) % BgTextures.Length;

            themeIndex = (themeIndex + 1) % 2;
            themeSource.clip = themeClips[themeIndex];
            themeSource.Play();

            float hue = Random.Range(0f, 1f);
            TunnelMaterial.SetColor("_ColorR", HSVToRGB(
                hue,
                Random.Range(0.8f, 1f),
                Random.Range(0.8f, 1f)
            ));
            hue = Mathf.Repeat(hue + Random.Range(0.2f, 0.4f), 1f);
            TunnelMaterial.SetColor("_ColorG", HSVToRGB(
                hue,
                Random.Range(0.8f, 1f),
                Random.Range(0.8f, 1f)
            ));
            hue = Mathf.Repeat(hue + Random.Range(0.2f, 0.4f), 1f);
            TunnelMaterial.SetColor("_ColorB", HSVToRGB(
                hue,
                Random.Range(0.8f, 1f),
                Random.Range(0.8f, 1f)
            ));
        }
    }

    public static Color HSVToRGB(float H, float S, float V)
    {
        if (S == 0f)
            return new Color(V, V, V);
        else if (V == 0f)
            return new Color();
        else
        {
            Color col = Color.black;
            float Hval = H * 6f;
            int sel = Mathf.FloorToInt(Hval);
            float mod = Hval - sel;
            float v1 = V * (1f - S);
            float v2 = V * (1f - S * mod);
            float v3 = V * (1f - S * (1f - mod));
            switch (sel + 1)
            {
                case 0:
                    col.r = V;
                    col.g = v1;
                    col.b = v2;
                    break;
                case 1:
                    col.r = V;
                    col.g = v3;
                    col.b = v1;
                    break;
                case 2:
                    col.r = v2;
                    col.g = V;
                    col.b = v1;
                    break;
                case 3:
                    col.r = v1;
                    col.g = V;
                    col.b = v3;
                    break;
                case 4:
                    col.r = v1;
                    col.g = v2;
                    col.b = V;
                    break;
                case 5:
                    col.r = v3;
                    col.g = v1;
                    col.b = V;
                    break;
                case 6:
                    col.r = V;
                    col.g = v1;
                    col.b = v2;
                    break;
                case 7:
                    col.r = V;
                    col.g = v3;
                    col.b = v1;
                    break;
            }
            col.r = Mathf.Clamp(col.r, 0f, 1f);
            col.g = Mathf.Clamp(col.g, 0f, 1f);
            col.b = Mathf.Clamp(col.b, 0f, 1f);
            return col;
        }
    }

}
