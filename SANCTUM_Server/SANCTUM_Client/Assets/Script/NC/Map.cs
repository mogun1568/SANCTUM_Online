using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

/*
    <direction>
    0 - left
    1 - top
    2 - right
    3 - bottom

    사분면
    0 1
    2 3
*/


public class AMap : MonoBehaviour
{
    [SerializeField] static int MAX_MAP_SIZE = 128; //무조건 짝수, 최소 6
    [SerializeField] double weighted_num = 0; //가중치 0~1 실수
    [SerializeField] GameObject NodePrefab;
    [SerializeField] GameObject GroundPrefab;
    [SerializeField] GameObject RoadMom;
    [SerializeField] GameObject NodeMom;
    [SerializeField] GameObject EnviroemntMom;
    [SerializeField] GameObject StartPoint;
    [SerializeField] GameObject EndPoint;
    [SerializeField] float GroundSetHeight = 0;
    [SerializeField] float map_set_height = 0.5f;
    [SerializeField] GameObject TreePrefab;
    float NodeSetHeight;
    protected float monster_height = 0.7f;
    public static List<List<bool>> map_array = new List<List<bool>>(); // true = 길
    protected static int[] start_point = new int[3];                         // 시작포인터 [0]=x,[1]=y,[2]=direction (주의: 길 끝을 나타내는 것임(길 끝 다음 블럭이 아님))
    protected int[] end_point = new int[3];                           // end포인터 [0]=x,[1]=y,[2]=direction
    public static int current_map_size;

    // for dfs
    protected List<List<bool>> dfs_temp_map_array = new List<List<bool>>(); // dfs 맵용
    protected int dfs_num;

    List<List<float>> map_height = new List<List<float>>();
    List<List<int>> enviroment_map = new List<List<int>>();
    public List<List<map_block_raise>> map_Block_Raises = new List<List<map_block_raise>>();

    public class map_block_raise
    {
        public Boolean exist_block = false;
        public GameObject nodeblock = null;
        public GameObject groundblock = null;
        public GameObject enviromentblock = null;
        public float rand_speed = 0.0f;
        public float rand_map_height = 0.0f;
    }

    Coroutine runningCoroutine = null;
    /*
    protected void dfs_find_corner(int x, int y, int dir, int sn) // 코너 찾기용
    {
        dfs_temp_map_array[y][x] = true;
        if ((sn==0&&x == start_point[0] && y == start_point[1]) || (sn == 1 && x == end_point[0] && y == end_point[1]))
        {
            points.AddLast(new Vector3((x - (MAX_MAP_SIZE - current_map_size) / 2) * 4, 2, -4 * (y - (MAX_MAP_SIZE - current_map_size) / 2)));
            return;
        }

        if (map_array[y][x - 1] && !dfs_temp_map_array[y][x - 1])
        {
            if (dir != 0)
                points.AddLast(new Vector3((x - (MAX_MAP_SIZE - current_map_size) / 2) * 4, 2, -4 * (y - (MAX_MAP_SIZE - current_map_size) / 2)));
            dfs_find_corner(x - 1, y, 0, sn);
        }
        if (map_array[y - 1][x] && !dfs_temp_map_array[y - 1][x])
        {
            if (dir != 1)
                points.AddLast(new Vector3((x - (MAX_MAP_SIZE - current_map_size) / 2) * 4, 2, -4 * (y - (MAX_MAP_SIZE - current_map_size) / 2)));
            dfs_find_corner(x, y - 1, 1, sn);
        }
        if (map_array[y][x + 1] && !dfs_temp_map_array[y][x + 1])
        {
            if (dir != 2)
                points.AddLast(new Vector3((x - (MAX_MAP_SIZE - current_map_size) / 2) * 4, 2, -4 * (y - (MAX_MAP_SIZE - current_map_size) / 2)));
            dfs_find_corner(x + 1, y, 2, sn);
        }
        if (map_array[y + 1][x] && !dfs_temp_map_array[y + 1][x])
        {
            if (dir != 3)
                points.AddLast(new Vector3((x - (MAX_MAP_SIZE - current_map_size) / 2) * 4, 2, -4 * (y - (MAX_MAP_SIZE - current_map_size) / 2)));
            dfs_find_corner(x, y + 1, 3, sn);
        }
    }
    */

    IEnumerator MapBlockRaiseFunc()
    {
        while (true)
        {
            bool check = true;
            for (int i = 0; i < MAX_MAP_SIZE; i++)
                for (int j = 0; j < MAX_MAP_SIZE; j++)
                {
                    if (map_Block_Raises[i][j].exist_block)
                    {
                        check = false;
                        if (map_Block_Raises[i][j].nodeblock != null)
                        {
                            map_Block_Raises[i][j].nodeblock.transform.Translate(0, map_Block_Raises[i][j].rand_speed * Time.deltaTime, 0);
                            map_Block_Raises[i][j].nodeblock.GetComponent<Renderer>().material.SetFloat("_Tweak_transparency", -(float)Math.Log10(map_Block_Raises[i][j].groundblock.transform.position.y/ map_Block_Raises[i][j].rand_map_height*10));
                        }
                        map_Block_Raises[i][j].groundblock.transform.Translate(0, map_Block_Raises[i][j].rand_speed * Time.deltaTime, 0);
                        map_Block_Raises[i][j].groundblock.GetComponent<Renderer>().material.SetFloat("_Tweak_transparency", -(float)Math.Log10(map_Block_Raises[i][j].groundblock.transform.position.y / map_Block_Raises[i][j].rand_map_height * 10));
                        if (map_Block_Raises[i][j].enviromentblock != null)
                        {
                            map_Block_Raises[i][j].enviromentblock.transform.Translate(0, map_Block_Raises[i][j].rand_speed * Time.deltaTime, 0);
                            foreach (Component t_component in map_Block_Raises[i][j].enviromentblock.GetComponentsInChildren<Renderer>())
                            {
                                foreach (Material t_material in t_component.GetComponent<Renderer>().materials)
                                {
                                    t_material.SetFloat("_Tweak_transparency", -(float)Math.Log10(map_Block_Raises[i][j].groundblock.transform.position.y / map_Block_Raises[i][j].rand_map_height * 10));
                                }
                            }
                        }
                        if (map_Block_Raises[i][j].groundblock.transform.position.y > -0.0001f)
                        {
                            if (map_Block_Raises[i][j].nodeblock != null)
                            {
                                map_Block_Raises[i][j].nodeblock.transform.Translate(0, GroundSetHeight + GroundPrefab.transform.localScale.y + map_height[i][j] - map_Block_Raises[i][j].nodeblock.transform.position.y, 0);
                                map_Block_Raises[i][j].nodeblock.GetComponent<Renderer>().material.SetFloat("_Tweak_transparency", 0);
                            }
                            map_Block_Raises[i][j].groundblock.transform.Translate(0, GroundSetHeight- map_Block_Raises[i][j].groundblock.transform.position.y, 0);
                            map_Block_Raises[i][j].groundblock.GetComponent<Renderer>().material.SetFloat("_Tweak_transparency", 0);
                            if (map_Block_Raises[i][j].enviromentblock != null)
                            {
                                map_Block_Raises[i][j].enviromentblock.transform.Translate(0, GroundSetHeight + GroundPrefab.transform.localScale.y + map_height[i][j] * 2 - map_Block_Raises[i][j].enviromentblock.transform.position.y, 0);
                                foreach (Component t_component in map_Block_Raises[i][j].enviromentblock.GetComponentsInChildren<Renderer>())
                                {
                                    foreach (Material t_material in t_component.GetComponent<Renderer>().materials)
                                    {
                                        t_material.SetFloat("_Tweak_transparency", 0);
                                    }
                                }
                            }
                            map_Block_Raises[i][j].exist_block = false;
                        }
                    }
                }
            if (check)
            {
                StartCoroutine("PortalTransparent2");
                StartCoroutine("PortalTransparent1");
                StartCoroutine("EnviromentScale");
                StopCoroutine("MapBlockRaiseFunc");
            }
            yield return null;
        }
    }

    IEnumerator EnviromentScale()
    {
        float rand_num = 0;
        bool ck;
        while (true)
        {
            ck = true;
            for (int y = 0; y < MAX_MAP_SIZE; y++)
            {
                for (int x = 0; x < MAX_MAP_SIZE; x++)
                {
                    if (!map_array[y][x])
                    {
                        if (map_Block_Raises[y][x].nodeblock != null)
                        {
                            Transform[] t = map_Block_Raises[y][x].nodeblock.GetComponentsInChildren<Transform>();
                            for (int i = 1; i < map_Block_Raises[y][x].nodeblock.GetComponentsInChildren<Transform>().Length; i++)
                            {
                                rand_num = UnityEngine.Random.Range(0.2f, 0.8f);
                                if (t[i].localScale.y < 2)
                                {
                                    t[i].localScale += new Vector3(rand_num * Time.deltaTime/2, rand_num * Time.deltaTime, rand_num * Time.deltaTime/2);
                                    ck = false;
                                }
                            }
                        }
                        if (map_Block_Raises[y][x].enviromentblock != null)
                        {
                            foreach (Transform tras in map_Block_Raises[y][x].enviromentblock.GetComponentsInChildren<Transform>())
                            {
                                if (tras.localScale.y < 0.9)
                                {
                                    rand_num = UnityEngine.Random.Range(0.2f, 0.8f);
                                    tras.localScale += new Vector3(rand_num * Time.deltaTime, rand_num * Time.deltaTime, rand_num * Time.deltaTime);
                                    ck = false;
                                }
                            }
                        }
                    }
                }
            }
            if (ck)
                StopCoroutine("EnviromentScale");
            yield return null;
        }
    }

    IEnumerator PortalTransparent2()
    {
        float TimeSave = 0;
        while (true)
        {
            foreach (Component t_component in StartPoint.GetComponentsInChildren<Renderer>())
            {
                foreach (Material t_material in t_component.GetComponent<Renderer>().materials)
                {
                    t_material.SetFloat("_Tweak_transparency", -1 + TimeSave*0.1f);
                    TimeSave += Time.deltaTime;
                    if (TimeSave>10)
                    {
                        foreach (Component t2_component in StartPoint.GetComponentsInChildren<Renderer>())
                        {
                            foreach (Material t2_material in t2_component.GetComponent<Renderer>().materials)
                            {
                                if (t2_component.GetComponent<ParticleSystem>() != null)
                                {
                                    t2_component.GetComponent<ParticleSystem>().Play();
                                }
                            }
                        }
                        StopCoroutine("PortalTransparent2");
                    }
                }
            }
            yield return null;
        }
    }

    IEnumerator PortalTransparent1()
    {
        //float TimeSave = 0;
        while (true)
        {
            //foreach (Component t_component in EndPoint.GetComponentsInChildren<Renderer>())
            //{
            //    foreach (Material t_material in t_component.GetComponent<Renderer>().materials)
            //    {
            //        t_material.SetFloat("_Tweak_transparency", -1 + TimeSave * 0.1f);
            //        TimeSave += Time.deltaTime;
            //        if (TimeSave > 10)
            //        {
            //            foreach (Component t2_component in EndPoint.GetComponentsInChildren<Renderer>())
            //            {
            //                foreach (Material t2_material in t2_component.GetComponent<Renderer>().materials)
            //                {
            //                    if (t2_component.GetComponent<ParticleSystem>() != null)
            //                    {
            //                        t2_component.GetComponent<ParticleSystem>().Play();
            //                    }
            //                }
            //            }
            //            StopCoroutine("PortalTransparent1");
            //        }
            //    }
            //}
            //if (EndPoint.transform.position.y - 1f * Time.deltaTime > GroundSetHeight + GroundPrefab.transform.localScale.y)
            //{
            //    EndPoint.transform.position -= new Vector3(0, 50f * Time.deltaTime, 0);
            //}
            //else
            //{
            //    StopCoroutine("PortalTransparent1");
            //}

            if (EndPoint.transform.localScale.y + 0.001f * Time.deltaTime < 0.003f)
            {
                EndPoint.transform.localScale += new Vector3(0.003f*Time.deltaTime, 0.003f * Time.deltaTime, 0.003f * Time.deltaTime);
            }
            else
            {
                StopCoroutine("PortalTransparent1");
            }

            yield return null;
        }
    }

    public int GetArrayPointX(int x)
    {
        return x/4 -8+ MAX_MAP_SIZE / 2;
    }

    public int GetArrayPointY(int y)
    {
        return -y/4 -8+ MAX_MAP_SIZE / 2;
    }

    void map_block_generate(int x, int y)
    {
        Material[] temp_render = new Material[3];
        Component[] temp_component = new Component[3];
        map_Block_Raises[y][x].rand_map_height = UnityEngine.Random.Range(-4.0f, -2.0f);
        map_Block_Raises[y][x].exist_block = true;
        map_Block_Raises[y][x].groundblock = Instantiate(GroundPrefab, new Vector3(abs_x(x) * 4, GroundSetHeight+map_Block_Raises[y][x].rand_map_height, -abs_y(y) * 4), Quaternion.identity, RoadMom.transform);
        map_Block_Raises[y][x].groundblock.GetComponent<Renderer>().material.SetFloat("_Tweak_transparency", -1f);
        if (!map_array[y][x])
        {
            map_Block_Raises[y][x].nodeblock = Instantiate(NodePrefab, new Vector3(abs_x(x) * 4, GroundSetHeight + GroundPrefab.transform.localScale.y + map_height[y][x]+ map_Block_Raises[y][x].rand_map_height, -abs_y(y) * 4), Quaternion.identity, NodeMom.transform);
            map_Block_Raises[y][x].nodeblock.transform.localScale = new Vector3(2, map_height[y][x], 2);
            map_Block_Raises[y][x].nodeblock.GetComponent<Renderer>().material.SetFloat("_Tweak_transparency", -1f);
            Transform[] t = map_Block_Raises[y][x].nodeblock.transform.GetComponentsInChildren<Transform>();
            for (int i = 1; i < map_Block_Raises[y][x].nodeblock.GetComponentsInChildren<Transform>().Length; i++)
            {
                int rand_num = UnityEngine.Random.Range(0, 6);
                if(rand_num!=0)
                    t[i].GetComponent<MeshRenderer>().enabled = false;
                t[i].localScale = new Vector3(0.001f, 0.001f, 0.001f);
            }

            if (enviroment_map[y][x] == 1)
            {
                map_Block_Raises[y][x].enviromentblock=Instantiate(TreePrefab, new Vector3(abs_x(x) * 4, GroundSetHeight + GroundPrefab.transform.localScale.y + map_height[y][x] * 2+ map_Block_Raises[y][x].rand_map_height, -abs_y(y) * 4), Quaternion.identity,EnviroemntMom.transform);
                map_Block_Raises[y][x].nodeblock.GetComponent<Node>().enviroment = true;
                
                foreach(Transform tras in map_Block_Raises[y][x].enviromentblock.GetComponentsInChildren<Transform>())
                {
                    tras.localScale = new Vector3(0.001f, 0.001f, 0.001f);
                }

                temp_component = map_Block_Raises[y][x].enviromentblock.GetComponentsInChildren<Renderer>();
                foreach (Component t_component in map_Block_Raises[y][x].enviromentblock.GetComponentsInChildren<Renderer>())
                {
                    foreach (Material t_material in t_component.GetComponent<Renderer>().materials)
                    {
                        t_material.SetFloat("_Tweak_transparency", -1f);
                    }
                }
            }
            else
            {
                map_Block_Raises[y][x].enviromentblock = null;
            }
        }
        else
        {
            map_Block_Raises[y][x].nodeblock = null;
        }
        map_Block_Raises[y][x].rand_speed = UnityEngine.Random.Range(1.0f, 3.0f);
    }

    float Get_map_height(int x, int y)
    {
        float sum = 0;
        float randomFloat = UnityEngine.Random.Range(-1f, 1f);
        sum += randomFloat;
        for(int i = -1;i<2;i++)
            for(int j = -1;j<2;j++)
                sum += map_height[y+i][x+j];
        for (int i = -2; i < 3; i++)
            for (int j = -2; j < 3; j++)
                if (i == -2 || i == 2 || j == -2 || j == 2)
                    sum += map_height[y + 1][x + j] / 4;
        sum = sum + randomFloat - map_height[y][x];
        sum /= 13;
        if (sum < 0)
            sum = 0.1f;
        map_height[y][x] = sum;
        return sum;
    }

    protected int abs_x(int x)
    {
        if(x<MAX_MAP_SIZE/2)
        {
            return 7 + (x-MAX_MAP_SIZE/2+1);
        }
        else
        {
            return 7 + (x-MAX_MAP_SIZE/2+1);
        }
    }

    protected int abs_y(int y)
    {
        if (y < MAX_MAP_SIZE / 2)
        {
            return 7 + (y - MAX_MAP_SIZE / 2+1);
        }
        else
        {
            return 7 + (y - MAX_MAP_SIZE / 2+1);
        }
    }

    protected void fill_map_node(int x, int y) // 코너 찾기용 아ㅔ
    {
        dfs_temp_map_array[y][x] = true;
        points.AddLast(new Vector3(abs_x(x) * 4, monster_height, -4 * abs_y(y)));
        if (x == end_point[0] && y == end_point[1])
            return;
        if (map_array[y][x - 1] && !dfs_temp_map_array[y][x - 1])
            fill_map_node(x - 1, y);
        if (map_array[y - 1][x] && !dfs_temp_map_array[y - 1][x])
            fill_map_node(x, y - 1);
        if (map_array[y][x + 1] && !dfs_temp_map_array[y][x + 1])
            fill_map_node(x + 1, y);
        if (map_array[y + 1][x] && !dfs_temp_map_array[y + 1][x])
            fill_map_node(x, y + 1);
    }

    protected void reset_dfs_arr(int num) // num=0일시 dfs_arr을 전부 false로 초기화, num=1일시 dfs_arr에 map_array복사
    {
        if (num == 0)
        {
            for (int i = 0; i < MAX_MAP_SIZE; i++)
                for (int j = 0; j < MAX_MAP_SIZE; j++)
                    dfs_temp_map_array[i][j] = false;
        }
        else
        {
            for (int i = 0; i < MAX_MAP_SIZE; i++)
                for (int j = 0; j < MAX_MAP_SIZE; j++)
                    dfs_temp_map_array[i][j] = map_array[i][j];
        }
    }

    protected void only_dfs_dir(int dir, int sn, ref int[] re_arr) // sn이 0=start, 1=end, dir 방향 0=왼쪽, 1=오른쪽, re_arr=x,y(+-1반환)
    {
        int star_point;
        if (sn == 0)
        {
            star_point = start_point[2];
        }
        else
        {
            star_point = end_point[2];
        }

        if (star_point == 0)
        {
            if (dir == 0)
            {
                re_arr[0] = 0;
                re_arr[1] = 1;
            }
            else
            {
                re_arr[0] = 0;
                re_arr[1] = -1;
            }
        }
        else if (star_point == 1)
        {
            if (dir == 0)
            {
                re_arr[0] = -1;
                re_arr[1] = 0;
            }
            else
            {
                re_arr[0] = 1;
                re_arr[1] = 0;
            }
        }
        else if (star_point == 2)
        {
            if (dir == 0)
            {
                re_arr[0] = 0;
                re_arr[1] = -1;
            }
            else
            {
                re_arr[0] = 0;
                re_arr[1] = 1;
            }
        }
        else
        {
            if (dir == 0)
            {
                re_arr[0] = 1;
                re_arr[1] = 0;
            }
            else
            {
                re_arr[0] = -1;
                re_arr[1] = 0;
            }
        }
    }

    protected void cal_road_distributions(ref int[] re_arr) // 맵을 배열 넣으면 사분면의 길 갯수 입력한 배열에 반환
    {
        int[,] temp_arr = new int[2, 2];
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                for (int v = (MAX_MAP_SIZE - current_map_size) / 2 + current_map_size * i / 2; v < (MAX_MAP_SIZE - current_map_size) / 2 + current_map_size * i / 2 + current_map_size / 2; v++)
                {
                    for (int w = (MAX_MAP_SIZE - current_map_size) / 2 + current_map_size * j / 2; w < (MAX_MAP_SIZE - current_map_size) / 2 + current_map_size * j / 2 + current_map_size / 2; w++)
                    {
                        if (map_array[v][w])
                            temp_arr[i, j]++;
                    }
                }
            }
        }
        re_arr[0] = temp_arr[0, 0];
        re_arr[1] = temp_arr[0, 1];
        re_arr[2] = temp_arr[1, 0];
        re_arr[3] = temp_arr[1, 1];
    }

    protected bool next_block_available(int direction, int x, int y) // 좌표와 방향을 입력하면 그 방향으로 1블럭 진행 가능한지 논리형 리턴
    {
        if (direction == 0)
        {
            if (x - 2 >= 0 && y - 1 >= 0)
                if (map_array[y - 1][x - 2])
                    return false;
            if (x - 1 >= 0 && y - 1 >= 0)
                if (map_array[y - 1][x - 1])
                    return false;
            if (x - 2 >= 0)
                if (map_array[y][x - 2])
                    return false;
            if (x - 1 >= 0)
                if (map_array[y][x - 1])
                    return false;
            if (x - 2 >= 0 && y + 1 < MAX_MAP_SIZE)
                if (map_array[y + 1][x - 2])
                    return false;
            if (x - 1 >= 0 && y + 1 < MAX_MAP_SIZE)
                if (map_array[y + 1][x - 1])
                    return false;
        }
        else if (direction == 1)
        {
            if (x - 1 >= 0 && y - 2 >= 0)
                if (map_array[y - 2][x - 1])
                    return false;
            if (y - 2 >= 0)
                if (map_array[y - 2][x])
                    return false;
            if (x + 1 < MAX_MAP_SIZE && y - 2 >= 0)
                if (map_array[y - 2][x + 1])
                    return false;
            if (x - 1 >= 0 && y - 1 >= 0)
                if (map_array[y - 1][x - 1])
                    return false;
            if (y - 1 >= 0)
                if (map_array[y - 1][x])
                    return false;
            if (x + 1 < MAX_MAP_SIZE && y - 1 >= 0)
                if (map_array[y - 1][x + 1])
                    return false;
        }
        else if (direction == 2)
        {
            if (x + 1 < MAX_MAP_SIZE && y - 1 >= 0)
                if (map_array[y - 1][x + 1])
                    return false;
            if (x + 2 < MAX_MAP_SIZE && y - 1 >= 0)
                if (map_array[y - 1][x + 2])
                    return false;
            if (x + 1 < MAX_MAP_SIZE)
                if (map_array[y][x + 1])
                    return false;
            if (x + 2 < MAX_MAP_SIZE)
                if (map_array[y][x + 2])
                    return false;
            if (x + 1 < MAX_MAP_SIZE && y + 1 < MAX_MAP_SIZE)
                if (map_array[y + 1][x + 1])
                    return false;
            if (x + 2 < MAX_MAP_SIZE && y + 1 < MAX_MAP_SIZE)
                if (map_array[y + 1][x + 2])
                    return false;
        }
        else if (direction == 3)
        {
            if (x - 1 >= 0 && y + 1 < MAX_MAP_SIZE)
                if (map_array[y + 1][x - 1])
                    return false;
            if (y + 1 < MAX_MAP_SIZE)
                if (map_array[y + 1][x])
                    return false;
            if (x + 1 < MAX_MAP_SIZE && y + 1 < MAX_MAP_SIZE)
                if (map_array[y + 1][x + 1])
                    return false;
            if (x - 1 >= 0 && y + 2 < MAX_MAP_SIZE)
                if (map_array[y + 2][x - 1])
                    return false;
            if (y + 2 < MAX_MAP_SIZE)
                if (map_array[y + 2][x])
                    return false;
            if (x + 1 < MAX_MAP_SIZE && y + 2 < MAX_MAP_SIZE)
                if (map_array[y + 2][x + 1])
                    return false;
        }
        else
        {
            Console.WriteLine("ERROR(next block avail)");
        }
        return true;
    }

    protected bool dfs_on_side_path(int x, int y) // 입력된 포인터가 현재 맵 크기 +1칸인 사각형 길 위에 있는지 확인
    {
        if (x >= (MAX_MAP_SIZE - current_map_size) / 2 - 1 && x <= (MAX_MAP_SIZE - current_map_size) / 2 + current_map_size)
            if (y == (MAX_MAP_SIZE - current_map_size) / 2 - 1 || y == (MAX_MAP_SIZE - current_map_size) / 2 + current_map_size)
                return true;
        if (y >= (MAX_MAP_SIZE - current_map_size) / 2 - 1 && y <= (MAX_MAP_SIZE - current_map_size) / 2 + current_map_size)
            if (x == (MAX_MAP_SIZE - current_map_size) / 2 - 1 || x == (MAX_MAP_SIZE - current_map_size) / 2 + current_map_size)
                return true;
        return false;
    }

    protected void dfs_calc_distance(int point_x, int point_y, int c_x, int c_y, int len) // 포인터 x,y에서 c_x,c_y까지의 거리 측정 후, 전역변수 dfs_len에 저장
    {
        dfs_temp_map_array[c_y][c_x] = true;
        if (point_x == c_x && point_y == c_y)
        {
            dfs_num = len;
            return;
        }

        if (c_x > 0 && !dfs_temp_map_array[c_y][c_x - 1] && next_block_available(0, c_x, c_y))
            if (dfs_on_side_path(c_x - 1, c_y))
                dfs_calc_distance(point_x, point_y, c_x - 1, c_y, len + 1);
        if (c_y > 0 && !dfs_temp_map_array[c_y - 1][c_x] && next_block_available(1, c_x, c_y))
            if (dfs_on_side_path(c_x, c_y - 1))
                dfs_calc_distance(point_x, point_y, c_x, c_y - 1, len + 1);
        if (c_x + 1 < MAX_MAP_SIZE && !dfs_temp_map_array[c_y][c_x + 1] && next_block_available(2, c_x, c_y))
            if (dfs_on_side_path(c_x + 1, c_y))
                dfs_calc_distance(point_x, point_y, c_x + 1, c_y, len + 1);
        if (c_y + 1 < MAX_MAP_SIZE && !dfs_temp_map_array[c_y + 1][c_x] && next_block_available(3, c_x, c_y))
            if (dfs_on_side_path(c_x, c_y + 1))
                dfs_calc_distance(point_x, point_y, c_x, c_y + 1, len + 1);
        if (len > dfs_num)
            dfs_num = len;
    }

    protected void dfs_calc_distance_empty(int point_x, int point_y, int c_x, int c_y, int len) // 포인터 x,y에서 c_x,c_y까지의 거리 측정 후, 전역변수 dfs_len에 저장
    {
        dfs_temp_map_array[c_y][c_x] = true;
        if (point_x == c_x && point_y == c_y)
        {
            dfs_num = len;
            return;
        }

        if (c_x > 0 && !dfs_temp_map_array[c_y][c_x - 1])
            if (dfs_on_side_path(c_x - 1, c_y))
                dfs_calc_distance_empty(point_x, point_y, c_x - 1, c_y, len + 1);
        if (c_y > 0 && !dfs_temp_map_array[c_y - 1][c_x])
            if (dfs_on_side_path(c_x, c_y - 1))
                dfs_calc_distance_empty(point_x, point_y, c_x, c_y - 1, len + 1);
        if (c_x + 1 < MAX_MAP_SIZE && !dfs_temp_map_array[c_y][c_x + 1])
            if (dfs_on_side_path(c_x + 1, c_y))
                dfs_calc_distance_empty(point_x, point_y, c_x + 1, c_y, len + 1);
        if (c_y + 1 < MAX_MAP_SIZE && !dfs_temp_map_array[c_y + 1][c_x])
            if (dfs_on_side_path(c_x, c_y + 1))
                dfs_calc_distance_empty(point_x, point_y, c_x, c_y + 1, len + 1);
        if (len > dfs_num)
            dfs_num = len;
    }

    protected void dfs_fill_path(int point_x, int point_y, int c_x, int c_y, int len, int target_num, int sn) // 포인터 c_x, c_y에서 point_x,point_y까지 target_num만큼의 길을 채움 len은 1, sn(0=start, 1=end point)
    {
        dfs_temp_map_array[c_y][c_x] = true;
        map_array[c_y][c_x] = true;
        if (sn == 0)
        {
            points.AddFirst(new Vector3(abs_x(c_x) * 4, monster_height, -4 * abs_y(c_y)));
        }
        else if (sn == 1)
        {
            points.AddLast(new Vector3(abs_x(c_x) * 4, monster_height, -4 * abs_y(c_y)));
        }

        if (len == target_num)
        {
            if (sn == 0)
            {
                start_point[0] = c_x;
                start_point[1] = c_y;
                start_point[2] = find_dir(start_point[0], start_point[1]);
                if (start_point[2] == -1)
                    Console.WriteLine("ERROR(dfs_fill)");
                return;
            }
            else
            {
                end_point[0] = c_x;
                end_point[1] = c_y;
                end_point[2] = find_dir(end_point[0], end_point[1]);
                if (end_point[2] == -1)
                    Console.WriteLine("ERROR(dfs_fill)");
                return;
            }
        }

        if (c_x > 0 && !dfs_temp_map_array[c_y][c_x - 1] && next_block_available(0, c_x, c_y))
            if (dfs_on_side_path(c_x - 1, c_y))
                dfs_fill_path(point_x, point_y, c_x - 1, c_y, len + 1, target_num, sn);
        if (c_y > 0 && !dfs_temp_map_array[c_y - 1][c_x] && next_block_available(1, c_x, c_y))
            if (dfs_on_side_path(c_x, c_y - 1))
                dfs_fill_path(point_x, point_y, c_x, c_y - 1, len + 1, target_num, sn);
        if (c_x + 1 < MAX_MAP_SIZE && !dfs_temp_map_array[c_y][c_x + 1] && next_block_available(2, c_x, c_y))
            if (dfs_on_side_path(c_x + 1, c_y))
                dfs_fill_path(point_x, point_y, c_x + 1, c_y, len + 1, target_num, sn);
        if (c_y + 1 < MAX_MAP_SIZE && !dfs_temp_map_array[c_y + 1][c_x] && next_block_available(3, c_x, c_y))
            if (dfs_on_side_path(c_x, c_y + 1))
                dfs_fill_path(point_x, point_y, c_x, c_y + 1, len + 1, target_num, sn);
    }

    protected int find_dir(int x, int y) // 길 확장 후 ***_point[2]에 해당하는 dir 반환하는 함수
    {
        System.Random rand = new System.Random();
        int rand_num = rand.Next(2);
        if (x < (MAX_MAP_SIZE - current_map_size) / 2 && y < (MAX_MAP_SIZE - current_map_size) / 2) // 좌측상단
        {
            if (rand_num == 0) // 왼쪽
            {
                return 0;
            }
            else // 위쪽
            {
                return 1;
            }
        }
        else if (x < (MAX_MAP_SIZE - current_map_size) / 2 && y >= current_map_size + (MAX_MAP_SIZE - current_map_size) / 2) // 좌측하단
        {
            if (rand_num == 0) // 왼쪽
            {
                return 0;
            }
            else // 아래쪽
            {
                return 3;
            }
        }
        else if (x >= current_map_size + (MAX_MAP_SIZE - current_map_size) / 2 && y < (MAX_MAP_SIZE - current_map_size) / 2) // 우측 상단
        {
            if (rand_num == 0) // 오른쪽
            {
                return 2;
            }
            else // 위쪽
            {
                return 1;
            }
        }
        else if (x >= current_map_size + (MAX_MAP_SIZE - current_map_size) / 2 && y >= current_map_size + (MAX_MAP_SIZE - current_map_size) / 2) // 우측하단
        {
            if (rand_num == 0) // 오른쪽
            {
                return 2;
            }
            else // 아래쪽
            {
                return 3;
            }
        }
        else if (x < (MAX_MAP_SIZE - current_map_size) / 2) // 좌측
        {
            return 0;
        }
        else if (y < (MAX_MAP_SIZE - current_map_size) / 2) // 상측
        {
            return 1;
        }
        else if (x >= current_map_size + (MAX_MAP_SIZE - current_map_size) / 2) // 우측
        {
            return 2;
        }
        else if (y >= current_map_size + (MAX_MAP_SIZE - current_map_size) / 2) // 하측
        {
            return 3;
        }
        return -1;
    }

    protected void quadrant_to_point(int quadrant, ref int[] re_arr) // 사분면과 반환받을 배열 입력하면 사분면의 포인터 반환
    {
        if (quadrant == 0)
        {
            re_arr[0] = (MAX_MAP_SIZE - current_map_size) / 2 - 1;
            re_arr[1] = (MAX_MAP_SIZE - current_map_size) / 2 - 1;
        }
        else if (quadrant == 1)
        {
            re_arr[0] = (MAX_MAP_SIZE - current_map_size) / 2 + current_map_size;
            re_arr[1] = (MAX_MAP_SIZE - current_map_size) / 2 - 1;
        }
        else if (quadrant == 2)
        {
            re_arr[0] = (MAX_MAP_SIZE - current_map_size) / 2 - 1;
            re_arr[1] = (MAX_MAP_SIZE - current_map_size) / 2 + current_map_size;
        }
        else if (quadrant == 3)
        {
            re_arr[0] = (MAX_MAP_SIZE - current_map_size) / 2 + current_map_size;
            re_arr[1] = (MAX_MAP_SIZE - current_map_size) / 2 + current_map_size;
        }
        else
        {
            Console.WriteLine("ERROR(quadrant to point)");
            return;
        }
    }

    protected int calc_distance_to_quadrant(int quadrant, int x, int y, int dir, int copy_map, int sn) // 사분면,좌표,dir(방향 0=left, 1=right), copy_map(기존의 맵을 복사해서 비교할지에 대한 변수 0=no, 1=yes), sn(0=start, 1=end point) 입력받은 포인터 x,y에서 입력한 사분면까지의 거리를 반환함.
    {
        int[] target_point = new int[2]; // 목표 포인터
        int dir_check;
        quadrant_to_point(quadrant, ref target_point);

        if (copy_map == 1)
        {
            reset_dfs_arr(1);
        }
        else
        {
            reset_dfs_arr(0);
        }
        dfs_num = 0;
        // dfs_temp_map_array[y][x] = true;

        if (sn == 0)
        {
            dir_check = start_point[2];
        }
        else
        {
            dir_check = end_point[2];
        }

        if (dir_check == 2) // 포인터의 진행 방향이 오른쪽인 경우
        {
            if (dir == 0) // 오른쪽, 위쪽
            {
                if (copy_map == 1)
                {
                    if (next_block_available(1, x, y))
                    {
                        dfs_calc_distance(target_point[0], target_point[1], x, y - 1, 2); // up right
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    dfs_calc_distance_empty(target_point[0], target_point[1], x, y - 1, 2);
                }
                return dfs_num;
            }
            else if (dir == 1)
            {
                if (copy_map == 1)
                {
                    if (next_block_available(3, x, y))
                    {
                        dfs_calc_distance(target_point[0], target_point[1], x, y + 1, 2); // up right
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    dfs_calc_distance_empty(target_point[0], target_point[1], x, y + 1, 2);
                }
                return dfs_num;
            }
            else
            {
                return -1;
            }
        }
        else if (dir_check == 0) // 왼쪽
        {
            if (dir == 0)
            {
                if (copy_map == 1)
                {
                    if (next_block_available(3, x, y))
                    {
                        dfs_calc_distance(target_point[0], target_point[1], x, y + 1, 2); // up right
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    dfs_calc_distance_empty(target_point[0], target_point[1], x, y + 1, 2);
                }
                return dfs_num;
            }
            else if (dir == 1)
            {
                if (copy_map == 1)
                {
                    if (next_block_available(1, x, y))
                    {
                        dfs_calc_distance(target_point[0], target_point[1], x, y - 1, 2); // up right
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    dfs_calc_distance_empty(target_point[0], target_point[1], x, y - 1, 2);
                }
                return dfs_num;
            }
            else
            {
                return -1;
            }
        }
        else if (dir_check == 1)
        {
            if (dir == 0)
            {
                if (copy_map == 1)
                {
                    if (next_block_available(0, x, y))
                    {
                        dfs_calc_distance(target_point[0], target_point[1], x - 1, y, 2); // up right
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    dfs_calc_distance_empty(target_point[0], target_point[1], x - 1, y, 2);
                }
                return dfs_num;
            }
            else if (dir == 1)
            {
                if (copy_map == 1)
                {
                    if (next_block_available(2, x, y))
                    {
                        dfs_calc_distance(target_point[0], target_point[1], x + 1, y, 2); // up right
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    dfs_calc_distance_empty(target_point[0], target_point[1], x + 1, y, 2);
                }
                return dfs_num;
            }
            else
            {
                return -1;
            }
        }
        else if (dir_check == 3)
        {
            if (dir == 0)
            {
                if (copy_map == 1)
                {
                    if (next_block_available(2, x, y))
                    {
                        dfs_calc_distance(target_point[0], target_point[1], x + 1, y, 2); // up right
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    dfs_calc_distance_empty(target_point[0], target_point[1], x + 1, y, 2);
                }
                return dfs_num;
            }
            else if (dir == 1)
            {
                if (copy_map == 1)
                {
                    if (next_block_available(0, x, y))
                    {
                        dfs_calc_distance(target_point[0], target_point[1], x - 1, y, 2); // up right
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    dfs_calc_distance_empty(target_point[0], target_point[1], x - 1, y, 2);
                }
                return dfs_num;
            }
            else
            {
                return -1;
            }
        }
        else
        {
            Console.WriteLine("ERROR(calc distance to quadrant)");
            return -1;
        }
    }

    protected bool go_or_turn(int root_max, int root_poss, double w) // 포인터까지의 최대 길이와 진행가능한 거리, 가중치 입력하면 1블럭갈지 목표 포인터까지 갈지 계산
    {
        if (root_poss == 1)
        {
            return false;
        }
        System.Random rand = new System.Random();
        int rand_num = rand.Next();
        int sum_max_poss;
        sum_max_poss = rand_num % Convert.ToInt16(Math.Round(Math.Pow(root_max, 1 - w) * 0.2 + Math.Pow(root_poss, 1 + w) * 0.6));
        if (sum_max_poss < Convert.ToInt16(Math.Round(Math.Pow(root_poss, 1 + w))))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected int rand_go_block_num(int root_max, int root_poss, double w) // 포인터까지의 최대 길이와 진행가능한 거리, 가중치 입력하면 몇 블럭을 갈지 반환
    {
        System.Random rand = new System.Random();
        int rand_num = rand.Next();
        int com_num = Convert.ToInt16(rand_num % root_poss * (1 + root_poss / root_max)) + Convert.ToInt16(Math.Round(root_poss * w));
        if (com_num < 2)
        {
            return 2;
        }
        if (com_num > root_poss)
        {
            com_num = root_poss;
        }
        return com_num;
    }

    protected bool evaluate_root(int l_root_max, int l_root_poss, int r_root_max, int r_root_poss, double w) // 사분면, 포인터를 입력하면 전진할 방향(왼쪽(false), 오른쪽(true)) 반환, w = 가중치(0~1)가 클수록 멀고, 진행 가능한 블록이 많은 쪽으로 감
    {
        double left_path_weighted, right_path_weighted;   // 계산용
        int left_path_calculated, right_path_calculated; // 계산됨

        left_path_weighted = l_root_max * 0.6 + l_root_poss * 0.4;
        right_path_weighted = r_root_max * 0.6 + r_root_poss * 0.4;
        left_path_weighted = Math.Pow(left_path_weighted, 1 + w); // 기본 1 가중치 w(수정필요)
        left_path_weighted = Math.Round(left_path_weighted);
        left_path_calculated = Convert.ToInt16(left_path_weighted) + 1;
        right_path_weighted = Math.Pow(right_path_weighted, 1 + w);
        right_path_weighted = Math.Round(right_path_weighted);
        right_path_calculated = Convert.ToInt16(right_path_weighted) + 1;
        System.Random rand = new System.Random();
        int rand_num = rand.Next();
        rand_num = rand_num % (left_path_calculated + right_path_calculated);
        if (rand_num < left_path_calculated)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    protected int get_quadrant(int x, int y, double w) // 사분면 중 하나 가중치 계산 후 반환, w = 가중치(0~1)가 클수록 길이 없는 쪽으로 감
    {
        int[] road_dist = new int[4];
        int arr_sum = 0, sum_road_dist = 0;

        System.Random rand = new System.Random();
        int rand_num = rand.Next();
        cal_road_distributions(ref road_dist);
        for (int i = 0; i < 4; i++)
        {
            road_dist[i] = Convert.ToInt16(Math.Round(Math.Pow(road_dist[i], 1 + w))); // 기본 1 가중치 w(수정필요)
            arr_sum += road_dist[i];
        }
        rand_num = rand_num % arr_sum;
        for (int i = 0; i < 4; i++)
        {
            sum_road_dist += road_dist[i];
            if (rand_num < sum_road_dist)
            {
                return i;
            }
        }
        return -1;
    }

    protected void next_block_point(int sn, ref int[] re_arr) // sn=(0=start_point, 1=end_point), re_arr[]=끝점의 다음 블록 반환
    {
        if (sn == 0)
        {
            if (start_point[2] == 0)
            {
                re_arr[0] = start_point[0] - 1;
                re_arr[1] = start_point[1];
            }
            else if (start_point[2] == 1)
            {
                re_arr[0] = start_point[0];
                re_arr[1] = start_point[1] - 1;
            }
            else if (start_point[2] == 2)
            {
                re_arr[0] = start_point[0] + 1;
                re_arr[1] = start_point[1];
            }
            else if (start_point[2] == 3)
            {
                re_arr[0] = start_point[0];
                re_arr[1] = start_point[1] + 1;
            }
        }
        else if (sn == 1)
        {
            if (end_point[2] == 0)
            {
                re_arr[0] = end_point[0] - 1;
                re_arr[1] = end_point[1];
            }
            else if (end_point[2] == 1)
            {
                re_arr[0] = end_point[0];
                re_arr[1] = end_point[1] - 1;
            }
            else if (end_point[2] == 2)
            {
                re_arr[0] = end_point[0] + 1;
                re_arr[1] = end_point[1];
            }
            else if (end_point[2] == 3)
            {
                re_arr[0] = end_point[0];
                re_arr[1] = end_point[1] + 1;
            }
        }
    }

    public void expand_map() // 맵 사이즈 확대 (4방향으로 1칸씩)
    {
        System.Random rand = new System.Random();
        int rand_num = rand.Next();
        int start_select = rand_num % 2;
        int[] left_path_size = new int[2], right_path_size = new int[2]; //[0]=max,[1]=진행가능거리
        int dest_quadrant;
        int[] point = new int[2]; //[0]=x,[1]=y
        int go_block_num;
        int[] quad_point = new int[2];
        int[] cna = new int[2];
        if (start_select == 0) // start first
        {
            // start
            next_block_point(0, ref point);
            dest_quadrant = get_quadrant(point[0], point[1], 0);
            left_path_size[0] = calc_distance_to_quadrant(dest_quadrant, point[0], point[1], 0, 0, 0);
            left_path_size[1] = calc_distance_to_quadrant(dest_quadrant, point[0], point[1], 0, 1, 0);
            right_path_size[0] = calc_distance_to_quadrant(dest_quadrant, point[0], point[1], 1, 0, 0);
            right_path_size[1] = calc_distance_to_quadrant(dest_quadrant, point[0], point[1], 1, 1, 0);
            quadrant_to_point(dest_quadrant, ref quad_point);
            if (!evaluate_root(left_path_size[0], left_path_size[1], right_path_size[0], right_path_size[1], 0)) // 왼쪽으로 진행
            {
                if (go_or_turn(left_path_size[0], left_path_size[1], weighted_num)) // 한블럭 진행이 아닌 사분면의 포인터까지 진행
                {
                    go_block_num = rand_go_block_num(left_path_size[0], left_path_size[1], weighted_num);
                    reset_dfs_arr(0);
                    map_array[point[1]][point[0]] = true;
                    dfs_temp_map_array[point[1]][point[0]] = true;
                    only_dfs_dir(0, 0, ref cna);
                    points.AddFirst(new Vector3(abs_x(point[0]) * 4, monster_height, -4 * abs_y(point[1])));
                    dfs_fill_path(quad_point[0], quad_point[1], point[0] + cna[0], point[1] + cna[1], 2, go_block_num, 0);
                }
                else // 한블럭 진행
                {
                    map_array[point[1]][point[0]] = true;
                    start_point[0] = point[0];
                    start_point[1] = point[1];
                    start_point[2] = find_dir(point[0], point[1]);
                    points.AddFirst(new Vector3(abs_x(point[0]) * 4, monster_height, -4 * abs_y(point[1])));
                }
            }
            else // 오른쪽으로 진행
            {
                if (go_or_turn(right_path_size[0], right_path_size[1], weighted_num)) // 한블럭 진행이 아닌 사분면의 포인터까지 진행
                {
                    go_block_num = rand_go_block_num(right_path_size[0], right_path_size[1], weighted_num);
                    reset_dfs_arr(0);
                    map_array[point[1]][point[0]] = true;
                    dfs_temp_map_array[point[1]][point[0]] = true;
                    only_dfs_dir(1, 0, ref cna);
                    points.AddFirst(new Vector3(abs_x(point[0]) * 4, monster_height, -4 * abs_y(point[1])));
                    dfs_fill_path(quad_point[0], quad_point[1], point[0] + cna[0], point[1] + cna[1], 2, go_block_num, 0);
                }
                else // 한블럭 진행
                {
                    map_array[point[1]][point[0]] = true;
                    start_point[0] = point[0];
                    start_point[1] = point[1];
                    start_point[2] = find_dir(point[0], point[1]);
                    points.AddFirst(new Vector3(abs_x(point[0]) * 4, monster_height, -4 * abs_y(point[1])));
                }
            }
            // end
            next_block_point(1, ref point);
            dest_quadrant = get_quadrant(point[0], point[1], weighted_num);
            left_path_size[0] = calc_distance_to_quadrant(dest_quadrant, point[0], point[1], 0, 0, 1);
            left_path_size[1] = calc_distance_to_quadrant(dest_quadrant, point[0], point[1], 0, 1, 1);
            right_path_size[0] = calc_distance_to_quadrant(dest_quadrant, point[0], point[1], 1, 0, 1);
            right_path_size[1] = calc_distance_to_quadrant(dest_quadrant, point[0], point[1], 1, 1, 1);
            quadrant_to_point(dest_quadrant, ref quad_point);
            if (!evaluate_root(left_path_size[0], left_path_size[1], right_path_size[0], right_path_size[1], 0)) // 왼쪽으로 진행
            {
                if (go_or_turn(left_path_size[0], left_path_size[1], 0)) // 한블럭 진행이 아닌 사분면의 포인터까지 진행
                {
                    go_block_num = rand_go_block_num(left_path_size[0], left_path_size[1], weighted_num);
                    reset_dfs_arr(0);
                    map_array[point[1]][point[0]] = true;
                    dfs_temp_map_array[point[1]][point[0]] = true;
                    only_dfs_dir(0, 1, ref cna);
                    points.AddLast(new Vector3(abs_x(point[0]) * 4, monster_height, -4 * abs_y(point[1])));
                    dfs_fill_path(quad_point[0], quad_point[1], point[0] + cna[0], point[1] + cna[1], 2, go_block_num, 1);
                }
                else // 한블럭 진행
                {
                    map_array[point[1]][point[0]] = true;
                    end_point[0] = point[0];
                    end_point[1] = point[1];
                    end_point[2] = find_dir(point[0], point[1]);
                    points.AddLast(new Vector3(abs_x(point[0]) * 4, monster_height, -4 * abs_y(point[1])));
                }
            }
            else // 오른쪽으로 진행
            {
                if (go_or_turn(right_path_size[0], right_path_size[1], 0)) // 한블럭 진행이 아닌 사분면의 포인터까지 진행
                {
                    go_block_num = rand_go_block_num(right_path_size[0], right_path_size[1], weighted_num);
                    reset_dfs_arr(0);
                    map_array[point[1]][point[0]] = true;
                    dfs_temp_map_array[point[1]][point[0]] = true;
                    only_dfs_dir(1, 1, ref cna);
                    points.AddLast(new Vector3(abs_x(point[0]) * 4, monster_height, -4 * abs_y(point[1])));
                    dfs_fill_path(quad_point[0], quad_point[1], point[0] + cna[0], point[1] + cna[1], 2, go_block_num, 1);
                }
                else // 한블럭 진행
                {
                    map_array[point[1]][point[0]] = true;
                    end_point[0] = point[0];
                    end_point[1] = point[1];
                    end_point[2] = find_dir(point[0], point[1]);
                    points.AddLast(new Vector3(abs_x(point[0]) * 4, monster_height, -4 * abs_y(point[1])));
                }
            }
        }
        else // end first
        {
            // end
            next_block_point(1, ref point);
            dest_quadrant = get_quadrant(point[0], point[1], weighted_num);
            left_path_size[0] = calc_distance_to_quadrant(dest_quadrant, point[0], point[1], 0, 0, 1);
            left_path_size[1] = calc_distance_to_quadrant(dest_quadrant, point[0], point[1], 0, 1, 1);
            right_path_size[0] = calc_distance_to_quadrant(dest_quadrant, point[0], point[1], 1, 0, 1);
            right_path_size[1] = calc_distance_to_quadrant(dest_quadrant, point[0], point[1], 1, 1, 1);
            quadrant_to_point(dest_quadrant, ref quad_point);
            if (!evaluate_root(left_path_size[0], left_path_size[1], right_path_size[0], right_path_size[1], 0)) // 왼쪽으로 진행
            {
                if (go_or_turn(left_path_size[0], left_path_size[1], weighted_num)) // 한블럭 진행이 아닌 사분면의 포인터까지 진행
                {
                    go_block_num = rand_go_block_num(left_path_size[0], left_path_size[1], weighted_num);
                    reset_dfs_arr(0);
                    map_array[point[1]][point[0]] = true;
                    dfs_temp_map_array[point[1]][point[0]] = true;
                    only_dfs_dir(0, 1, ref cna);
                    points.AddLast(new Vector3(abs_x(point[0]) * 4, monster_height, -4 * abs_y(point[1])));
                    dfs_fill_path(quad_point[0], quad_point[1], point[0] + cna[0], point[1] + cna[1], 2, go_block_num, 1);
                }
                else // 한블럭 진행
                {
                    map_array[point[1]][point[0]] = true;
                    end_point[0] = point[0];
                    end_point[1] = point[1];
                    end_point[2] = find_dir(point[0], point[1]);
                    points.AddLast(new Vector3(abs_x(point[0]) * 4, monster_height, -4 * abs_y(point[1])));
                }
            }
            else // 오른쪽으로 진행
            {
                if (go_or_turn(right_path_size[0], right_path_size[1], weighted_num)) // 한블럭 진행이 아닌 사분면의 포인터까지 진행
                {
                    go_block_num = rand_go_block_num(right_path_size[0], right_path_size[1], weighted_num);
                    reset_dfs_arr(0);
                    map_array[point[1]][point[0]] = true;
                    dfs_temp_map_array[point[1]][point[0]] = true;
                    only_dfs_dir(1, 1, ref cna);
                    points.AddLast(new Vector3(abs_x(point[0]) * 4, monster_height, -4 * abs_y(point[1])));
                    dfs_fill_path(quad_point[0], quad_point[1], point[0] + cna[0], point[1] + cna[1], 2, go_block_num, 1);
                }
                else // 한블럭 진행
                {
                    map_array[point[1]][point[0]] = true;
                    end_point[0] = point[0];
                    end_point[1] = point[1];
                    end_point[2] = find_dir(point[0], point[1]);
                    points.AddLast(new Vector3(abs_x(point[0]) * 4, monster_height, -4 * abs_y(point[1])));
                }
            }
            // start
            next_block_point(0, ref point);
            dest_quadrant = get_quadrant(point[0], point[1], weighted_num);
            left_path_size[0] = calc_distance_to_quadrant(dest_quadrant, point[0], point[1], 0, 0, 0);
            left_path_size[1] = calc_distance_to_quadrant(dest_quadrant, point[0], point[1], 0, 1, 0);
            right_path_size[0] = calc_distance_to_quadrant(dest_quadrant, point[0], point[1], 1, 0, 0);
            right_path_size[1] = calc_distance_to_quadrant(dest_quadrant, point[0], point[1], 1, 1, 0);
            quadrant_to_point(dest_quadrant, ref quad_point);
            if (!evaluate_root(left_path_size[0], left_path_size[1], right_path_size[0], right_path_size[1], weighted_num)) // 왼쪽으로 진행
            {
                if (go_or_turn(left_path_size[0], left_path_size[1], weighted_num)) // 한블럭 진행이 아닌 사분면의 포인터까지 진행
                {
                    go_block_num = rand_go_block_num(left_path_size[0], left_path_size[1], weighted_num);
                    reset_dfs_arr(0);
                    map_array[point[1]][point[0]] = true;
                    dfs_temp_map_array[point[1]][point[0]] = true;
                    only_dfs_dir(0, 0, ref cna);
                    points.AddFirst(new Vector3(abs_x(point[0]) * 4, monster_height, -4 * abs_y(point[1])));
                    dfs_fill_path(quad_point[0], quad_point[1], point[0] + cna[0], point[1] + cna[1], 2, go_block_num, 0);
                }
                else // 한블럭 진행
                {
                    map_array[point[1]][point[0]] = true;
                    start_point[0] = point[0];
                    start_point[1] = point[1];
                    start_point[2] = find_dir(point[0], point[1]);
                    points.AddFirst(new Vector3(abs_x(point[0]) * 4, monster_height, -4 * abs_y(point[1])));
                }
            }
            else // 오른쪽으로 진행
            {
                if (go_or_turn(right_path_size[0], right_path_size[1], weighted_num)) // 한블럭 진행이 아닌 사분면의 포인터까지 진행
                {
                    go_block_num = rand_go_block_num(right_path_size[0], right_path_size[1], weighted_num);
                    reset_dfs_arr(0);
                    map_array[point[1]][point[0]] = true;
                    dfs_temp_map_array[point[1]][point[0]] = true;
                    only_dfs_dir(1, 0, ref cna);
                    points.AddFirst(new Vector3(abs_x(point[0]) * 4, monster_height, -4 * abs_y(point[1])));
                    dfs_fill_path(quad_point[0], quad_point[1], point[0] + cna[0], point[1] + cna[1], 2, go_block_num, 0);
                }
                else // 한블럭 진행
                {
                    map_array[point[1]][point[0]] = true;
                    start_point[0] = point[0];
                    start_point[1] = point[1];
                    start_point[2] = find_dir(point[0], point[1]);
                    points.AddFirst(new Vector3(abs_x(point[0]) * 4, monster_height, -4 * abs_y(point[1])));
                }
            }
        }
        current_map_size += 2;

        foreach (Component t_component in StartPoint.GetComponentsInChildren<Renderer>())
        {
            if(t_component.GetComponent<ParticleSystem>()!=null)
            {
                t_component.GetComponent<ParticleSystem>().Clear();
                t_component.GetComponent<ParticleSystem>().Stop();
            }
            foreach (Material t_material in t_component.GetComponent<Renderer>().materials)
            {
                t_material.SetFloat("_Tweak_transparency", -1);
            }
        }
        foreach (Component t_component in EndPoint.GetComponentsInChildren<Renderer>())
        {
            if (t_component.GetComponent<ParticleSystem>() != null)
            {
                t_component.GetComponent<ParticleSystem>().Clear();
                t_component.GetComponent<ParticleSystem>().Stop();
            }
            foreach (Material t_material in t_component.GetComponent<Renderer>().materials)
            {
                t_material.SetFloat("_Tweak_transparency", -1);
            }
        }

        EndPoint.transform.position = new Vector3(4 * abs_x(end_point[0]), GroundSetHeight + GroundPrefab.transform.localScale.y, -4 * abs_y(end_point[1]));
        EndPoint.transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
        StartPoint.transform.position = new Vector3(4 * abs_x(start_point[0]), GroundSetHeight + GroundPrefab.transform.localScale.y, -4 * abs_y(start_point[1]));
        foreach (Component t_component in StartPoint.GetComponentsInChildren<Renderer>())
        {
            foreach (Material t_material in t_component.GetComponent<Renderer>().materials)
            {
                t_material.SetFloat("_Tweak_transparency", -1);
            }
        }
        //foreach (Component t_component in EndPoint.GetComponentsInChildren<Renderer>())
        //{
        //    foreach (Material t_material in t_component.GetComponent<Renderer>().materials)
        //    {
        //        t_material.SetFloat("_Tweak_transparency", -1);
        //    }
        //}

        Vector3 temp_vect;
        LinkedListNode<Vector3> temp_node;
        temp_node = points.First;
        temp_vect = (temp_node.Next.Value - temp_node.Value)/4;

        StartPoint.transform.rotation = UnityEngine.Quaternion.Lerp(StartPoint.transform.rotation, UnityEngine.Quaternion.LookRotation(temp_vect), 10);
        /*
        if(temp_vect==Vector3.forward)
        {
            StartPoint.transform.rotation = UnityEngine.Quaternion.Euler(0, 180, 0);
        }
        else if(temp_vect==Vector3.back)
        {
            StartPoint.transform.rotation = UnityEngine.Quaternion.Euler(0, -180, 0);
        }
        else if(temp_vect==Vector3.left)
        {
            StartPoint.transform.rotation = UnityEngine.Quaternion.Euler(0, 90, 0);
        }
        else if(temp_vect==Vector3.right)
        {
            StartPoint.transform.rotation = UnityEngine.Quaternion.Euler(0, -90, 0);
        }
        */
        temp_node = points.Last;
        temp_vect = (temp_node.Value - temp_node.Previous.Value) / 4;
        EndPoint.transform.rotation = UnityEngine.Quaternion.Lerp(EndPoint.transform.rotation, UnityEngine.Quaternion.LookRotation(temp_vect), 10);
        /*
        if (temp_vect == Vector3.forward)
        {
            EndPoint.transform.rotation = UnityEngine.Quaternion.Euler(0, 180, 0);
        }
        else if (temp_vect == Vector3.back)
        {
            EndPoint.transform.rotation = UnityEngine.Quaternion.Euler(0, -180, 0);
        }
        else if (temp_vect == Vector3.left)
        {
            EndPoint.transform.rotation = UnityEngine.Quaternion.Euler(0, 90, 0);
        }
        else if (temp_vect == Vector3.right)
        {
            EndPoint.transform.rotation = UnityEngine.Quaternion.Euler(0, -90, 0);
        }
        */

        for (int r = 0; r < 2; r++)
            for (int i = (MAX_MAP_SIZE - current_map_size) / 2; i < (MAX_MAP_SIZE - current_map_size) / 2 + current_map_size; i++)
            {
                map_block_generate(i, (MAX_MAP_SIZE - current_map_size) / 2 + r * (current_map_size - 1));
            }
        for (int r = 0; r < 2; r++)
            for (int i = (MAX_MAP_SIZE - current_map_size) / 2+1; i < (MAX_MAP_SIZE - current_map_size) / 2 + current_map_size-1; i++)
            {
                map_block_generate((MAX_MAP_SIZE - current_map_size) / 2 + r * (current_map_size - 1), i);
            }
        if(runningCoroutine!=null)
        {
            StopCoroutine("MapBlockRaiseFunc");
        }
        runningCoroutine=StartCoroutine("MapBlockRaiseFunc");
    }

    public void get_start_point(ref int[] t_arr) // return start_point, input array's adress
    {
        t_arr[0] = start_point[0];
        t_arr[1] = start_point[1];
    }

    public void get_end_point(ref int[] t_arr) // return end_point, input array's adress
    {
        t_arr[0] = end_point[0];
        t_arr[1] = end_point[1];
    }

    public int get_map_size() // return map_size
    {
        return current_map_size;
    }

    public void output_map()
    {
        string output_text = "";
        for (int i = (MAX_MAP_SIZE - current_map_size) / 2; i < (MAX_MAP_SIZE - current_map_size) / 2 + current_map_size; i++)
        {
            for (int j = (MAX_MAP_SIZE - current_map_size) / 2; j < (MAX_MAP_SIZE - current_map_size) / 2 + current_map_size; j++)
                if (map_array[i][j])
                {
                    output_text += "#";
                }
                else
                {
                    output_text += "0";
                }
            output_text += "\r\n";
        }
    }

    public void Map_start()
    {
        for (int i = 0; i < MAX_MAP_SIZE; i++)
        {
            map_array.Add(new List<bool>());
            dfs_temp_map_array.Add(new List<bool>());
            map_height.Add(new List<float>());
            enviroment_map.Add(new List<int>());
            map_Block_Raises.Add(new List<map_block_raise>());
            for (int j = 0; j < MAX_MAP_SIZE; j++)
            {
                map_array[i].Add(false);
                dfs_temp_map_array[i].Add(false);
                map_height[i].Add(map_set_height);
                enviroment_map[i].Add(0);
                map_Block_Raises[i].Add(new map_block_raise());
            }
        }
        for (int i = 0; i < MAX_MAP_SIZE; i++)
            for (int j = 0; j < MAX_MAP_SIZE; j++)
            {
                if (UnityEngine.Random.Range(0, 5) == 0)
                {
                    map_height[i][j] = UnityEngine.Random.Range(1.0f, 3.0f);
                }
                if (UnityEngine.Random.Range(0, 10) == 0)
                {
                    enviroment_map[i][j] = 1;
                }
            }
        NodeSetHeight = GroundSetHeight + map_set_height / 2;
        for (int k = 0; k < 3; k++)
            for (int i = 2; i < MAX_MAP_SIZE - 2; i++)
                for (int j = 2; j < MAX_MAP_SIZE - 2; j++)
                    Get_map_height(j, i);
        
        current_map_size = 2;
        System.Random rand = new System.Random();
        int rand_num = rand.Next();
        int rand_number = rand.Next();
        for (int i = 0; i < MAX_MAP_SIZE; i++) // map_array reset
            for (int j = 0; j < MAX_MAP_SIZE; j++)
                map_array[i][j] = false;
        // make a 2 x 2 random map
        rand_num = rand_num % 2 + 1; // 길 갯수 2~3
        int[] rand_num_arr = new int[2];
        int[] rand_num_start = new int[3];
        if (rand_num == 0) // 길 1개(버그로 인해 삭제)
        {                  /*
                              rand_num_arr[0] = rand() % 2; // x
                              rand_num_arr[1] = rand() % 2; // y
                              rand_num_start[0] = rand() % 2;
                              map_array[(MAX_MAP_SIZE - current_map_size) / 2 + rand_num_arr[1]][(MAX_MAP_SIZE - current_map_size) / 2 + rand_num_arr[0]] = true;
                              start_point[0] = (MAX_MAP_SIZE - current_map_size) / 2 + rand_num_arr[0];
                              start_point[1] = (MAX_MAP_SIZE - current_map_size) / 2 + rand_num_arr[1];
                              end_point[0] = start_point[0];
                              end_point[1] = start_point[1];
                 
                              if (rand_num_arr[0] == 0) // 길x위치
                              {
                                  if (rand_num_arr[1] == 0) // 길y위치
                                  {
                                      if (rand_num_start[0] == 0)
                                      {
                                          start_point[2] = 0;
                                          end_point[2] = 1;
                                      }
                                      else
                                      {
                                          start_point[2] = 1;
                                          end_point[2] = 0;
                                      }
                                  }
                                  else
                                  {
                                      if (rand_num_start[0] == 0)
                                      {
                                          start_point[2] = 0;
                                          end_point[2] = 3;
                                      }
                                      else
                                      {
                                          start_point[2] = 3;
                                          end_point[2] = 0;
                                      }
                                  }
                              }
                              else
                              {
                                  if (rand_num_arr[1] == 0)
                                  {
                                      if (rand_num_start[0] == 0)
                                      {
                                          start_point[2] = 1;
                                          end_point[2] = 2;
                                      }
                                      else
                                      {
                                          start_point[2] = 2;
                                          end_point[2] = 1;
                                      }
                                  }
                                  else
                                  {
                                      if (rand_num_start[0] == 0)
                                      {
                                          start_point[2] = 2;
                                          end_point[2] = 3;
                                      }
                                      else
                                      {
                                          start_point[2] = 3;
                                          end_point[2] = 2;
                                      }
                                  }
                              }
                              */
        }
        else if (rand_num == 1) // 길 2개
        {
            rand_num_arr[0] = rand_number % 4;
            rand_num_arr[1] = rand_number % 2;
            rand_num_start[0] = rand_number % 2;
            rand_num_start[1] = rand_number % 2;
            if (rand_num_arr[0] == 0) // 기준점 왼쪽위
            {
                if (rand_num_arr[1] == 0) // 추가된 길이 x+1경우
                {
                    map_array[(MAX_MAP_SIZE - current_map_size) / 2][(MAX_MAP_SIZE - current_map_size) / 2] = true;
                    map_array[(MAX_MAP_SIZE - current_map_size) / 2][(MAX_MAP_SIZE - current_map_size) / 2 + 1] = true;
                    start_point[0] = (MAX_MAP_SIZE - current_map_size) / 2;
                    start_point[1] = (MAX_MAP_SIZE - current_map_size) / 2;
                    end_point[0] = (MAX_MAP_SIZE - current_map_size) / 2 + 1;
                    end_point[1] = (MAX_MAP_SIZE - current_map_size) / 2;
                    if (rand_num_start[0] == 0)
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 0;
                            end_point[2] = 1;
                        }
                        else
                        {
                            start_point[2] = 0;
                            end_point[2] = 2;
                        }
                    }
                    else
                    {
                        start_point[2] = 1;
                        end_point[2] = 2;
                    }
                }
                else // 추가된 길이 y+1인 경우
                {
                    map_array[(MAX_MAP_SIZE - current_map_size) / 2][(MAX_MAP_SIZE - current_map_size) / 2] = true;
                    map_array[(MAX_MAP_SIZE - current_map_size) / 2 + 1][(MAX_MAP_SIZE - current_map_size) / 2] = true;
                    start_point[0] = (MAX_MAP_SIZE - current_map_size) / 2;
                    start_point[1] = (MAX_MAP_SIZE - current_map_size) / 2;
                    end_point[0] = (MAX_MAP_SIZE - current_map_size) / 2;
                    end_point[1] = (MAX_MAP_SIZE - current_map_size) / 2 + 1;
                    if (rand_num_start[0] == 0)
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 1;
                            end_point[2] = 0;
                        }
                        else
                        {
                            start_point[2] = 1;
                            end_point[2] = 3;
                        }
                    }
                    else
                    {
                        start_point[2] = 0;
                        end_point[2] = 3;
                    }
                }
            }
            else if (rand_num_arr[0] == 1) // 기준점 오른쪽위
            {
                if (rand_num_arr[1] == 0)
                {
                    map_array[(MAX_MAP_SIZE - 2) / 2][(MAX_MAP_SIZE - 2) / 2 + 1] = true;
                    map_array[(MAX_MAP_SIZE - 2) / 2][(MAX_MAP_SIZE - 2) / 2] = true;
                    start_point[0] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    start_point[1] = (MAX_MAP_SIZE - 2) / 2;
                    end_point[0] = (MAX_MAP_SIZE - 2) / 2;
                    end_point[1] = (MAX_MAP_SIZE - 2) / 2;
                    if (rand_num_start[0] == 0)
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 2;
                            end_point[2] = 0;
                        }
                        else
                        {
                            start_point[2] = 2;
                            end_point[2] = 1;
                        }
                    }
                    else
                    {
                        start_point[2] = 1;
                        end_point[2] = 0;
                    }
                }
                else
                {
                    map_array[(MAX_MAP_SIZE - 2) / 2 ][(MAX_MAP_SIZE - 2) / 2+1] = true;
                    map_array[(MAX_MAP_SIZE - 2) / 2 + 1][(MAX_MAP_SIZE - 2) / 2 + 1] = true;
                    start_point[0] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    start_point[1] = (MAX_MAP_SIZE - 2) / 2;
                    end_point[0] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    end_point[1] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    if (rand_num_start[0] == 0)
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 1;
                            end_point[2] = 2;
                        }
                        else
                        {
                            start_point[2] = 1;
                            end_point[2] = 3;
                        }
                    }
                    else
                    {
                        start_point[2] = 2;
                        end_point[2] = 3;
                    }
                }
            }
            else if (rand_num_arr[0] == 2) // 기준점 왼쪽아래
            {
                if (rand_num_arr[1] == 0)
                {
                    map_array[(MAX_MAP_SIZE - 2) / 2 + 1][(MAX_MAP_SIZE - 2) / 2] = true;
                    map_array[(MAX_MAP_SIZE - 2) / 2][(MAX_MAP_SIZE - 2) / 2] = true;
                    start_point[0] = (MAX_MAP_SIZE - 2) / 2;
                    start_point[1] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    end_point[0] = (MAX_MAP_SIZE - 2) / 2;
                    end_point[1] = (MAX_MAP_SIZE - 2) / 2;
                    if (rand_num_start[0] == 0)
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 3;
                            end_point[2] = 1;
                        }
                        else
                        {
                            start_point[2] = 3;
                            end_point[2] = 0;
                        }
                    }
                    else
                    {
                        start_point[2] = 0;
                        end_point[2] = 1;
                    }
                }
                else
                {
                    map_array[(MAX_MAP_SIZE - 2) / 2 + 1][(MAX_MAP_SIZE - 2) / 2] = true;
                    map_array[(MAX_MAP_SIZE - 2) / 2 + 1][(MAX_MAP_SIZE - 2) / 2 + 1] = true;
                    start_point[0] = (MAX_MAP_SIZE - 2) / 2;
                    start_point[1] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    end_point[0] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    end_point[1] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    if (rand_num_start[0] == 0)
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 0;
                            end_point[2] = 3;
                        }
                        else
                        {
                            start_point[2] = 0;
                            end_point[2] = 2;
                        }
                    }
                    else
                    {
                        start_point[2] = 3;
                        end_point[2] = 2;
                    }
                }
            }
            else // 기준점 오른쪽 아래
            {
                if (rand_num_arr[1] == 0)
                {
                    map_array[(MAX_MAP_SIZE - 2) / 2 + 1][(MAX_MAP_SIZE - 2) / 2 + 1] = true;
                    map_array[(MAX_MAP_SIZE - 2) / 2 + 1][(MAX_MAP_SIZE - 2) / 2] = true;
                    start_point[0] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    start_point[1] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    end_point[0] = (MAX_MAP_SIZE - 2) / 2;
                    end_point[1] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    if (rand_num_start[0] == 0)
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 2;
                            end_point[2] = 0;
                        }
                        else
                        {
                            start_point[2] = 2;
                            end_point[2] = 3;
                        }
                    }
                    else
                    {
                        start_point[2] = 3;
                        end_point[2] = 0;
                    }
                }
                else
                {
                    map_array[(MAX_MAP_SIZE - 2) / 2 + 1][(MAX_MAP_SIZE - 2) / 2 + 1] = true;
                    map_array[(MAX_MAP_SIZE - 2) / 2][(MAX_MAP_SIZE - 2) / 2 + 1] = true;
                    start_point[0] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    start_point[1] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    end_point[0] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    end_point[1] = (MAX_MAP_SIZE - 2) / 2;
                    if (rand_num_start[0] == 0)
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 3;
                            end_point[2] = 1;
                        }
                        else
                        {
                            start_point[2] = 3;
                            end_point[2] = 2;
                        }
                    }
                    else
                    {
                        start_point[2] = 2;
                        end_point[2] = 1;
                    }
                }
            }
        }
        else if (rand_num == 2) // 길 3개
        {
            rand_num_arr[0] = rand_number % 4;
            rand_num_start[0] = rand_number % 2;
            rand_num_start[1] = rand_number % 2;
            rand_num_start[2] = rand_number % 2;
            if (rand_num_arr[0] == 0)
            {
                map_array[(MAX_MAP_SIZE - 2) / 2][(MAX_MAP_SIZE - 2) / 2 + 1] = true;
                map_array[(MAX_MAP_SIZE - 2) / 2 + 1][(MAX_MAP_SIZE - 2) / 2] = true;
                map_array[(MAX_MAP_SIZE - 2) / 2 + 1][(MAX_MAP_SIZE - 2) / 2 + 1] = true;
                if (rand_num_start[2] == 0)
                {
                    start_point[0] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    start_point[1] = (MAX_MAP_SIZE - 2) / 2;
                    end_point[0] = (MAX_MAP_SIZE - 2) / 2;
                    end_point[1] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    if (rand_num_start[0] == 0)
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 1;
                            end_point[2] = 3;
                        }
                        else
                        {
                            start_point[2] = 1;
                            end_point[2] = 0;
                        }
                    }
                    else
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 2;
                            end_point[2] = 3;
                        }
                        else
                        {
                            start_point[2] = 2;
                            end_point[2] = 0;
                        }
                    }
                }
                else
                {
                    start_point[0] = (MAX_MAP_SIZE - 2) / 2;
                    start_point[1] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    end_point[0] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    end_point[1] = (MAX_MAP_SIZE - 2) / 2;
                    if (rand_num_start[0] == 0)
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 3;
                            end_point[2] = 1;
                        }
                        else
                        {
                            start_point[2] = 0;
                            end_point[2] = 1;
                        }
                    }
                    else
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 3;
                            end_point[2] = 2;
                        }
                        else
                        {
                            start_point[2] = 0;
                            end_point[2] = 2;
                        }
                    }
                }
            }
            else if (rand_num_arr[0] == 1)
            {
                map_array[(MAX_MAP_SIZE - 2) / 2][(MAX_MAP_SIZE - 2) / 2] = true;
                map_array[(MAX_MAP_SIZE - 2) / 2 + 1][(MAX_MAP_SIZE - 2) / 2] = true;
                map_array[(MAX_MAP_SIZE - 2) / 2 + 1][(MAX_MAP_SIZE - 2) / 2 + 1] = true;
                if (rand_num_start[2] == 0)
                {
                    start_point[0] = (MAX_MAP_SIZE - 2) / 2;
                    start_point[1] = (MAX_MAP_SIZE - 2) / 2;
                    end_point[0] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    end_point[1] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    if (rand_num_start[0] == 0)
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 0;
                            end_point[2] = 2;
                        }
                        else
                        {
                            start_point[2] = 0;
                            end_point[2] = 3;
                        }
                    }
                    else
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 1;
                            end_point[2] = 2;
                        }
                        else
                        {
                            start_point[2] = 1;
                            end_point[2] = 3;
                        }
                    }
                }
                else
                {
                    start_point[0] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    start_point[1] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    end_point[0] = (MAX_MAP_SIZE - 2) / 2;
                    end_point[1] = (MAX_MAP_SIZE - 2) / 2;
                    if (rand_num_start[0] == 0)
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 2;
                            end_point[2] = 0;
                        }
                        else
                        {
                            start_point[2] = 3;
                            end_point[2] = 0;
                        }
                    }
                    else
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 2;
                            end_point[2] = 1;
                        }
                        else
                        {
                            start_point[2] = 3;
                            end_point[2] = 1;
                        }
                    }
                }
            }
            else if (rand_num_arr[0] == 2)
            {
                map_array[(MAX_MAP_SIZE - 2) / 2][(MAX_MAP_SIZE - 2) / 2] = true;
                map_array[(MAX_MAP_SIZE - 2) / 2][(MAX_MAP_SIZE - 2) / 2 + 1] = true;
                map_array[(MAX_MAP_SIZE - 2) / 2 + 1][(MAX_MAP_SIZE - 2) / 2 + 1] = true;
                if (rand_num_start[2] == 0)
                {
                    start_point[0] = (MAX_MAP_SIZE - 2) / 2;
                    start_point[1] = (MAX_MAP_SIZE - 2) / 2;
                    end_point[0] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    end_point[1] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    if (rand_num_start[0] == 0)
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 0;
                            end_point[2] = 2;
                        }
                        else
                        {
                            start_point[2] = 0;
                            end_point[2] = 3;
                        }
                    }
                    else
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 1;
                            end_point[2] = 2;
                        }
                        else
                        {
                            start_point[2] = 1;
                            end_point[2] = 3;
                        }
                    }
                }
                else
                {
                    start_point[0] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    start_point[1] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    end_point[0] = (MAX_MAP_SIZE - 2) / 2;
                    end_point[1] = (MAX_MAP_SIZE - 2) / 2;
                    if (rand_num_start[0] == 0)
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 2;
                            end_point[2] = 0;
                        }
                        else
                        {
                            start_point[2] = 3;
                            end_point[2] = 0;
                        }
                    }
                    else
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 2;
                            end_point[2] = 1;
                        }
                        else
                        {
                            start_point[2] = 3;
                            end_point[2] = 1;
                        }
                    }
                }
            }
            else
            {
                map_array[(MAX_MAP_SIZE - 2) / 2][(MAX_MAP_SIZE - 2) / 2] = true;
                map_array[(MAX_MAP_SIZE - 2) / 2][(MAX_MAP_SIZE - 2) / 2 + 1] = true;
                map_array[(MAX_MAP_SIZE - 2) / 2 + 1][(MAX_MAP_SIZE - 2) / 2] = true;
                if (rand_num_start[2] == 0)
                {
                    start_point[0] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    start_point[1] = (MAX_MAP_SIZE - 2) / 2;
                    end_point[0] = (MAX_MAP_SIZE - 2) / 2;
                    end_point[1] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    if (rand_num_start[0] == 0)
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 2;
                            end_point[2] = 3;
                        }
                        else
                        {
                            start_point[2] = 2;
                            end_point[2] = 0;
                        }
                    }
                    else
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 1;
                            end_point[2] = 3;
                        }
                        else
                        {
                            start_point[2] = 1;
                            end_point[2] = 0;
                        }
                    }
                }
                else
                {
                    start_point[0] = (MAX_MAP_SIZE - 2) / 2;
                    start_point[1] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    end_point[0] = (MAX_MAP_SIZE - 2) / 2 + 1;
                    end_point[1] = (MAX_MAP_SIZE - 2) / 2;
                    if (rand_num_start[0] == 0)
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 3;
                            end_point[2] = 2;
                        }
                        else
                        {
                            start_point[2] = 0;
                            end_point[2] = 2;
                        }
                    }
                    else
                    {
                        if (rand_num_start[1] == 0)
                        {
                            start_point[2] = 3;
                            end_point[2] = 1;
                        }
                        else
                        {
                            start_point[2] = 0;
                            end_point[2] = 1;
                        }
                    }
                }
            }
        }
    }

    GameObject tempObject;
    public static LinkedList<Vector3> points;
    public Vector3 temp_struct_speed;

    //void Awake()
    //{
    //    points = new LinkedList<Vector3>();
    //    Map_start();
    //    reset_dfs_arr(0);
    //    fill_map_node(start_point[0], start_point[1]);
    //    for (int i = (MAX_MAP_SIZE - current_map_size) / 2; i < (MAX_MAP_SIZE - current_map_size) / 2 + current_map_size; i++)
    //        for (int j = (MAX_MAP_SIZE - current_map_size) / 2; j < (MAX_MAP_SIZE - current_map_size) / 2 + current_map_size; j++)
    //        {
    //            map_block_generate(j, i);
    //        }
    //    for (int i = 0; i < 5; i++)
    //        expand_map();
    //}
}