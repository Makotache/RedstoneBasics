using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnTileDirection
{
    Right,
    Left,
    None
}

public class InteractTileManager : MonoBehaviour
{

    private bool _isRemoveItem = false;
    public bool isRemoveItem
    {
        get => _isRemoveItem;
        set
        {
            _isRemoveItem = value;
            _turnTileDirection = TurnTileDirection.None;
            _configTile = false;
        }
    }

    private TurnTileDirection _turnTileDirection = TurnTileDirection.None;
    public TurnTileDirection turnTileDirection
    {
        get => _turnTileDirection;
        set
        {
            _isRemoveItem = false;
            _turnTileDirection = value;
            _configTile = false;
        }
    }

    private bool _configTile = false;
    public bool configTile
    {
        get => _configTile;
        set
        {
            _isRemoveItem = false;
            _turnTileDirection = TurnTileDirection.None;
            _configTile = value;
        }
    }

    private bool _isDraggingItem = false;
    private bool isDraggingItem
    {
        get => _isDraggingItem;
        set
        {
            _isDraggingItem = value;
            _iconItem.SetActive(_isDraggingItem);
        }
    }
    private Camera _camera;

    private SpriteRenderer _srpiteRendrer;
    [SerializeField]
    private GameObject _iconItem;
    private RedstoneItem _itemToMove;


    void Start()
    {
        _srpiteRendrer = _iconItem.GetComponent<SpriteRenderer>();
        _camera = Camera.main;
    }

    void Update()
    {
        Vector3 mouse_pos = _camera.ScreenToWorldPoint(Input.mousePosition);
        mouse_pos.z = -8;

        Debug.DrawRay(mouse_pos, Vector3.forward * 10, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(mouse_pos, Vector3.forward * 10);

        //on dois cliquer sur une tile ou un item pour lancer une interaction 
        if (hit.collider != null && Input.GetMouseButtonDown(0))
        {
            if(hit.collider.CompareTag("RedstoneTile"))
            {
                //interagir avec les tuiles présente, pour les supprimer
                if (isRemoveItem)
                {
                    RedstoneBase redstoneBase = hit.collider.gameObject.GetComponent<RedstoneBase>();
                    //on remplace l'objet que si il est autorisé a être remplacé
                    if (redstoneBase.canBeReplace && redstoneBase.redstoneTileType != RedstoneTileType.Empty)
                    {
                        //on supprime la tuile
                        GridManager gm = redstoneBase.gridManager;
                        GameObject new_redstoneBase = gm.ReplaceTile(RedstoneTileType.Empty, redstoneBase);
                        //new_redstoneBase.GetComponent<RedstoneBase>().ReloadAroundRedstoneAfterStart();
                    }
                }
                //interagir avec les tuiles présente, pour les tournés
                //en fonction de si on a un sens de rotation et si on a bien touché une tuile
                else if (turnTileDirection != TurnTileDirection.None)
                {
                    RedstoneBase redstoneBase = hit.collider.gameObject.GetComponent<RedstoneBase>();
                    //on tourne l'objet que si il est autorisé a tourné
                    if (redstoneBase.canBeTurn)
                    {
                        //on tourne en fonction du sens donné
                        redstoneBase.TurnTile(turnTileDirection == TurnTileDirection.Right);
                        //redstoneBase.ReloadAroundRedstone();
                    }
                }
                else if(configTile)
                {
                    RedstoneBase redstoneBase = hit.collider.gameObject.GetComponent<RedstoneBase>();
                    //on configure l'objet que si il est autorisé a être configuré
                    if (redstoneBase.canBeConfigurate)
                    {
                        //on configure en fonction du sens donné
                        redstoneBase.Configurate();
                        //redstoneBase.ReloadAroundRedstone();
                    }
                }
            }
            //pour mettre les items en tant que tuile
            //on ne dois pas avoir a appuyer les bouttons de suppression/tourner les tuiles, et on doit visé un item et cet item ne doit pas être lock
            else if (CanMoveItem() && turnTileDirection == TurnTileDirection.None && hit.collider.CompareTag("RedstoneItem") && !hit.collider.GetComponent<RedstoneItem>().lockState)
            {
                //on ne dois pas tourner les items donc on désative le fais de tourner
                turnTileDirection = TurnTileDirection.None;

                //et on récupère l'item a placer
                _itemToMove = hit.collider.gameObject.GetComponent<RedstoneItem>();

                //on change le sprite en fonction de celui de l'item a placer
                _srpiteRendrer.sprite = _itemToMove.spriteItem;

                //on active le mode deplacement d'item
                isDraggingItem = true;
            }
        }

        if (isDraggingItem)
        {
            if (Input.GetMouseButtonUp(0))
            {
                isDraggingItem = false;
                //Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                //RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

                //on remplace la tile visé par le type de l'item
                if (hit.collider != null && hit.collider.CompareTag("RedstoneTile"))
                {
                    RedstoneBase redstoneBase = hit.collider.gameObject.GetComponent<RedstoneBase>();
                    if(redstoneBase.canBeReplace)
                    {
                        GridManager gm = redstoneBase.gridManager;
                        gm.ReplaceTile(_itemToMove.currentRedstoneTileType, redstoneBase);
                    }

                }
            }
            else
            {
                _iconItem.transform.position = mouse_pos;
            }
        }
    }

    private bool CanMoveItem()
    {
        return !isRemoveItem && turnTileDirection == TurnTileDirection.None && !configTile;
    }
}
