using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIWindow : MonoBehaviour
{
    public bool isWin;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGameOver(int score, int target)
    {
        isWin = (score >= target);
        Transform tr = gameObject.transform.GetChild(0);
        tr.gameObject.SetActive(true);
        Text header = tr.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
        header.text = (isWin) ? "Вы выиграли!" : "Вы проиграли";

        tr.GetChild(1).GetChild(0).gameObject.SetActive(isWin);
        tr.GetChild(1).GetChild(1).gameObject.SetActive(!isWin);

        tr.GetChild(2).GetChild(0).gameObject.SetActive(isWin);
        tr.GetChild(2).GetChild(1).gameObject.SetActive(!isWin);
        tr.GetChild(2).GetChild(2).gameObject.SetActive(!isWin);
        tr.GetChild(2).GetChild(7).gameObject.SetActive(isWin);
        tr.GetChild(2).GetChild(8).gameObject.SetActive(!isWin);

        tr.GetChild(2).GetChild(4).gameObject.GetComponent<Image>().fillAmount = ((float)score) / (float)target;

        tr.GetChild(2).GetChild(9).gameObject.GetComponent<Text>().text = score + " / " + target;
    }

    public void OnClose()
    {
        Application.Quit();
    }

    public void OnRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
