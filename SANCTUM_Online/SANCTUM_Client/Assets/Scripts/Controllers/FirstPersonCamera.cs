using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public static float mouseSensitivitiy = 100f;

    public Transform parentBody;

    float xRotation = 0f;

    //Turret turretData;
    TowerControl towerControl;

    GameObject mainCamera;

    void OnEnable()
    {
        Managers.Game.isFPM = true;
        mainCamera = Camera.main.gameObject;
        mainCamera.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        towerControl = GetComponentInParent<TowerControl>();

    }

    void Update()
    {
        if (!Managers.Game.isLive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            // 코루틴을 다른 스크립트에서 쓸때도 StartCoroutine() 써줘야 함
            ExitFirstPersonMode();
            StartCoroutine(Managers.Game.WaitForItemSelection());
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivitiy * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivitiy * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // 위아래 각도 제한

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        parentBody.Rotate(Vector3.up * mouseX);

        if (Input.GetMouseButtonDown(0))  // 마우스 좌클릭을 감지
        {
            FireBullet();
        }
    }


    void FireBullet()
    {
        Managers.Sound.Play("Effects/Arrow", Define.Sound.Effect);
        GameObject bulletGO = Managers.Resource.Instantiate("Tower/Prefab/Bullet/StandardBullet", transform.position, transform.rotation);
        bulletGO.transform.GetChild(0).gameObject.SetActive(false);
        //bulletGO.transform.SetPositionAndRotation(transform.position, transform.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        bullet.isFPM = true;
        bullet.damage = towerControl._stat.BulletDamage * 1.5f;
        float bulletForce = towerControl._stat.BulletSpeed * 1.5f;

        bullet.firePoint = towerControl.transform.position;
        bullet.range = towerControl._stat.Range;

        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        // 총알에 힘을 가해 발사
        bulletRigidbody.velocity = transform.forward * bulletForce;
    }

    public void ExitFirstPersonMode()
    {
        towerControl.isFPM = false;

        /* (Managers.UI.getPopStackTop().name == "FPSUI")
        {
            return;
        }*/

        Managers.Game.isFPM = false;

        Managers.UI.ClosePopupUI();
        Managers.Game.invenUI.SetActive(true);
        //Managers.UI.ShowPopupUI<UI_Inven>("InvenUI");

        Cursor.lockState = CursorLockMode.None;
        mainCamera.SetActive(true);
        gameObject.SetActive(false);
    }
}
