using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    public Text dishText;
    public Text peopleText;
    public Image dishProgress;

    private const int allPeople = 15;
    private const float deltaTimePeople = 3f;
    private int endOrders = 0;
    private int currentPeople;
    private int targetDishesCount;
    private int score;
    private float remainingTime = 0;
    private Order[] orders;
    private Transform peoplesTransform;
    private bool isStop = true;

    public delegate void OnEndOrder(int destroedPeople);

    // Start is called before the first frame update
    void Start()
    {
        targetDishesCount = 0;
        currentPeople = 0;
        OnEndOrder onEnd = DestroyOrder;
        orders = new Order[allPeople];
        for (int i = 0; i < allPeople; i++)
        {
            orders[i] = new Order(onEnd);
            targetDishesCount += orders[i].dishesTypes.Length;
        }
        targetDishesCount -= 2;
        // targetDishesCount = targetDishesCount/ 2;  //for test

        peoplesTransform = this.gameObject.transform;
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStop)
        {
            if (currentPeople < peoplesTransform.childCount && (currentPeople + endOrders) < allPeople)
            {
                remainingTime += Time.deltaTime;

                if (remainingTime >= deltaTimePeople)
                {
                    AddPeople();
                }
            }

            else if (endOrders == allPeople)
            {
                GameOver();
            }
        }
        peopleText.text = (currentPeople + endOrders).ToString();
        dishText.text = score + "/" + targetDishesCount;
        dishProgress.fillAmount = ((float)score) / targetDishesCount;
    }

    private void AddPeople()
    {
        remainingTime = 0;
        int newPeopleIdx = -1;
        for (int i = 0; i < peoplesTransform.childCount; i ++)
        {
            if(!peoplesTransform.GetChild(i).gameObject.activeSelf)
            {
                newPeopleIdx = i;
                break;
            }
        }
        if (newPeopleIdx != -1)
        {
            GameObject newPeople = peoplesTransform.GetChild(newPeopleIdx).gameObject;
            newPeople.SetActive(true);
            Order nextOrder = orders[currentPeople + endOrders];
            nextOrder.StartOrder(newPeopleIdx);
            Component copy = CopyComponent(nextOrder, newPeople);

            currentPeople++;
        }
    }

    public void DestroyOrder(int destroedPeople)
    {
        peoplesTransform.GetChild(destroedPeople).gameObject.SetActive(false);
        currentPeople--;
        endOrders++;
    }

    public int FindHungryPeople(string dishName)
    {
        int hungryIndex = -1;
        float minTime = 19f;
        for (int i = 0; i < 4; i++)
        {
            GameObject people = peoplesTransform.GetChild(i).gameObject;
            if (people.activeSelf)
            {
                Order peopleOrder = people.GetComponent<Order>();
                Debug.Log("people "+i+" has dish " + peopleOrder.HasDish(dishName) + " - remaing= " + peopleOrder.remainingTime + " min= " + minTime);
                if (peopleOrder.HasDish(dishName) & peopleOrder.remainingTime < minTime )
                {
                    minTime = peopleOrder.remainingTime;
                    hungryIndex = i;
                }
            }
            
        }

        return hungryIndex;
    }

    public bool TryFeedPeople(string dishName)
    {
        int hungryPeopleIndex = FindHungryPeople(dishName);
        if (hungryPeopleIndex != -1)
        {
            Order order = peoplesTransform.GetChild(hungryPeopleIndex).gameObject.GetComponent<Order>();
            order.DeleteDish(dishName);
            score++;
            return true;
        }
        else return false;
    }

    private Component CopyComponent(Component original, GameObject destination)
    {
        System.Type type = original.GetType();
        Component copy = destination.GetComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy;
    }

    private void GameOver()
    {
        isStop = true;
        FindObjectOfType<UIWindow>().OnGameOver(score, targetDishesCount);
    }

    public void OnPlay(GameObject playButton)
    {
        isStop = false;
        Time.timeScale = 1;
        playButton.SetActive(false);
    }

}
