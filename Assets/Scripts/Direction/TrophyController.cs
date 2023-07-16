using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrophyController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float scaleSpeed = 0.1f;
    [SerializeField] private float maxScale = 3f;
    [SerializeField] private float rotationTransitionSpeed = 2f;

    [SerializeField] private Text gameTypeText;
    [SerializeField] private Text matchInfoText;
    [SerializeField] private string gameType = "OOO배 게임 개최";
    [SerializeField] private string matchInfo = "10 미터 경기";

    [SerializeField] private float initialTextDelay = 8f;
    [SerializeField] private float textCharacterDelay = 0.3f;

    private RectTransform rectTransform;
    private bool shouldRotateAndScale = true;
    private Quaternion targetRotation;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        targetRotation = rectTransform.rotation;
    }

    private void Update()
    {
        if (shouldRotateAndScale)
        {
            // Rotate the image around the Y-axis
            rectTransform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

            // Increase the scale of the image gradually
            Vector3 newScale = rectTransform.localScale + Vector3.one * scaleSpeed * Time.deltaTime;
            rectTransform.localScale = newScale;

            // Check if the scale has reached the maxScale
            if (newScale.x >= maxScale)
            {
                shouldRotateAndScale = false;
                StartCoroutine(TransitionRotation());
            }
        }
    }

    private IEnumerator TransitionRotation()
    {
        Quaternion startRotation = rectTransform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * rotationTransitionSpeed;
            rectTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime);
            yield return null;
        }

        rectTransform.rotation = targetRotation;
    }

    private void Start()
    {
        StartCoroutine(ShowTextOneByOne(gameType, gameTypeText, initialTextDelay, textCharacterDelay));
        StartCoroutine(ShowTextOneByOne(matchInfo, matchInfoText, initialTextDelay, textCharacterDelay));
    }

    private IEnumerator ShowTextOneByOne(string text, Text textComponent, float initialDelay, float characterDelay)
    {
        yield return new WaitForSeconds(initialDelay);

        textComponent.text = "";
        foreach (char c in text)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(characterDelay);
        }
    }
}
