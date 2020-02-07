using UnityEngine;
using DG.Tweening;
using System.Collections;

public class GhostTrail : MonoBehaviour
{
    private SpriteRenderer _sprite;
    public Color trailColor;
    public Color fadeColor;
    public float ghostInterval;
    public float fadeTime;

    public void ShowGhost()
    {
        Sequence s = DOTween.Sequence();

        _sprite = PlayerController.PlayerInstance.m_sprite;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform currentGhost = transform.GetChild(i);
            s.AppendCallback(() => currentGhost.position = PlayerController.PlayerInstance.Position);
            s.AppendCallback(() => currentGhost.GetComponent<SpriteRenderer>().sprite = _sprite.sprite);
            s.AppendCallback(() => currentGhost.GetComponent<SpriteRenderer>().flipX =
            PlayerController.PlayerInstance.FaceDir.x < 0 ? true : false);
            s.Append(currentGhost.GetComponent<SpriteRenderer>().material.DOColor(trailColor, 0));
            s.AppendCallback(() => FadeSprite(currentGhost));
            s.AppendInterval(ghostInterval);
        }
    }

    public void FadeSprite(Transform current)
    {
        current.GetComponent<SpriteRenderer>().material.DOKill();
        current.GetComponent<SpriteRenderer>().material.DOColor(fadeColor, fadeTime);
    }

}
