using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
   Define.CameraMode _mode = Define.CameraMode.QuarterView;

    [SerializeField]
    Vector3 pos, move, dragOrigin;  // dragOrigin: �巡�� ���� ��ġ ���� ����

    [SerializeField]
    float moveSpeed = 30f, scrollSpeed = 5f, minY = 15f, maxY = 80f;

    void Start()
    {

    }

    void LateUpdate()
    {
        if (Managers.Game.GameIsOver)
        {
            this.enabled = false;
            return;
        }

        if (!Managers.Game.isLive)
        {
            return;
        }


        if (_mode == Define.CameraMode.QuarterView)
        {
            // ���콺 ���� ��ư�� ������ ���� �巡�� ���� ��ġ�� �����մϴ�.
            if (Input.GetMouseButtonDown(1))
            {
                dragOrigin = Input.mousePosition;
            }

            // ���콺 ���� ��ư�� ������ �ִ� ���� �巡���� �Ÿ���ŭ ī�޶� �̵��մϴ�.
            if (Input.GetMouseButton(1))
            {
                pos = Camera.main.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
                move = Quaternion.Euler(60f, 45f, 0f) * new Vector3(pos.x * moveSpeed, 0, pos.y * moveSpeed);

                // ī�޶��� y ��ġ�� ������Ű�� ���� yPosition ������ ����մϴ�.
                move.y = 0;

                transform.Translate(move, Space.World);
                dragOrigin = Input.mousePosition;
            }

            pos = transform.position;

            float scroll = Input.GetAxis("Mouse ScrollWheel");

            // �հ� �ڵ尡 ������
            pos += Camera.main.transform.forward * (scroll * 300 * scrollSpeed * Time.deltaTime);

            if (pos.y <= minY && scroll > 0)
            {
                while (pos.y <= minY)
                {
                    pos -= Camera.main.transform.forward * (scroll * Time.deltaTime);
                }
            }
            if (pos.y >= maxY && scroll < 0)
            {
                while (pos.y >= maxY)
                {
                    pos -= Camera.main.transform.forward * (scroll * Time.deltaTime);
                }
            }

            //pos.x = Mathf.Clamp(pos.x, 0, 100);
            //pos.z = Mathf.Clamp(pos.z, -100, 0);

            transform.position = pos;
        }
    }

    public void SetQuarterView()   // Vector3 delta
    {
        _mode = Define.CameraMode.QuarterView;
    }
}