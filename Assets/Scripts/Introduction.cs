using System.Collections;
using TMPro;
using UnityEngine;

public class Introduction : MonoBehaviour
{
    [SerializeField] private float _writeSpeed = 0.1f;
    [SerializeField] private string[] _dialogs;
    [SerializeField] private TextMeshProUGUI _textInformaticMessage;
    private int _currentIndexText = 0;
    private string _currentIndex;
    private int _indexVisible = 0;
    [SerializeField] private GameObject _informaticGameObject;
    [SerializeField] private GameObject _comandanteGameObject;

    private void Start()
    {
        ShowNextText();
    }

    private void ShowNextText()
    {
        if (_currentIndexText < _dialogs.Length)
        {
            _currentIndex = _dialogs[_currentIndexText];
            _textInformaticMessage.text = "";
            _indexVisible = 0;
            StartCoroutine(ShowTextSlow());
            _currentIndexText++;
        }
        else
        {                    
            _informaticGameObject.SetActive(false);
            _comandanteGameObject.SetActive(true);
        }
    }

    private IEnumerator ShowTextSlow()
    {
        while (_indexVisible <= _currentIndex.Length)
        {
            _textInformaticMessage.text = _currentIndex.Substring(0, _indexVisible);
            _indexVisible++;
            yield return new WaitForSeconds(_writeSpeed);
        }

        yield return new WaitForSeconds(1f); 
        _textInformaticMessage.text = "";

        ShowNextText();
    }
}
