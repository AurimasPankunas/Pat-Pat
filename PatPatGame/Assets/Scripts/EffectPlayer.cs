using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class EffectPlayer : MonoBehaviour
{
    /// <summary>
    /// Spawn a temporary particle effect (detached from parent) with custom options
    /// </summary>
    public void Play(ParticleSystem prefab, Vector3 position, Quaternion rotation, EffectOptions options)
    {
        ParticleSystem ps = Instantiate(prefab, position, rotation);

        SetupAndPlay(ps, options);
    }

    /// <summary>
    /// Play a temporary particle effect that follows a parent transform
    /// </summary>
    public void PlayAttached(ParticleSystem prefab, Transform parent, EffectOptions options)
    {
        var ps = Instantiate(prefab, parent);
        ps.transform.localPosition = Vector3.zero;

        SetupAndPlay(ps, options);
        
    }

    private void ApplyOptions(ParticleSystem ps, EffectOptions options)
    {
        var main = ps.main;

        if (options.color.HasValue)
            main.startColor = options.color.Value;
        if (options.duration.HasValue)
            StartCoroutine(StopAfterTime(ps, options.duration.Value));
            // main.duration = options.duration.Value;
    }

    private void SetupAndPlay(ParticleSystem ps, EffectOptions options)
    {
        var main = ps.main;
        main.stopAction = ParticleSystemStopAction.Destroy;

        ApplyOptions(ps, options);

        ps.Play();
    }

    private IEnumerator StopAfterTime(ParticleSystem ps, float t)
    {
        yield return new WaitForSeconds(t);

        if (ps == null)
        yield break;
        
        ps.Stop();
    }
}

public struct EffectOptions
{
    public Color? color;
    public float? duration;
}
