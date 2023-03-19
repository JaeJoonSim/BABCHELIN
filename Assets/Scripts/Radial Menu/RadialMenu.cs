using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public partial class RadialMenu : MonoBehaviour
{
    [Tooltip("조각 개수")]
    [SerializeField] private int _pieceCount = 8;

    [Tooltip("등장에 걸리는 시간")]
    [SerializeField] private float _appearanceDuration = .3f;
    [Tooltip("소멸에 걸리는 시간")]
    [SerializeField] private float _disppearanceDuration = .3f;
    [Tooltip("중앙으로부터 각 조각의 거리")]
    [SerializeField] private float _pieceDist = 180f;

    [Tooltip("인식이 안되는 중앙 거리")]
    [SerializeField] private float _centerRange = 0.1f;

    [Tooltip("복제될 게임 오브젝트")]
    [SerializeField] private GameObject _pieceSample;
    [Tooltip("화살표의 부모 트랜스폼")]
    [SerializeField] private RectTransform _arrow;

    private Image[] _pieceImages;
    private RectTransform[] _pieceRects;
    private Vector2[] _pieceDirections;

    private float _arrowRotationZ;

    [SerializeField]
    private int _selectedIndex = -1;

    private const float NotSelectedPieceAlpha = 0.5f;
    private static readonly Color SelectedPieceColor = new Color(1f, 1f, 1f, 1f);
    private static readonly Color NotSelectedPieceColor = new Color(1f, 1f, 1f, NotSelectedPieceAlpha);

    private void Awake()
    {
        InitPieceImages();
        InitPieceDirections();
        InitStateDicts();

        HideGameObject();
    }

    private void InitPieceImages()
    {
        _pieceSample.SetActive(true);

        _pieceImages = new Image[_pieceCount];
        _pieceRects = new RectTransform[_pieceCount];

        for (int i = 0; i < _pieceCount; i++)
        {
            var clone = Instantiate(_pieceSample, transform);
            clone.name = $"Piece {i}";
            
            _pieceImages[i] = clone.GetComponent<Image>();
            _pieceRects[i] = _pieceImages[i].rectTransform;
        }

        _pieceSample.SetActive(false);
    }

    private void InitPieceDirections()
    {
        _pieceDirections = new Vector2[_pieceCount];

        float angle = 360f / _pieceCount;

        for (int i = 0; i < _pieceCount; i++)
        {
            //float angle = i * (Mathf.PI * 2.0f) / _pieceCount;

            _pieceDirections[i] = new ClockwisePolarCoord(1f, angle * i).ToVector2();
            //_pieceDirections[i] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) *1f;
        }
    }

    private void ShowGameObject()
    {
        gameObject.SetActive(true);
    }

    private void HideGameObject()
    {
        gameObject.SetActive(false);
    }

    private void SetPieceAlpha(int index, float alpha)
    {
        _pieceImages[index].color = new Color(1f, 1f, 1f, alpha);
    }

    private void SetPieceDistance(int index, float distance)
    {
        _pieceRects[index].anchoredPosition = _pieceDirections[index] * distance;
    }

    private void SetPieceScale(int index, float scale)
    {
        _pieceRects[index].localScale = new Vector3(scale, scale, 1f);
    }

    private void SetAllPieceDistance(float distance)
    {
        for (int i = 0; i < _pieceCount; i++)
        {
            _pieceRects[i].anchoredPosition = _pieceDirections[i] * distance;
        }
    }

    private void SetAllPieceAlpha(float alpha)
    {
        for (int i = 0; i < _pieceCount; i++)
        {
            _pieceImages[i].color = new Color(1f, 1f, 1f, alpha);
        }
    }

    private void SetAllPieceScale(float scale)
    {
        for (int i = 0; i < _pieceCount; i++)
        {
            _pieceRects[i].localScale = new Vector3(scale, scale, 1f);
        }
    }

    private void SetAllPieceImageEnabled(bool enabled)
    {
        for (int i = 0; i < _pieceCount; i++)
        {
            _pieceImages[i].enabled = enabled;
        }
    }

    private void SetArrow(bool show)
    {
        _arrow.gameObject.SetActive(show);

        if (show)
        {
            _arrow.eulerAngles = Vector3.forward * _arrowRotationZ;
        }
    }

    public void Show()
    {
        ForceToEnterAppearanceState();
    }

    public int Hide()
    {
        ForceToEnterDisappearanceState();
        SetArrow(false);

        return _selectedIndex;
    }
    
    public void SetPieceImageSprites(Sprite[] sprites)
    {
        int i = 0;
        int len = sprites.Length;
        for (; i < _pieceCount && i < len; i++)
        {
            if (sprites[i] != null)
            {
                _pieceImages[i].sprite = sprites[i];
            }
        }
    }
}
