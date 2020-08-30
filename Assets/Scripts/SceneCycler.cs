using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class SceneCycler : MonoBehaviour
{
    // There are 3 tagged stages of player:
    // UncheckedPlayer-    Has not been found by the script yet, placed in array if slot is empty, deleted if slot already occupied. 
    // Player-             Active, was placed in "Player" GameObject array
    // InactivePlayer-     Inactive, was placed in "Player" GameObject array
    private GameObject[] Player;
    
    // Requires placing scenes in Editor
    public string[] cycledScenes = new string [3];

    // Remembers scene number, and which scene each Player array object occupies.
    private int sceneIteration;
    private int previousIteration;
    

    // maxTime used in editor to place wait time amount,
    // timePerScene sets bar size and determines time left.
    public float timePerScene;
    private float maxTime;
    
    // Slots
    public GameObject canvasObj;
    private GameObject uncheckedPlayer;
    private Image timerUI;
    
    // Remembered player properties
    private Vector2[] playerLocation = new Vector2[3];
    private Vector2[] playerVelocity = new Vector2[3];
    private Rigidbody2D[] playerRb2d = new Rigidbody2D[3];

    void Awake()
    {
        // The amount of players will depend on the amount of scenes in cycledScenes array.
        Player = new GameObject[cycledScenes.Length];    
        
        maxTime = timePerScene;
        
        // Destroy self if there's already a sceneCycler in place.
        // Is given the tag "Untagged" in the beginning so it does not find itself.
        if (GameObject.FindWithTag("SceneCycler"))
            Destroy(gameObject);
        else
            gameObject.tag = "SceneCycler";

        DontDestroyOnLoad(gameObject);
        timerUI = canvasObj.transform.GetChild(0).GetComponent<Image>();
        DontDestroyOnLoad(canvasObj);
        StartOrder();
    }

    // Update is called once per frame
    void Update()
    {
        // Fill amount of timer UI based on timer percentage.
        timerUI.fillAmount = timePerScene / maxTime;
        
        // Change scene upon current scene being different than cycledScenes iterated scene, currentScene = new scene.
        if (sceneIteration != previousIteration)
        {
            previousIteration = sceneIteration;
            Debug.Log("Scene change detected, changing scene to '" + cycledScenes[sceneIteration] + "'.");
            SceneManager.LoadScene(cycledScenes[sceneIteration]);
            StartOrder();
        }
    }
    
    // Find obj by "Player" tag, destroy if array space is occupied, fill array space if not occupied.
    IEnumerator FindPlayer()
    {
        // Slight delay is given, or the object will fail to be found.
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

    // Iterates current time downward by 0.01f.
    // Upon time <= 0, it stops, changes scene iteration, and disables current player.
    IEnumerator TimerIteration()
    {
        // Cycle down if above/not equal to 0.
        if (!(timePerScene <= 0))
        {
            yield return new WaitForSecondsRealtime(0.01f);
            timePerScene -= 0.01f;
            StartCoroutine(TimerIteration());
        }
        
        // Restart time, set current scene player to inactive, add to scene iteration (restart iteration if it goes above length).
        else
        {
            timePerScene = maxTime;
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
