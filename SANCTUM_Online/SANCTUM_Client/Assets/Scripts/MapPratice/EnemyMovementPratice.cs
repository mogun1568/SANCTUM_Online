using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementPratice : MonoBehaviour
{
    //NewMap map;
    //[SerializeField] float moveSpeed = 3.0f;
    //LinkedListNode<NodeInfo> currentRoad, nextRoad;

    //void Start()
    //{
    //    map = GameObject.Find("GameObject").GetComponent<NewMap>();

    //    currentRoad = map.roads.First;
    //    transform.position = new Vector3(currentRoad.Value.R, 1, currentRoad.Value.C);
    //    nextRoad = currentRoad.Next;
    //}

    //void Update()
    //{
    //    // ���� Ÿ�� ��ġ���� �̵�
    //    if (nextRoad != null)
    //    {
    //        Vector3 targetPosition = new Vector3(nextRoad.Value.R, 1, nextRoad.Value.C);
    //        Vector3 direction = (targetPosition - transform.position).normalized;

    //        // Translate �޼��带 ����Ͽ� ������Ʈ�� �̵�
    //        transform.Translate(direction * moveSpeed * Time.deltaTime);

    //        // ���� Ÿ�� ��ġ�� �����ϸ� ���� Ÿ������ ����
    //        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
    //        {
    //            // ���� Ÿ�� ����
    //            currentRoad = nextRoad;
    //            nextRoad = nextRoad.Next;
    //        }
    //    } else
    //    {
    //        Destroy(gameObject);
    //    }
    //}
}
