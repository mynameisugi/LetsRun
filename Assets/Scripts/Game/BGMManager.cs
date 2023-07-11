using UnityEngine;

public class BGMManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] normalBGMs;
    [SerializeField]
    private AudioClip[] raceBGMs;

    private AudioSource audioSource;

    private AudioClip queuedClip = null;
    private float fade = 1f;

    public bool IsRaceBGM { get; private set; } = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayNormalBGM();
    }

    private void Update()
    {
        audioSource.volume = fade * (IsRaceBGM ? 0.2f : 0.5f) * GameSettings.Values.BGM;
        if (queuedClip != null) fade -= Time.deltaTime;
        if (fade <= 0f)
        {
            fade = 1f;
            audioSource.Stop();
            audioSource.clip = queuedClip;
            audioSource.volume = GameSettings.Values.BGM;
            audioSource.Play();
            queuedClip = null;
        }
    }

    public void PlayNormalBGM()
    {
        queuedClip = normalBGMs[Random.Range(0, normalBGMs.Length)];
        IsRaceBGM = false;
    }

    public void PlayRaceBGM()
    {
        queuedClip = raceBGMs[Random.Range(0, raceBGMs.Length)];
        IsRaceBGM = true;
    }

}