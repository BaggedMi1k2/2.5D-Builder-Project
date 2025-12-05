using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridPlacement : MonoBehaviour
{
    //level plane
    public GameObject placementPlane;


    // placment settings
    public float gridSize = 1f;
    public int maxPlacements = 5;
    public LayerMask blockingLayers;

    //placable objects
    public List<GameObject> placeableObjects;
    private int currentIndex = 0;
    public GameObject thingToPlace;

    //UI
    public TMP_Text placementText;
    public TMP_Text selectedObjectText;

    private GameObject ghostObject;
    public HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();
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
        UpdateGhostVisibility();

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

            if (mat.shader.name.Contains("TextMeshPro"))
            {
                // TMP uses _FaceColor instead of _Color
                if (mat.HasProperty("_FaceColor"))
                {
                    Color color = mat.GetColor("_FaceColor");
                    color.a = 0.5f;
                    mat.SetColor("_FaceColor", color);
                }
            }
            else
            {
                
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

            Vector3 ghostSize = thingToPlace.GetComponent<Renderer>().bounds.size * 0.5f / 2f;
            Collider[] hits = Physics.OverlapBox(snappedPosition, ghostSize, Quaternion.identity, blockingLayers);

            if (occupiedPositions.Contains(snappedPosition) || currentPlacements >= maxPlacements || hits.Length > 0)
                SetGhostColor(Color.red);
            else
            SetGhostColor(new Color(1f, 1f, 1f, 0.5f));
        }
    }

    void UpdateGhostVisibility()
    {
        if (ghostObject == null || placementPlane == null)
            return;

        // If plane GameObject is inactive OR its renderer is disabled, hide ghost
        bool planeVisible = placementPlane.activeInHierarchy;

        Renderer planeRenderer = placementPlane.GetComponent<Renderer>();
        if (planeRenderer != null && !planeRenderer.enabled)
            planeVisible = false;

        ghostObject.SetActive(planeVisible);
    }
    public void HideGhost()
    {
        if (ghostObject != null)
            ghostObject.SetActive(false);
    }

    public void ShowGhost()
    {
        if (ghostObject != null)
            ghostObject.SetActive(true);
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
        Vector3 ghostSize = thingToPlace.GetComponent<Renderer>().bounds.size * 0.5f / 2f;

        SoundManager.Instance.PlaySound2D("Place");

        Collider[] hits = Physics.OverlapBox(placementPosition, ghostSize, Quaternion.identity, blockingLayers);

        if (!occupiedPositions.Contains(placementPosition) && currentPlacements < maxPlacements && hits.Length == 0)
        {
            GameObject placed = Instantiate(thingToPlace, placementPosition, Quaternion.identity);
            placed.tag = "PlacedObject";

            Bomb bomb = placed.GetComponent<Bomb>(); //this allows the player to place a new block on the tile when the bomb explodes 
            if (bomb != null)
                bomb.gridPlacement = this;

            Teleporter teleporter = placed.GetComponent<Teleporter>(); //teleporter teleporter teleporter teleporter
            if (teleporter != null)
                teleporter.gridPlacement = this;

            occupiedPositions.Add(placementPosition);
            currentPlacements++;
            UpdatePlacementText();
        }
    }

    void DestroyObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        SoundManager.Instance.PlaySound2D("Break");

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
