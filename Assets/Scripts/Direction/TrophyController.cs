using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrophyController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f; // 회전 속도
    [SerializeField] private float scaleSpeed = 0.1f; // 크기 변경 속도
    [SerializeField] private float maxScale = 3f; // 최대 크기
    [SerializeField] private float rotationTransitionSpeed = 2f; // 회전 전환 속도

    [SerializeField] private Text gameTypeText; // 게임 유형을 표시하는 Text UI
    [SerializeField] private Text matchInfoText; // 매치 정보를 표시하는 Text UI
    [SerializeField] private string gameType; // 게임 유형
    [SerializeField] private string matchInfo; // 매치 정보

    [SerializeField] private float textCharacterDelay = 0.3f; // 한 글자씩 텍스트를 표시하는 딜레이 시간

    [SerializeField] private ParticleSystem fireworksParticleSystem; // 불꽃놀이 효과를 재생하는 파티클 시스템
    [SerializeField] private float fireworksDuration = 10f; // 불꽃놀이 효과 지속 시간

    [SerializeField] private CanvasGroup canvasGroup; // 페이드아웃 효과를 적용할 캔버스의 CanvasGroup 컴포넌트
    [SerializeField] private float fadeOutDuration = 2f; // 페이드아웃 지속 시간

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
            rectTransform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime); // Y축을 기준으로 이미지 회전

            Vector3 newScale = rectTransform.localScale + Vector3.one * scaleSpeed * Time.deltaTime; // 크기를 점차 증가
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
            rectTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime); // 회전 전환
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
            textComponent.text += c; // 한 글자씩 텍스트에 추가
            yield return new WaitForSeconds(characterDelay);
        }

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
        fireworksParticleSystem.Stop(); // 일정 시간 후 불꽃놀이 효과 정지

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
    }
}
