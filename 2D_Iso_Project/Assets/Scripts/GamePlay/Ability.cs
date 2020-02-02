using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ability : MonoBehaviour
{
    protected bool isAble = true;
    public void Enable() => isAble = true;
    public void Disable() => isAble = false;

    public bool notUsing = true;
    public float duration = 0.1f;
    IEnumerator NotUsingAfterDuration()
    {
        notUsing = false;
        yield return new WaitForSeconds(duration);
        notUsing = true;
    }

    public bool notInInterval = true;
    public float Invertal = 1f;
    IEnumerator CanAfterInvertal()
    {
        notInInterval = false;
        yield return new WaitForSeconds(Invertal);
        notInInterval = true;
    }


    public void InitSetParams(float _duration, float _Invertal)
    {
        isAble = true;
        notUsing = true;
        notInInterval = true;
        duration = _duration;
        Invertal = _Invertal;
    }

    public bool Usable => isAble && notUsing && notInInterval;
    public bool IsUsing => !notUsing;

    public void Use()
    {
        StartCoroutine(NotUsingAfterDuration());
        StartCoroutine(CanAfterInvertal());
    }
}

