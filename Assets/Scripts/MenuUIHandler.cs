using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

    [DefaultExecutionOrder(1000)]
public class MenuUIHandler : MonoBehaviour
{
    [SerializeField] Button newGame;
    [SerializeField] Button exitGame;
    [SerializeField] Button settings;


    // Start is called before the first frame update
    void Start()
    {
        

    }

    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }

    // Update is called once per frame


    public void TimeTrialMode()
    {
        SceneManager.LoadScene(2);

    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
