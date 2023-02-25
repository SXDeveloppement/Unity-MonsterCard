using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "ScriptableObjects/Equipment")]
public class Equipment : ScriptableObject 
{
    public new string name;
    public Sprite artwork;
    public int healthPoint;
    public int powerPoint;
    public int guardPoint;
    public int speedPoint;
}
