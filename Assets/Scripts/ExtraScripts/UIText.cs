using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIText : MonoBehaviour
{
    private Text myText;
    private string sceneName;

    // Update is called once per frame
    void Awake()
    {
        myText = GetComponent<Text>();
        StartCoroutine(TextChange());
    }

    IEnumerator TextChange()
    {
        sceneName = SceneManager.GetActiveScene().name;
        if (myText.text != "Current scene: " + sceneName)
        {
            myText.text = "Current scene: " + sceneName;
        }
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(TextChange());
    }
}
