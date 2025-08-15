using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DuoMap : MonoBehaviour
{
    public static DuoMap Inst;

    private void Awake()
    {
        Inst = this;
    }

    [Header("[Grid]")]
    public Grid grid;
    
    [Header("[Monster]")]
    public Tilemap monsterTileMap;
    
    [Header("[Hero]")]
    public Tilemap heroTileMap;
    public Tilemap hero1PlayerTileMap;
    public Tilemap hero2PlayerTileMap;
    
    [Header("[Highlight]")]
    public Tilemap selectHighlightTileMap;

    [Header("[Spawn]")]
    public Tilemap spawn1PlayerTileMap;
    public Tilemap spawn2PlayerTileMap;

    [Header("[Path]")]
    public Tilemap path1PlayerTilemap;
    public Tilemap path2PlayerTilemap;
    public TileBase pathTile;

    private void Start()
    {
        // 1p만 적용
        hero2PlayerTileMap.gameObject.SetActive(false);
    }

    public Tilemap GetMyHeroTileMap(int playerIndex = 0)
    {
        return playerIndex switch
        {
            0 => hero1PlayerTileMap,
            1 => hero2PlayerTileMap,
            _ => throw new ArgumentOutOfRangeException(nameof(playerIndex), playerIndex, null)
        };
    }

    public TilemapRenderer GetMyHeroTileMapRenderer(int playerIndex = 0)
    {
        return playerIndex switch
        {
            0 => hero1PlayerTileMap.GetComponent<TilemapRenderer>(),
            1 => hero2PlayerTileMap.GetComponent<TilemapRenderer>(),
            _ => throw new ArgumentOutOfRangeException(nameof(playerIndex), playerIndex, null)
        };
    }

    public Tilemap GetSpawnTileMap(int playerIndex = 0)
    {
        return playerIndex switch
        {
            0 => spawn1PlayerTileMap,
            1 => spawn2PlayerTileMap,
            _ => throw new ArgumentOutOfRangeException(nameof(playerIndex), playerIndex, null)
        };
    }

    public Tilemap GetPathTileMap(int playerIndex = 0)
    {
        return playerIndex switch
        {
            0 => path1PlayerTilemap,
            1 => path2PlayerTilemap,
            _ => throw new ArgumentOutOfRangeException(nameof(playerIndex), playerIndex, null)
        };
    }

    public TileBase GetPathTileBase()
    {
        return pathTile;
    }
}
