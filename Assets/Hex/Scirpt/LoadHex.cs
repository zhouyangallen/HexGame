using UnityEngine;
using System.Collections;

public class LoadHex : MonoBehaviour
{

    //private static int HEX_SINGLE_MAX = 4;
    private static int HEX_COUNT_ARRAY = 3;
    //1左边，2右边，3左上，4右上，5左下，6右下
    private static int HEX_CENTER_SIDE = 0;
    private static int HEX_LEFT_SIDE = 1;
    private static int HEX_RIGHT_SIDE = 2;
    private static int HEX_LEFT_UP_SIDE = 3;
    private static int HEX_RIGHT_UP_SIDE = 4;
    private static int HEX_LEFT_DOWN_SIDE = 5;
    private static int HEX_RIGHT_DOWN_SIDE = 6;

    //选中后hex方体放大，上移到手指上方才能看清楚
    private const int mFingerOffset = 4;
    private int isHaveTouched = -1;//0是按下，但是不在空间区域内，1是在区域内
    private Vector3 mouseFirstPosition;
    private Camera mCamera;
    private int mTouchIndex;
    private float mHexColliderSizeNormal = 0;
    private float mHexColliderSizeSmall = 0;
    //上一次加载的hex颜色
    private int mPriviousColor = -1;


    HexObjectsArray[] mHexObjectsArray = new HexObjectsArray[HEX_COUNT_ARRAY];
    //用以实例化的hex profabs
    private GameObject[] HexPrefabs = new GameObject[5];

    //在当前hex的六个方位角度计算
    static Vector3 LeftUp = new Vector2(-Mathf.Sqrt(3.0f) / 4.0f, 3.0f / 4.0f);
    static Vector3 RightUp = new Vector2(Mathf.Sqrt(3.0f) / 4.0f, 3.0f / 4.0f);
    static Vector3 LeftDown = new Vector2(-Mathf.Sqrt(3.0f) / 4.0f, -3.0f / 4.0f);
    static Vector3 RightDown = new Vector2(Mathf.Sqrt(3.0f) / 4.0f, -3.0f / 4.0f);
    static Vector3 Left = new Vector2(-1, 0);
    static Vector3 Right = new Vector2(1, 0);
    //三个hex方体的初始位置
    Vector3[] LoadHexPosition = new Vector3[HEX_COUNT_ARRAY];

    //hex方体种类
    public static int[][] AllHexArray = {
        new int[]{ HEX_CENTER_SIDE },
        new int[]{ HEX_CENTER_SIDE, HEX_RIGHT_SIDE, HEX_LEFT_DOWN_SIDE, HEX_LEFT_DOWN_SIDE },
        new int[]{ HEX_CENTER_SIDE, HEX_RIGHT_UP_SIDE, HEX_RIGHT_UP_SIDE, HEX_RIGHT_DOWN_SIDE },
        new int[]{ HEX_CENTER_SIDE, HEX_RIGHT_DOWN_SIDE, HEX_LEFT_DOWN_SIDE, HEX_LEFT_SIDE },
        new int[]{ HEX_CENTER_SIDE, HEX_LEFT_SIDE, HEX_LEFT_SIDE, HEX_LEFT_SIDE },
        new int[]{ HEX_CENTER_SIDE, HEX_RIGHT_UP_SIDE, HEX_RIGHT_SIDE, HEX_RIGHT_DOWN_SIDE },
        new int[]{ HEX_CENTER_SIDE, HEX_LEFT_UP_SIDE, HEX_RIGHT_UP_SIDE, HEX_RIGHT_DOWN_SIDE },
        new int[]{ HEX_CENTER_SIDE, HEX_RIGHT_SIDE, HEX_LEFT_DOWN_SIDE, HEX_RIGHT_SIDE },
        new int[]{ HEX_CENTER_SIDE, HEX_RIGHT_SIDE, HEX_LEFT_UP_SIDE, HEX_RIGHT_SIDE },
        new int[]{ HEX_CENTER_SIDE, HEX_RIGHT_UP_SIDE, HEX_RIGHT_UP_SIDE, HEX_RIGHT_UP_SIDE },
        new int[]{ HEX_CENTER_SIDE, HEX_RIGHT_DOWN_SIDE, HEX_RIGHT_DOWN_SIDE, HEX_RIGHT_DOWN_SIDE },
        new int[]{ HEX_CENTER_SIDE, HEX_LEFT_DOWN_SIDE, HEX_LEFT_DOWN_SIDE, HEX_RIGHT_SIDE },
        new int[]{ HEX_CENTER_SIDE, HEX_RIGHT_DOWN_SIDE, HEX_RIGHT_DOWN_SIDE, HEX_LEFT_SIDE },
        new int[]{ HEX_CENTER_SIDE, HEX_LEFT_DOWN_SIDE, HEX_RIGHT_DOWN_SIDE, HEX_RIGHT_SIDE },
        new int[]{ HEX_CENTER_SIDE, HEX_RIGHT_DOWN_SIDE, HEX_RIGHT_SIDE, HEX_RIGHT_UP_SIDE }
    };
    //下3个hex,1左边，2右边，3左上，4右上，5左下，6右下
    public static int[][] HexArray = {
        new int[]{ HEX_CENTER_SIDE },
        new int[]{ HEX_CENTER_SIDE, HEX_RIGHT_DOWN_SIDE, HEX_LEFT_DOWN_SIDE, HEX_LEFT_SIDE },
        new int[]{ HEX_CENTER_SIDE, HEX_LEFT_SIDE, HEX_LEFT_SIDE, HEX_LEFT_SIDE } };

    //当前的3个hex,用于整体移动
    class HexObjectsArray
    {
        public GameObject[] mHexObject;
        private Vector3[] mOriginPosition;
        //变大后的位置
        private Vector3[] mBigPosition;
        private int[] mHexType;
        //初始化单个hex方体
        public void initPrefabs(Vector3 originPosition, GameObject HexPrefab, float colliderSize, int[] hexType)
        {
            mHexType = hexType;
            int hexCount = hexType.Length;
            mHexObject = new GameObject[hexCount];
            mOriginPosition = new Vector3[hexCount];
            mBigPosition = new Vector3[hexCount];

            //float r = HexPrefab.gameObject.GetComponent<CircleCollider2D>().bounds.size.x;
            float d = Mathf.Sqrt(3.0f) / 2.0f * colliderSize;
            Vector3 newLeftUp = LeftUp * colliderSize;
            Vector3 newRightUp = RightUp * colliderSize;
            Vector3 newLeft = Left * d;
            Vector3 newRight = Right * d;
            Vector3 newLeftDown = LeftDown * colliderSize;
            Vector3 newRightDown = RightDown * colliderSize;
            //每个子hex的偏移，偏移值都是基于上个hex的位置坐偏移
            
            Vector3 offsetPosition = new Vector3(0, 0, 0);
            for (int i = 0; i< hexCount; i++)
            {
                if (hexType[i] == HEX_CENTER_SIDE)
                {
                    mHexObject[i] = (GameObject)Instantiate(HexPrefab, originPosition, HexPrefab.transform.rotation);
                }
                if (hexType[i] == HEX_LEFT_SIDE)
                {
                    mHexObject[i] = (GameObject)Instantiate(HexPrefab, originPosition, HexPrefab.transform.rotation);
                    mHexObject[i].transform.position = originPosition + offsetPosition + newLeft;
                    offsetPosition += newLeft;
                    //mHexObjectsArray[j].mHexObject[i].tag = ""+j;
                }
                if (hexType[i] == HEX_RIGHT_SIDE)
                {
                    mHexObject[i] = (GameObject)Instantiate(HexPrefab, originPosition, HexPrefab.transform.rotation);
                    mHexObject[i].transform.position = originPosition + offsetPosition + newRight;
                    offsetPosition += newRight;
                    //mHexObjectsArray[j].mHexObject[i].tag = "" + j;
                }
                if (hexType[i] == HEX_LEFT_UP_SIDE)
                {
                    mHexObject[i] = (GameObject)Instantiate(HexPrefab, originPosition, HexPrefab.transform.rotation);
                    mHexObject[i].transform.position = originPosition + offsetPosition + newLeftUp;
                    offsetPosition += newLeftUp;
                    //mHexObjectsArray[j].mHexObject[i].tag = "" + j;
                }
                if (hexType[i] == HEX_RIGHT_UP_SIDE)
                {
                    mHexObject[i] = (GameObject)Instantiate(HexPrefab, originPosition, HexPrefab.transform.rotation);
                    mHexObject[i].transform.position = originPosition + offsetPosition + newRightUp;
                    offsetPosition += newRightUp;
                    //mHexObjectsArray[j].mHexObject[i].tag = "" + j;
                }
                if (hexType[i] == HEX_LEFT_DOWN_SIDE)
                {
                    mHexObject[i] = (GameObject)Instantiate(HexPrefab, originPosition, HexPrefab.transform.rotation);
                    mHexObject[i].transform.position = originPosition + offsetPosition + newLeftDown;
                    offsetPosition += newLeftDown;
                    //mHexObjectsArray[j].mHexObject[i].tag = "" + j;
                }
                if (hexType[i] == HEX_RIGHT_DOWN_SIDE)
                {
                    mHexObject[i] = (GameObject)Instantiate(HexPrefab, originPosition, HexPrefab.transform.rotation);
                    mHexObject[i].transform.position = originPosition + offsetPosition + newRightDown;
                    offsetPosition += newRightDown;
                    //mHexObjectsArray[j].mHexObject[i].tag = "" + j;
                }
                mOriginPosition[i] = mHexObject[i].transform.position;
                mHexObject[i].GetComponent<SpriteRenderer>().sortingOrder = 10;
            }

            adjust(originPosition, true);
        }
        //调整位置到展示的中间
        private void adjust(Vector3 originPosition, bool isFirst)
        {
            //左边，右边，上边，下边的最远距离
            Vector4 calcCenter = new Vector4();
            int len = mHexObject.Length;
            for (int i = 0; i < len; i++)
            {
                Vector3 offset = mHexObject[i].transform.position - originPosition;
                calcCenter.x = offset.x < calcCenter.x ? offset.x : calcCenter.x;
                calcCenter.y = offset.x > calcCenter.y ? offset.x : calcCenter.y;
                calcCenter.z = offset.y > calcCenter.z ? offset.y : calcCenter.z;
                calcCenter.w = offset.y < calcCenter.w ? offset.y : calcCenter.w;
            }
            Vector3 moveOffset = new Vector3(-(calcCenter.x + calcCenter.y) / 2, -(calcCenter.z + calcCenter.w) / 2, mHexObject[0].transform.position.z);
            move(moveOffset);
            int hexCount = mOriginPosition.Length;
            if (isFirst)
            {
                for (int i = 0; i < hexCount; i++)
                    mOriginPosition[i] = mHexObject[i].transform.position;
            }
            else
            {
                for (int i = 0; i < hexCount; i++)
                    mBigPosition[i] = mHexObject[i].transform.position;
            }
        }

        public void move(Vector3 distance)
        {
            int hexCount = mHexObject.Length;
            for (int i = 0; i < hexCount; i++)
                mHexObject[i].transform.position += distance;
        }
        public void moveBaseFirstPosition(Vector3 offset)
        {
            int hexCount = mHexObject.Length;
            for (int i = 0; i < hexCount; i++)
                mHexObject[i].transform.position = mBigPosition[i] + offset;
        }
        public void calcTouch(Vector3 touch, float scale, float colliderSize)
        {
            int hexCount = mHexObject.Length;
            for (int i = 0; i < hexCount; i++)
            {
                mHexObject[i].transform.localScale = new Vector3(scale, scale, scale);
            }

            //float r = mHexObject[0].gameObject.GetComponent<CircleCollider2D>().bounds.size.x;
            float d = Mathf.Sqrt(3.0f) / 2.0f * colliderSize;
            Vector3 newLeftUp = LeftUp * colliderSize;
            Vector3 newRightUp = RightUp * colliderSize;
            Vector3 newLeft = Left * d;
            Vector3 newRight = Right * d;
            Vector3 newLeftDown = LeftDown * colliderSize;
            Vector3 newRightDown = RightDown * colliderSize;

            //每个子hex的偏移，偏移值都是基于上个hex的位置坐偏移
            Vector3 offsetPosition = new Vector3(0, 0, 0);
            Vector3 originPosition = mHexObject[0].transform.position;
            for (int i = 0; i < hexCount; i++)
            {
                if (mHexType[i] == HEX_CENTER_SIDE)
                {
                }
                if (mHexType[i] == HEX_LEFT_SIDE)
                {
                    mHexObject[i].transform.position = originPosition + offsetPosition + newLeft;
                    offsetPosition += newLeft;
                }
                if (mHexType[i] == HEX_RIGHT_SIDE)
                {
                    mHexObject[i].transform.position = originPosition + offsetPosition + newRight;
                    offsetPosition += newRight;
                }
                if (mHexType[i] == HEX_LEFT_UP_SIDE)
                {
                    mHexObject[i].transform.position = originPosition + offsetPosition + newLeftUp;
                    offsetPosition += newLeftUp;
                }
                if (mHexType[i] == HEX_RIGHT_UP_SIDE)
                {
                    mHexObject[i].transform.position = originPosition + offsetPosition + newRightUp;
                    offsetPosition += newRightUp;
                }
                if (mHexType[i] == HEX_LEFT_DOWN_SIDE)
                {
                    mHexObject[i].transform.position = originPosition + offsetPosition + newLeftDown;
                    offsetPosition += newLeftDown;
                }
                if (mHexType[i] == HEX_RIGHT_DOWN_SIDE)
                {
                    mHexObject[i].transform.position = originPosition + offsetPosition + newRightDown;
                    offsetPosition += newRightDown;
                }
            }
            //选中后hex方体放大，上移到手指上方才能看清楚
            move(new Vector3(0, mFingerOffset * colliderSize, 0));
            adjust(touch, false);
        }
        public void selected()
        {

        }
        public void backToOriginPosition()
        {
            int hexCount = mHexObject.Length;
            for (int i = 0; i < hexCount; i++)
                mHexObject[i].transform.position = mOriginPosition[i];
        }
    }


    //初始化所有三个hex方体总入口
    void initHexs()
    {
        //初始化三个hex方体展示的位置，并把定位的三个hex隐藏
        GameObject[] LoadFirstHex = new GameObject[HEX_COUNT_ARRAY];
        for (int i = 0; i < HEX_COUNT_ARRAY; i++)
        {
            LoadFirstHex[i] = GameObject.Find("LoadFirstHex" + (i + 1));
            if (LoadFirstHex[i] != null)
            {
                LoadHexPosition[i] = LoadFirstHex[i].transform.position;
                LoadFirstHex[i].GetComponent<Renderer>().enabled = false;
            }

        }

        //随机选择颜色,但是不能喝上一次颜色相同
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, HexPrefabs.Length);
        } while (mPriviousColor == randomIndex);
        mPriviousColor = randomIndex;
        //随机选hex方体邢专
        for (int i = 0; i < HEX_COUNT_ARRAY; i++)
        {
            int index = Random.Range(0, AllHexArray.Length);
            HexArray[i] = AllHexArray[index];
        }

        for (int i = 0; i < HEX_COUNT_ARRAY; i++)
        {
            mHexObjectsArray[i] = new HexObjectsArray();
            mHexObjectsArray[i].initPrefabs(LoadHexPosition[i], HexPrefabs[randomIndex], mHexColliderSizeSmall, HexArray[i]);

            //执行加载新的hex方体的动画
            foreach (GameObject hex in mHexObjectsArray[i].mHexObject)
            {
                hex.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            }
            GameObject.Find("BackgroundScript").GetComponent<LoadHexAnimation>().animationLoadHexSB(mHexObjectsArray[i].mHexObject, LoadHexAnimation.ANIMATION_SMALL_BIG);
        }
    }

    void destroyHexs()
    {

    }
    void calcResult()
    {

    }

    //被别的脚本调用的函数
    public void callTouch(int index, Vector3 touch)
    {
        if(mHexObjectsArray[index] != null)
            mHexObjectsArray[index].calcTouch(touch, 1f, mHexColliderSizeNormal);
    }

    //被别的脚本调用的函数
    public void callMoveBaseFirstPosition(int index, Vector3 offset)
    {
        if(mHexObjectsArray[index]!=null)
            mHexObjectsArray[index].moveBaseFirstPosition(offset);
    }

    //被别的脚本调用的函数
    public void callBack(int index)
    {
        if (mHexObjectsArray[index] == null)
            return;
        bool isSuccess = GameObject.Find("BackgroundScript").GetComponent<CalcLine>().addHexsForMap(mHexObjectsArray[index].mHexObject);
        if (isSuccess)
        {
            mHexObjectsArray[index] = null;
        }
        else
        {
            mHexObjectsArray[index].calcTouch(LoadHexPosition[index], 0.5f, mHexColliderSizeSmall);
            mHexObjectsArray[index].backToOriginPosition();
        }

        //hex方体使用完，需要重新生成
        if(mHexObjectsArray[0] == null && mHexObjectsArray[1] == null && mHexObjectsArray[2] == null)
        {
            initHexs();
        }
    }

    // Use this for initialization
    void Start()
    {
        mCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        //hex collider的大小
        GameObject collider = GameObject.Find("HexColliderSize");
        mHexColliderSizeNormal = collider.GetComponent<CircleCollider2D>().bounds.size.x;
        collider.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        mHexColliderSizeSmall = collider.GetComponent<CircleCollider2D>().bounds.size.x;
        //hex的不同颜色
        HexPrefabs[0] = GameObject.Find("RedHexInstantiate");
        HexPrefabs[1] = GameObject.Find("GreenHexInstantiate");
        HexPrefabs[2] = GameObject.Find("BlueHexInstantiate");
        HexPrefabs[3] = GameObject.Find("RoseHexInstantiate");
        HexPrefabs[4] = GameObject.Find("YellowHexInstantiate");
        
        initHexs();
    }

    // Update is called once per frame
    void Update()
    {
        bool isMouseDown = Input.GetMouseButton(0);
        if (isHaveTouched == -1)
        {
            if (isMouseDown)
            {
                mouseFirstPosition = mCamera.ScreenToWorldPoint(Input.mousePosition);
                //检测鼠标拾取物体
                RaycastHit2D hit = Physics2D.Raycast(mouseFirstPosition, Vector2.zero);
                if (hit.collider != null)
                {
                    string hitObjectName = hit.collider.gameObject.name;
                    if (hitObjectName.Equals("LoadFirstHex1"))
                        mTouchIndex = 0;
                    if (hitObjectName.Equals("LoadFirstHex2"))
                        mTouchIndex = 1;
                    if (hitObjectName.Equals("LoadFirstHex3"))
                        mTouchIndex = 2;
                    callTouch(mTouchIndex, mouseFirstPosition);
                    isHaveTouched = 1;

                }
            }
        }

        if (isHaveTouched == 1)
        {
            Vector3 offsetMousePosition = mCamera.ScreenToWorldPoint(Input.mousePosition) - mouseFirstPosition;
            callMoveBaseFirstPosition(mTouchIndex, offsetMousePosition);
        }
        if (isMouseDown == false)
        {
            if (isHaveTouched == 1)
            {
                callBack(mTouchIndex);
                isHaveTouched = -1; ;
            }
        }
    }
}
