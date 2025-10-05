using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public static class ScaleAnimator
{
    public static void PlayScale(Transform target)
    {
        PlayScaleAsync(target).Forget();
    }

    private static async UniTask PlayScaleAsync(Transform target)
    {

        try
        {
            target.localScale = Vector3.zero;

            await LerpScale(target, Vector3.zero, Vector3.one * 1.2f, 0.2f);

            await LerpScale(target, Vector3.one * 1.2f, Vector3.one, 0.1f);
        }
        catch (OperationCanceledException)
        {
            //
        }
    }

    private static async UniTask LerpScale(Transform target, Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            target.localScale = Vector3.Lerp(from, to, t);
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        target.localScale = to;
    }
}