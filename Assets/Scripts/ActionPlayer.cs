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

        // Carte qui cible un monstre
        if (actionPlayed.GetComponent<CardDisplay>() != null) {
            if (target.GetComponent<MonsterDisplay>() != null) {
                Debug.Log("Active " + actionPlayed.name);
                actionPlayed.GetComponent<CardDisplay>().activeCard(target);
            }
        }
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