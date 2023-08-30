using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Board[] _puzzleBoards;
    [SerializeField] private int _currentPuzzleIndex = 0;
    [SerializeField] private string[] _musics;
    [SerializeField] private GameObject _gameObjectPaz;
    private bool _puzzleCompleted = false;
    private FMOD.Studio.EventInstance _instanceFmod;
    private bool _permitirMensajePaz = true;


    private void Start()
    {
       _instanceFmod = FMODUnity.RuntimeManager.CreateInstance($"event:/{_musics[_currentPuzzleIndex]}");
        ActivatePuzzle(_currentPuzzleIndex);
       
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
            _currentPuzzleIndex = (_currentPuzzleIndex + 1) % _puzzleBoards.Length; //Poner nueva variable de ultimoindex
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

        // Activa solo el rompecabezas actual
        _puzzleBoards[index].gameObject.SetActive(true);

        // FMODUnity.RuntimeManager.PlayOneShot($"event:/MusicEvents/{_musics[index]}");
        _instanceFmod.start();


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
}
