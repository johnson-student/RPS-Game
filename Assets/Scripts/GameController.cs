using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Rock - Paper - Scissors   (1 = Rock, 2 = Paper, 3 = Scissors)
public class GameController : MonoBehaviour
{
    Image imgYou;       // picture of the player's choice
    Image imgCom;       // picture of the computer's choice
    Text txtYou;        // player's score text
    Text txtCom;        // computer's score text
    Text txtResult;     // "You win" / "Draw" / ...

    int cntYou = 0;     // player's score
    int cntCom = 0;     // computer's score

    void Start()
    {
        InitGame();
    }

    // Optional keyboard shortcut: press 1, 2 or 3 (Input Manager - Old)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) CheckResult(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) CheckResult(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) CheckResult(3);
    }

    // Find the UI objects by name and set the start state
    void InitGame()
    {
        imgYou    = GameObject.Find("ImgYou").GetComponent<Image>();
        imgCom    = GameObject.Find("ImgCom").GetComponent<Image>();
        txtYou    = GameObject.Find("TxtYou").GetComponent<Text>();
        txtCom    = GameObject.Find("TxtCom").GetComponent<Text>();
        txtResult = GameObject.Find("TxtResult_").GetComponent<Text>();

        cntYou = 0;
        cntCom = 0;

        // img_4 is the "?" picture
        imgYou.sprite = Resources.Load<Sprite>("img_4");
        imgCom.sprite = Resources.Load<Sprite>("img_4");
        imgCom.rectTransform.localScale = new Vector3(-1, 1, 1); // flip computer image

        txtYou.text = "0";
        txtCom.text = "0";
        txtResult.text = "Select the button below";
    }

    // Called by the 3 buttons (On Click) - the clicked button is passed in
    public void OnButtonClick(GameObject clickedButton)
    {
        Debug.Log(clickedButton.name + " clicked");

        // "1_Rock Button" -> first character "1" -> 1
        int you = int.Parse(clickedButton.name.Substring(0, 1));
        Debug.Log("Your result is: " + you);

        CheckResult(you);
    }

    // Called by the Menu button: go back to the menu scene
    public void OnMenuClick()
    {
        SceneManager.LoadScene("MenuScene");
    }

    // Computer picks, then we decide the winner
    void CheckResult(int you)
    {
        int com = Random.Range(1, 4);   // 1, 2 or 3 (the max 4 is not included)
        int k = you - com;

        if (k == 0)
        {
            txtResult.text = "Draw";
            txtResult.color = Color.yellow;
        }
        else if (k == 1 || k == -2)
        {
            txtResult.text = "You win";
            txtResult.color = Color.green;
            cntYou++;
        }
        else
        {
            txtResult.text = "Computer wins";
            txtResult.color = Color.red;
            cntCom++;
        }

        SetResult(you, com);
    }

    // Show the pictures and the scores
    void SetResult(int you, int com)
    {
        imgYou.sprite = Resources.Load<Sprite>("img_" + you);
        imgCom.sprite = Resources.Load<Sprite>("img_" + com);
        imgCom.rectTransform.localScale = new Vector3(-1, 1, 1); // flip horizontally

        txtYou.text = cntYou.ToString();
        txtCom.text = cntCom.ToString();

        StopAllCoroutines();
        StartCoroutine(PopResultText());
    }

    // Small animation: the result text grows then shrinks back to normal size
    IEnumerator PopResultText()
    {
        float time = 0f;
        while (time < 0.3f)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(1.5f, 1f, time / 0.3f);
            txtResult.rectTransform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }
        txtResult.rectTransform.localScale = Vector3.one;
    }
}
