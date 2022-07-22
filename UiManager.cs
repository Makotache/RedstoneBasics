using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class UiManager : MonoBehaviour
{
    [SerializeField] private Toggle _remove;
    [SerializeField] private Toggle _config;
    [SerializeField] private Toggle _turnRight;
    [SerializeField] private Toggle _turnLeft;
    [SerializeField] private List<RedstoneItem> _items;
    private List<Toggle> _toggles = new List<Toggle>();
    [SerializeField] private GameObject pauseGO;
    [SerializeField] private GameObject victoryGO;
    [SerializeField] private GameObject objectiveGO;
    [SerializeField] private TextMeshProUGUI textObjective;

    public void Init(int level)
    {
        _toggles.AddRange(new Toggle[] { _remove, _config, _turnRight, _turnLeft });

        if (level != -1)
        {
            EnableAll(false);
        }
    }

    public void Win()
    {
        victoryGO.SetActive(true);
        pauseGO.GetComponent<Button>().interactable = false;
        objectiveGO.GetComponent<Button>().interactable = false;
        EnableAll(false);
    }

    public void SetVictoryText(string text)
    {
        textObjective.text = text;
    }

    public void FreeMode()
    {
        //objectiveGO.SetActive(false);
    }

    public void InvertActiveRemove(bool value)
    {
        InvertActive(value, _remove);
    }

    public void InvertActiveConfig(bool value)
    {
        InvertActive(value, _config);
    }

    public void InvertActiveTurnRight(bool value)
    {
        InvertActive(value, _turnRight);
    }

    public void InvertActiveTurnLeft(bool value)
    {
        InvertActive(value, _turnLeft);
    }

    private void InvertActive(bool value, Toggle tog)
    {
        _toggles.Where(t => t != tog).ToList().ForEach(t => t.interactable = !value);
        _items.ForEach(i => i.lockState = value);
    }

    public void PauseResume(bool isPause)
    {
        if(isPause)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void EnableAll(bool value)
    {
        _toggles.ForEach(t => t.interactable = value);
        _items.ForEach(i => i.lockState = !value);
    }


    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
