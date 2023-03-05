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

    public bool ownedByOppo;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void IDropHandler.OnDrop(PointerEventData eventData) {
        if (gameManager.dragged) {
            GameObject cardPlayed = eventData.pointerDrag;
            GameObject targetSlot = eventData.pointerCurrentRaycast.gameObject;

            // On vérifie que la cible soit bien un emplacement de contre attaque
            bool isSlot = false;
            if (targetSlot == GO_Slot1 || targetSlot == GO_Slot2 || targetSlot == GO_Slot3 || targetSlot == GO_Slot4) {
                isSlot = true;
            }

            // Si l'emplacement est vide et que la carte est dans la main
            if (isSlot && targetSlot.transform.childCount == 0 && cardPlayed.GetComponent<CardDisplay>().status == Status.Hand) {
                // On vérifie les conditions de ciblage pour pouvoir placer la carte
                bool targetCondition = false;
                TargetType[] cardPlayedTargetType = cardPlayed.GetComponent<CardDisplay>().card.targetType;
                foreach (TargetType targetType in cardPlayedTargetType) {
                    if (!ownedByOppo && (targetType == TargetType.SlotHidden || targetType == TargetType.SlotVisible)) {

                        if (targetType == TargetType.SlotHidden) {
                            gameManager.tryToPutOnBoard(cardPlayed, targetSlot, false);
                        } else {
                            gameManager.tryToPutOnBoard(cardPlayed, targetSlot, true);
                        }

                        targetCondition = true;
                        break;
                    }
                }

                // On place la carte si les conditions de ciblages sont respectées
                if (!targetCondition) {
                    Debug.Log("ERR : bad target [" + targetSlot.name + "] / ownByOppo = " + ownedByOppo.ToString());
                }
            }
        }
    }
}
