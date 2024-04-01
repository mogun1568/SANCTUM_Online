using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType { Tower, Element, TowerOnlyItem, WorldOnlyItem }

    [Header("# Main Info")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    public Sprite itemIcon;

    [Header("# Tower Upgrade Data")]
    public float damage;
    public float range;
    public float fireRate;

    [Header("# World Upgrade Data")]
    public float Life;

    [Header("# Return Exp")]
    public int returnExp;

    [Header("# Bullet")]
    public int bulletIndex;

    //[Header("# Tower Prefab")]
    //public GameObject towerPrefab;

    //[Header("# Element Prefabs")]
    //public GameObject[] elementPrefabs;
}