using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public GameObject LoadingCanvas;
    public List<GameObject> story;
    public GameObject PauseCanvas;
    public GameObject ProgressionCanvas;
    public GameObject GameOverCanvas;
    public GameObject HUD;
    public int enemyCount;
    int levelloadtime = 10;
    bool levelended = false;
    public static GameManager Instance
    {
        get
        {
            if (GameManager.instance == null)
            {
                GameManager.instance = FindObjectOfType<GameManager>();
            }
            return GameManager.instance;
        }
    }
    private void Start()
    {
        levelloadtime = 10;
        levelended = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(SceneManager.GetActiveScene().buildIndex!=0)
            {
                PauseGame();
            }      
        }
    }
    public void PauseGame()
    {
        if(Time.timeScale!=0)
        {
          
            PauseCanvas.SetActive(true);
            Time.timeScale = 0f;
        } 
        else if(PauseCanvas.activeSelf==true)
        {
            PauseCanvas.SetActive(false);
            Time.timeScale = 1f;
        }
    }
    public void ResumeGame()
    {
        PauseCanvas.SetActive(false);
        Time.timeScale = 1f;
    }
    public void QuiteGame()
    {
        Time.timeScale = 1f;
        Loadlevel(0);
    }
    public void QuiteApplication()
    {
        Application.Quit();
    }
    public void LevelEnded()
    {
        int currentlevel = SceneManager.GetActiveScene().buildIndex;
        levelended = true;
        HUD.SetActive(false);
        if (currentlevel < 4)
        {
            float upgradePoints = PlayerPrefs.GetFloat("UpgradePoints", 0);
            PlayerPrefs.SetFloat("UpgradePoints", upgradePoints + 1);          
            ProgressionCanvas.SetActive(true);
        }
        else
            NextLevel();
    }
    public void NextLevel()
    {
        int currentlevel = SceneManager.GetActiveScene().buildIndex;
        currentlevel++;
        if (currentlevel > SceneManager.sceneCountInBuildSettings - 2)
        {
            currentlevel = 0;
        }
        else
        {
            PlayerPrefs.SetInt("CurrentLevel", currentlevel);
        }    
        Loadlevel(currentlevel);

    }
    public void GameOver()
    {
        HUD.SetActive(false);
        GameOverCanvas.SetActive(true);
    }
    public void Retry()
    {
        int currentlevel = SceneManager.GetActiveScene().buildIndex;
        Loadlevel(currentlevel);
    }
    public void ReturnToMenu()
    {
        Loadlevel(0);
    }
    public void Loadlevel(int level)
    {
        LoadingCanvas.SetActive(true);
        if(level!=0 && level!=6)
        {
            story[level - 1].SetActive(true);
        }   
        else
        {
            int currentlevel = SceneManager.GetActiveScene().buildIndex;
            if(currentlevel==5 && levelended==true)
            {
                story[5].SetActive(true);
            }
        }
        if(level==0 || level==6)
        {
            levelloadtime = 3;
            StartCoroutine("LoadAsynchronously", level);
        }
        else
        {
            levelloadtime = 10;
            StartCoroutine("LoadAsynchronously", level);
        }
     
    }
    IEnumerator LoadAsynchronously(int levelindex)
    {

        float timer = 0f;
        float minLoadTime = levelloadtime;

        AsyncOperation operation = SceneManager.LoadSceneAsync(levelindex);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            timer += Time.deltaTime;

            if (timer > minLoadTime)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        yield return null;

    }
}
