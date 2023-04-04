using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public Card card;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text manaText;
    public TMP_Text priorityText;
    public TMP_Text typeText;
    public Image artworkImage;
    public SpriteRenderer illustration;
    public GameObject folderBackgrounds;
    public GameObject GO_MulliganSelection;

    public CardStatus status;
    public bool selectedForMulligan = false; // Choici pour être mulligan
    public bool hiddenCard;
    public bool putOnBoardThisTurn = true;
    public bool putOnBoardThisTurnTemp = true;
    public bool attackAura = false;
    public bool attackAuraTemp = false;

    //public MonsterDisplay monsterOwnThis; // GO du monstre qui possède cette carte

    private bool init = true;

    // Variable pour les animations
    //public AnimationCurve scaleCurve;
    //public float duration = 0.5f;

    private string cardDescriptionCached;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start() {
        gameManager = GameObject.FindAnyObjectByType<GameManager>();

        // Si card est défini
        if (card != null) {
            // On affiche le bon background en fonction du type élémentaire de la carte
            foreach (Transform childBackground in folderBackgrounds.transform) {
                childBackground.gameObject.SetActive(false);
                if (childBackground.gameObject.name == card.elementalAffinity.ToString()) {
                    childBackground.gameObject.SetActive(true);
                }
            }

            nameText.text = card.name;
            descriptionText.text = card.description;
            manaText.text = card.manaCost.ToString();
            priorityText.text = card.priority.ToString();
            typeText.text = card.type.ToString();
            if (artworkImage != null)
                artworkImage.sprite = card.artwork;
            if (illustration != null)
                illustration.sprite = card.artwork;

            cardDescriptionCached = card.description;
        }
    }

    // Update is called once per frame
    void Update() {
        if (init) {
            init = false;
            refreshDescriptionDamage();
        }

        // On affiche l'aura d'activation de la carte ECHO
        if (putOnBoardThisTurn != putOnBoardThisTurnTemp && card.type == CardType.Echo) {
            putOnBoardThisTurnTemp = putOnBoardThisTurn;
            //GetComponent<SbireDisplay>().attackAura.SetActive(!putOnBoardThisTurn);
            attackAura = true;
        }

        // On toggle l'attaque aura
        if (attackAura != attackAuraTemp) {
            attackAuraTemp = attackAura;
            GetComponent<SbireDisplay>().attackAura.SetActive(attackAura);
        }
    }

    // Action lors d'une activation qui cible cette carte
    public bool OnDrop(GameObject cardPlayed) {
        bool isPutOnBoard = false;

        if (GameManager.dragged) {
            GameObject target = gameObject;

            if (cardPlayed.GetComponent<CardDisplay>().targetIsAllowed(target)) {
                // Ciblage d'une aura ou d'un enchantement
                if (card.type == CardType.Aura || card.type == CardType.Enchantment) {
                    gameManager.activeCardOnTarget(cardPlayed, target);
                    isPutOnBoard = true;
                }
                // Ciblage d'un sbire
                else if (card.type == CardType.Sbire) {
                    // Par un spell
                    if (cardPlayed.GetComponent<CardDisplay>().card.type != CardType.Sbire) {
                        gameManager.activeCardOnTarget(cardPlayed, target);
                        isPutOnBoard = true;
                    }
                    // Par un autre sbire
                    else {
                        isPutOnBoard = true;
                        bool sbireHaveTaunt = false;
                        foreach (CardDisplay cardDisplay in gameManager.GO_CounterAttackAreaOppo.GetComponentsInChildren<CardDisplay>()) {
                            if (cardDisplay.card.type == CardType.Sbire) {
                                sbireHaveTaunt = cardDisplay.GetComponent<SbireDisplay>().haveTank();
                                if (sbireHaveTaunt)
                                    break;
                            }
                        }

                        if (!sbireHaveTaunt || GetComponent<SbireDisplay>().haveTank()) {
                            gameManager.AddAction(cardPlayed, target);
                            FindAnyObjectByType<GameManager>().AddActionRedirect(cardPlayed, target);
                            isPutOnBoard = true;
                        } else {
                            Debug.Log("ERR : Bad target, one sbire or more have Taunt");
                        }
                    }
                }
            } else {
                Debug.Log("ERR : bad target [" + target.name + "] / ownByOppo = " + GetComponent<OwnedByOppo>().monsterOwnThis.ownedByOppo.ToString());
            }
        }

        return isPutOnBoard;
    }

    /// <summary>
    /// Lorsque que l'on active une capacité qui cible cette carte
    /// </summary>
    /// <param name="abilityDisplay"></param>
    public bool OnDropAbility(AbilityDisplay abilityDisplay) {
        bool abilityIsDone = false;

        if (GameManager.dragged) {
            GameObject target = gameObject;

            if (abilityDisplay.loopTargetAllowed(target)) {
                GameManager.activeAbilityOnTarget(abilityDisplay, target);
                abilityIsDone = true;
            } else {
                Debug.Log("ERR : bad target [" + target.name + "] / ownByOppo = " + GetComponent<OwnedByOppo>().monsterOwnThis.ownedByOppo.ToString());
            }
        }

        return abilityIsDone;
    }

    /// <summary>
    /// Vérifie les conditions de ciblages
    /// </summary>
    /// <param name="target"></param>
    /// <returns>Boolean</returns>
    public bool targetIsAllowed(GameObject target) {
        // Dans la main
        if (status == CardStatus.Hand) {
            if (loopTargetAllowed(target))
                return true;
        }
        // Dans un emplacement de contre attaque
        else if (status == CardStatus.SlotHidden || status == CardStatus.SlotVisible) {
            // Que la cible n'est pas un autre emplacement de contre attaque
            if (target.GetComponent<SlotDisplay>() == null) {
                if (loopTargetAllowed(target))
                    return true;
            }
        }

        return false;
    }

    // Boucle de vérification de ciblage
    private bool loopTargetAllowed(GameObject target) {
        foreach (TargetType targetType in card.targetType) {
            TargetType targetType2 = targetType;
            if (targetType2 == TargetType.SlotHidden)
                targetType2 = TargetType.SlotVisible;
            
            if (targetType2 == GameManager.typeTarget(target))
                return true;
            // Si c'est un sbire sur le terrain qui attaque un autre sbire
            else if (status == CardStatus.SlotVisible && card.type == CardType.Sbire && GameManager.typeTarget(target) == TargetType.OpponantCardSbire)
                return true;
            // Si c'est un sbire sur le terrain qui attaque un monstre
            else if (status == CardStatus.SlotVisible && card.type == CardType.Sbire && GameManager.typeTarget(target) == TargetType.OpponantMonster)
                return true;
        }
        return false;
    }

    // On active les effets de la carte
    public void activeCard(GameObject target) {
        // Si la carte est dans un emplacement de contre attaque, on enlève le GO de la carte dans SlotDisplay de l'emplacement
        if ((status == CardStatus.SlotHidden || status == CardStatus.SlotVisible)
            && gameObject.transform.parent.parent.GetComponent<SlotDisplay>() != null) {
            gameObject.transform.parent.parent.GetComponent<SlotDisplay>().cardOnSlot = null;
        }

        card.activeEffect(target, GetComponent<OwnedByOppo>().monsterOwnThis);
        gameObject.GetComponent<ZoomCard2D>().destroyPlaceholder();

        // Si c'est une carte de Type.Spell ou Type.Echo
        if (card.type == CardType.Spell || card.type == CardType.Echo) {
            gameManager.inGrave(gameObject);
        }
    }

    // On desactive les effets de la carte (aura, enchantement)
    public void disableCard() {
        // Si la carte est dans un emplacement d'aura, on enlève le GO de la carte dans SlotDisplay de l'emplacement
        if (status == CardStatus.AuraSlot && gameObject.transform.parent.parent.GetComponent<AuraDisplay>() != null) {
            gameObject.transform.parent.parent.GetComponent<AuraDisplay>().cardOnSlot = null;
        }
        // Si la carte est dans un emplacement d'aura, on enlève le GO de la carte dans SlotDisplay de l'emplacement
        else if (status == CardStatus.EnchantmentSlot && gameObject.transform.parent.parent.GetComponentInChildren<EquipmentDisplay>() != null) {
            gameObject.transform.parent.parent.GetComponentInChildren<EquipmentDisplay>().cardOnSlot = null;
        }

        card.disableEffect(GetComponent<OwnedByOppo>().monsterOwnThis);
        gameManager.inGrave(gameObject);
    }

    // On place la carte face visible
    public void putOnBoard(GameObject target, bool isVisible) {
        if (isVisible) {
            showVisibleFace();
            hiddenCard = false;
        } else {
            showHiddenFace();
            hiddenCard = true;
        }

        // Si la carte est dans un emplacement de contre attaque, on enlève le GO de la carte dans SlotDisplay de l'emplacement
        if ((status == CardStatus.SlotHidden || status == CardStatus.SlotVisible)
            && gameObject.transform.parent.parent.GetComponent<SlotDisplay>() != null) {
            gameObject.transform.parent.parent.GetComponent<SlotDisplay>().cardOnSlot = null;
        }

        if (target.GetComponent<SlotDisplay>() != null) {
            if (hiddenCard) {
                status = CardStatus.SlotHidden;
            } else {
                status = CardStatus.SlotVisible;
            }
            transform.SetParent(target.GetComponent<SlotDisplay>().slotCard.transform);
            target.GetComponent<SlotDisplay>().cardOnSlot = this.gameObject;
        } else if (target.GetComponent<AuraDisplay>() != null) {
            
            status = CardStatus.AuraSlot;
            transform.SetParent(target.GetComponent<AuraDisplay>().slotCard.transform);
            target.GetComponent<AuraDisplay>().cardOnSlot = this.gameObject;
        } else if (target.GetComponent<EquipmentDisplay>() != null) {
            status = CardStatus.EnchantmentSlot;
            transform.SetParent(target.transform.parent.parent.GetChild(1));
        }

        ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        GetComponent<LayoutElement>().ignoreLayout = false;

        // Si c'est un sbire
        if (card.type == CardType.Sbire) {
            GetComponent<SbireDisplay>().invokeSbire();
        }
        
        GetComponent<ZoomCard2D>().reinitCard();
    }

    /// <summary>
    /// On replace la carte dans le deck du monstre qui la possède 
    /// </summary>
    public void ReturnInDeck() {
        GetComponent<OwnedByOppo>().monsterOwnThis.deckList.Add(card);
        Destroy(gameObject);
    }

    /// <summary>
    /// Carte en mode mulligan
    /// </summary>
    public void MulliganMode() {
        status = CardStatus.Mulligan;
        attackAura = true;
        transform.localScale = Vector3.one;
    }

    // On met a jour la description de la carte avec les dégâts qui seront réellement infligés au monstre adverse
    public void refreshDescriptionDamage() {
        if (card != null) {
            // Si c'est une carte sbire
            if (card.type == CardType.Sbire) {
                int sbireBasePower = card.sbirePowerPoint;
                int outputSbireDamage;
                if (!GetComponent<OwnedByOppo>().monsterOwnThis.ownedByOppo) {
                    outputSbireDamage = GameManager.calculateDamage(GameManager.GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>(), card.elementalAffinity, sbireBasePower, GameManager.GO_MonsterInvoked.GetComponent<MonsterDisplay>());
                } else {
                    outputSbireDamage = GameManager.calculateDamage(GameManager.GO_MonsterInvoked.GetComponent<MonsterDisplay>(), card.elementalAffinity, sbireBasePower, GameManager.GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>());
                }
                GetComponent<SbireDisplay>().sbirePowerAvailable = outputSbireDamage;
            }
            // Si c'est un sort
            else {
                if (GameManager.fullDamageIntegred(cardDescriptionCached, card.elementalAffinity, GetComponent<OwnedByOppo>().monsterOwnThis) != null) {
                    descriptionText.text = GameManager.fullDamageIntegred(cardDescriptionCached, card.elementalAffinity, GetComponent<OwnedByOppo>().monsterOwnThis);
                }
            }
        }
    }

    // On retourne la carte face caché
    public void showHiddenFace() {
        gameObject.transform.Find("Front").gameObject.SetActive(false);
        gameObject.transform.Find("Back").gameObject.SetActive(true);
    }

    // On retourne la carte face visible
    public void showVisibleFace() {
        gameObject.transform.Find("Front").gameObject.SetActive(true);
        gameObject.transform.Find("Back").gameObject.SetActive(false);
    }

    /// <summary>
    /// Affiche ou cache l'icon de sélection d'une carte lors d'un mulligan
    /// </summary>
    public void ToggleMulliganSelect() {
        if (GO_MulliganSelection.activeSelf) {
            GO_MulliganSelection.SetActive(false);
            GetComponent<SbireDisplay>().attackAura.SetActive(true);
            selectedForMulligan = false;
        } else {
            GO_MulliganSelection.SetActive(true);
            GetComponent<SbireDisplay>().attackAura.SetActive(false);
            selectedForMulligan = true;
        }
    }

    // Animation de retournement de carte face visibile
    //public IEnumerator flipFront() {
    //    float time = 0f;
    //    bool endLoop = false;
    //    while (!endLoop) {
    //        float scale;
    //        if (time >= 1f) {
    //            endLoop = true;
    //            scale = scaleCurve.Evaluate(1f);
    //        } else {
    //            scale = scaleCurve.Evaluate(time);
    //        }
            
    //        if (time < 0.5f) {
    //            Transform cardBack = transform.Find("Back");
    //            cardBack.localScale = new Vector3(scale, cardBack.localScale.y, cardBack.localScale.z);
    //        } else {
    //            showVisibleFace();
    //            Transform cardFront = transform.Find("Front");
    //            cardFront.localScale = new Vector3(scale, cardFront.localScale.y, cardFront.localScale.z);
    //        }           

    //        time = time + Time.deltaTime / duration;
    //        yield return new WaitForFixedUpdate();
    //    }
    //}

    // Animation de retournement de carte face visibile
    //public IEnumerator flipBack() {
    //    float time = 0f;
    //    bool endLoop = false;
    //    while (!endLoop) {
    //        float scale;
    //        if (time >= 1f) {
    //            endLoop = true;
    //            scale = scaleCurve.Evaluate(1f);
    //        } else {
    //            scale = scaleCurve.Evaluate(time);
    //        }

    //        if (time < 0.5f) {
    //            Transform cardFront = transform.Find("Front");
    //            cardFront.localScale = new Vector3(scale, cardFront.localScale.y, cardFront.localScale.z);
    //        } else {
    //            showHiddenFace();
    //            Transform cardBack = transform.Find("Back");
    //            cardBack.localScale = new Vector3(scale, cardBack.localScale.y, cardBack.localScale.z);
    //        }

    //        time = time + Time.deltaTime / duration;
    //        yield return new WaitForFixedUpdate();
    //    }
    //}
}
