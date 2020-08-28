using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class SceneCycler : MonoBehaviour
{
    public GameObject[] Player = new GameObject[3];
    public string[] cycledScenes = new string [3];

    public int sceneIteration;
    public int previousIteration;
    
    private Image timerUI;

    // maxTime used in editor to place wait time amount,
    // currentTime sets bar size and determines time left.
    public float currentTime;
    private float maxTime;
    private GameObject myself;
    public GameObject canvasObj;
    public GameObject uncheckedPlayer;

    public Vector2[] playerLocation = new Vector2[3];
    public Vector2[] playerVelocity = new Vector2[3];
    public Rigidbody2D[] playerRb2d = new Rigidbody2D[3];
    // Start is called before the first frame update
    void Awake()
    {
        maxTime = currentTime;
        // Destroy self if there's already a scenecycler in place.
        var findOther = GameObject.FindWithTag("SceneCycler");
        if (findOther)
            Destroy(gameObject);
        else
        {
            myself = findOther;
            gameObject.tag = "SceneCycler";
        }
        DontDestroyOnLoad(gameObject);
        timerUI = canvasObj.transform.GetChild(0).GetComponent<Image>();
        DontDestroyOnLoad(canvasObj);
        StartOrder();
    }

    // Update is called once per frame
    void Update()
    {
        // Fill amount of timer UI based on timer percentage.
        timerUI.fillAmount = currentTime / maxTime;
        
        // Change scene upon current scene being different than cycledScenes iterated scene, currentScene = new scene.
        if (sceneIteration != previousIteration)
        {
            previousIteration = sceneIteration;
            Debug.Log("Scene change detected, changing scene to '" + cycledScenes[sceneIteration] + "'.");
            SceneManager.LoadScene(cycledScenes[sceneIteration]);
            StartOrder();
        }
    }
    
    // Find obj by "Player" tag, delete tagged obj "UncheckedPlayer" if found, give "UncheckedPlayer" the "Player" tag if none found.
    IEnumerator FindPlayer()
    {
        yield return new WaitForSecondsRealtime(0.01f);
        uncheckedPlayer = GameObject.FindWithTag("UncheckedPlayer");
        if (uncheckedPlayer && Player[sceneIteration])
        {
            Destroy(uncheckedPlayer);
            ActivatePlayer();
        }
        else
        {
            CreatePlayer();
        }
    }

    // Iterates current time downward by 0.01f, stops repeating and changes iteration upon time equaling or going below 0.
    IEnumerator TimerIteration()
    {
        // Cycle down if above/not equal to 0.
        if (!(currentTime <= 0))
        {
            yield return new WaitForSecondsRealtime(0.01f);
            currentTime -= 0.01f;
            StartCoroutine(TimerIteration());
        }
        
        // Restart time, set current scene player to inactive, add to scene iteration (restart iteration if it goes above length).
        else
        {
            currentTime = maxTime;
            if (sceneIteration == cycledScenes.Length -1)
            {
                DeactivatePlayer();
                sceneIteration = 0;
                Debug.Log("Scene going above, cycle restarted to " + sceneIteration);
            }
            else
            {
                DeactivatePlayer();
                sceneIteration += 1;
                Debug.Log("Scene iterated to " + sceneIteration);
            }
        }
    }

    private void ActivatePlayer()
    {
        Player[sceneIteration].SetActive(true);
        Player[sceneIteration].tag = "Player";
        Player[sceneIteration].transform.position = playerLocation[sceneIteration];
        playerRb2d[sceneIteration].velocity = playerVelocity[sceneIteration];
    }
    
    private void DeactivatePlayer()
    {
        Player[sceneIteration].tag = "InactivePlayer";
        playerLocation[sceneIteration] = Player[sceneIteration].transform.position;
        playerVelocity[sceneIteration] = playerRb2d[sceneIteration].velocity;
        Player[sceneIteration].SetActive(false);
    }

    private void CreatePlayer()
    {
        Player[sceneIteration] = uncheckedPlayer;
        Player[sceneIteration].tag = "Player";
        playerRb2d[sceneIteration] = uncheckedPlayer.GetComponent<Rigidbody2D>();
        DontDestroyOnLoad(Player[sceneIteration]);
    }

    void StartOrder()
    {
        StartCoroutine(FindPlayer());
        //yield return new WaitForSeconds(0.01f);
        StartCoroutine(TimerIteration());
    }
}
