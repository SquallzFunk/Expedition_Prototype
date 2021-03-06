﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEWHexTileMap : MonoBehaviour
{
    
    void Start()
    {
        GenerateMap();
    }
    
//-------- VARIABLES ---------//     
    
    public GameObject HexPrefab;

    // Create meshes to hold more detailed tile information 
    public Mesh MeshWater;
    public Mesh MeshFlat;
    public Mesh MeshHill;
    public Mesh MeshMountain;

    public GameObject ForestPrefab;
    public GameObject JunglePrefab;

    // Create specific meshes for more detailed map generation
    public Material MatWater;
    public Material MatDesert;
    public Material MatGrassland;
    public Material MatMountain;
    public Material MatPlains;

    // Determining factors in moisture distribution 
    public float MoistureJungle = 1f;
    public float MoistureForest = 0.8f;
    public float MoistureGrasslands = 0.33f;
    public float MoisturePlains = 0f;

    // Tiles with height above whatever is whatever
    public float HeightMountain = 1f;
    public float HeightHill = 0.6f;
    public float HeightFlat = 0.0f;

    public int numRows;
    public int numColumns;
    
    // Wrapping options
    public bool allowWrapEastWest = true;
    public bool allowWrapNorthSouth = false;

    private Hex[,] hexes;
    private Dictionary<Hex, GameObject> hexToGameObjectMap;

    
    public Hex GetHexAt(int x, int y)
    {
        if (hexes == null)
        {
            Debug.LogError("Hexes array not yet instantiated!");
            return null;
        }

        if (allowWrapEastWest)
        {
            x = x % numColumns;
            if (x < 0)
            {
                x += numColumns;
            }
        }

        if (allowWrapNorthSouth)
        {
            y = y % numRows;
            if (y < 0)
            {
                y += numRows;
            }
        }
        
        return hexes[x, y];
    }
      
    public virtual void GenerateMap()
    {
        hexes = new Hex[numColumns, numRows];
        hexToGameObjectMap = new Dictionary<Hex, GameObject>();
        
        // Generate a map filled with ocean
        for (int column = 0; column < numColumns; column++)
        { 
            for (int row = 0; row < numRows; row++)
            {
                // Instantiate a Hex
                Hex h = new Hex( this, column, row);
                h.Elevation = -0.5f; // <------------------ BASE OCEAN LEVEL!

                hexes[column, row] = h;  

                Vector3 pos = h.PositionFromCamera(Camera.main.transform.position, numRows, numColumns);
                
                GameObject hexGO = (GameObject)Instantiate(HexPrefab, pos, Quaternion.identity, this.transform);

                hexToGameObjectMap[h] = hexGO;

                hexGO.name = string.Format("HEX: {0},{1}", column, row);
                hexGO.GetComponent<HexComponent>().Hex = h;
                hexGO.GetComponent<HexComponent>().NEWHexTileMap = this;

                hexGO.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", column, row);     
            }
        }
        
        UpdateHexVisuals();
        
        // StaticBatchingUtility.Combine(this.gameObject); FOR RENDERING ALL TILES AS STATIC FOR EFFICIENCY
    }

    public void UpdateHexVisuals()
    {
        for (int column = 0; column < numColumns; column++)
        {
            for (int row = 0; row < numRows; row++)
            {
                Hex h = hexes[column, row];
                GameObject hexGO = hexToGameObjectMap[h];
                
                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                MeshFilter mf = hexGO.GetComponentInChildren<MeshFilter>();

                // MOISTURE
                if (h.Elevation >= HeightFlat && h.Elevation < HeightMountain)
                {
                    if (h.Moisture >= MoistureJungle)
                    {
                        mr.material = MatGrassland;

                        // TODO: Spawn Jungle
                        Vector3 p = hexGO.transform.position;
                        if (h.Elevation >= HeightHill)
                        {
                            p.y += 0.3f;
                        }

                        GameObject.Instantiate(JunglePrefab, p, Quaternion.identity, hexGO.transform);
                    }
                    else if (h.Moisture >= MoistureForest)
                    {
                        mr.material = MatGrassland;

                        // Spawn Forest
                        Vector3 p = hexGO.transform.position;
                        if (h.Elevation >= HeightHill)
                        {
                            p.y += 0.3f;
                        }                            

                        GameObject.Instantiate(ForestPrefab, p, Quaternion.identity, hexGO.transform);
                    }
                    else if (h.Moisture >= MoistureGrasslands)
                    {
                        mr.material = MatGrassland;
                    }
                    else if (h.Moisture >= MoisturePlains)
                    {
                        mr.material = MatPlains;
                    }
                    else
                    {
                        mr.material = MatDesert;
                    }
                }

                // ELEVATION 
                if (h.Elevation >= HeightMountain)
                {
                    mr.material = MatMountain;
                    mf.mesh = MeshMountain;
                }
                else if (h.Elevation >= HeightHill)
                {
                    mf.mesh = MeshHill;
                }
                else if (h.Elevation >= HeightFlat)
                {
                    mf.mesh = MeshFlat;
                }
                else
                {
                    mr.material = MatWater;
                    mf.mesh = MeshWater;
                }

            }
        }
    }

    public Hex[] GetHexesWithinRangeOf(Hex centerHex, int range)
    {
        List<Hex> results = new List<Hex>();
        
        for (int dx = -range;  dx < range-1; dx++)
        {
            for (int dy = Mathf.Max(-range+1, -dx-range); dy < Mathf.Min(range, -dx+range-1); dy++)
            {
                results.Add (  GetHexAt(centerHex.Q + dx, centerHex.R +dy) );
            }
        }

        return results.ToArray();
    }
}
