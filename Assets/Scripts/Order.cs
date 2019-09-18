using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Order : MonoBehaviour
{
    private const float orderTime = 18f;
    public string[] menu = new string[3];
    public float remainingTime;
    public TaskManager.OnEndOrder OnDestroyed;
    public bool isStarted = false;
    public int[] dishesTypes;
    public int position;


    public Order(TaskManager.OnEndOrder OnEnd)
    {
        int dishesCount = UnityEngine.Random.Range(1, 4);
        dishesTypes = new int[dishesCount];
        for (int i = 0; i < dishesCount; i++)
        {
            dishesTypes[i] = UnityEngine.Random.Range(1, 4);
        }

        OnDestroyed = OnEnd;
        menu[0] = "Burger_";
        menu[1] = "HotDog_";
        menu[2] = "glass_";

        Debug.Log("new order");
    }

    // Update is called once per frame
    public void Update()
    {
        if (isStarted && position != -1)
        {
            if (remainingTime == orderTime)
            {
                SetDishesImage();
            }
            remainingTime = remainingTime - Time.deltaTime;
            Transform orderUITransform = gameObject.transform.GetChild(0);
            Image timer = orderUITransform.GetChild(0).gameObject.GetComponent<Image>();
            timer.fillAmount = 1.0f / orderTime * remainingTime;
            if (remainingTime <= 0)
            {
                Debug.Log("trying destroy");
                DestroyDishes();
                OnDestroyed(position);
                position = -1;
            }
        }

    }
    private void DestroyDishes()
    {
        int count = gameObject.transform.childCount;
        for (int i = 1; i < count; i++)
        {
            DestroyDishByIndex(i);
        }
    }

    private void DestroyDishByIndex(int idx)
    {
        GameObject dish = gameObject.transform.GetChild(idx).gameObject;
        Destroy(dish);
    }

    public void AddTime()
    {
        remainingTime += 6;
        if (remainingTime > orderTime)
        {
            remainingTime = orderTime;
        }
    }

    public void StartOrder(int peopleNum)
    {
        isStarted = true;
        remainingTime = orderTime;
        position = peopleNum;
    }

    public bool HasDish(string dishName)
    {
        int idx = GetDishTypeByName(dishName);
        if (idx != 0)
        {
            return Array.Exists(dishesTypes, type => type == idx);
        }
        return false;
    }

    public int GetDishTypeByName(string name)
    {
        return Array.IndexOf(menu, name) + 1;
    }

    public void DeleteDish(string dishName)
    {
        int type = GetDishTypeByName(dishName);
        
        if (type != 0)
        {
            if (dishesTypes.Length == 1)
            {
                DestroyDishes();
                OnDestroyed(position);
                position = -1;
            }
            else
            {
                int[] newArray = new int[dishesTypes.Length - 1];
                int n = Array.IndexOf(dishesTypes, type);
                DestroyDishes();

                Array.Copy(dishesTypes, 0, newArray, 0, n);
                Array.Copy(dishesTypes, n + 1, newArray, n, dishesTypes.Length - n - 1);

                dishesTypes = new int[newArray.Length];
                Array.Copy(newArray, dishesTypes, newArray.Length);

                AddTime();
                SetDishesImage();
            }
        }
    }

    private void SetDishesImage()
    {
        Transform tr = gameObject.transform;
        for (int i = 0; i < dishesTypes.Length; i++)
        {
            GameObject product = Resources.Load<GameObject>(menu[dishesTypes[i] - 1]+"1") as GameObject; ;
            float x = -1.32f;
            float y = GetYPosition(i, dishesTypes.Length);  
            Vector3 pos = tr.position + new Vector3(x, y, 1f);
            GameObject inst = Instantiate(product, pos, Quaternion.identity);
            inst.transform.parent = tr;
        }
    }

    private float GetYPosition(int idx, int count)
    {
        //return 2 - (2.58f / count * idx - 0.9f) / 2;
        float y;
        if (count == 1 || count == 3 && idx == 1)
        {
            y = 0.85f;
        }
        else if (count == 2)
        {
            if (idx == 0)
            {
                y = 1.25f;
            }
            else y = 0.25f;
        }
        else if (idx == 0)
        {
            y = 1.7f;
        }
        else y = -0.05f;
        return y;
    }
}
