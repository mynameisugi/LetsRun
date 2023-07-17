using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrophyController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f; // Rotation speed
    [SerializeField] private float scaleSpeed = 0.1f; // Size change rate
    [SerializeField] private float maxScale = 3f; // Max size
    [SerializeField] private float rotationTransitionSpeed = 2f; // Rotation conversion rate
    [SerializeField] private float movementSpeed = 5f; // Speed at which the trophy moves towards target position

    [SerializeField] private Text gameTypeText; // Text UI to display the game type
    [SerializeField] private Text matchInfoText; // Text UI to display match information
    [SerializeField] private string gameType; // Game type
    [SerializeField] private string matchInfo; // Match information

    [SerializeField] private float transitionDuration = 3f; // Transition duration for moving text to X = 0

    [SerializeField] private ParticleSystem fireworksParticleSystem; // Particle system to play fireworks effect
    [SerializeField] private float fireworksDuration = 10f; // Fireworks effect duration

    [SerializeField] private CanvasGroup canvasGroup; // CanvasGroup component of the canvas to apply the fadeout effect to
    [SerializeField] private float fadeOutDuration = 2f; // Fadeout duration

    private RectTransform rectTransform;
    private bool shouldRotateAndScale = true;
    private Quaternion targetRotation;
    private Vector3 targetPosition = Vector3.zero; // Target position to move towards

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        targetRotation = rectTransform.rotation;
    }

    private void Update()
    {
        if (shouldRotateAndScale)
        {
            rectTransform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime); // Rotate the image around the Y axis

            Vector3 newScale = rectTransform.localScale + Vector3.one * scaleSpeed * Time.deltaTime; // Incrementally increase the size
            rectTransform.localScale = newScale;

            if (newScale.x >= maxScale)
            {
                shouldRotateAndScale = false;
                StartCoroutine(TransitionRotation());
            }
        }
        else
        {
            rectTransform.rotation = Quaternion.RotateTowards(rectTransform.rotation, targetRotation, rotationTransitionSpeed * Time.deltaTime); // Toggle rotation

            rectTransform.anchoredPosition = Vector3.MoveTowards(rectTransform.anchoredPosition, targetPosition, movementSpeed * Time.deltaTime); // Move towards target position
        }
    }

    private IEnumerator TransitionRotation()
    {
        Quaternion startRotation = rectTransform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * rotationTransitionSpeed;
            rectTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime); // Toggle rotation
            yield return null;
        }

        rectTransform.rotation = targetRotation;

        StartCoroutine(ShowText(gameType, gameTypeText));
        StartCoroutine(ShowText(matchInfo, matchInfoText));
    }

    private IEnumerator ShowText(string text, Text textComponent)
    {
        textComponent.text = text; // Assign the entire text at once
        Vector2 startPosition = textComponent.rectTransform.anchoredPosition;
        Vector2 targetPosition = new Vector2(0f, startPosition.y);
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);
            textComponent.rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        textComponent.rectTransform.anchoredPosition = targetPosition;
        PlayFireworks();
    }



    private void PlayFireworks()
    {
        fireworksParticleSystem.Play(); // Play fireworks effect

        StartCoroutine(StopFireworks());
    }

    private IEnumerator StopFireworks()
    {
        yield return new WaitForSeconds(fireworksDuration);
        fireworksParticleSystem.Stop(); // Stop the fireworks effect after a certain amount of time

        StartCoroutine(FadeOutCanvas());
    }

    private IEnumerator FadeOutCanvas()
    {
        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeOutDuration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t); // Canvas fade out
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}
