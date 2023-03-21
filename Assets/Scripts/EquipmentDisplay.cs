using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class EquipmentDisplay : MonoBehaviour 
{
    public Equipment equipment;
    public TMP_Text nameText;
    public TMP_Text powerText;
    public TMP_Text guardText;
    public TMP_Text speedText;
    public TMP_Text hpText;
    public Image artworkImage;
    public SpriteRenderer illustration;

    public int slotId;
    //public MonsterDisplay monsterOwnThis;
    public GameObject slotCard;
    public GameObject cardOnSlot;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        slotCard = transform.parent.parent.GetChild(1).gameObject;

        nameText.text = equipment.name;
        powerText.text = equipment.powerPoint.ToString();
        guardText.text = equipment.guardPoint.ToString();
        speedText.text = equipment.speedPoint.ToString();
        hpText.text = equipment.healthPoint.ToString();
        if (artworkImage != null)
            artworkImage.sprite = equipment.artwork;
        if (illustration != null)
            illustration.sprite = equipment.artwork;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool onDrop(GameObject cardPlayed) {
        bool isPutOnBoard = false;
        if (GameManager.dragged) {
            GameObject targetSlot = this.gameObject;

            // Si l'emplacement est vide et que la carte dans la main ou dans la zone de contre attaque
            if (cardOnSlot == null && (cardPlayed.GetComponent<CardDisplay>().status == CardStatus.Hand || cardPlayed.GetComponent<CardDisplay>().status == CardStatus.SlotHidden)) {
                // On vérifie les conditions de ciblage pour pouvoir placer la carte
                if (cardPlayed.GetComponent<CardDisplay>().targetIsAllowed(targetSlot)) {
                    if (gameManager.tryToPutOnBoard(cardPlayed, targetSlot, true)) {
                        cardOnSlot = cardPlayed;
                        cardPlayed.GetComponent<CardDisplay>().activeCard(gameObject);
                        isPutOnBoard = true;
                    }
                } else {
                    Debug.Log("ERR : bad target [" + targetSlot.name + "] / ownByOppo = " + GetComponent<OwnedByOppo>().monsterOwnThis.ownedByOppo.ToString());
                }
            }
        }

        return isPutOnBoard;
    }

    // On détruit l'enchantement de l'équipement
    public void destroyEnchantment() {
        Destroy(cardOnSlot);
        cardOnSlot = null;
    }
}
