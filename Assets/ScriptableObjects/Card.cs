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
    public Type type;
    public TargetType[] targetType;
    public From from;
    public CardEffect[] effects;

    public int sbirePowerPoint = -1;
    public int sbireHealthPoint = -1;
    public SbirePassifEffect[] sbirePassifEffects;


    public void activeEffect(GameObject target) {
        foreach(var effect in effects) {
            effect.ExecuteEffect(target, this);
        }
    }

    public void disableEffect() {
        foreach (var effect in effects) {
            effect.DisableEffect();
        }
    }
}