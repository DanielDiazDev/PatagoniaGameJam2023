using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Board[] _puzzleBoards;
    private int _currentPuzzleIndex = 0;
    private bool _puzzleCompleted = false;
    // public static GameManager Instance { get; private set; }

    private void Awake()
    {
        //if (Instance == null)
        //{
        //    Instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        //{
        //    Destroy(gameObject);
        //    return;
        //}
    }
    private void Start()
    {
        ActivatePuzzle(_currentPuzzleIndex);
    }

    private void Update()
    {
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
        // _puzzleBoards[index].gameObject.SetActive(true);
        foreach (Board puzzleBoard in _puzzleBoards)
        {
            puzzleBoard.gameObject.SetActive(false);
        }

        // Activa solo el rompecabezas actual
        _puzzleBoards[index].gameObject.SetActive(true);
    }

    private void DeactivatePuzzle(int index)
    {
        _puzzleBoards[index].gameObject.SetActive(false);
    }
}
