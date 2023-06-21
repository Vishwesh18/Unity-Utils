using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public static class TweenExtensions
{

    public static Sequence FadeIn(this CanvasGroup obj, float time = 0.2f)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(obj.DOFade(1, time));

        return seq;
    }

    public static Sequence FadeOut(this CanvasGroup obj, float time = 0.2f)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(obj.DOFade(1, time));

        return seq;
    }


    public static Sequence FadeInWithCallback(this CanvasGroup obj, Action callback, float time = 0.2f)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(obj.DOFade(1, time));

        seq.OnComplete(() =>
        {
            callback.Invoke();
        });

        return seq;
    }

    public static Sequence FadeOutWithCallback(this CanvasGroup obj, Action callback, float time = 0.2f)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(obj.DOFade(0, time));

        seq.OnComplete(() =>
        {
            callback.Invoke();
        });

        return seq;
    }
}