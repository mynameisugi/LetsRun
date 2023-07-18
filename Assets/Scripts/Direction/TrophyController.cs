using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrophyController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f; // 회전 속도
    [SerializeField] private float scaleSpeed = 0.1f; // 크기 변경 속도
    [SerializeField] private float maxScale = 3f; // 최대 크기
    [SerializeField] private float rotationTransitionSpeed = 2f; // 회전 전환 속도
    [SerializeField] private float movementSpeed = 5f; // 목표 위치로 이동하는 속도

    [SerializeField] private Text gameTypeText; // 게임 유형을 표시하는 텍스트 UI
    [SerializeField] private Text matchInfoText; // 매치 정보를 표시하는 텍스트 UI
    [SerializeField] private string gameType; // 게임 유형
    [SerializeField] private string matchInfo; // 매치 정보

    [SerializeField] private float transitionDuration = 3f; // 텍스트 이동 전환 시간

    [SerializeField] private ParticleSystem fireworksParticleSystem; // 불꽃놀이 효과를 재생하는 파티클 시스템
    [SerializeField] private float fireworksDuration = 10f; // 불꽃놀이 효과 지속 시간

    [SerializeField] private CanvasGroup canvasGroup; // 페이드아웃 효과를 적용할 캔버스 그룹 컴포넌트
    [SerializeField] private float fadeOutDuration = 2f; // 페이드아웃 지속 시간

    private RectTransform rectTransform;
    private bool shouldRotateAndScale = true;
    private Quaternion targetRotation;
    private Vector3 targetPosition = Vector3.zero; // 이동할 목표 위치

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
            rectTransform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime); // Y축을 중심으로 이미지 회전

            Vector3 newScale = rectTransform.localScale + Vector3.one * scaleSpeed * Time.deltaTime; // 크기 증가
            rectTransform.localScale = newScale;

            if (newScale.x >= maxScale)
            {
                shouldRotateAndScale = false;
                StartCoroutine(TransitionRotation());
            }
        }
        else
        {
            rectTransform.rotation = Quaternion.RotateTowards(rectTransform.rotation, targetRotation, rotationTransitionSpeed * Time.deltaTime); // 회전 전환

            rectTransform.anchoredPosition = Vector3.MoveTowards(rectTransform.anchoredPosition, targetPosition, movementSpeed * Time.deltaTime); // 목표 위치로 이동
        }
    }

    private IEnumerator TransitionRotation()
    {
        Quaternion startRotation = rectTransform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * rotationTransitionSpeed;
            rectTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime); // 회전 전환
            yield return null;
        }

        rectTransform.rotation = targetRotation;

        StartCoroutine(ShowText(gameType, gameTypeText));
        StartCoroutine(ShowText(matchInfo, matchInfoText));
    }

    private IEnumerator ShowText(string text, Text textComponent)
    {
        textComponent.text = text; // 텍스트 전체를 한 번에 할당
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
        fireworksParticleSystem.Play(); // 불꽃놀이 효과 재생

        StartCoroutine(StopFireworks());
    }

    private IEnumerator StopFireworks()
    {
        yield return new WaitForSeconds(fireworksDuration);
        fireworksParticleSystem.Stop(); // 일정 시간 후 불꽃놀이 효과 중지

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
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t); // 캔버스 페이드아웃
            yield return null;
        }

        canvasGroup.alpha = 0f;

        yield return new WaitForSeconds(3f);

        Destroy(objectToDestroy);
    }
}
