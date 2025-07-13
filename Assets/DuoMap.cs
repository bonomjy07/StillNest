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

    private void Start()
    {
        // 멀티 적용전 꺼두기
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
}
