using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrophyController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f; // ȸ�� �ӵ�
    [SerializeField] private float scaleSpeed = 0.1f; // ũ�� ���� �ӵ�
    [SerializeField] private float maxScale = 3f; // �ִ� ũ��
    [SerializeField] private float rotationTransitionSpeed = 2f; // ȸ�� ��ȯ �ӵ�

    [SerializeField] private Text gameTypeText; // ���� ������ ǥ���ϴ� Text UI
    [SerializeField] private Text matchInfoText; // ��ġ ������ ǥ���ϴ� Text UI
    [SerializeField] private string gameType; // ���� ����
    [SerializeField] private string matchInfo; // ��ġ ����

    [SerializeField] private float textCharacterDelay = 0.3f; // �� ���ھ� �ؽ�Ʈ�� ǥ���ϴ� ������ �ð�

    [SerializeField] private ParticleSystem fireworksParticleSystem; // �Ҳɳ��� ȿ���� ����ϴ� ��ƼŬ �ý���
    [SerializeField] private float fireworksDuration = 10f; // �Ҳɳ��� ȿ�� ���� �ð�

    [SerializeField] private CanvasGroup canvasGroup; // ���̵�ƿ� ȿ���� ������ ĵ������ CanvasGroup ������Ʈ
    [SerializeField] private float fadeOutDuration = 2f; // ���̵�ƿ� ���� �ð�

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
            rectTransform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime); // Y���� �������� �̹��� ȸ��

            Vector3 newScale = rectTransform.localScale + Vector3.one * scaleSpeed * Time.deltaTime; // ũ�⸦ ���� ����
            rectTransform.localScale = newScale;

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
            rectTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime); // ȸ�� ��ȯ
            yield return null;
        }

        rectTransform.rotation = targetRotation;

        StartCoroutine(ShowTextOneByOne(gameType, gameTypeText, textCharacterDelay));
        StartCoroutine(ShowTextOneByOne(matchInfo, matchInfoText, textCharacterDelay));
    }

    private IEnumerator ShowTextOneByOne(string text, Text textComponent, float characterDelay)
    {
        textComponent.text = "";
        foreach (char c in text)
        {
            textComponent.text += c; // �� ���ھ� �ؽ�Ʈ�� �߰�
            yield return new WaitForSeconds(characterDelay);
        }

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
    }
}
