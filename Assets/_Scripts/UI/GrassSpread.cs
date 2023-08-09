using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GrassSpread : MonoBehaviour
{
    public Tilemap tilemap;
    public List<TileBase> grassTiles;
    public List<GameObject> plantPrefabs;

    public float spreadRadius = 3f;
    public float spreadRate = 1f;

    private List<Vector3Int> tiles = new List<Vector3Int>();
    private int currentTileIndex = 0;
    private float elapsedTime = 0f;

    private void Start()
    {
        Vector3Int center = tilemap.WorldToCell(transform.position);
        for (int x = center.x - Mathf.RoundToInt(spreadRadius); x <= center.x + Mathf.RoundToInt(spreadRadius); x++)
        {
            for (int y = center.y - Mathf.RoundToInt(spreadRadius); y <= center.y + Mathf.RoundToInt(spreadRadius); y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                float distance = Vector3Int.Distance(center, tilePos);
                if (distance <= spreadRadius)
                {
                    tiles.Add(tilePos);
                }
            }
        }

        Vector3Int centerCell = tilemap.WorldToCell(transform.position);
        tiles.Sort((a, b) =>
        {
            float aDistance = Vector3Int.Distance(a, centerCell);
            float bDistance = Vector3Int.Distance(b, centerCell);
            if (Mathf.Approximately(aDistance, bDistance))
            {
                Vector3Int aDiff = a - centerCell;
                Vector3Int bDiff = b - centerCell;
                float aAngle = Mathf.Atan2(aDiff.y, aDiff.x);
                float bAngle = Mathf.Atan2(bDiff.y, bDiff.x);
                return aAngle.CompareTo(bAngle);
            }
            else
            {
                return aDistance.CompareTo(bDistance);
            }
        });
    }

    private int lastProcessedIndex = -1;

    private void Update()
    {
        if (elapsedTime >= spreadRate && currentTileIndex < tiles.Count)
        {
            TileBase randomGrassTile = grassTiles[Random.Range(0, grassTiles.Count)];
            Vector3Int tilePos = tiles[currentTileIndex];

            if (Random.Range(0f, 1f) < 0.05f)
            {
                Vector3 plantPos = tilemap.GetCellCenterWorld(tilePos);
                plantPos.z = transform.position.z;
                Instantiate(plantPrefabs[Random.Range(0, plantPrefabs.Count)], plantPos, Quaternion.identity);

                tilemap.SetTile(tilePos, null);
                TileBase newGrassTile = Instantiate(randomGrassTile);
                tilemap.SetTile(tilePos, newGrassTile);
            }
            else
            {
                tilemap.SetTile(tilePos, randomGrassTile);
            }

            lastProcessedIndex = currentTileIndex;
            currentTileIndex++;
            elapsedTime = 0f;
        }
        else if (lastProcessedIndex >= 0 && lastProcessedIndex < tiles.Count)
        {
            TileBase randomGrassTile = grassTiles[Random.Range(0, grassTiles.Count)];
            Vector3Int tilePos = tiles[lastProcessedIndex];

            if (Random.Range(0f, 1f) < 0.05f)
            {
                Vector3 plantPos = tilemap.GetCellCenterWorld(tilePos);
                plantPos.z = transform.position.z;
                Instantiate(plantPrefabs[Random.Range(0, plantPrefabs.Count)], plantPos, Quaternion.identity);

                tilemap.SetTile(tilePos, null);
                TileBase newGrassTile = Instantiate(randomGrassTile);
                tilemap.SetTile(tilePos, newGrassTile);
            }
            else
            {
                tilemap.SetTile(tilePos, randomGrassTile);
            }

            lastProcessedIndex++;
        }
        else
        {
            elapsedTime += Time.deltaTime;
        }
    }

}
