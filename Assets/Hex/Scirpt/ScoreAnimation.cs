using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreAnimation : MonoBehaviour
{
    class ScoreObject
    {
        public GameObject mScoreGameObject;
        public float offset;
    }
    private List<ScoreObject> mScoreGameObject = new List<ScoreObject>();
    private int[] mScore;
    GameObject mPrefab;
    private Vector3 mTargetPosition;
    private float mTargetLineY, mHexR;
    private const float mSpeed = 10f;
    private float mMove = -1000f;

    public void addAnimation(Vector3[] position, int[] score)
    {
        mMove = -1000f;
        mScore = score;
        Vector3 offset = new Vector3(-mHexR / 2, mHexR / 2, 0);
        foreach (Vector3 p in position)
        {
            GameObject scoreGO = (GameObject)Instantiate(mPrefab, p + offset, mPrefab.transform.rotation);
            //scoreGO.GetComponent<Animator>().Play("SmallScore", -1);
            ScoreObject scoreObject = new ScoreObject();
            scoreObject.mScoreGameObject = scoreGO;
            scoreObject.offset = mHexR / 5;
            mScoreGameObject.Add(scoreObject);
        }
        mMove = 10f;
    }

    // Use this for initialization
    void Start()
    {
        mPrefab = GameObject.Find("SmallScoreAnimation");
        //mPrefab.GetComponent<Animation>().Play();
        mTargetPosition = GameObject.Find("CurScore3").transform.position;
        mTargetLineY = mTargetPosition.y * 0.95f;
        mHexR = GameObject.Find("HexColliderSize").GetComponent<CircleCollider2D>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (mMove == -1000f)
            return;
        if (mMove > 0)
        {
            mMove -= Time.deltaTime * mSpeed;
            return;
        }
            
        if (mScoreGameObject.Count > 0)
        {
            int len = mScoreGameObject.Count;
            for (int i = len - 1; i >= 0; i--)
            {
                ScoreObject scoreObject = mScoreGameObject[i];
                //做完后退一点的动作后向总分数移动
                if (scoreObject.offset < 0)
                {
                    scoreObject.mScoreGameObject.transform.position =
                        Vector3.MoveTowards(scoreObject.mScoreGameObject.transform.position,
                        mTargetPosition, Time.deltaTime * mSpeed);
                    if (scoreObject.mScoreGameObject.transform.position.y > mTargetLineY)
                    {
                        DestroyObject(scoreObject.mScoreGameObject);
                        mScoreGameObject.RemoveAt(i);
                        GameObject.Find("BackgroundScript").GetComponent<ShowScore>().addScore(mScore[i]);
                    }
                }
                else
                {
                    Vector3 moveOffset = Vector3.MoveTowards(scoreObject.mScoreGameObject.transform.position,
                        -mTargetPosition, Time.deltaTime * mSpeed);
                    scoreObject.mScoreGameObject.transform.position = moveOffset;
                    scoreObject.offset -= Time.deltaTime * mSpeed;
                    
                }
            }//end for
        }//end if
    }
}
