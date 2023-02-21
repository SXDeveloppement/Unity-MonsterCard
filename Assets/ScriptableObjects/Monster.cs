using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster", menuName = "ScriptableObjects/Monster")]
public class Monster : ScriptableObject
{
    public new string name;
    public Sprite artwork;
    public int healthPoint;
    public int powerPoint;
    public int guardPoint;
    public int speedPoint;
    public ElementalAffinity[] elementalAffinity;
}
