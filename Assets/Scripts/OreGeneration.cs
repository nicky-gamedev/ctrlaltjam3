using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OreGeneration : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject ore;

    [Header("Chunk Settings")]
    [SerializeField] private Vector2 chunkDimensions;
    [SerializeField] private Vector2 chunkPosition;
    [SerializeField] [Range(0f,1f)] private float oreRarity;
    [SerializeField] private int initialChunks;

    [Header("Perlin Noise settings")]
    [SerializeField] private float scale;
    [SerializeField] private float width;
    [SerializeField] private float height;

    private void Start()
    {
        for (int i = 0; i < initialChunks; i++)
        {
            GenerateChunk();
        }          
    }

    public void GenerateChunk()
    {
        var root = new GameObject();
        root.transform.position = chunkPosition;
        for (int i = 0; i < chunkDimensions.x; i++)
        {
            for (int j = 0; j < chunkDimensions.y; j++)
            {
                float xCoord = i / width * scale * Random.value;
                float yCoord = j / height* scale * Random.value;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                Debug.Log(sample);
                if(sample >= oreRarity)
                {
                    var newOre = Instantiate(ore, root.transform);
                    newOre.transform.localPosition = new Vector3(i, -j, 0);
                }
            }
        }

        chunkPosition.y -= chunkDimensions.y;
    }
}
