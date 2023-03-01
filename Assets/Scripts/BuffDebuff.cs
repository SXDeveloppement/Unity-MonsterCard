using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuffDebuff : MonoBehaviour
{
    public GameObject targetMonster;
    public BuffDebuffType buffDebuffType;
    public int amount;
    public int turn;

    public void addTurn(int addNumberTurn) {
        this.turn += addNumberTurn;

        if (this.turn <= 0) {
            applyRemove(false, true);
        }
    }

    public void applyRemove(bool isApplied = true, bool refresh = true) {
        int finalAmount;
        if (isApplied) {
            finalAmount = this.amount;
        } else {
            finalAmount = - this.amount;
        }

        switch (this.buffDebuffType) {
            case BuffDebuffType.Power:
                this.targetMonster.GetComponent<MonsterDisplay>().buffPower += finalAmount;
                break;
            case BuffDebuffType.Guard:
                this.targetMonster.GetComponent<MonsterDisplay>().buffGuard += finalAmount;
                break;
            case BuffDebuffType.Speed:
                this.targetMonster.GetComponent<MonsterDisplay>().buffSpeed += finalAmount;
                break;
            case BuffDebuffType.Mana:
                this.targetMonster.GetComponent<MonsterDisplay>().buffMana += finalAmount;
                break;
            case BuffDebuffType.DamageRaw:
                this.targetMonster.GetComponent<MonsterDisplay>().buffDamageRaw += finalAmount;
                break;
            case BuffDebuffType.DamagePercent:
                this.targetMonster.GetComponent<MonsterDisplay>().buffDamagePercent += finalAmount;
                break;
            default:
                Debug.Log("Buff / debuff type not found");
                break;
        }

        if (!isApplied) {
            this.targetMonster.GetComponent<MonsterDisplay>().removeBuffDebuff(this, refresh);
        }

    }

    public override string ToString() {
        return "[" + buffDebuffType.ToString() + "," + amount.ToString() + "," + turn.ToString() + "]";
    }
}
