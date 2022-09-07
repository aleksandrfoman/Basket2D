using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum GameState
{
    Pause,
    MainMenu,
    Game
}

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;
    [SerializeField]
    private CameraController cameraController;
    private Camera camera;
    [SerializeField]
    private Player player;
    [SerializeField]
    private Trajectory trajectory;
    [SerializeField]
    private Hoop currentHoop,startHoop;
    [SerializeField]
    private List<Hoop> hoopsList;
    [SerializeField]
    private float pushForce = 4f;
    private bool isDragging = false;
    private Vector2 startPoint,endPoint,direction,force;
    private float distance;
    [SerializeField]
    private float hoopSpawnDist = 3.5f;
    [SerializeField]
    private Hoop hoopPrefab;
    [SerializeField]
    private GameState gameState;
    private bool isPause = false;
    [Header("Ui")]
    [SerializeField]
    private GraphicRaycaster graphicRaycaster;
    private int score;
    [SerializeField]
    private TMP_Text scoreText;
    private bool isNight;
    [SerializeField]
    private Color nightColor, dayColor;
    [SerializeField]
    private Image imageLight;
    [SerializeField]
    private TMP_Text starText;
    private int currentStar;
    private int maxStar;
    [SerializeField]
    private GameObject gamePanel, pausePanel, mainMenuPanel;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        isPause = false;
        gameState = GameState.Game;
        camera = Camera.main;
        UpdateStarText();
        SetBackgroundColor();
        SetHoop(startHoop);
    }

    private void Update()
    {
        if (gameState == GameState.Game)
        {
            Time.timeScale = 1;

            if (currentHoop != null)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (CheckButtonDown())
                    {
                        OpenGamePanel();
                        isDragging = true;
                        OnDragStart();
                    }
                }
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    if (CheckButtonDown())
                    {
                        isDragging = false;
                        OnDragEnd();
                    }
                }
                if (isDragging)
                {
                    OnDrag();
                }
            }

            if (player.transform.position.y <= cameraController.transform.position.y - 7f)
            {
                RestartGame();
            }
        }
        else if(gameState == GameState.Pause)
        {
            Time.timeScale = 0;
        }
    }

    private bool CheckButtonDown()
    {
        PointerEventData ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(ped, results);

        if (results.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #region UI

    public void AddScore()
    {
        score += 2;
        scoreText.text = score.ToString();
    }
    public void ChagneBackgroundColor()
    {
        isNight = !isNight;

        if (isNight)
        {
            PlayerPrefs.SetInt("IsNight", 1);
        }
        else
        {
            PlayerPrefs.SetInt("IsNight", 0);
        }
        SetBackgroundColor();
    }

    public void AddStar()
    {
        currentStar++;
        if (currentStar >= maxStar)
        {
            PlayerPrefs.SetInt("Star", currentStar);
        }
        UpdateStarText();
    }

    private void UpdateStarText()
    {

        currentStar = PlayerPrefs.GetInt("Star", 0);
        starText.text = currentStar.ToString();
    }

    public void SetBackgroundColor()
    {
        var value = PlayerPrefs.GetInt("IsNight", 0);
        if(value == 1)
        {
            isNight = true;
            camera.backgroundColor = nightColor;
            imageLight.color = nightColor;
        }
        else
        {
            isNight = false;
            camera.backgroundColor = dayColor;
            imageLight.color = dayColor;
        }
    }

    public void ChangePause()
    {
        isPause = !isPause;
        if (isPause)
        {
            gameState = GameState.Pause;
            OpenPausePanel();
        }
        else
        {
            gameState = GameState.Game;
            OpenGamePanel();
        }
        Debug.Log(gameState);
    }

    public void OpenGamePanel()
    {
        mainMenuPanel.SetActive(false);
        pausePanel.SetActive(false);
        gamePanel.SetActive(true);
    }
    public void OpenMainMenuPanel()
    {
        mainMenuPanel.SetActive(true);
        pausePanel.SetActive(false);
        gamePanel.SetActive(false);
    }

    public void OpenPausePanel()
    {
        mainMenuPanel.SetActive(false);
        pausePanel.SetActive(true);
        gamePanel.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion

    #region Spawner

    public void AddHoop()
    {
        var spawnObj = Instantiate(hoopPrefab, new Vector3(Random.Range(-2f, 2f), hoopsList[hoopsList.Count - 1].transform.position.y + hoopSpawnDist, 0f), Quaternion.identity);
        spawnObj.gameObject.name = "Hope"+spawnObj.transform.position.ToString();
        hoopsList.Add(spawnObj);
    }

    public void RemoveHoop()
    {
        Destroy(hoopsList[0].gameObject);
        hoopsList.RemoveAt(0);
        cameraController.ChangeY(hoopSpawnDist);
    }

    public Hoop GetFirstHope()
    {
        return hoopsList[0];
    }
    #endregion

    #region Core
    private void OnDragStart()
    {
        player.DeactivateRb();
        startPoint = camera.ScreenToWorldPoint(Input.mousePosition);
        trajectory.Show();
    }

    public void SetHoop(Hoop newHoop)
    {
        currentHoop = newHoop;
        player.transform.parent = currentHoop.ParentPoint;
        player.DeactivateRb();

    }

    public void DeacitaveteHoop()
    {
        currentHoop = null;
        player.transform.parent = null;
    }

    private void OnDrag()
    {
        endPoint = camera.ScreenToWorldPoint(Input.mousePosition);
        distance = Vector2.Distance(startPoint, endPoint);
        direction = (startPoint - endPoint).normalized;
        distance = Mathf.Clamp(distance, 0f, 3f);
        Debug.Log(distance);
        force = direction * distance * pushForce;

        currentHoop.UpdateScaleHoopNet(distance);
        currentHoop.RotateHoop(direction);

        trajectory.UpdateDots(player.pos, force);
    }
    private void OnDragEnd()
    {
        player.ActivateRb();
        player.Push(force);
        currentHoop.ResetScaleHoopNet();
        trajectory.Hide();
    }
    #endregion
}
