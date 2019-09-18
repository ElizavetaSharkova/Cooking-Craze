using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DishModel : MonoBehaviour
{
    public bool needPrevState;
    private int state;
    public int statesCount;

    private float[] cookingTimesSec = { 5, 5, 7 };

    public int State
    {
        get => state;
    }

    public void incState()
    {
        if (state < statesCount-1)
        {
            state++;
            Debug.Log("new state: " + state);
            changeVisualState(true);
        }
    }

    public void resetState()
    {
        changeVisualState(false);
        state = 0;
        Debug.Log("reset state: " + state);
    }

    // Start is called before the first frame update
    void Start()
    {
        state = 0;
        statesCount = gameObject.transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        Image timerImage = GetTimerImage();
        if (timerImage)
        {
            timerImage.fillAmount += 1.0f / cookingTimesSec[State] * Time.deltaTime;
            if (timerImage.fillAmount >= 1)
            {
                incState();
                timerImage.fillAmount = 0;
            }
        }
    }

    private void changeVisualState(bool isActive)
    {
        Transform tr = this.gameObject.transform;
        tr.GetChild(State).gameObject.SetActive(isActive);
        Debug.Log("state " + State + " isActive " + tr.GetChild(State).gameObject.activeSelf);
        int firstHiddenState = (statesCount == 2 & !needPrevState & isActive) ? -1 : 0;
        for (int i = State-1; i > firstHiddenState; i--)
        {
            tr.GetChild(i).gameObject.SetActive(needPrevState & isActive);
            Debug.Log("state " + i + " isActive " + tr.GetChild(i).gameObject.activeSelf.ToString());
        }

        if (!isActive)
        {
            tr.GetChild(0).gameObject.SetActive(true);
            Debug.Log("state " + 0 + " isActive " + tr.GetChild(0).gameObject.activeSelf);
        }

    }

    public Image GetTimerImage()
    {
        Transform tr = this.gameObject.transform;
        Transform timerCanvasTransform = (tr.GetChild(State).childCount > 0) ? tr.GetChild(State).GetChild(0) : null;
        if (timerCanvasTransform)
        {
            Transform timerImageTransform = (timerCanvasTransform.childCount > 0) ? timerCanvasTransform.GetChild(0) : null;
            if (timerImageTransform)
            {
                return timerImageTransform.gameObject.GetComponent<Image>();
            }
        }
        return null;
    }
}
