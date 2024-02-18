using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [SerializeField] GameObject finishPrefab;

    [SerializeField] Level[] levels;

    Level currentLevel;

    public bool finished = false;
    public bool win = false;

    private void OnEnable()
    {
        Stack.OnLastPlace -= OnLastPlace;
        Stack.OnLastPlace += OnLastPlace;

        Stack.OnPlace -= OnPlace;
        Stack.OnPlace += OnPlace;

        InputManager.OnPress -= OnPress;
        InputManager.OnPress += OnPress;
    }
    private void OnDisable()
    {
        Stack.OnLastPlace -= OnLastPlace;

        InputManager.OnPress -= OnPress;
    }
    private void OnPlace(Stack sender, bool successful)
    {
        if (sender.id == 0)
        {
            return;
        }

        if (sender.inPerfectPlace)
            AudioManager.Instance.IncreasePitch();
        else
            AudioManager.Instance.ResetPitch();


        AudioManager.Instance.Play();
    }

    private void OnLastPlace(Stack sender, bool successful)
    {
        FinishLevel(successful);
    }
    private void OnPress()
    {
        if (!finished || !win)
            return;

        SpawnLevel();
    }

    private void Start()
    {
        SpawnLevel();
    }

    private void SpawnLevel()
    {
        finished = false;
        win = false;

        int persistent = StackManager.Instance.persistentPlacedCount;
        currentLevel = levels[PlayerPrefs.GetInt("LevelId", 0)];
        var f = Instantiate(finishPrefab);
        var pos = (Vector3.forward) * currentLevel.length * GameUtil.DefaultStackScale.z;

        if (persistent > 0)
            pos.z += GameUtil.DefaultStackScale.z * persistent;

        f.transform.position = pos;

        StackManager.Instance.StartSpawning(currentLevel.length);

        StackManager.CAN_PLACE = true;
    }

    public void FinishLevel(bool success)
    {
        if (finished)
            return;

        StackManager.CAN_PLACE = false;
        finished = true;
        if (win = success)
        {
            var currentLevelId = PlayerPrefs.GetInt("LevelId", 0);
            if (currentLevelId + 1 >= levels.Length)
                currentLevelId = 0;
            PlayerPrefs.SetInt("LevelId", currentLevelId + 1);

            StackManager.Instance.persistentPlacedCount++; //Also Count For Finish Line
        }
        else
        {
            DOVirtual.DelayedCall(3f, () =>
            {
                StackManager.CAN_PLACE = true;
                SceneManager.LoadScene(0);
            });
        }
    }
}
