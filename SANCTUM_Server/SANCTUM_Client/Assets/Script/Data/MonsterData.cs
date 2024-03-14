using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "ScriptableObject/MonsterData")]
public class MonsterData : ScriptableObject
{
    public enum MonsterType { General, Attack, Boss }

    [Header("# Main Info")]
    public MonsterType monsterType;
    public int MonsterId;
    //public string MonsterName;

    [Header("# basic stat")]
    public float startHealth;
    public float startSpeed;

    [Header("# Attack")]
    public float damage;
    public float fireRate;

    [Header("# Exp")]
    public int exp;

    //[Header("# Monster Prefab")]
    //public GameObject monsterPrefab;

}
