using UnityEngine;
using System.Collections;

public class ShowScore : MonoBehaviour {

    private int mCurScore, mTmpScore;

    public void addScore(int score)
    {
        mTmpScore += score;
    }

    public Sprite[] mNumberSprite = new Sprite[10];
    private GameObject[] mScoresRender = new GameObject[6];
	// Use this for initialization
	void Start () {
        int len = mScoresRender.Length;
        for(int i=0;i< len; i++)
        {
            mScoresRender[i] = GameObject.Find("CurScore" + (i+1));
            mScoresRender[i].GetComponent<SpriteRenderer>().enabled = false;
        }
        mScoresRender[3].GetComponent<SpriteRenderer>().enabled = true;
    }

    void updateShowScore(GameObject[] ScoresRender)
    {
        if (mCurScore < 100)
        {
            ScoresRender[3].GetComponent<SpriteRenderer>().sprite = mNumberSprite[(mCurScore % 10)];
            ScoresRender[3].GetComponent<SpriteRenderer>().enabled = true;
            ScoresRender[2].GetComponent<SpriteRenderer>().sprite = mNumberSprite[(mCurScore/10)];
            ScoresRender[2].GetComponent<SpriteRenderer>().enabled = true;
        }
        else if (mCurScore < 1000)
        {
            ScoresRender[4].GetComponent<SpriteRenderer>().sprite = mNumberSprite[(mCurScore % 10)];
            ScoresRender[4].GetComponent<SpriteRenderer>().enabled = true;
            ScoresRender[3].GetComponent<SpriteRenderer>().enabled = mNumberSprite[(mCurScore / 10 % 10)];
            ScoresRender[3].GetComponent<SpriteRenderer>().enabled = true;
            ScoresRender[2].GetComponent<SpriteRenderer>().sprite = mNumberSprite[(mCurScore / 100)];
            ScoresRender[2].GetComponent<SpriteRenderer>().enabled = true;
        }
        else if (mCurScore < 10000)
        {
            ScoresRender[4].GetComponent<SpriteRenderer>().sprite = mNumberSprite[(mCurScore % 10)];
            ScoresRender[4].GetComponent<SpriteRenderer>().enabled = true;
            ScoresRender[3].GetComponent<SpriteRenderer>().enabled = mNumberSprite[(mCurScore / 10 % 10)];
            ScoresRender[3].GetComponent<SpriteRenderer>().enabled = true;
            ScoresRender[2].GetComponent<SpriteRenderer>().sprite = mNumberSprite[(mCurScore / 100 % 10)];
            ScoresRender[2].GetComponent<SpriteRenderer>().enabled = true;
            ScoresRender[1].GetComponent<SpriteRenderer>().sprite = mNumberSprite[(mCurScore / 1000)];
            ScoresRender[1].GetComponent<SpriteRenderer>().enabled = true;
        }
        else if (mCurScore < 100000)
        {
            ScoresRender[5].GetComponent<SpriteRenderer>().sprite = mNumberSprite[(mCurScore % 10)];
            ScoresRender[5].GetComponent<SpriteRenderer>().enabled = true;
            ScoresRender[4].GetComponent<SpriteRenderer>().enabled = mNumberSprite[(mCurScore / 10 % 10)];
            ScoresRender[4].GetComponent<SpriteRenderer>().enabled = true;
            ScoresRender[3].GetComponent<SpriteRenderer>().enabled = mNumberSprite[(mCurScore / 100 % 10)];
            ScoresRender[3].GetComponent<SpriteRenderer>().enabled = true;
            ScoresRender[2].GetComponent<SpriteRenderer>().sprite = mNumberSprite[(mCurScore / 1000 % 10)];
            ScoresRender[2].GetComponent<SpriteRenderer>().enabled = true;
            ScoresRender[1].GetComponent<SpriteRenderer>().sprite = mNumberSprite[(mCurScore / 10000)];
            ScoresRender[1].GetComponent<SpriteRenderer>().enabled = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
	    if(mTmpScore != 0)
        {
            mCurScore += 1;
            mTmpScore -= 1;
            updateShowScore(mScoresRender);
        }

	}
}
