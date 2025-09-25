using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridPlacement : MonoBehaviour
{
    // placment settings
    public float gridSize = 1f;
    public int maxPlacements = 5;

    //placable objects
    public List<GameObject> placeableObjects;
    private int currentIndex = 0;
    public GameObject thingToPlace;

    //UI
    public TMP_Text placementText;
    public TMP_Text selectedObjectText;

    private GameObject ghostObject;
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();
    private int currentPlacements = 0;

    private void Start()
    {
        if (placeableObjects.Count > 0)
        {
            thingToPlace = placeableObjects[currentIndex];
            CreateGhost();
            UpdateSelectedObjectText();
        }
        UpdatePlacementText();

    }

    private void Update()
    {
        HandleObjectSelection();
        UpdateGhostPosition();

        if (Input.GetMouseButtonDown(0))
            PlaceObject();
        if (Input.GetMouseButtonDown(1))
            DestroyObject();
    }

    void HandleObjectSelection()
    {
        // Example: keys 1–9 pick objects from the list
        for (int i = 0; i < placeableObjects.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                currentIndex = i;
                thingToPlace = placeableObjects[currentIndex];
                Destroy(ghostObject);
                CreateGhost();
                UpdateSelectedObjectText();
            }
        }
    }

    void CreateGhost()
    {
        ghostObject = Instantiate(thingToPlace);
        ghostObject.GetComponent<Collider>().enabled = false;

        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            Color color = renderer.material.color;
            color.a = 0.5f;  
            mat.color = color;

            mat.SetFloat("_Mode", 2);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }
    }

    void UpdateGhostPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 point = hit.point;
            Vector3 snappedPosition = new Vector3
            (
                Mathf.Round(point.x / gridSize) * gridSize,
                Mathf.Round(point.y / gridSize) * gridSize,
                Mathf.Round(point.z / gridSize) * gridSize
            );

            ghostObject.transform.position = snappedPosition;

            if (occupiedPositions.Contains(snappedPosition) || currentPlacements >= maxPlacements)
            SetGhostColor(Color.red);
            else
            SetGhostColor(new Color(1f, 1f, 1f, 0.5f));
        }
    }

    void SetGhostColor( Color color )
    {
        Renderer[] renderers=ghostObject.GetComponentsInChildren<Renderer>();

        foreach(Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            mat.color  = color;
        }
    }

    void PlaceObject()
    {
        Vector3 placementPosition=ghostObject.transform.position;
        if (!occupiedPositions.Contains(placementPosition) && currentPlacements < maxPlacements)
        {
            Instantiate(thingToPlace, placementPosition, Quaternion.identity);

            occupiedPositions.Add(placementPosition);
            currentPlacements++;
            UpdatePlacementText();
        }
    }

    void DestroyObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (hitObj.CompareTag("PlacedObject"))
            {
                Vector3 snappedPos = new Vector3
                (
                    Mathf.Round(hitObj.transform.position.x / gridSize) * gridSize,
                    Mathf.Round(hitObj.transform.position.y / gridSize) * gridSize,
                    Mathf.Round(hitObj.transform.position.z / gridSize) * gridSize
                );

                occupiedPositions.Remove(snappedPos);
                Destroy(hitObj);
                currentPlacements--;
                UpdatePlacementText();
            }
        }
    }

    void UpdatePlacementText()
    {
        if (placementText != null)
            placementText.text = $"Objects Left: {maxPlacements - currentPlacements}";
    }
    void UpdateSelectedObjectText()
    {
        if (selectedObjectText != null && thingToPlace != null)
            selectedObjectText.text = $"Selected: {thingToPlace.name}";
    }
}


// based on Solo Game Devs youtube tutorial 
// https://www.youtube.com/watch?v=ur1TeqxFtV4
