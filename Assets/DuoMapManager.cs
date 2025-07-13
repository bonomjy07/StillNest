using UnityEngine;
using UnityEngine.Tilemaps;

public class DuoMapManager : Singleton<DuoMapManager>
{
    public Grid grid;
    public Tilemap monsterTileMap;
    public Tilemap heroTileMap;
}
