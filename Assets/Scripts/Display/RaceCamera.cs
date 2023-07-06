using UnityEngine;

public class RaceCamera : MonoBehaviour
{
    private float focusTime = 1f;
    private int focusEntry = 0;

    private void Update()
    {
        var race = GameManager.Instance().Race.CurrentRace;
        if (!race)
        {
            return;
        }
        focusTime -= Time.deltaTime;
        if (focusTime <= 0f) RerollFocus();
        var entry = race.GetEntry(focusEntry);
        if (!entry) { RerollFocus(); return; }
        Vector3 focus = entry.transform.position;
        Quaternion lastRot = transform.rotation;
        transform.LookAt(focus, Vector3.up);
        transform.rotation = Quaternion.Slerp(lastRot, transform.rotation, Time.deltaTime * 2f);

        void RerollFocus()
        {
            focusEntry = Random.Range(0, 8);
            focusTime = Random.Range(1f, 4f);
        }

        /*
        static Vector3 GetFocus(Race curRace)
        {
            List<Transform> entries = new();
            for (int i = 0; i < 8; ++i)
            {
                var entry = curRace.GetEntry(i);
                if (entry) entries.Add(entry.transform);
            }
            Vector3 focus = Vector3.zero;
            foreach (var tf in entries) focus += tf.position;
            return focus / entries.Count;
        } */
    }
}
