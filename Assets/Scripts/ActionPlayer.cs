using UnityEngine;
using UnityEditor;

public class ActionPlayer : MonoBehaviour {
    public GameObject actionPlayed; // L'action qui a été joué
    public GameObject target; // La cible de l'action
    public bool skip; // action de passer
    public bool swap; // action de swap

    private int skipPriority = 0;
    private int swapPriority = 4;
    private int abilityPriority = 3;
    private int counterAttackSlotPriority = 1;
    private int sbireAttackPriority = 0;

    public static ActionPlayer ActionPlayerCreate(GameObject actionPlayed, GameObject target, bool skip = false, bool swap = false) {
        GameObject GO_ActionPlayer = new GameObject {name = "ActionPlayer"};
        ActionPlayer actionPlayer = GO_ActionPlayer.AddComponent<ActionPlayer>();
        actionPlayer.actionPlayed = actionPlayed;
        actionPlayer.target = target;
        actionPlayer.skip = skip;
        actionPlayer.swap = swap;

        return actionPlayer;
    }

    public void Active() {
        // Passer

        // Swap de monstre

        // Carte (cast)
        // ---> qui cible un monstre
        // ---> qui cible un sbire
        // ---> qui cible une carte
        // ---> qui cible un slot
        // --->---> Counter attack
        // --->--->---> Slot visible
        // --->--->---> Slot hidden (pas un cast)
        // --->---> Aura
        // --->---> Equipment

        // Sbire (attack)
        // ---> qui cible un monstre
        // ---> qui cible un sbire

        // Capacité (activation)
        // ---> qui cible un monstre
        // ---> qui cible un sbire
        // ---> qui cible une carte

        // Une Carte (non sbire du terrain)
        if (actionPlayed.GetComponent<CardDisplay>() != null
            && (actionPlayed.GetComponent<CardDisplay>().card.type != CardType.Sbire
            || actionPlayed.GetComponent<CardDisplay>().status != CardStatus.SlotVisible)) {
            // Qui cible un monstre
            if (target.GetComponent<MonsterDisplay>() != null) {
                Debug.Log("Active " + actionPlayed.name);
                actionPlayed.GetComponent<CardDisplay>().activeCard(target);
            }
            // Qui cible une carte
            else if (target.GetComponent<CardDisplay>() != null) {
                // Une carte Sbire
                if (target.GetComponent<CardDisplay>().card.type == CardType.Sbire) {
                    actionPlayed.GetComponent<CardDisplay>().activeCard(target);
                }
                // Une carte Aura
                else if (target.GetComponent<CardDisplay>().card.type == CardType.Aura) {
                    actionPlayed.GetComponent<CardDisplay>().activeCard(target);
                }
                // Une carte Enchantement
                else if (target.GetComponent<CardDisplay>().card.type == CardType.Enchantment) {
                    actionPlayed.GetComponent<CardDisplay>().activeCard(target);
                }
                // Une carte Echo
                else if (target.GetComponent<CardDisplay>().card.type == CardType.Echo) {
                    actionPlayed.GetComponent<CardDisplay>().activeCard(target);
                }
            }
            // Qui cible un emplacement d'aura
            else if (target.GetComponent<AuraDisplay>() != null) {
                // On place la carte sur le terrain
                actionPlayed.GetComponent<CardDisplay>().putOnBoard(target, true);
                // On lie la carte joué a l'emplacement
                target.GetComponent<AuraDisplay>().cardOnSlot = actionPlayed;
                // On active la carte
                actionPlayed.GetComponent<CardDisplay>().activeCard(target);
            }
            // Qui cible un emplacement d'equipement
            else if (target.GetComponent<EquipmentDisplay>() != null) {
                // On place la carte sur le terrain
                actionPlayed.GetComponent<CardDisplay>().putOnBoard(target, true);
                // On stock le GO de la carte dans MonsterDisplay
                GameManager.GO_MonsterInvoked.GetComponent<MonsterDisplay>().cardEnchantments[target.GetComponent<EquipmentDisplay>().slotId] = actionPlayed.GetComponent<CardDisplay>().card;
                actionPlayed.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
                actionPlayed.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
                actionPlayed.transform.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                // On lie la carte joué a l'emplacement
                target.GetComponent<EquipmentDisplay>().cardOnSlot = actionPlayed;
                // On lie l'equipement a la carte
                actionPlayed.GetComponent<ZoomCard2D>().equipment = target;
                // On active la carte
                actionPlayed.GetComponent<CardDisplay>().activeCard(target);
            }
            // Qui cible un emplacement de contre attaque
            else if (target.GetComponent<SlotDisplay>() != null) {
                // Cast la carte face visible
                //---> Sbire
                if (actionPlayed.GetComponent<CardDisplay>().card.type == CardType.Sbire) {
                    // On place la carte sur le terrain
                    actionPlayed.GetComponent<CardDisplay>().putOnBoard(target, true);
                }
                //---> Echo
                else if (actionPlayed.GetComponent<CardDisplay>().card.type == CardType.Echo) {
                    // On place la carte sur le terrain
                    actionPlayed.GetComponent<CardDisplay>().putOnBoard(target, true);
                }
                // Place la carte face caché
                //---> Spell
                else if (actionPlayed.GetComponent<CardDisplay>().card.type == CardType.Spell) {
                    // On place la carte sur le terrain
                    actionPlayed.GetComponent<CardDisplay>().putOnBoard(target, false);
                }
                //---> Aura
                else if (actionPlayed.GetComponent<CardDisplay>().card.type == CardType.Aura) {
                    // On place la carte sur le terrain
                    actionPlayed.GetComponent<CardDisplay>().putOnBoard(target, false);
                }
                //---> Enchantment
                else if (actionPlayed.GetComponent<CardDisplay>().card.type == CardType.Enchantment) {
                    // On place la carte sur le terrain
                    actionPlayed.GetComponent<CardDisplay>().putOnBoard(target, false);
                }
                //---> Counter attack
                else if (actionPlayed.GetComponent<CardDisplay>().card.type == CardType.CounterAttack) {
                    // On place la carte sur le terrain
                    actionPlayed.GetComponent<CardDisplay>().putOnBoard(target, false);
                }
            }
        }
        // Un sbire sur le terrain
        else if (actionPlayed.GetComponent<CardDisplay>() != null && actionPlayed.GetComponent<CardDisplay>().card.type == CardType.Sbire && actionPlayed.GetComponent<CardDisplay>().status == CardStatus.SlotVisible) {
            // Qui cible un monstre
            if (target.GetComponent<MonsterDisplay>() != null) {
                StartCoroutine(actionPlayed.GetComponent<SbireDisplay>().fightMonster(target.GetComponent<MonsterDisplay>()));
            }
            // Qui cible une carte sbire sur le terrain
            else if (target.GetComponent<CardDisplay>() != null && target.GetComponent<CardDisplay>().card.type == CardType.Sbire && target.GetComponent<CardDisplay>().status == CardStatus.SlotVisible) {
                StartCoroutine(actionPlayed.GetComponent<SbireDisplay>().fightVersus(target.GetComponent<SbireDisplay>()));
            }
        }
        // Une capacité
        else if (actionPlayed.GetComponent<AbilityDisplay>() != null) {
            // Qui cible un monstre
            if (target.GetComponent<MonsterDisplay>() != null) {
                actionPlayed.GetComponent<AbilityDisplay>().activeAbility(target);
            }
            // Qui cible une carte
            else if (target.GetComponent<CardDisplay>() != null) {
                actionPlayed.GetComponent<AbilityDisplay>().activeAbility(target);
            }
        }

        // On détruit le GO
        Destroy(gameObject);
    }

    /// <summary>
    /// Renvoi le rang de priorité de l'action
    /// </summary>
    /// <returns></returns>
    public int CalculatePriority() {
        int priority = 0;

        // Passe / Prio 0
        if (skip) {
            priority = 10000 * skipPriority;
        }
        // Swap / Prio 4
        else if (swap) {
            priority = 10000 * swapPriority;
        }
        // Capacité / Prio 3
        else if (actionPlayed.GetComponent<AbilityDisplay>() != null) {
            //priority = 10000 * abilityPriority + actionPlayed.GetComponent<AbilityDisplay>().monsterOwnThis.totalSpeed();
            priority = 10000 * abilityPriority + actionPlayed.GetComponent<OwnedByOppo>().monsterOwnThis.totalSpeed();
        }
        // C'est une carte
        else if (actionPlayed.GetComponent<CardDisplay>() != null) {
            CardDisplay cardDisplay = actionPlayed.GetComponent<CardDisplay>();
            // La cible n'est pas un emplacement de contre attaque, jouer une carte si elle provient de la main, en face caché, ou une carte echo sur le terrain
            if (target.GetComponent<SlotDisplay>() == null
                && (cardDisplay.status == CardStatus.Hand 
                || cardDisplay.status == CardStatus.SlotHidden
                || cardDisplay.status == CardStatus.SlotVisible && cardDisplay.card.type == CardType.Echo)) {
                //priority = 10000 * cardDisplay.card.priority + cardDisplay.monsterOwnThis.totalSpeed();
                priority = 10000 * cardDisplay.card.priority + cardDisplay.GetComponent<OwnedByOppo>().monsterOwnThis.totalSpeed();
            }
            // Poser une carte de la main sur un emplacement de contre attaque (face caché, contre attaque, invoque un sbire, pose une carte echo)
            else if (target.GetComponent<SlotDisplay>() != null && cardDisplay.status == CardStatus.Hand) {
                priority = 10000 * counterAttackSlotPriority + cardDisplay.GetComponent<OwnedByOppo>().monsterOwnThis.totalSpeed();
            }
            // Une attaque de sbire
            else if (cardDisplay.status == CardStatus.SlotVisible && cardDisplay.card.type == CardType.Sbire) {
                priority = 10000 * sbireAttackPriority + cardDisplay.GetComponent<OwnedByOppo>().monsterOwnThis.totalSpeed();
            }
        }

        return priority;
    }
}