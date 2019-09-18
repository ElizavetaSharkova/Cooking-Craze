using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductController : MonoBehaviour
{
    private const string meatStr = "meat";
    private const string sosigeStr = "sosige";
    private const string burgerStr = "Burger";
    private const string hotDogStr = "HotDog";
    private const string colaStr = "glass";
    private const int maxSlotsCount = 3;
    private float[] cookingTimesSec = { 5, 7 };
    private const float doubleClickTime = 0.25f;

    private float lastClickTime;
    private Vector3 lastClickPosition;

    private GameObject plates;
    private GameObject pans;
    private TaskManager taskManager;


    delegate void OnEnd();

    // Start is called before the first frame update
    void Start()
    {
        plates = GameObject.Find("plates");
        pans = GameObject.Find("pans");
        taskManager = FindObjectOfType<TaskManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPosition = Input.mousePosition;
            GameObject hitObject = GetHitGameObject(clickPosition);

            if (hitObject != null)
            {
                if (isDoubleClick(clickPosition))
                {
                    onDoubleClick(hitObject);
                }
                else
                {
                    onFirstClick(hitObject);
                    lastClickTime = Time.time;
                    lastClickPosition = clickPosition;
                }
            }
        }

    }

    private GameObject GetHitGameObject(Vector3 clickPosition)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(clickPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        Collider2D collider = hit.collider;

        return (collider) ? hit.collider.gameObject : null ;
    }

    private bool isDoubleClick(Vector3 clickPosition)
    {
        return (lastClickPosition == clickPosition & lastClickTime + doubleClickTime > Time.time);
    }

    private void onDoubleClick(GameObject hitObject)
    {
        string objectName = hitObject.name;
        Debug.Log("onSecondClick: " + objectName);
        if (objectName.Contains("meat_") || objectName.Contains("sosige_"))
        {
            DishModel dish = hitObject.GetComponent<DishModel>();
            if (dish.State == 3)
            {
                StartCoroutine(DeleteIngredient(dish));
            }
        }
    }

    private void onFirstClick(GameObject hitObject)
    {
        string objectName = hitObject.name;
        Debug.Log("onClick: " + objectName);
        switch (objectName)
        {
            case "bread_burger":
                AddIngredient(true, false, 0);
                break;
            case "bread_hotdog":
                AddIngredient(false, false, 0);
                break;
            case "meatBox":
                AddIngredient(true, true, 0);
                break;
            case "sosigesBox":
                AddIngredient(false, true, 0);
                break;
            case "tomato":
                AddIngredient(true, false, 2);
                break;
            case "cheese":
                AddIngredient(true, false, 3);
                break;
            case "mustard":
                AddIngredient(false, false, 2);
                break;
        }

        DishModel dish = hitObject.GetComponent<DishModel>();
        if (dish)
        {
            if (objectName.Contains("sosige_"))
            {

                if (dish.State == 2)
                {
                    if (AddIngredient(false, false, 1))
                    {
                        dish.resetState();
                    }
                }
            }
            else if (objectName.Contains("meat_"))
            {
                if (dish.State == 2)
                {
                    if (AddIngredient(true, false, 1))
                    {
                        dish.resetState();
                    }
                }
            }
            else if (dish.State == dish.statesCount - 1)
            {
                string[] menu = new string[3];
                menu[0] = burgerStr + "_";
                menu[1] = hotDogStr + "_";
                menu[2] = colaStr + "_";

                for (int i = 0; i < menu.Length; i++)
                {
                    if (objectName.Contains(menu[i]))
                    {
                        if (taskManager.TryFeedPeople(menu[i]))
                        {
                            dish.resetState();
                        }
                        break;
                    }
                }
            }
        }
    }

    public bool AddIngredient(bool isBurger, bool isMeat, int oldState)
    {
        GameObject rootObj = (isMeat) ? pans : plates;
        GameObject slot = GetEmptySlot( rootObj, isBurger, oldState);
        if (slot)
        {
            DishModel dish = slot.GetComponent<DishModel>();
            dish.incState();
            return true;
        }
        return false;
    }

    private GameObject GetEmptySlot(GameObject rootObj, bool isBurger, int oldState)
    {
        string productStr = GetProductString(rootObj, isBurger);

        for (int i = 1; i < maxSlotsCount+1 ; i++)
        {
            GameObject childObject = GameObject.Find(productStr + "_" + i);
            if (childObject)
            {
                int state = childObject.GetComponent<DishModel>().State;
                if (state == oldState)
                {
                    return childObject;
                }
            }
            else break;

        }
        return null;
    }

    private string GetProductString(GameObject rootObj, bool isBurger)
    {
        string productStr = "";
        if (rootObj.name == "plates")
        {
            productStr = (isBurger == true) ? burgerStr : hotDogStr;
        }
        else if (rootObj.name == "pans")
        {
            productStr = (isBurger == true) ? meatStr : sosigeStr;
        }
        return productStr;
    }

    private IEnumerator DeleteIngredient(DishModel dish)
    {
        dish.resetState();
        GameObject trash = GameObject.Find("trash");
        Transform tr = trash.transform;
        ChangingTrashVisual(tr, true);

        yield return new WaitForSeconds(1.5f);

        ChangingTrashVisual(tr, false);
    }

    private void ChangingTrashVisual(Transform trashTansform, bool isOpen)
    {
        trashTansform.GetChild(0).gameObject.SetActive(isOpen);
        trashTansform.GetChild(1).gameObject.SetActive(!isOpen);
    }

}
