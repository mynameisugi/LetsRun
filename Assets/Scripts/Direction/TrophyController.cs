using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrophyController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f; // ȸ�� �ӵ�
    [SerializeField] private float scaleSpeed = 0.1f; // ũ�� ���� �ӵ�
    [SerializeField] private float maxScale = 3f; // �ִ� ũ��
    [SerializeField] private float rotationTransitionSpeed = 2f; // ȸ�� ��ȯ �ӵ�
    [SerializeField] private float movementSpeed = 5f; // ��ǥ ��ġ�� �̵��ϴ� �ӵ�

    [SerializeField] private Text gameTypeText; // ���� ������ ǥ���ϴ� �ؽ�Ʈ UI
    [SerializeField] private Text matchInfoText; // ��ġ ������ ǥ���ϴ� �ؽ�Ʈ UI
    [SerializeField] private string gameType; // ���� ����
    [SerializeField] private string matchInfo; // ��ġ ����

    [SerializeField] private float transitionDuration = 3f; // �ؽ�Ʈ �̵� ��ȯ �ð�

    [SerializeField] private ParticleSystem fireworksParticleSystem; // �Ҳɳ��� ȿ���� ����ϴ� ��ƼŬ �ý���
    [SerializeField] private float fireworksDuration = 10f; // �Ҳɳ��� ȿ�� ���� �ð�

    [SerializeField] private CanvasGroup canvasGroup; // ���̵�ƿ� ȿ���� ������ ĵ���� �׷� ������Ʈ
    [SerializeField] private float fadeOutDuration = 2f; // ���̵�ƿ� ���� �ð�

    private RectTransform rectTransform;
    private bool shouldRotateAndScale = true;
    private Quaternion targetRotation;
    private Vector3 targetPosition = Vector3.zero; // �̵��� ��ǥ ��ġ

    public GameObject objectToDestroy;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        targetRotation = rectTransform.rotation;
    }

    private void Update()
    {
        if (shouldRotateAndScale)
        {
            rectTransform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime); // Y���� �߽����� �̹��� ȸ��

            Vector3 newScale = rectTransform.localScale + Vector3.one * scaleSpeed * Time.deltaTime; // ũ�� ����
            rectTransform.localScale = newScale;

            if (newScale.x >= maxScale)
            {
                shouldRotateAndScale = false;
                StartCoroutine(TransitionRotation());
            }
        }
        else
        {
            rectTransform.rotation = Quaternion.RotateTowards(rectTransform.rotation, targetRotation, rotationTransitionSpeed * Time.deltaTime); // ȸ�� ��ȯ

            rectTransform.anchoredPosition = Vector3.MoveTowards(rectTransform.anchoredPosition, targetPosition, movementSpeed * Time.deltaTime); // ��ǥ ��ġ�� �̵�
        }
    }

    private IEnumerator TransitionRotation()
    {
        Quaternion startRotation = rectTransform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * rotationTransitionSpeed;
            rectTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime); // ȸ�� ��ȯ
            yield return null;
        }

        rectTransform.rotation = targetRotation;

        StartCoroutine(ShowText(gameType, gameTypeText));
        StartCoroutine(ShowText(matchInfo, matchInfoText));
    }

    private IEnumerator ShowText(string text, Text textComponent)
    {
        textComponent.text = text; // �ؽ�Ʈ ��ü�� �� ���� �Ҵ�
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
        fireworksParticleSystem.Play(); // �Ҳɳ��� ȿ�� ���

        StartCoroutine(StopFireworks());
    }

    private IEnumerator StopFireworks()
    {
        yield return new WaitForSeconds(fireworksDuration);
        fireworksParticleSystem.Stop(); // ���� �ð� �� �Ҳɳ��� ȿ�� ����

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
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t); // ĵ���� ���̵�ƿ�
            yield return null;
        }

        canvasGroup.alpha = 0f;

        yield return new WaitForSeconds(3f);

        Destroy(objectToDestroy);
    }
}
