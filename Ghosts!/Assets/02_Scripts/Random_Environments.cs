using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Random_Environments : MonoBehaviour
{
    System.Random random;
    public Light lanternFire_Light;
    public GameObject Lantern_Fire;
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

    public AudioSource Lightning_Audio;
    public AudioSource FireIgnite_Audio;

    void Start()
    {
        random = new System.Random();

        isBlinking = false;
        isLightning = false;

        next_blink = random.Next(Blink_return_min_Time, Blink_return_max_Time);
        next_Lightning = random.Next(Lightning_return_min_Time, Lightning_return_max_Time);
    }

    void Update()
    {
        blink_Lantern();
        Window_Lightning();
    }

    void blink_Lantern()
    {
        if (!isBlinking && Time.time > current_Blink + next_blink)
        {
            next_blink = random.Next(Blink_return_min_Time, Blink_return_max_Time);

            current_Blink = Time.time;

            StartCoroutine(blink());

            isBlinking = true;
        }
    }

    void Window_Lightning()
    {
        if(!isLightning && Time.time > current_Lightning + next_Lightning)
        {
            next_Lightning = random.Next(Lightning_return_min_Time, Lightning_return_max_Time);

            current_Lightning = Time.time;

            StartCoroutine(Lightning());

            isLightning = true;
        }
    }

    IEnumerator blink()
    {
        lanternFire_Light.intensity = 0;
        Lantern_Fire.SetActive(false);
        yield return new WaitForSeconds(1.0f);

        while (lanternFire_Light.intensity <= 2)
        {
            lanternFire_Light.intensity += 0.1f;
            yield return new WaitForSeconds(0.05f);
        }

        FireIgnite_Audio.Play();

        Lantern_Fire.SetActive(true);

        isBlinking = false;
    }

    IEnumerator Lightning()
    {
        Lightning_Audio.Play();

        Window_Light.intensity = random.Next(5, 7);
        yield return new WaitForSeconds(0.1f);
        Window_Light.intensity = random.Next(3, 5);
        yield return new WaitForSeconds(0.05f);
        Window_Light.intensity = random.Next(3, 4);
        yield return new WaitForSeconds(0.05f);

        Window_Light.intensity = 1;

        isLightning = false;
    }
}
