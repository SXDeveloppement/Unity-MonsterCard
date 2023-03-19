using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Destroy Enchantment", menuName = "ScriptableObjects/Effects/Destroy enchantment")]
public class DestroyEnchantment : CardEffect
{
    public override void ExecuteEffect(GameObject target, ElementalAffinity elementalAffinity, MonsterDisplay attacker) {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // On efface l'enchantement de la liste d'enchantement du monstre
        int index = target.transform.parent.parent.GetSiblingIndex();
        target.GetComponent<CardDisplay>().monsterOwnThis.cardEnchantments[index] = ScriptableObject.CreateInstance<Card>();

        //gameManager.inGrave(target);
        target.GetComponent<CardDisplay>().disableCard();
    }

    public override void DisableEffect(MonsterDisplay monsterDisplay) {
        throw new System.NotImplementedException();
    }
}
