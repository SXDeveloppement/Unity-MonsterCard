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
        // Affichage du tooltip de la capacité sur le terrain
        if (GetComponent<AbilityDisplay>() != null)
            GetComponent<AbilityDisplay>().GO_Tooltip.SetActive(true);
        // Affichage du tooltip de la capacité dans le fenêtre de team
        else
            GetComponent<AbilityLayoutTeamDisplay>().GO_Tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        // Affichage du tooltip de la capacité sur le terrain
        if (GetComponent<AbilityDisplay>() != null)
            GetComponent<AbilityDisplay>().GO_Tooltip.SetActive(false);
        // Affichage du tooltip de la capacité dans le fenêtre de team
        else
            GetComponent<AbilityLayoutTeamDisplay>().GO_Tooltip.SetActive(false);
    }
}
