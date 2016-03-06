using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadHexAnimation : MonoBehaviour {
    public const int ANIMATION_SMALL_BIG = 1;
    public const int ANIMATION_MOVE = 2;
    public const int ANIMATION_BIG_SMALL = 3;

    //动画结构
    class AnimationHex
    {
        public GameObject[] hexs;
        public int type;
        public Vector3 targetPosition;
        public Vector3 targetValue;
        public Vector3 speed;
    }
    //待执行动画列表
    List<AnimationHex> AnimationList = new List<AnimationHex>();

    /*给外面script调用的方法，由小变大的动画加载hex**/
    public bool animationLoadHexSB(GameObject[] hexs, int type)
    {
        AnimationHex animationHex = new AnimationHex();
        animationHex.hexs = hexs;
        animationHex.type = type;
        animationHex.speed = new Vector3(2f, 2f, 2f);
        animationHex.targetValue = new Vector3(0.5f, 0.5f, 0.5f);
        AnimationList.Add(animationHex);
        return true;
    }

    /*给外面script调用的方法，由大变小的动画加载hex**/
    public bool animationLoadHexBS(GameObject[] hexs, int type)
    {
        AnimationHex animationHex = new AnimationHex();
        animationHex.hexs = hexs;
        animationHex.type = type;
        animationHex.speed = new Vector3(2f, 2f, 2f);
        animationHex.targetValue = new Vector3(0.01f, 0.01f, 0.01f);
        AnimationList.Add(animationHex);
        return true;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //遍历动画列表
        int len = AnimationList.Count;
        for (int i = len - 1; i >= 0; i--)
        {
            AnimationHex animationHex = AnimationList[i];
            if (animationHex.type == ANIMATION_SMALL_BIG)
            {
                //遍历当前逐渐变大动画里面的每一个小hex
                foreach (GameObject hex in animationHex.hexs)
                {
                    //判断是否已经达到正常大小
                    if (hex.transform.localScale.x >= animationHex.targetValue.x)
                    {
                        //把方体中每个子hex设置为想要大小，然后删除此方体的动画
                        foreach (GameObject subHex in animationHex.hexs)
                        {
                            subHex.transform.localScale = animationHex.targetValue;
                        }
                        AnimationList.RemoveAt(i);
                        break;
                    }
                    //逐渐变大
                    hex.transform.localScale += animationHex.speed * Time.deltaTime;
                }

            }
            if (animationHex.type == ANIMATION_BIG_SMALL)
            {
                //遍历当前逐渐变小动画里面的每一个hex
                foreach (GameObject hex in animationHex.hexs)
                {
                    //如果有好几条线同时有连成线，那么就有交叉点，此点会被提前删除
                    if (hex == null)
                        continue;
                    //判断是否已经达到正常大小
                    if (hex.transform.localScale.x <= animationHex.targetValue.x)
                    {
                        
                        AnimationList.RemoveAt(i);
                        //删除把方体中每个子hex
                        foreach (GameObject subHex in animationHex.hexs)
                        {
                            Destroy(subHex);
                        }
                        break;
                    }
                    //逐渐变小
                    hex.transform.localScale -= animationHex.speed * Time.deltaTime;
                }
            }
        }
	}
}
