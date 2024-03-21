using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TowerControl))]
public class TowerStat : Stat
{
    [SerializeField] protected string _towerType = "StandardTower";
    [SerializeField] protected int _level;
    [SerializeField] protected float _bulletSpeed;

    public string TowerType { get { return _towerType; } set { _towerType = value; } }
    public int Level { get { return _level; } set { _level = value; } }
    public float BulletSpeed { get { return _bulletSpeed; } set { _bulletSpeed = value; } }

    public void IsStandard()
    {
        _level = 1;
        _maxHp = 100f;
        _hp = _maxHp;
        _bulletDamage = 50f;
        _bulletSpeed = 50f;
        _range = 15f;
        _fireRate = 1f;
        _fireCountdown = 0f;
        _exp = 1;
    }
}
