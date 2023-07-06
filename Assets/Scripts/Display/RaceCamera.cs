using UnityEngine;

public class RaceCamera : MonoBehaviour
{
    private float focusTime = 1f;
    private int focusEntry = 0;
    private Vector3 focusAt = Vector3.forward;

    private void Update()
    {
        var race = GameManager.Instance().Race.CurrentRace;
        focusTime -= Time.deltaTime;
        if (focusTime <= 0f) RerollFocus(race);
        if (race)
        {
            var entry = race.GetEntry(focusEntry);
            if (!entry) { RerollFocus(race); return; }
            focusAt = entry.transform.position;
        }
        Quaternion lastRot = transform.rotation;
        transform.LookAt(focusAt, Vector3.up);
        transform.rotation = Quaternion.Slerp(lastRot, transform.rotation, Time.deltaTime * 2f);

        void RerollFocus(bool race)
        {
            focusTime = Random.Range(1f, 4f);
            if (!race) focusTime *= 4f;
            if (race) focusEntry = Random.Range(0, 8);
            else focusAt = transform.position + new Vector3(Random.value - 0.5f, -Random.value, Random.value - 0.5f).normalized;
        }
    }
}
