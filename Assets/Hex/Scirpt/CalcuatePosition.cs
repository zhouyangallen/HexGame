using UnityEngine;
using System.Collections;

public class CalcuatePosition : MonoBehaviour
{

    public GameObject RedHex, HexObject;
    Vector2 LeftUp = new Vector2(-Mathf.Sqrt(3.0f) / 4.0f, 3.0f / 4.0f);
    Vector2 RightUp = new Vector2(Mathf.Sqrt(3.0f) / 4.0f, 3.0f / 4.0f);
    Vector2 LeftDown = new Vector2(-Mathf.Sqrt(3.0f) / 4.0f, 3.0f / 4.0f);
    Vector2 RightDown = new Vector2(Mathf.Sqrt(3.0f) / 4.0f, 3.0f / 4.0f);
    Vector2 Left = new Vector2(-1, 0);
    Vector2 Right = new Vector2(1, 0);
    //1左边，2右边，3左上，4右上，5左下，6右下
    int[,] HexArray = { { 0, 2, 4, 3 }, { 0, 2, 4, 3 }, { 0, 1, 1, 1 } };

    class HexObjectsArray
    {
        public GameObject[] mHexObject = new GameObject[4];
    }

    HexObjectsArray[] mHexObjectsArray = new HexObjectsArray[3];

    void Start()
    {
        HexObject = GameObject.Find("RedHexObject");
        float r = HexObject.gameObject.GetComponent<CircleCollider2D>().bounds.size.x;
        float d = Mathf.Sqrt(3.0f) / 2.0f * r;
        LeftUp = LeftUp * r;
        RightUp = RightUp * r;
        Left = Left * d;
        Right = Right * d;


        for (int j = 0; j < 3; j++)
        {
            Vector2 originPosition = new Vector2((j - 1) * 3 * r, 0);
            mHexObjectsArray[j] = new HexObjectsArray();
            mHexObjectsArray[j].mHexObject[0] = (GameObject)Instantiate(RedHex, originPosition, RedHex.transform.rotation);
            Vector2 offsetPosition = new Vector2(0, 0);
            for (int i = 1; i < 4; i++)
            {
                mHexObjectsArray[j].mHexObject[i] = (GameObject)Instantiate(RedHex, originPosition, RedHex.transform.rotation);
                if (HexArray[j, i] == 1)
                {
                    mHexObjectsArray[j].mHexObject[i] = (GameObject)Instantiate(RedHex, originPosition, RedHex.transform.rotation);
                    mHexObjectsArray[j].mHexObject[i].transform.position = originPosition + offsetPosition + Left;
                    offsetPosition += Left;
                    //mHexObjectsArray[j].mHexObject[i].tag = ""+j;
                }
                if (HexArray[0, i] == 2)
                {
                    mHexObjectsArray[j].mHexObject[i] = (GameObject)Instantiate(RedHex, originPosition, RedHex.transform.rotation);
                    mHexObjectsArray[j].mHexObject[i].transform.position = originPosition + offsetPosition + Right;
                    offsetPosition += Right;
                    //mHexObjectsArray[j].mHexObject[i].tag = "" + j;
                }
                if (HexArray[0, i] == 3)
                {
                    mHexObjectsArray[j].mHexObject[i] = (GameObject)Instantiate(RedHex, originPosition, RedHex.transform.rotation);
                    mHexObjectsArray[j].mHexObject[i].transform.position = originPosition + offsetPosition + LeftUp;
                    offsetPosition += LeftUp;
                    //mHexObjectsArray[j].mHexObject[i].tag = "" + j;
                }
                if (HexArray[0, i] == 4)
                {
                    mHexObjectsArray[j].mHexObject[i] = (GameObject)Instantiate(RedHex, originPosition, RedHex.transform.rotation);
                    mHexObjectsArray[j].mHexObject[i].transform.position = originPosition + offsetPosition + RightUp;
                    offsetPosition += RightUp;
                    //mHexObjectsArray[j].mHexObject[i].tag = "" + j;
                }
                if (HexArray[0, i] == 5)
                {
                    mHexObjectsArray[j].mHexObject[i] = (GameObject)Instantiate(RedHex, originPosition, RedHex.transform.rotation);
                    mHexObjectsArray[j].mHexObject[i].transform.position = originPosition + offsetPosition + LeftDown;
                    offsetPosition += LeftDown;
                    //mHexObjectsArray[j].mHexObject[i].tag = "" + j;
                }
                if (HexArray[0, i] == 6)
                {
                    mHexObjectsArray[j].mHexObject[i] = (GameObject)Instantiate(RedHex, originPosition, RedHex.transform.rotation);
                    mHexObjectsArray[j].mHexObject[i].transform.position = originPosition + offsetPosition + RightDown;
                    offsetPosition += RightDown;
                    //mHexObjectsArray[j].mHexObject[i].tag = "" + j;
                }
            }
        }




        //Vector2 position = new Vector2(0, 0);
        //GameObject obj = (GameObject)Instantiate(RedHex, position, RedHex.transform.rotation);
        //GameObject obj2 = (GameObject)Instantiate(RedHex, position, RedHex.transform.rotation);
        //obj2.transform.position = position + Right;
        //GameObject obj3 = (GameObject)Instantiate(RedHex, position, RedHex.transform.rotation);
        //obj3.transform.position = position + Right + RightUp;
        //GameObject obj4 = (GameObject)Instantiate(RedHex, position, RedHex.transform.rotation);
        //obj4.transform.position = position + Right + RightUp + LeftUp;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
