using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyControl))]
public class EnemyStat : Stat
{
    [SerializeField] public static float _addHp;
    [SerializeField] public static float _bossAddHp;
    [SerializeField] protected float _startSpeed;
    [SerializeField] protected float _speed;
    [SerializeField] protected float _lifeLost;

    public static float AddHp { get { return _addHp; } set { _addHp = value; } }
    public static float BossAddHp { get { return _bossAddHp; } set { _bossAddHp = value; } }
    public float StartSpeed { get { return _startSpeed; } set { _startSpeed = value; } }
    public float Speed { get { return _speed; } set { _speed = value; } }
    public float LifeLost { get { return _lifeLost; } set { _lifeLost = value; } }

    void OnEnable()
    {
        _maxHp = 100f + _addHp;
        _hp = _maxHp;
        _startSpeed = 8f;
        _speed = _startSpeed;
        _bulletDamage = 0;
        _range = 0;
        _fireRate = 0;
        _fireCountdown = 0f;
        _exp = 1;
        _lifeLost = 1;
    }

    public void IsAttack()
    {
        _startSpeed = 10f;
        _speed = _startSpeed;
        _bulletDamage = 10f;
        _range = 12f;
        _fireRate = 1f;
        _lifeLost = 2;
    }

    public void IsBoss()
    {
        _maxHp = 500f + _bossAddHp;
        _hp = _maxHp;
        _startSpeed = 8f;
        _speed = _startSpeed;
        _bulletDamage = 20f;
        _range = 12f;
        _fireRate = 1f;
        _exp = 3;
        _lifeLost = 5;
    }
}
