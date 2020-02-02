﻿using UnityEngine;
using DG.Tweening;
using System.Collections;

public class GhostTrail : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private Transform ghostsParent;
    private CharacterMove movement;
    public Color trailColor;
    public Color fadeColor;
    public float ghostInterval;
    public float fadeTime;

    void Awake()
    {
        ghostsParent = transform.parent;
        movement = ghostsParent.GetComponentInChildren<CharacterMove>();
    }

    public void ShowGhost()
    {
        Sequence s = DOTween.Sequence();

        _sprite = PlayerController.PlayerInstance.m_sprite;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform currentGhost = transform.GetChild(i);
            float dis = ((float)transform.childCount - i) / 1.5f;
            currentGhost.localPosition = -movement.FaceDir * dis;
            currentGhost.GetComponent<SpriteRenderer>().sprite = _sprite.sprite;
            currentGhost.GetComponent<SpriteRenderer>().flipX = movement.FaceDir.x < 0 ? true : false;
            s.Append(currentGhost.GetComponent<SpriteRenderer>().material.DOColor(trailColor, 0));
            s.AppendInterval(ghostInterval);
            s.AppendCallback(() => FadeSprite(currentGhost));
        }
    }

    public void FadeSprite(Transform current)
    {
        current.GetComponent<SpriteRenderer>().material.DOKill();
        current.GetComponent<SpriteRenderer>().material.DOColor(fadeColor, fadeTime);
    }

}
