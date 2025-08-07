using UnityEngine;
using System.Collections;


/// <summary>
/// Handles screen shake effects using either rotation or position jitter.
/// Usage: <c>ScreenShake.Instance.StartShake(duration, strength, frequency, shakeType);</c>
/// </summary>
public class ScreenShake : MonoBehaviour
{

    private static ScreenShake _instance;

    public static ScreenShake Instance
    {
        get
        {
            if (_instance == null)
            {
                var instances = FindObjectsByType<ScreenShake>(FindObjectsSortMode.None);

                if (instances.Length == 0)
                {
                    Debug.LogError("No ScreenShake instance found in the scene.");
                }
                else if (instances.Length > 1)
                {
                    Debug.LogWarning("Multiple ScreenShake instances found. Using the first one.");
                }

                _instance = instances.Length > 0 ? instances[0] : null;
            }

            return _instance;
        }
    }


    [Tooltip("Enable to shake a custom Transform instead of this GameObject.")]
    [SerializeField] private bool useCustomTarget = false;

    [Tooltip("The target Transform to apply shake effects to.")]
    [SerializeField] private Transform customTarget;

    private AnimationCurve fadeOutCurve = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 0.0f));
    private Coroutine shakeCoroutine;

    private Transform Target
    {
        get
        {
            if (useCustomTarget)
            {
                if (customTarget == null)
                {
                    Debug.LogWarning($"ScreenShake on '{gameObject.name}': 'Use Custom Target' is enabled but no target has been assigned. Falling back to self transform.");
                    return transform;
                }
                return customTarget;
            }
            else
            {
                return transform;
            }
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Debug.LogWarning("Duplicate ScreenShake instance detected. Destroying extra.");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Triggers a screen shake effect.
    /// </summary>
    /// <param name="duration">How long the shake will last in seconds.</param>
    /// <param name="strength">The strength of the shake.</param>
    /// <param name="frequency">How often the shake changes per second.</param>
    /// <param name="shakeType">Shake either the rotation or position.</param>
    public void StartShake(float duration, float strength, float frequency, ShakeType shakeType)
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);

        shakeCoroutine = shakeType switch
        {
            ShakeType.Rotation => StartCoroutine(ShakeRotation(duration, strength, frequency)),
            ShakeType.Position => StartCoroutine(ShakePosition(duration, strength, frequency)),
            _ => null
        };
    }

    private IEnumerator ShakeRotation(float duration, float maxAngle, float frequency)
    {
        Quaternion originalRotation = Target.localRotation;

        float timeElapsed = 0.0f;
        float interval = 1.0f / frequency;
        float timeSinceLastTarget = interval;

        Quaternion currentTargetRotation = Quaternion.identity;
        Quaternion currentRotation = originalRotation;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            timeSinceLastTarget += Time.deltaTime;

            float strength = fadeOutCurve.Evaluate(timeElapsed / duration);

            if (timeSinceLastTarget >= interval)
            {
                Vector3 randomEuler = new Vector3(
                    Random.Range(-1.0f, 1.0f),
                    Random.Range(-1.0f, 1.0f),
                    Random.Range(-1.0f, 1.0f)
                ) * maxAngle * strength;

                currentTargetRotation = Quaternion.Euler(randomEuler) * originalRotation;
                timeSinceLastTarget = 0.0f;
            }

            currentRotation = Quaternion.Slerp(currentRotation, currentTargetRotation, Time.deltaTime * frequency);
            Target.localRotation = currentRotation;

            yield return null;
        }

        float resetTime = 0.1f;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime / resetTime;
            Target.localRotation = Quaternion.Slerp(Target.localRotation, originalRotation, t);
            yield return null;
        }

        Target.localRotation = originalRotation;
    }

    private IEnumerator ShakePosition(float duration, float maxOffset, float frequency)
    {
        Vector3 originalPosition = Target.localPosition;
        Vector3 currentPosition = originalPosition;
        Vector3 targetPosition = originalPosition;

        float timeElapsed = 0f;
        float interval = 1f / frequency;
        float timeSinceLastTarget = interval;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            timeSinceLastTarget += Time.deltaTime;

            float strength = fadeOutCurve.Evaluate(timeElapsed / duration);

            if (timeSinceLastTarget >= interval)
            {
                Vector3 randomOffset = Random.insideUnitSphere * maxOffset * strength;
                targetPosition = originalPosition + randomOffset;
                timeSinceLastTarget = 0f;
            }

            currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * frequency);
            Target.localPosition = currentPosition;

            yield return null;
        }

        float resetTime = 0.1f;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime / resetTime;
            Target.localPosition = Vector3.Lerp(Target.localPosition, originalPosition, t);
            yield return null;
        }

        Target.localPosition = originalPosition;
    }
}

public enum ShakeType
{
    Rotation,
    Position
}
