using UnityEngine;
using System.Collections;

public class ChangingFloat : PropertyAttribute
{

    public float MinValue = 0f;
    public float MaxValue = 0f;

    public float MinTime = 0f;
    public float MaxTime = 0f;

    public float Constant = 0f;

    public bool Periodic = false;

    private float target = 0f;
    private float value = 0f;
    private float countdown = 0f;
    
    public float Value
    {
        get
        {
            return value;
        }
    }

	public void Update () {
        countdown -= Time.deltaTime;
        if (countdown < 0)
        {
            countdown = MinTime + Random.value * (MaxTime - MinTime);
            target = MinValue + Random.value * (MaxValue - MinValue);
        }

        float diff = target - value;
        if (Periodic)
        {
            while (diff >= MaxValue)
                diff += MinValue - MaxValue;
            while (diff < MinValue)
                diff += MaxValue - MinValue;
        }
        value += diff * (1 - Mathf.Exp(-Constant * Time.deltaTime));
        //value = target;
	}
}
