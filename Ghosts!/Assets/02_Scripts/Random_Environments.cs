using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Random_Environments : MonoBehaviour
{
    System.Random random;
    public Light lanternFire_Light;
    public Light Window_Light;

    bool isBlinking;
    bool isLightning;

    public int Blink_return_min_Time;
    public int Blink_return_max_Time;
    public int Lightning_return_min_Time;
    public int Lightning_return_max_Time;

    float current_Blink;
    float current_Lightning;

    float next_blink;
    float next_Lightning;

    void Start()
    {
        random = new System.Random();

        isBlinking = false;
        isLightning = false;

        next_blink = 0f;
        next_Lightning = 0f;
    }

    void Update()
    {
        blink_Lantern();
    }

    void blink_Lantern()
    {
        if (!isBlinking && Time.time > current_Blink + Blink_return_Time)
        {
                current_Blink = Time.time;
                Debug.Log(current_Blink);

                StartCoroutine(blink());

                isBlinking = true;
            next_blink


        }
    }

    IEnumerator blink()
    {
        lanternFire_Light.intensity = 0;
        yield return new WaitForSeconds(1.0f);

        while(lanternFire_Light.intensity <= 2)
        {
            lanternFire_Light.intensity += 0.1f;
            yield return null;
        }

        isBlinking = false;
    }
}
