using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Direct Damage Effect", menuName = "ScriptableObjects/Effects/Direct damage effect")]
public class DirectDamageEffect : CardEffect
{
    public int damageAmount;

    public override void ExecuteEffect(GameObject target, ElementalAffinity elementalAffinity, MonsterDisplay attacker) {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        int calculateDamage = 0;
        // Si la cible est un monstre 
        if (target.GetComponent<MonsterDisplay>() != null) {
            // Un monstre adverse
            if (target.GetComponent<MonsterDisplay>().ownedByOppo) {
                calculateDamage = GameManager.calculateDamage(GameManager.GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>(), elementalAffinity, damageAmount, attacker);
            } 
            // Un monstre allié
            else {
                calculateDamage = GameManager.calculateDamage(GameManager.GO_MonsterInvoked.GetComponent<MonsterDisplay>(), elementalAffinity, damageAmount, attacker);
            }
            target.GetComponent<MonsterDisplay>().takeDamage(calculateDamage);
        } 
        // Si la cible est un sbire
        else if (target.GetComponent<CardDisplay>() != null) {
            // Un sbire adverse
            //if (target.GetComponent<CardDisplay>().monsterOwnThis.ownedByOppo) {
            if (target.GetComponent<OwnedByOppo>().monsterOwnThis.ownedByOppo) {
                calculateDamage = GameManager.calculateDamage(GameManager.GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>(), elementalAffinity, damageAmount, attacker);
            }
            // Un sbire allié
            else {
                calculateDamage = GameManager.calculateDamage(GameManager.GO_MonsterInvoked.GetComponent<MonsterDisplay>(), elementalAffinity, damageAmount, attacker);
            }
            target.GetComponent<SbireDisplay>().takeDamage(calculateDamage);
        }
    }

    public override void DisableEffect(MonsterDisplay monsterDisplay) {
        throw new System.NotImplementedException();
    }
}
