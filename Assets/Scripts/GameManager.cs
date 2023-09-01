using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Board[] _puzzleBoards;
    [SerializeField] private int _currentPuzzleIndex = 0;
   // [SerializeField] private string _music;
    [SerializeField] private GameObject _gameObjectPaz;
    //[SerializeField] private float _musicLayer1;
    //[SerializeField] private float _musicLayer2;
    private bool _puzzleCompleted = false;
    private FMOD.Studio.EventInstance _instanceFmod;
    private bool _permitirMensajePaz = true;


    private void Start()
    {
       _instanceFmod = FMODUnity.RuntimeManager.CreateInstance("event:/MusicEvents/GameplayMusic/play_gameplay_music_112bpm");
        ActivatePuzzle(_currentPuzzleIndex);
        _instanceFmod.start();


    }

    private void Update()
    {
        if (_permitirMensajePaz)
        {
            if (_currentPuzzleIndex == 5)
            {
                _permitirMensajePaz = false;
                
                _gameObjectPaz.SetActive(true);
                StartCoroutine(Desactivar());

            }
        }
       
        if (_puzzleCompleted)
        {
            _puzzleCompleted = false;

            DeactivatePuzzle(_currentPuzzleIndex);
            _currentPuzzleIndex = (_currentPuzzleIndex + 1) % _puzzleBoards.Length; 
            if (_currentPuzzleIndex == 0)
            {
                // Todos los rompecabezas han sido completados, cambia de escena
                SceneManager.LoadScene("End");
                
            }
            else
            {
                ActivatePuzzle(_currentPuzzleIndex);
            }
        }
    }

    public void CompletePuzzle()
    {
        _puzzleCompleted = true;
    
    }

    private void ActivatePuzzle(int index)
    {
        foreach (Board puzzleBoard in _puzzleBoards)
        {
            puzzleBoard.gameObject.SetActive(false);
        }

        _puzzleBoards[index].gameObject.SetActive(true);


        var musicLayer1 = 0.0f;
        var musicLayer2 = 0.0f;
        var musicLayer3 = 0.0f;
        switch (index)
        {
            case 0:
                musicLayer1 = 1;
                musicLayer2 = 1;
                musicLayer3 = 1;
                break;
            case 1:
                musicLayer1 = 2;
                musicLayer2 = 1;
                musicLayer3 = 1;
                break;
            case 2:
                musicLayer1 = 3;
                musicLayer2 = 1;
                musicLayer3 = 1;
                break;
            case 3:
                musicLayer1 = 1;
                musicLayer2 = 1;
                musicLayer3 = 2;
                break;
            case 4:
                musicLayer1 = 1;
                musicLayer2 = 2;
                musicLayer3 = 2;
                break;
            case 5:
                musicLayer1 = 1;
                musicLayer2 = 3;
                musicLayer3 = 2;
                break;
        }

        SetMusicParameters(musicLayer1, musicLayer2, musicLayer3);

    }

    private void DeactivatePuzzle(int index)
    {
        _instanceFmod.release();
        _puzzleBoards[index].gameObject.SetActive(false);
    }

    private IEnumerator Desactivar()
    {
        yield return new WaitForSeconds(1f);
        _gameObjectPaz.SetActive(false);
        
    }

    public void SetMusicParameters(float musicLayer1, float musicLayer2, float musicLayer3)
    {
        _instanceFmod.setParameterByName("Gameplay Music 1 Layers", musicLayer1);
        _instanceFmod.setParameterByName("Gameplay Music 2 Layers", musicLayer2);
        _instanceFmod.setParameterByName("Parte Gameplay", musicLayer3);
    }
}
