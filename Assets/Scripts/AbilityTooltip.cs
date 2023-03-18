using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilityTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void OnPointerEnter(PointerEventData eventData) {
        GetComponent<AbilityDisplay>().GO_Tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        GetComponent<AbilityDisplay>().GO_Tooltip.SetActive(false);
    }
}
