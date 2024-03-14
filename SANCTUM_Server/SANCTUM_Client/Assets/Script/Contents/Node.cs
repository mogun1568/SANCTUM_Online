using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Node : MonoBehaviour
{
    [HideInInspector] public GameObject turret;

    int upgradedNum; // ���� ���� 3������ ����
    string element;    // ����� ����
    int countItem = 0;

    BuildManager buildManager;

    [HideInInspector] public bool enviroment;

    void OnMouseDown()
    {
        //if (turret != null)
        //{
        //    Managers.Select.SelectNode(this);
        //}

        if (Managers.Game.GameIsOver || !Managers.Game.isLive)
        {
            return;
        }

        Managers.Select.SelectNode(this, turret);
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position + new Vector3(0f, transform.localScale.y, 0f);
    }

    public void UseItem()
    {
        if (turret && !turret.activeSelf)
        {
            turret = null;
            countItem = 0;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (enviroment)
        {
            return;
        }

        Data.Item itemData = Managers.Select.getItemData();

        if (turret == null)
        {
            switch (itemData.itemType)
            {
                case "Tower":
                    BuildTurret();
                    break;
                default:
                    Debug.Log("You don't use item!");
                    break;
            }
        }
        else
        {
            switch (itemData.itemType)
            {
                case "Tower":
                    Debug.Log("There's already a tower here!");
                    break;
                case "Element":
                    ApplicateElement(itemData);
                    break;
                case "TowerOnlyItem":
                    UseTowerOnlyItem(itemData);
                    break;
            }
        }

        countItem += itemData.returnExp;
    }

    void BuildTurret()
    {
        Managers.Sound.Play("Effects/Build", Define.Sound.Effect);
        GameObject _turret = Managers.Resource.Instantiate("Tower/Prefab/BallistaTowerlvl02", GetBuildPosition(), Quaternion.identity);
        turret = _turret;

        PracticeEffect("Launch Smoke");

        element = "null";
        upgradedNum = 0;

        Debug.Log("Turret build!");
        Managers.Select.itemUITextDecrease();
    }

    void ApplicateElement(Data.Item itemData)
    {
        if (upgradedNum > 0 && element != itemData.itemName)
        {
            Debug.Log("already ues element!");
            return;
        }
        element = itemData.itemName;

        if (upgradedNum >= 3)
        {
            Debug.Log("Upgrade Done!");
            return;
        }

        Managers.Sound.Play("Effects/Build", Define.Sound.Effect);
        Debug.Log($"{itemData.itemName} Upgrade {upgradedNum} -> {upgradedNum + 1}");
        upgradedNum++;

        // ���� Ÿ�� ����
        GameObject _turret = Managers.Resource.Instantiate($"Tower/Prefab/{itemData.itemName}Tower/{itemData.itemName}Towerlvl0{upgradedNum.ToString()}", GetBuildPosition(), Quaternion.identity);

        // Ÿ�� ���� �̵�
        TowerStat curTowerStat = turret.GetComponent<TowerControl>()._stat;
        TowerStat newTowerStat = _turret.GetComponent<TowerControl>()._stat;

        newTowerStat.TowerType = element;
        newTowerStat.HP = curTowerStat.HP; // += 50 �������� �����
        newTowerStat.Range = curTowerStat.Range;
        newTowerStat.FireRate = curTowerStat.FireRate;
        newTowerStat.BulletDamage = curTowerStat.BulletDamage;
        newTowerStat.BulletSpeed = curTowerStat.BulletSpeed;

        // Ÿ�� ����
        Managers.Resource.Destroy(turret);
        turret = _turret;

        turret.GetComponent<TowerControl>().itemData = itemData;

        if (element == "Water")
        {
            Transform healEffect = turret.transform.GetChild(turret.transform.childCount - 1);
            healEffect.localScale = new Vector3(newTowerStat.Range * 2, healEffect.localScale.y, newTowerStat.Range * 2);
        }

        PracticeEffect("Launch Smoke");


        //Debug.Log("Applicate Element!");
        Managers.Select.itemUITextDecrease();
    }

    void UseTowerOnlyItem(Data.Item itemData)
    {
        Managers.Sound.Play("Effects/Soundiron_Shimmer_Charms_Short_07 [2023-06-13 121009]", Define.Sound.Effect);
        TowerStat towerStat = turret.GetComponent<TowerControl>()._stat;

        switch (itemData.itemName)
        {
            case "DamageUp":
                Debug.Log($"Damage Up {towerStat.BulletDamage} -> {towerStat.BulletDamage * itemData.upgradeAmount}");
                towerStat.BulletDamage *= itemData.upgradeAmount;
                break;
            case "RangeUp":
                Debug.Log($"Range Up {towerStat.Range} -> {towerStat.Range * itemData.upgradeAmount}");
                towerStat.Range *= itemData.upgradeAmount;

                if (element == "Water")
                {
                    Transform healEffect = turret.transform.GetChild(turret.transform.childCount - 1);
                    healEffect.localScale = new Vector3(towerStat.Range * 2, healEffect.localScale.y, towerStat.Range * 2);
                }
                break;
            case "FireRateUp":
                Debug.Log($"Range Up {towerStat.FireRate} -> {towerStat.FireRate * itemData.upgradeAmount}");
                towerStat.FireRate *= itemData.upgradeAmount;
                break;
        }
        Managers.Select.itemUITextDecrease();
    }

    void UseWolrdOnlyItem(ItemData data)
    {

    }

    public void FirstPersonMode()
    {
        Debug.Log("First Person Mode");
        TowerControl towerControl = turret.GetComponent<TowerControl>();
        towerControl.isFPM = true;

        // ��Ȱ��ȭ�� ������Ʈ�� �׳� GetComponent�� ��ã�� GetComponents<>(true)�� �迭�� ã�Ƽ� ����ؾ� ��
        turret.GetComponentsInChildren<Camera>(true)[0].gameObject.SetActive(true);

        Managers.Game.invenUI.SetActive(false);
        Managers.UI.ShowPopupUI<FPSUI>("FPSModeUI");
    }

    public void DemoliteTower()
    {
        // ���� exp += �� node�� returnExp; // �� �ڵ� �߰� ����
        int remainExp = Managers.Game.nextExp;

        for (int i = 1; i < countItem / 2; i++)
        {
            remainExp += Mathf.RoundToInt(remainExp * 1.5f);
            Debug.Log(remainExp);

            // 0/3 -> 0/5 -> 0/8
        }
        if (countItem % 2 != 0)
        {
            remainExp += remainExp / 2;
            Debug.Log(remainExp);

            // 4/8
        }
        Managers.Game.GetExp(remainExp);
        countItem = 0;

        Managers.Sound.Play("Effects/Explosion", Define.Sound.Effect);
        PracticeEffect("Void Explosion");

        // ���Ÿ� �� �� ö�ŵ� ������Ʈ�� ������ �� ��� ��
        Managers.Resource.Destroy(turret);
        turret = null;

        Debug.Log("Demolite Tower");
    }

    void PracticeEffect(string name)
    {
        GameObject effect = Managers.Resource.Instantiate($"Tower/Prefab/{name}", GetBuildPosition(), Quaternion.identity);
        StartCoroutine(DestroyEffect(effect));
    }

    IEnumerator DestroyEffect(GameObject effect)
    {
        yield return new WaitForSeconds(5f);

        Managers.Resource.Destroy(effect);
    }
}