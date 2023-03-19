using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "ScriptableObjects/Card")]
public class Card : ScriptableObject {
    public new string name;
    [TextArea(3,4)]
    public string description;
    public Sprite artwork;
    public int manaCost;
    public int priority;
    public ElementalAffinity elementalAffinity;
    public CardType type;
    public TargetType[] targetType;
    public IsFrom from;
    public CardEffect[] effects;

    public int sbirePowerPoint = -1;
    public int sbireHealthPoint = -1;
    public SbirePassifEffect[] sbirePassifEffects;


    public void activeEffect(GameObject target, MonsterDisplay attacker) {
        foreach(var effect in effects) {
            effect.ExecuteEffect(target, elementalAffinity, attacker);
        }
    }

    public void disableEffect(MonsterDisplay monsterDisplay) {
        foreach (var effect in effects) {
            effect.DisableEffect(monsterDisplay);
        }
    }
}