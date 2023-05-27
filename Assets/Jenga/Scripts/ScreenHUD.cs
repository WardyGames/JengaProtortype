using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenHUD : MonoBehaviour
{
    [SerializeField] Button nextStackButton;
    [SerializeField] Button previousStackButton;
    [SerializeField] Button testMyStackButton;
    [SerializeField] Button restartButton;
    [SerializeField] Button closeButton;

    public static event Action NextJengaStack;
    public static event Action PreviousJengaStack;
    public static event Action TestJengaStack;
    void Awake()
    {
        nextStackButton.onClick.AddListener(NextStack);
        previousStackButton.onClick.AddListener(PreviousStack);
        testMyStackButton.onClick.AddListener(TestMyStack);
        restartButton.onClick.AddListener(Restart);
        closeButton.onClick.AddListener(Application.Quit);
    }

    void NextStack()
    {
        NextJengaStack?.Invoke();
    }

    void PreviousStack()
    {
        PreviousJengaStack?.Invoke();
    }

    void TestMyStack()
    {
        TestJengaStack?.Invoke();
    }

    void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
