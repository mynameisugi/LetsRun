using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandWatch : MonoBehaviour, IDragHandler, IEndDragHandler{

    private Vector3 panelLocation;
    public float percentThreshold = 0.2f;
    public float easing = 0.5f;
    private void Start() {
        panelLocation = transform.position;
    }

    public void OnDrag(PointerEventData _data) {
        float difference = _data.pressPosition.x - _data.position.x;
        transform.position = panelLocation - new Vector3(difference, 0, 0);
    }

    public void OnEndDrag(PointerEventData _data) {
        float percentage = (_data.pressPosition.x - _data.position.x) / Screen.width;
        if (Mathf.Abs(percentage) >= percentThreshold) {
            Vector3 newLocation = panelLocation;
            if (percentage > 0) {
                newLocation += new Vector3(-Screen.width, 0, 0);
            } else if (percentage < 0) {
                newLocation += new Vector3(Screen.width, 0, 0);
            }
            //transform.position = newLocation;
            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
        } else {
            /*transform.position = panelLocation; */
            StartCoroutine(SmoothMove(transform.position, panelLocation, easing));
        }
    }
    IEnumerator SmoothMove(Vector3 _startpos, Vector3 _endpos, float _seconds) {
        float t = 0f;
        while (t <= 1.0f) {
            t += Time.deltaTime / _seconds;
            transform.position = Vector3.Lerp(_startpos, _endpos, Mathf.SmoothStep(0f,1f,t));
            yield return null;
        }
    }
}
