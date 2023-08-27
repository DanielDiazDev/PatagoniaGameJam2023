using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private Transform _boardTransform;
    [SerializeField] private Piece _piecePrefab;
    [SerializeField] private int _size;
    public GameObject clickableArea; // Asigna el GameObject vacío en el Inspector
    public string messageToShow = "¡Hiciste clic en la zona!";
    private List<Piece> _pieces;
    private int _emptyLocation;
  //  private bool _shuffling;

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
            //Aqui iria la funcion de tocar 

            foreach (Piece piece in _pieces)
            {
                piece.gameObject.SetActive(false);
            }
            GameManager.Instance.CompletePuzzle();
           
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

    private void SwapPieces(int indexA, int indexB)
    {
        (_pieces[indexA], _pieces[indexB]) = (_pieces[indexB], _pieces[indexA]);
        (_pieces[indexA].transform.localPosition, _pieces[indexB].transform.localPosition) = (_pieces[indexB].transform.localPosition, _pieces[indexA].transform.localPosition);
        _emptyLocation = indexA; 
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
//if (Input.GetMouseButtonDown(0))
//{
//    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
//    if (hit)
//    {
//        for (int i = 0; i < _pieces.Count; i++)
//        {
//            if (_pieces[i].transform == hit.transform)
//            {

//                if (SwapIfValid(i, -_size, _size)) { break; }
//                if (SwapIfValid(i, +_size, _size)) { break; }
//                if (SwapIfValid(i, -1, 0)) { break; }
//                if (SwapIfValid(i, +1, _size - 1)) { break; }
//            }

//        }
//    }
//}

//private IEnumerator WaitShuffle(float duration)
//{
//    yield return new WaitForSeconds(duration);
//    Shuffle();
//    _shuffling = false;
//}