using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ZoomEquipment : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{

    public float scaleZoomBoard = 2;

    Vector3 cachedScale;
    Vector3 cachedPosition;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void OnPointerEnter(PointerEventData eventData) {
        cachedScale = transform.localScale;
        transform.localScale = new Vector3(scaleZoomBoard, scaleZoomBoard, scaleZoomBoard);
        cachedPosition = transform.localPosition;
        transform.localPosition = new Vector3(cachedPosition.x, cachedPosition.y, cachedPosition.z - 2f);
    }

    public void OnPointerExit(PointerEventData eventData) {
        transform.localScale = cachedScale;
        transform.localPosition = cachedPosition;
    }

}
