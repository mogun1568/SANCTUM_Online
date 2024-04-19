using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LocationInfo
{
    public int R { get; set; }
    public int C { get; set; }
    public int Direction { get; set; }

    public LocationInfo(int r = 0, int c = 0, int direction = 0)
    {
        R = r;
        C = c;
        Direction = direction;
    }

    public void SetValues(int newR, int newC, int newDirection = 0)
    {
        R = newR;
        C = newC;
        Direction = newDirection;
    }
}

public class NewMap : CreatureController
{
    GameObject roadParent;
    GameObject GroundParent;
    GameObject EnvironmentParent;

    [HideInInspector] public GameObject startObj;
    GameObject endObj;

    //GameObject enemyPrefab;

    protected const int mapDefaultLength = 31;

    public LinkedList<LocationInfo> roads = new LinkedList<LocationInfo>();

    public LocationInfo startPoint = new LocationInfo();
    public LocationInfo endPoint = new LocationInfo();

    protected int mapLength = 3;
    protected int NodeSize = 4;

    int startdirR, startdirC;
    //int interval = 100;

    void Start()
    {
        Init();
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        if (mapLength > mapDefaultLength / 2 - 1)
    //        {
    //            return;
    //        }

    //        ExpendMap();

    //        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //    }

    //    if (Input.GetKeyDown(KeyCode.A))
    //    {
    //        Instantiate(enemyPrefab);
    //    }
    //}


    /*void Print2DArray()
    {
        string s = "(" + startPoint.R + ", " + startPoint.C + "), (" + endPoint.R + ", " + endPoint.C + ")";
        Debug.Log(s);

        string patternString = "";

        for (int i = 0; i < mapDefaultLength; i++)
        {
            for (int j = 0; j < mapDefaultLength; j++)
            {
                patternString += map[i, j].ToString();
            }
            patternString += "\n"; // �� �� ��� �� �� �ٲ�
        }

        Debug.Log(patternString);

        //string boolString = "";

        //for (int i = 0; i < mapDefaultLength; i++)
        //{
        //    for (int j = 0; j < mapDefaultLength; j++)
        //    {
        //        if (visit[i, j])
        //        {
        //            boolString += 1;
        //        } else
        //        {
        //            boolString += 0;
        //        }
        //    }
        //    boolString += "\n"; // �� �� ��� �� �� �ٲ�
        //}

        //Debug.Log(boolString);
    }*/

    protected virtual void Init()
    {
        transform.position = Pos;
        startdirR = Pos.x;
        startdirC = Pos.z;

        roadParent = Managers.Resource.Instantiate("Map/Parent", default, default, transform);
        roadParent.name = "" + "Roads";
        GroundParent = Managers.Resource.Instantiate("Map/Parent", default, default, transform);
        GroundParent.name = "" + "Nodes";
        EnvironmentParent = Managers.Resource.Instantiate("Map/Parent", default, default, transform);
        EnvironmentParent.name = "" + "Environments";

        startObj = Managers.Resource.Instantiate("Map/Start", default, default, transform);
        endObj = Managers.Resource.Instantiate("Map/End", default, default, transform);
    }  

    // protected�� �ٲ�����
    public virtual void ExpendMap()
    {
        // �̰� �� ����
        //Print2DArray();

        //foreach (var node in roads)
        //{
        //    Debug.Log($"Node Values: R={node.R}, C={node.C}");
        //}
    }

    public void UpdatePosition()
    {
        startObj.transform.position = new Vector3(startPoint.R * NodeSize + startdirR, 1, startPoint.C * NodeSize + startdirC);
        startObj.transform.rotation = Quaternion.Euler(0, startPoint.Direction * 90, 0);
        StartCoroutine(SetScaleCoroutine(startObj.transform, 1));
        endObj.transform.position = new Vector3(endPoint.R * NodeSize + startdirR, 1, endPoint.C * NodeSize + startdirC);
        endObj.transform.rotation = Quaternion.Euler(0, endPoint.Direction * 90 + 180, 0);
        StartCoroutine(SetScaleCoroutine(endObj.transform, 0.003f));
    }

    public virtual void CreateNode(string type, int r, int c, bool haveEnvironment = false)
    {
        r += startdirR;
        c += startdirC;

        GameObject node;

        if (roadParent == null)
        {
            Debug.Log("null");
        }

        if (type == "RoadS")
        {
            node = Managers.Resource.Instantiate($"Map/ForestGroundDirt", new Vector3(r, 0, c), Quaternion.identity, roadParent.transform);
            //Instantiate(roadPrefab, new Vector3(r, 0, c), Quaternion.identity, parent.transform);
            roads.AddFirst(new LocationInfo(r, c));
        } else if (type == "RoadE") 
        {
            node = Managers.Resource.Instantiate($"Map/ForestGroundDirt", new Vector3(r, 0, c), Quaternion.identity, roadParent.transform);
            //Instantiate(roadPrefab, new Vector3(r, 0, c), Quaternion.identity, parent.transform);
            roads.AddLast(new LocationInfo(r, c));
        } else
        {
            float y = Random.Range(-0.25f, 0.25f);
            node = Managers.Resource.Instantiate($"Map/ForestGround01", new Vector3(r, y, c), Quaternion.identity, GroundParent.transform);
            //Instantiate(groundPrefab, new Vector3(r, y, c), Quaternion.identity, parent.transform);
            SetChildCount(node.transform);
            if (haveEnvironment)
            {
                node.GetComponent<Node>().environment = true;
                CreateEnvironment(r, y, c);
            }
        }

        StartCoroutine(MoveObjectCoroutine(node.transform));
        SetTransparency(node);
    }

    protected void CreateEnvironment(int r, float y, int c)
    {
        GameObject EnvironmentObj = Managers.Resource.Instantiate($"Map/Environment/TFF_Birch_Tree_01A", new Vector3(r, y + 1, c), Quaternion.identity, EnvironmentParent.transform);
        //Instantiate(EnvironmentPrefab, new Vector3(r, y + NodeSize - 0.5f, c), Quaternion.identity, EnvironmentParent.transform);
        StartCoroutine(SetScaleCoroutine(EnvironmentObj.transform, 0.8f));
    }

    void SetChildCount(Transform go)
    {
        // �ڽ� ������Ʈ ���� ��Ȱ��ȭ (Node ��ũ��Ʈ�� �ű�� ���� ��)
        foreach (Transform child in go)
        {
            child.gameObject.SetActive(false);
        }

        int activeObjectCount = Random.Range(0, 4);

        while (activeObjectCount > 0)
        {
            int randomIndex = Random.Range(1, go.childCount);

            while (go.GetChild(randomIndex).gameObject.activeSelf)
            {
                randomIndex = Random.Range(1, go.childCount);
            }

            go.GetChild(randomIndex).gameObject.SetActive(true);

            // �ڷ�ƾ���� ũ�� ���� �Լ� �߰� ����
            StartCoroutine(SetScaleCoroutine(go.GetChild(randomIndex).gameObject.transform, 0.8f));

            activeObjectCount--;
        }
    }

    void SetTransparency(GameObject go)
    {
        // ������Ʈ�� Material�� Transparency Level�� shader ��ũ��Ʈ������ _Tweak_transparency ��
        int transparencyPropertyID = Shader.PropertyToID("_Tweak_transparency");

        go.GetComponent<Renderer>().material.SetFloat(transparencyPropertyID, -1);

        StartCoroutine(FadeTransparencyCoroutine(go.GetComponent<Renderer>().material, transparencyPropertyID));
    }

    IEnumerator MoveObjectCoroutine(Transform go)
    {
        // �̵��� �Ÿ��� �ð� ����
        float distanceToMove = -2f;
        float duration = 2f;

        // ���� ��ġ�� ���� ��ġ ���
        Vector3 endPos = go.position;
        Vector3 startPos = endPos + new Vector3(0, distanceToMove, 0);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // ���� �ð��� ���� ������ ��ġ ���
            go.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);

            // ��� �ð� ������Ʈ
            elapsedTime += Time.deltaTime;

            // �� ������ ���
            yield return null;
        }

        go.position = endPos;
    }

    IEnumerator FadeTransparencyCoroutine(Material transparentMaterial, int transparencyPropertyID)
    {
        float currentTransparency = transparentMaterial.GetFloat(transparencyPropertyID);
        float targetTransparency = 0.0f;
        float duration = 2f; // ���濡 �ɸ��� �ð�

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // _Tweak_transparency ���� ������ ���� (Lerp �Լ� ���)
            float newTransparency = Mathf.Lerp(currentTransparency, targetTransparency, elapsedTime / duration);

            // Material�� ����� _Tweak_transparency �� ����
            transparentMaterial.SetFloat(transparencyPropertyID, newTransparency);

            // ��� �ð� ������Ʈ
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // ������ ���� (�÷�Ʈ�� ��Ȯ���� ����)
        transparentMaterial.SetFloat(transparencyPropertyID, targetTransparency);
    }

    IEnumerator SetScaleCoroutine(Transform go, float Scale)
    {
        float duration = 2f;
        float elapsedTime = 0f;

        Vector3 initialScale = new Vector3(0.001f, 0.001f, 0.001f);
        Vector3 targetScale = new Vector3(Scale, Scale, Scale);

        while (elapsedTime < duration)
        {
            // ���� �ð��� ���� ������ ũ�� ���
            go.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);

            // ��� �ð� ������Ʈ
            elapsedTime += Time.deltaTime;

            // �� ������ ���
            yield return null;
        }

        go.localScale = targetScale;
    }

    public void clear()
    {
        roads.Clear();
    }
}
