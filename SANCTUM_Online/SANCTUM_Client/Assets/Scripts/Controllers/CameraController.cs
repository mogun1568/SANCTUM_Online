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
    float moveSpeed = 50f, scrollSpeed = 5f, minY = 15f, maxY = 80f;

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

        if (Managers.Game.isPopup)
        {
            return;
        }


        if (_mode == Define.CameraMode.QuarterView)
        {
            CameraMove2();

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            int cameraDefault = Managers.Object.MyMap.MapDefaultSize * 2 - 24;
            Managers.Game._mainCamera.transform.position = Managers.Object.MyMap.Pos + new Vector3(cameraDefault, 40, cameraDefault);
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (Managers.Object._players.Count < 2)
                return;

            int cameraDefault = Managers.Object._players[1].MapDefaultSize * 2 - 24;
            Managers.Game._mainCamera.transform.position = Managers.Object._players[1].Pos + new Vector3(cameraDefault, 40, cameraDefault);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (Managers.Object._players.Count < 3)
                return;

            int cameraDefault = Managers.Object._players[2].MapDefaultSize * 2 - 24;
            Managers.Game._mainCamera.transform.position = Managers.Object._players[2].Pos + new Vector3(cameraDefault, 40, cameraDefault);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (Managers.Object._players.Count < 4)
                return;

            int cameraDefault = Managers.Object._players[3].MapDefaultSize * 2 - 24;
            Managers.Game._mainCamera.transform.position = Managers.Object._players[3].Pos + new Vector3(cameraDefault, 40, cameraDefault);
        }
    }

    void CameraMove1()
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
    }

    void CameraMove2()
    {
        float moveHorizontal = Input.GetAxis("Horizontal"); // A, D Ű �Է�
        float moveVertical = Input.GetAxis("Vertical"); // W, S Ű �Է�

        move = Quaternion.Euler(60f, 45f, 0f) * new Vector3(moveHorizontal, 0.0f, moveVertical * 2);

        move.y = 0;

        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }

    void CameraMove3()
    {
        float edgeThickness = 10f;  // ȭ�� �����ڸ� �β�

        Vector3 moveDirection = Vector3.zero;

        // ȭ���� �����ڸ��� �����Ͽ� �̵� ���� ����
        if (Input.mousePosition.x >= Screen.width - edgeThickness)
        {
            moveDirection += Vector3.right;
        }
        if (Input.mousePosition.x <= edgeThickness)
        {
            moveDirection += Vector3.left;
        }
        if (Input.mousePosition.y >= Screen.height - edgeThickness)
        {
            moveDirection += Vector3.forward;
        }
        if (Input.mousePosition.y <= edgeThickness)
        {
            moveDirection += Vector3.back;
        }

        moveDirection = Quaternion.Euler(60f, 45f, 0f) * moveDirection;

        // y�� ������ ���� y���� 0���� ������ �� �̵�
        moveDirection.y = 0;

        // �̵�
        transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime, Space.World);
    }

    public void SetQuarterView()   // Vector3 delta
    {
        _mode = Define.CameraMode.QuarterView;
    }
}