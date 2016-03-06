using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CalcLine : MonoBehaviour
{
    GameObject mPrefabHex;
    GameObject mRedPrefabHex;

    class LayerHex
    {
        public GameObject hex;
        public int layer;
        public int indexOfArray;
        public float x, y;
    }
    //每层hex个数 5,6,7,8,9,8,7,6,5
    LayerHex[] mLayerHex = new LayerHex[61];
    float mHexColliderR, mHexColliderD;
    GameObject[] mLayerFilledHexs = new GameObject[61];

    // Use this for initialization
    void Start()
    {
        mRedPrefabHex = GameObject.Find("RedPrefab");
        mPrefabHex = GameObject.Find("BGFirstHex");
        mHexColliderR = mPrefabHex.gameObject.GetComponent<CircleCollider2D>().bounds.size.x / 2;
        mHexColliderD = Mathf.Sqrt(3.0f) / 2.0f * mHexColliderR;
        //每一层hex的距离
        float layerSpace = 3.0f / 2.0f * mHexColliderR;

        LayerHex firstHex = new LayerHex();
        firstHex.x = mPrefabHex.transform.position.x;
        firstHex.y = mPrefabHex.transform.position.y;
        int layer = 0, indexOfAllHex = 0;
        for (int i = 5; i < 10; i++)
        {
            for (int j = 0; j < i; j++)
            {
                initAboveLayers(layer, j, indexOfAllHex, layerSpace, firstHex);
                indexOfAllHex++;
            }
            layer++;
        }

        for (int i = 8; i > 4; i--)
        {
            for (int j = 0; j < i; j++)
            {
                initUnderLayers(layer, j, indexOfAllHex, layerSpace, firstHex);
                indexOfAllHex++;
            }
            layer++;
        }

        foreach (LayerHex layerHex in mLayerHex)
        {
            Vector3 position = new Vector3(layerHex.x, layerHex.y, 0);
            Instantiate(mPrefabHex, position, mPrefabHex.transform.rotation);
        }
    }

    void initAboveLayers(int layer, int indexOfLayer, int indexOfAllHex, float layerSpace, LayerHex firstHex)
    {
        LayerHex layerHex = new LayerHex();
        layerHex.layer = layer;
        layerHex.y = firstHex.y - layer * layerSpace;
        layerHex.x = firstHex.x - layer * mHexColliderD + 2 * mHexColliderD * indexOfLayer;
        mLayerHex[indexOfAllHex] = layerHex;
    }
    void initUnderLayers(int layer, int indexOfLayer, int indexOfAllHex, float layerSpace, LayerHex firstHex)
    {
        LayerHex layerHex = new LayerHex();
        layerHex.layer = layer;
        layerHex.y = firstHex.y - layer * layerSpace;
        layerHex.x = firstHex.x - (8 - layer) * mHexColliderD + 2 * mHexColliderD * indexOfLayer;
        mLayerHex[indexOfAllHex] = layerHex;
    }

    public bool addHexsForMap(GameObject[] hexs)
    {
        int len = hexs.Length;
        LayerHex[] hexsPosition = new LayerHex[len];
        for (int i = 0; i < len; i++)
        {
            hexsPosition[i] = searchPositionInMap(hexs[i]);
            if (hexsPosition[i] == null ||  hexsPosition[i].hex != null)
                return false;
        }
        for (int i = 0; i < len; i++)
        {
            //保存用来计算是否练成线，以消除
            mLayerFilledHexs[hexsPosition[i].indexOfArray] = hexs[i];

            Vector3 position = new Vector3(hexsPosition[i].x, hexsPosition[i].y, 0);
            hexs[i].GetComponent<SpriteRenderer>().sortingOrder = 1;
            hexs[i].transform.position = position;
            hexsPosition[i].hex = hexs[i];
            //hexsPosition[i].hex = (GameObject)Instantiate(mRedPrefabHex, position, mRedPrefabHex.transform.rotation);
        }

        calcResult();
        return true;
    }

    List<int> mSuccessHexs = new List<int>();
    public void calcResult()
    {
        mSuccessHexs.Clear();
        List<int> successLine = new List<int>();
        //前5个遍历左斜线和右斜线
        for (int i = 0; i < 5; i++)
        {
            searchLeftLine(i, successLine, 5 + i);
            searchRightLine(i, successLine, 9- i);
        }
        //遍历只有右斜线
        searchRightLine(5, successLine, 8);
        searchRightLine(11, successLine, 7);
        searchRightLine(18, successLine, 6);
        searchRightLine(26, successLine, 5);
        //遍历只有左斜线
        searchLeftLine(10, successLine, 8);
        searchLeftLine(17, successLine, 7);
        searchLeftLine(25, successLine, 6);
        searchLeftLine(34, successLine, 5);
        //遍历横向
        searchHorizontalLine(0, successLine, 5);
        searchHorizontalLine(5, successLine, 6);
        searchHorizontalLine(11, successLine, 7);
        searchHorizontalLine(18, successLine, 8);
        searchHorizontalLine(26, successLine, 9);
        searchHorizontalLine(35, successLine, 8);
        searchHorizontalLine(43, successLine, 7);
        searchHorizontalLine(50, successLine, 6);
        searchHorizontalLine(56, successLine, 5);
        
        //首先删除可能相交的结点，保留一个即可，这是由于有两条线交叉的原因
        int len = mSuccessHexs.Count;
        mSuccessHexs.Sort();
        for (int i = len-1; i >0; i--)
        {
            if (mSuccessHexs[i] == mSuccessHexs[i-1])
            {
                mSuccessHexs.RemoveAt(i);
            }
        }
        len = mSuccessHexs.Count;
        //获取所有练成线的hex，用以执行动画
        GameObject[] allDestroyHexs = new GameObject[len];
        for(int i=0;i< len; i++)
        {
            allDestroyHexs[i] = mLayerFilledHexs[mSuccessHexs[i]];
            mLayerFilledHexs[mSuccessHexs[i]] = null;
        }

        //获取分数的动画
        len = allDestroyHexs.Length;
        Vector3[] scoresPosition = new Vector3[len];
        int[] scoresValues = new int[len];
        for (int i = 0; i < len; i++)
        {
            scoresPosition[i] = allDestroyHexs[i].transform.position;
            scoresValues[i] = 10;
        }
        GameObject.Find("BackgroundScript").GetComponent<ScoreAnimation>().addAnimation(scoresPosition, scoresValues);

        GameObject.Find("BackgroundScript").GetComponent<LoadHexAnimation>().animationLoadHexBS(allDestroyHexs, LoadHexAnimation.ANIMATION_BIG_SMALL);
        
        mSuccessHexs.Clear();
    }
    void searchHorizontalLine(int j, List<int> successLine, int limitHexsIndex)
    {
        for (int i = 0; i < limitHexsIndex && mLayerFilledHexs[j] != null; i++)
        {
            successLine.Add(j);
            j ++;
        }
        if (successLine.Count == limitHexsIndex)
        {
            mSuccessHexs.AddRange(successLine);
        }
        successLine.Clear();
    }
    void searchLeftLine(int j, List<int> successLine, int limitHexsIndex)
    {
        for (int i=0; i < limitHexsIndex && mLayerFilledHexs[j] != null; i++)
        {
            successLine.Add(j);
            j = j + getNextOffset(j);
        }
        if (successLine.Count == limitHexsIndex)
        {
            mSuccessHexs.AddRange(successLine);
        }
        successLine.Clear();
    }
    void searchRightLine( int j, List<int> successLine, int limitHexsIndex)
    {
        for (int i = 0; i < limitHexsIndex && mLayerFilledHexs[j] != null; i++)
        {
            successLine.Add(j);
            j = j + getNextOffset(j) + 1;
        }
        if (successLine.Count == limitHexsIndex)
        {
            mSuccessHexs.AddRange(successLine);
        }
        successLine.Clear();
    }
    int getNextOffset(int index)
    {
        if (index < 5)
            return 5;
        else if (index < 11)
            return 6;
        else if (index < 18)
            return 7;
        else if (index < 26)
            return 8;
        else if (index < 35)
            return 8;
        else if (index < 43)
            return 7;
        else if (index < 50)
            return 6;
        else if (index < 56)
            return 5;

        return 5;
    }

    LayerHex searchPositionInMap(GameObject hex)
    {

        float d = 100000f;
        LayerHex findHex = null;
        int indexOfArray = -1;
        int len = mLayerHex.Length;
        for(int i= 0;i< len;i++)
        {
            LayerHex layerHex = mLayerHex[i];
            float tmp = Mathf.Sqrt(Mathf.Pow((layerHex.x - hex.transform.position.x), 2) 
                + Mathf.Pow((layerHex.y - hex.transform.position.y), 2));
            if (d > tmp)
            {
                d = tmp;
                layerHex.indexOfArray = i;
                findHex = layerHex;
                indexOfArray = i;
            }
        }
        if (d > mHexColliderD)
            return null;
        
        return findHex;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
