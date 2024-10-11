using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WarningSceneManager : MonoBehaviour
{
    public GameObject warningObject;

    Animator warningObjectAnimator;

    public void Awake()
    {
        warningObjectAnimator = warningObject.GetComponent<Animator>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        warningObject.SetActive(true);

    }

    public void StartNextTitleSceneCoroutine()
    {
        StartCoroutine("NextTitleSceneCoroutine");
    }

    IEnumerator NextTitleSceneCoroutine()
    {
        warningObjectAnimator.SetTrigger("windowOut");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("TitleScene");
    }
}
