using System;
using System.Collections;
using UnityEngine;

public class SquashAndStretch : MonoBehaviour
{
    [Flags]
    public enum SquashStretchAxis
    {
        None = 0,
        X    = 1,
        Y    = 2,
        Z    = 4,
    }

    [Header("Squash and Stretch Core")]
    [SerializeField, Tooltip("Defaults to current GO if not set")] private Transform _transformToAffect;
    [SerializeField]               private SquashStretchAxis _axisToAffect      = SquashStretchAxis.Y;
    [SerializeField, Range(0, 1f)] private float             _animationDuration = 0.25f;
    [SerializeField]               private bool              _canBeOverwritten  = false;
    [SerializeField]               private bool              _playOnStart       = false;

    [Header("Animation Settings")]
    [SerializeField] private float _initialScale = 1f;
    [SerializeField] private float _maximumScale        = 1.3f;
    [SerializeField] private bool  _resetToInitialScale = true;
    [SerializeField] private bool  _isReversed          = false;

    [SerializeField] private AnimationCurve _squashAndStretchCurve = new AnimationCurve(
         new Keyframe(0f,    0f),
         new Keyframe(0.25f, 1f),
         new Keyframe(1f,    0f)
        );

    [Header("Looping Settings")]
    [SerializeField] private bool _loooping = false;
    [SerializeField] private float _loopingDelay = 0.5f;

    private Coroutine      _squashAndStretchCoroutine;
    private WaitForSeconds _lopingDelayWaitForSeconds;
    private Vector3        _initialScaleVector;

    private bool AffectX => (_axisToAffect & SquashStretchAxis.X) != 0;
    private bool AffectY => (_axisToAffect & SquashStretchAxis.Y) != 0;
    private bool AffectZ => (_axisToAffect & SquashStretchAxis.Z) != 0;

    private void Awake()
    {
        if (!_transformToAffect)
            _transformToAffect = transform;

        _initialScaleVector        = _transformToAffect.localScale;
        _lopingDelayWaitForSeconds = new WaitForSeconds(_loopingDelay);
    }

    private void Start()
    {
        if (_playOnStart)
        {
            CheckForStartCoroutine();
        }
    }

    [ContextMenu("Play Squash and Stretch")]
    public void PlaySquashAndStretch()
    {
        if (_loooping && !_canBeOverwritten)
            return;

        CheckForStartCoroutine();
    }

    private void CheckForStartCoroutine()
    {
        if (_axisToAffect == SquashStretchAxis.None)
        {
            Debug.LogWarning("No axis to affect set. Squash and Stretch will not run.", gameObject);
            return;
        }

        if (_squashAndStretchCoroutine != null)
        {
            StopCoroutine(_squashAndStretchCoroutine);
            if (_resetToInitialScale)
            {
                transform.localScale = _initialScaleVector;
            }
        }

        _squashAndStretchCoroutine = StartCoroutine(SquashAndStretchEffect());
    }

    private IEnumerator SquashAndStretchEffect()
    {
        do
        {
            float   elapsedTime   = 0;
            Vector3 originalScale = _initialScaleVector;
            Vector3 modifiedScale = originalScale;

            while (elapsedTime < _animationDuration)
            {
                elapsedTime += Time.deltaTime;

                float curvePosition;

                if (_isReversed)
                {
                    curvePosition = 1 - (elapsedTime / _animationDuration);
                }
                else
                {
                    curvePosition = elapsedTime / _animationDuration;
                }

                float curveValue    = _squashAndStretchCurve.Evaluate(curvePosition);
                float remappedValue = _initialScale + (curveValue * (_maximumScale - _initialScale));
                ;
                float minimumThreshold = 0.0001f;
                if (Mathf.Abs(remappedValue) < minimumThreshold)
                {
                    remappedValue = minimumThreshold;
                }

                if (AffectX)
                    modifiedScale.x = originalScale.x * remappedValue;
                else
                    modifiedScale.x = originalScale.x / remappedValue;

                if (AffectY)
                    modifiedScale.y = originalScale.y * remappedValue;
                else
                    modifiedScale.y = originalScale.y / remappedValue;

                if (AffectZ)
                    modifiedScale.z = originalScale.z * remappedValue;
                else
                    modifiedScale.z = originalScale.z / remappedValue;

                transform.localScale = modifiedScale;
                yield return null;
            }

            if (_resetToInitialScale)
                transform.localScale = originalScale;

            if (_loooping)
            {
                yield return _lopingDelayWaitForSeconds;
            }
        } while (_loooping);
    }

    public void SetLooping(bool shouldLoop) => _loooping = shouldLoop;
}