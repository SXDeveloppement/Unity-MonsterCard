using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AuraDisplay : MonoBehaviour
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

            // Si l'emplacement est vide et que la carte est dans la main ou dans la zone de contre attaque
            if (cardOnSlot == null && (cardPlayed.GetComponent<CardDisplay>().status == CardStatus.Hand || cardPlayed.GetComponent<CardDisplay>().status == CardStatus.SlotHidden)) {
                // On vérifie les conditions de ciblage pour pouvoir placer la carte
                if (cardPlayed.GetComponent<CardDisplay>().targetIsAllowed(targetSlot)) {
                    if (gameManager.tryToPutOnBoard(cardPlayed, targetSlot, true)) {
                        //// On place la carte sur le terrain
                        //cardPlayed.GetComponent<CardDisplay>().putOnBoard(targetSlot, true);
                        //// On lie la carte joué a l'emplacement
                        //cardOnSlot = cardPlayed;
                        //// On active la carte
                        //cardPlayed.GetComponent<CardDisplay>().activeCard(gameObject);
                        
                        isPutOnBoard = true;
                    }
                } else {
                    Debug.Log("ERR : bad target [" + targetSlot.name + "] / ownByOppo = " + ownedByOppo.ToString());
                }
            }
        }

        return isPutOnBoard;
    }
}
