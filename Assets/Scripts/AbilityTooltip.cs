using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilityTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private Vector3 positionCached;


    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (GameManager.dragged) return;

        // Affichage du tooltip de la capacité
        if (GetComponent<AbilityDisplay>() != null) {
            GetComponent<AbilityDisplay>().ShowHideTooltip(true);
            float floatNumber = 0;
            // Si c'est une capacité sur le terrain
            if (GetComponent<AbilityDisplay>().abilityStatus == AbilityStatus.Board)
                floatNumber = Constante.SCALE_ABILITY_BOARD_ZOOM;
            // Si c'est une capacité dans la pile d'action
            else if (GetComponent<AbilityDisplay>().abilityStatus == AbilityStatus.Action)
                floatNumber = Constante.SCALE_ABILITY_ACTION_ZOOM;
            // Si c'est une capacité dans le fenêtre de team
            else if (GetComponent<AbilityDisplay>().abilityStatus == AbilityStatus.TeamLayout)
                floatNumber = Constante.SCALE_ABILITY_TEAM_ZOOM;

            transform.localScale = Constante.ScaleComparedParent(floatNumber, transform.parent.gameObject);
            positionCached = transform.localPosition;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - 2f);
        }
        // Affichage du tooltip de la capacité dans le fenêtre de team
        else
            GetComponent<AbilityLayoutTeamDisplay>().GO_Tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        // Affichage du tooltip de la capacité sur le terrain
        if (GetComponent<AbilityDisplay>() != null) {
            float floatNumber = 0;
            // Si c'est une capacité sur le terrain
            if (GetComponent<AbilityDisplay>().abilityStatus == AbilityStatus.Board) {
                GetComponent<AbilityDisplay>().ShowHideTooltip(false);
                floatNumber = Constante.SCALE_ABILITY_BOARD;
            }
            // Si c'est une capacité dans la pile d'action
            else if (GetComponent<AbilityDisplay>().abilityStatus == AbilityStatus.Action) {
                floatNumber = Constante.SCALE_ABILITY_ACTION;
            }
            // Si c'est une capacité dans le fenêtre de team
            else if (GetComponent<AbilityDisplay>().abilityStatus == AbilityStatus.TeamLayout) {
                GetComponent<AbilityDisplay>().ShowHideTooltip(false);
                floatNumber = Constante.SCALE_ABILITY_TEAM;
            }
              
            transform.localScale = Constante.ScaleComparedParent(floatNumber, transform.parent.gameObject);
            transform.localPosition = positionCached;
        }
        // Affichage du tooltip de la capacité dans le fenêtre de team
        else
            GetComponent<AbilityLayoutTeamDisplay>().GO_Tooltip.SetActive(false);
    }
}
