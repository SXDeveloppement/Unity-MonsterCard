using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buff Effect", menuName = "ScriptableObjects/Effects/Buff effect")]
public class BuffEffect : CardEffect
{
    public BuffDebuffType buffDebuffType;
    public int amount;
    public int turnAmount;

    public override void ExecuteEffect(GameObject target, ElementalAffinity elementalAffinity) {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        target.GetComponent<MonsterDisplay>().addBuffDebuff(buffDebuffType, amount, turnAmount);
        //if (amount < 0) {
        //    gameManager.GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>().addBuffDebuff(buffDebuffType, amount, turnAmount);
        //} else {
        //    gameManager.GO_MonsterInvoked.GetComponent<MonsterDisplay>().addBuffDebuff(buffDebuffType, amount, turnAmount);
        //}
    }

    public override void DisableEffect() {
        throw new System.NotImplementedException();
    }
}
