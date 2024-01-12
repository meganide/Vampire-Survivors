using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InfiniteTerrain : MonoBehaviour {
    [SerializeField] private Tilemap backgroundTilemap;

    private int backgroundTilemapWidth = 28;
    private int backgroundTilemapHeight = 18;
    private List<Tilemap> tilemaps = new List<Tilemap>();

    private Grid grid;
    private GameObject player;
    private Vector3 playerPosition;
    private LayerMask backgroundLayerMask;


    private void Awake() {
        grid = GameObject.FindObjectOfType<Grid>();
        player = GameObject.FindGameObjectWithTag("Player");
        backgroundLayerMask = LayerMask.GetMask("Background");
    }

    private void Start() {
        tilemaps.Add(backgroundTilemap);
    }

    void Update() {
        playerPosition = player.transform.position;

        SpawnTilemaps();
        DeleteDistantTilemaps();
    }

    private void SpawnTilemaps() {
        Tilemap currentBackgroundTilemap = GetTilemapAtPosition(playerPosition);
        if (currentBackgroundTilemap != null) {
            Vector3 tilemapPosition = currentBackgroundTilemap.gameObject.transform.position;
            CheckNeighboringTilemaps(tilemapPosition);
        }
    }

    private Tilemap? GetTilemapAtPosition(Vector3 position) {
        Collider2D backgroundCollider = Physics2D.OverlapBox(position, new Vector2(1, 1), 0, backgroundLayerMask);
        if (backgroundCollider != null) {
            Tilemap currentBackgroundTilemap = backgroundCollider.GetComponentInParent<Tilemap>();
            if (currentBackgroundTilemap != null) {
                return currentBackgroundTilemap;
            }
        }

        return null;
    }

    private void CheckNeighboringTilemaps(Vector3 currentTilemapPosition) {
        Vector3[] neighborOffsets = new Vector3[]
        {
            new Vector3(0, backgroundTilemapHeight, 0),   // Top
            new Vector3(backgroundTilemapWidth, 0, 0),   // Right
            new Vector3(0, -backgroundTilemapHeight, 0),  // Bottom
            new Vector3(-backgroundTilemapWidth, 0, 0),  // Left

            new Vector3(-backgroundTilemapWidth, backgroundTilemapHeight, 0),  // Top-Left
            new Vector3(backgroundTilemapWidth, backgroundTilemapHeight, 0),   // Top-Right
            new Vector3(-backgroundTilemapWidth, -backgroundTilemapHeight, 0), // Bottom-Left
            new Vector3(backgroundTilemapWidth, -backgroundTilemapHeight, 0)   // Bottom-Right
        };

        foreach (var offset in neighborOffsets) {
            Vector3 neighborPosition = offset + currentTilemapPosition;
            Tilemap? neighborTilemap = GetTilemapAtPosition(neighborPosition);
            if (neighborTilemap == null) {
                InstantiateTilemap(neighborPosition);
            }
        }
    }


    private void InstantiateTilemap(Vector3 position) {
        Tilemap tilemap = Instantiate(backgroundTilemap, position, Quaternion.identity);
        tilemap.transform.SetParent(grid.transform);
        tilemaps.Add(tilemap);
    }

    private void DeleteDistantTilemaps() {
        List<Tilemap> tilemapsToRemove = new List<Tilemap>();

        foreach (var tilemap in tilemaps) {
            float distanceToTilemap = Vector2.Distance(playerPosition, tilemap.transform.position);
            if (distanceToTilemap > 36) {
                tilemapsToRemove.Add(tilemap);
            }
        }

        foreach (var tilemapToRemove in tilemapsToRemove) {
            tilemaps.Remove(tilemapToRemove);
            Destroy(tilemapToRemove.gameObject);
        }
    }
}

