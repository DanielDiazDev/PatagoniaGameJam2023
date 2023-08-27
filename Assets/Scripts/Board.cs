using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class Board : MonoBehaviour
{
    [SerializeField] private Transform _boardTransform;
    [SerializeField] private Piece _piecePrefab;
    [SerializeField] private int _size;
    [SerializeField] private GameObject _imageComplete;
    [SerializeField] private int clickCount;
    [SerializeField] private int maxCount;
    [SerializeField] private List<BoxCollider2D> colliders;
    [SerializeField] private GameObject _button;
    [SerializeField] private GameObject _textIntern;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _targetText;
    [SerializeField] private GameManager _gameManager;

    private List<Piece> _pieces;
    private int _emptyLocation;

    private void Start()
    {
        _pieces = new List<Piece>();
        CreatePieces(0.01f);
        Shuffle();
    }
    private void Update()
    {
        if (CheckCompletion())
        {

            foreach (Piece piece in _pieces)
            {
                piece.gameObject.SetActive(false);
            }
            _targetText.SetActive(true);
            _imageComplete.SetActive(true);
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                foreach (var collider in colliders)
                {
                    if (collider.OverlapPoint(mousePosition))
                    {
                        clickCount++;
                        Debug.Log("Click Count: " + clickCount);
                        collider.enabled = false;
                        break;
                    }
                }
                if (clickCount == maxCount)
                {
                    _imageComplete.SetActive(false);
                    _textIntern.SetActive(true);
                    _button.SetActive(true);

                }
            }
        }



        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                var selectedPiece = hit.transform.GetComponent<Piece>();
                if (selectedPiece != null)
                {
                    int selectedIndex = _pieces.IndexOf(selectedPiece);

                    if (selectedIndex != -1 && selectedIndex != _emptyLocation)
                    {
                        SwapPieces(selectedIndex, _emptyLocation);
                    }
                }
            }
        }
    }
    public void ChangePuzzle()
    {
        _animator.enabled = true;
        Invoke("FadeIn", 2);
        _targetText.SetActive(false);
        _imageComplete.SetActive(false);
        _textIntern.SetActive(false);
        _button.SetActive(false);
        _animator.enabled = false;
        //Agregar sonido al fade?
        _gameManager.CompletePuzzle();

    }
    private void FadeIn()
    {

        _animator.Play("FadeIn");

    }


    private void SwapPieces(int indexA, int indexB)
    {
        (_pieces[indexA], _pieces[indexB]) = (_pieces[indexB], _pieces[indexA]);
        (_pieces[indexA].transform.localPosition, _pieces[indexB].transform.localPosition) = (_pieces[indexB].transform.localPosition, _pieces[indexA].transform.localPosition);
        _emptyLocation = indexA;

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX Events/SlidingPieces");

    }
    private void CreatePieces(float spaceBeetweenPieces)
    {
        float width = 1 / (float)_size;
        for (int row = 0; row < _size; row++)
        {
            for (int col = 0; col < _size; col++)
            {
                var piece = Instantiate(_piecePrefab, _boardTransform);

                _pieces.Add(piece);

                CalculateLocalPosition(piece, width, row, col);
                piece.transform.localScale = ((2 * width) - spaceBeetweenPieces) * Vector3.one;
                piece.name = $"{(row * _size) + col}";
                if ((row == _size - 1) && col == _size - 1)
                {
                    _emptyLocation = (_size * _size) - 1;
                    piece.gameObject.SetActive(false);

                }
                else
                {
                    float space = spaceBeetweenPieces / 2;
                    var mesh = piece.GetComponent<MeshFilter>().mesh;
                    Vector2[] uv = new Vector2[4];
                    ApplyAdjustedUV(width, row, col, space, uv);
                    mesh.uv = uv;

                }

            }
        }

    }

    private void CalculateLocalPosition(Piece piece, float width, int row, int col)
    {
        float offsetX = -1 + (2 * width * col) + width;
        float offsetY = 1 - (2 * width * row) - width;
        Vector3 position = new Vector3(offsetX, offsetY, 0);
        piece.transform.localPosition = position;
    }

    private void ApplyAdjustedUV(float width, int row, int col, float space, Vector2[] uv)
    {
        uv[0] = new Vector2((width * col) + space, 1 - ((width * (row + 1)) - space));
        uv[1] = new Vector2((width * (col + 1)) - space, 1 - ((width * (row + 1)) - space));
        uv[2] = new Vector2((width * col) + space, 1 - ((width * row) + space));
        uv[3] = new Vector2((width * (col + 1)) - space, 1 - ((width * row) + space));
    }
    private bool SwapIfValid(int i, int offset, int colCheck)
    {
        if (((i % _size) != colCheck) && ((i + offset) == _emptyLocation))
        {
            (_pieces[i], _pieces[i + offset]) = (_pieces[i + offset], _pieces[i]);
            (_pieces[i].transform.localPosition, _pieces[i + offset].transform.localPosition) = ((_pieces[i + offset].transform.localPosition, _pieces[i].transform.localPosition));
            _emptyLocation = i;
            return true;
        }
        return false;
    }
    private bool CheckCompletion()
    {
        for (int i = 0; i < _pieces.Count; i++)
        {
            if (_pieces[i].name != $"{i}")
            {
                return false;
            }
        }


        return true;
    }


    private void Shuffle()
    {
        int count = 0;
        int last = 0;
        while (count < (_size * _size * _size))
        {
            int rnd = UnityEngine.Random.Range(0, _size * _size);
            if (rnd == last) { continue; }
            last = _emptyLocation;
            if (SwapIfValid(rnd, -_size, _size))
            {
                count++;
            }
            else if (SwapIfValid(rnd, +_size, _size))
            {
                count++;
            }
            else if (SwapIfValid(rnd, -1, 0))
            {
                count++;
            }
            else if (SwapIfValid(rnd, +1, _size - 1))
            {
                count++;
            }
        }
    }

}