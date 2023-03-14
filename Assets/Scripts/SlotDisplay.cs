using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotDisplay : MonoBehaviour
{
    public GameObject slotCard;
    public GameObject cardOnSlot;
    
    public bool ownedByOppo;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindAnyObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool onDrop(GameObject cardPlayed) {
        bool isPutOnBoard = false;
        if (GameManager.dragged) {
            GameObject targetSlot = this.gameObject;

            // Si l'emplacement est vide et que la carte est dans la main
            if (cardOnSlot == null && cardPlayed.GetComponent<CardDisplay>().status == Status.Hand) {
                // On vérifie les conditions de ciblage pour pouvoir placer la carte
                bool targetCondition = false;
                TargetType[] cardPlayedTargetType = cardPlayed.GetComponent<CardDisplay>().card.targetType;
                foreach (TargetType targetType in cardPlayedTargetType) {
                    if (!ownedByOppo && (targetType == TargetType.SlotHidden || targetType == TargetType.SlotVisible)) {
                        if (targetType == TargetType.SlotHidden) {
                            isPutOnBoard = gameManager.tryToPutOnBoard(cardPlayed, targetSlot, false);
                        } else {
                            isPutOnBoard = gameManager.tryToPutOnBoard(cardPlayed, targetSlot, true);
                        }

                        targetCondition = true;
                        break;
                    }
                }

                // Message d'erreur si les conditions de ciblage ne sont pas bonnes
                if (!targetCondition) {
                    Debug.Log("ERR : bad target [" + targetSlot.name + "] / ownByOppo = " + ownedByOppo.ToString());
                }
            } else {
                Debug.Log("ERR : Slot not empty OR card not in hand");
            }
        }

        if (isPutOnBoard)
            cardOnSlot = cardPlayed;

        return isPutOnBoard;
    }
}
