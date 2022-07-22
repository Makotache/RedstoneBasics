using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedstoneItem : MonoBehaviour
{
    [SerializeField]
    private RedstoneTileType _currentRedstoneTileType = RedstoneTileType.Empty;
    public RedstoneTileType currentRedstoneTileType { get => _currentRedstoneTileType; set => _currentRedstoneTileType = value; }

    [SerializeField]
    private SpriteRenderer _padLock;

    [SerializeField]
    public bool changeLockState;

    [SerializeField]
    private bool _lockState;

    public bool lockState
    {
        get => _lockState;
        set
        {
            if(changeLockState)
            {
                _lockState = value;
                _padLock.enabled = _lockState;
            }
        }
    }


    [SerializeField]
    private Sprite[] _spriteItems;

    public Sprite spriteItem { get; private set; }

    private SpriteRenderer _spriteRenderer;
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _spriteItems[(int)currentRedstoneTileType];
        spriteItem = _spriteItems[(int)currentRedstoneTileType];
        gameObject.name = "RedstoneItem" + _currentRedstoneTileType;
    }


    private void OnValidate()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _spriteItems[(int)currentRedstoneTileType];
        spriteItem = _spriteItems[(int)currentRedstoneTileType];
        gameObject.name = "RedstoneItem" + _currentRedstoneTileType;
        lockState = lockState;
    }
}
