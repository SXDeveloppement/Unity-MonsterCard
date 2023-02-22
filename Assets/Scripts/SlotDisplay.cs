using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotDisplay : MonoBehaviour, IDropHandler
{
    public GameObject GO_Slot1;
    public GameObject GO_Slot2;
    public GameObject GO_Slot3;
    public GameObject GO_Slot4;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void IDropHandler.OnDrop(PointerEventData eventData) {
        GameObject cardPlayed = eventData.pointerDrag;
        GameObject targetSlot = eventData.pointerCurrentRaycast.gameObject;

        // Si l'emplacement est vide
        if (targetSlot.transform.childCount == 0) {
            // On vérifie les conditions de ciblage pour pouvoir placer la carte
            TargetType[] cardPlayedTargetType = cardPlayed.GetComponent<CardDisplay>().card.targetType;
            foreach (TargetType targetType in cardPlayedTargetType) {
                if (targetType == TargetType.SlotHidden || targetType == TargetType.SlotVisible) {
                    if (targetType == TargetType.SlotHidden) {
                        cardPlayed.GetComponent<CardDisplay>().showHiddenFace();
                    }

                    if (targetType == TargetType.SlotVisible) {
                        cardPlayed.GetComponent<CardDisplay>().showVisibleFace();
                    }

                    cardPlayed.transform.SetParent(targetSlot.transform);
                    cardPlayed.GetComponent<CardDisplay>().status = Status.Board;
                    cardPlayed.GetComponent<ZoomCard>().reinitCard();
                }
            }
        }
    }
}
