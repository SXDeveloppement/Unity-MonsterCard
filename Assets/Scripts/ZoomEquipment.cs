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

    GameManager gameManager;
    bool pointerIsEnter = false;

    // Start is called before the first frame update
    void Start() {
        gameManager = GameObject.FindAnyObjectByType<GameManager>();

    }

    // Update is called once per frame
    void Update() {

    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (GameManager.dragged) return;

        cachedScale = transform.localScale;
        transform.localScale = new Vector3(scaleZoomBoard, scaleZoomBoard, scaleZoomBoard);
        cachedPosition = transform.localPosition;
        transform.localPosition = new Vector3(cachedPosition.x, cachedPosition.y, cachedPosition.z - 2f);
        pointerIsEnter = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (GameManager.dragged) return;
        if (!pointerIsEnter) return;

        transform.localScale = cachedScale;
        transform.localPosition = cachedPosition;
        pointerIsEnter = false;
    }

}
