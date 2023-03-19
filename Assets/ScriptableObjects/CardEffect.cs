using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    public abstract void ExecuteEffect(GameObject target, ElementalAffinity elementalAffinity, MonsterDisplay attacker);

    public abstract void DisableEffect(MonsterDisplay monsterDisplay);
}
