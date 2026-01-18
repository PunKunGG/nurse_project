// KnowledgePopupUI.cs
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class KnowledgePopupUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panel;
    [SerializeField] private TMP_Text knowledgeText;
    [SerializeField] private Button closeButton;

    [Header("Animation")]
    [SerializeField] private float animDuration = 0.20f;
    [SerializeField] private float startScale = 0.85f;

    [Header("Auto Close")]
    [SerializeField] private float autoCloseSeconds = 2.5f;

    private Coroutine autoCloseCo;
    private Coroutine animCo;

    public event Action OnClosed;

    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);

        HideInstant();
    }

    /// <summary>
    /// Show popup with message. Resets the auto-close timer.
    /// </summary>
    public void Show(string message)
    {
        if (knowledgeText != null)
            knowledgeText.text = string.IsNullOrWhiteSpace(message) ? " " : message;

        gameObject.SetActive(true);

        // cancel any prior animations/timers
        if (animCo != null) StopCoroutine(animCo);
        if (autoCloseCo != null) StopCoroutine(autoCloseCo);

        // make interactable immediately (modal behavior)
        if (canvasGroup != null)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        // animate in
        animCo = StartCoroutine(Animate(show: true));

        // auto close
        autoCloseCo = StartCoroutine(AutoClose());
    }

    public void Hide()
    {
        if (!gameObject.activeSelf) return;

        if (autoCloseCo != null) StopCoroutine(autoCloseCo);
        if (animCo != null) StopCoroutine(animCo);

        animCo = StartCoroutine(Animate(show: false));
    }

    public void HideInstant()
    {
        if (autoCloseCo != null) StopCoroutine(autoCloseCo);
        if (animCo != null) StopCoroutine(animCo);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (panel != null)
            panel.localScale = Vector3.one * startScale;

        gameObject.SetActive(false);
    }

    private IEnumerator AutoClose()
    {
        yield return new WaitForSecondsRealtime(Mathf.Max(0.1f, autoCloseSeconds));
        Hide();
    }

    private IEnumerator Animate(bool show)
    {
        if (canvasGroup == null || panel == null)
        {
            // Fallback: no animation if references are missing
            if (!show) gameObject.SetActive(false);
            yield break;
        }

        float t = 0f;

        float fromA = show ? 0f : 1f;
        float toA   = show ? 1f : 0f;

        Vector3 fromS = show ? Vector3.one * startScale : Vector3.one;
        Vector3 toS   = show ? Vector3.one : Vector3.one * startScale;

        // Initialize at start values (prevents visual popping)
        canvasGroup.alpha = fromA;
        panel.localScale  = fromS;

        while (t < animDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / animDuration);

            canvasGroup.alpha = Mathf.Lerp(fromA, toA, k);
            panel.localScale  = Vector3.Lerp(fromS, toS, k);

            yield return null;
        }

        canvasGroup.alpha = toA;
        panel.localScale  = toS;

        if (!show)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
            OnClosed?.Invoke();
        }
    }
}
