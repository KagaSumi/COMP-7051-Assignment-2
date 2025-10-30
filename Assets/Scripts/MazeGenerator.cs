// MazeGenerator.cs

using UnityEngine;
using System.Collections.Generic;
using Unity.AI.Navigation;

// Helper class to represent a single cell in the maze grid
public class MazeCell
{
    public bool visited = false;
    public bool wallNorth = true;
    public bool wallEast = true;
    public bool wallSouth = true;
    public bool wallWest = true;
}

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Dimensions")]
    [Range(5, 100)]
    public int width = 10;
    [Range(5, 100)]
    public int height = 10;

    [Header("Maze Prefabs & Materials")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public Transform mazeParent;

    [Header("Object Placement")]
    public GameObject winZone;
    public GameObject enemyObject;
    public GameObject playerObject;

    [Header("AI Navigation")]
    public NavMeshSurface navMeshSurface;

    [Header("Wall Textures")]
    public Material northMaterial;
    public Material southMaterial;
    public Material eastMaterial;
    public Material westMaterial;
    public Material floorMaterial;

    private Vector2Int playerSpawnCoords;
    private MazeCell[,] mazeGrid;

    void Start()
    {
        InitializeGrid();
        GenerateMaze(0, 0);
        DrawMaze();
        PositionPlayer();
        PositionEnemy();
        PositionWinZone();
        
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
            Debug.Log("NavMesh built at runtime!");
        }
    }

    // Sets up the initial grid with all walls intact
    private void InitializeGrid()
    {
        mazeGrid = new MazeCell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Create a new MazeCell object at each grid position.
                mazeGrid[x, y] = new MazeCell();
            }
        }
    }

    // Recursive backtracking algorithm to carve paths
    private void GenerateMaze(int x, int y)
    {
        mazeGrid[x, y].visited = true;

        // While there are unvisited neighbors
        while (true)
        {
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(x, y);
            if (neighbors.Count == 0)
            {
                break; // No unvisited neighbors, backtrack
            }

            // Pick a random neighbor
            int randomIndex = Random.Range(0, neighbors.Count);
            Vector2Int chosenNeighbor = neighbors[randomIndex];

            // Remove wall between current cell and chosen neighbor
            RemoveWall(x, y, chosenNeighbor.x, chosenNeighbor.y);

            // Recursively visit the neighbor
            GenerateMaze(chosenNeighbor.x, chosenNeighbor.y);
        }
    }

    // Checks for valid neighbors that haven't been visited yet
    private List<Vector2Int> GetUnvisitedNeighbors(int x, int y)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // North
        if (y + 1 < height && !mazeGrid[x, y + 1].visited) neighbors.Add(new Vector2Int(x, y + 1));
        // East
        if (x + 1 < width && !mazeGrid[x + 1, y].visited) neighbors.Add(new Vector2Int(x + 1, y));
        // South
        if (y - 1 >= 0 && !mazeGrid[x, y - 1].visited) neighbors.Add(new Vector2Int(x, y - 1));
        // West
        if (x - 1 >= 0 && !mazeGrid[x - 1, y].visited) neighbors.Add(new Vector2Int(x - 1, y));

        return neighbors;
    }

    // Removes the walls between two adjacent cells
    private void RemoveWall(int currentX, int currentY, int nextX, int nextY)
    {
        if (nextX > currentX) // Moving East
        {
            mazeGrid[currentX, currentY].wallEast = false;
            mazeGrid[nextX, nextY].wallWest = false;
        }
        else if (nextX < currentX) // Moving West
        {
            mazeGrid[currentX, currentY].wallWest = false;
            mazeGrid[nextX, nextY].wallEast = false;
        }
        else if (nextY > currentY) // Moving North
        {
            mazeGrid[currentX, currentY].wallNorth = false;
            mazeGrid[nextX, nextY].wallSouth = false;
        }
        else if (nextY < currentY) // Moving South
        {
            mazeGrid[currentX, currentY].wallSouth = false;
            mazeGrid[nextX, nextY].wallNorth = false;
        }
    }

    // Instantiates the prefabs based on the generated maze data
    private void DrawMaze()
    {
        float wallHeight = 5f; // Adjust as needed
        float wallThickness = 0.5f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Instantiate Floor
                Vector3 floorPos = new Vector3(x * 10, 0, y * 10);
                GameObject floor = Instantiate(floorPrefab, floorPos, Quaternion.identity, mazeParent);
                floor.GetComponent<Renderer>().material = floorMaterial;

                // Instantiate North Wall
                if (mazeGrid[x, y].wallNorth)
                {
                    Vector3 wallPos = new Vector3(x * 10, wallHeight / 2, y * 10 + 5 - wallThickness / 2);
                    GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.identity, mazeParent);
                    // Assigns materials to the two child planes
                    wall.transform.GetChild(0).GetComponent<Renderer>().material = northMaterial;
                    wall.transform.GetChild(1).GetComponent<Renderer>().material = southMaterial;
                }

                // Instantiate East Wall
                if (mazeGrid[x, y].wallEast)
                {
                    Vector3 wallPos = new Vector3(x * 10 + 5 - wallThickness / 2, wallHeight / 2, y * 10);
                    GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.Euler(0, 90, 0), mazeParent);
                    wall.transform.GetChild(0).GetComponent<Renderer>().material = eastMaterial;
                    wall.transform.GetChild(1).GetComponent<Renderer>().material = westMaterial;
                }

                // If we are on the bottom row, draw the South boundary wall
                if (y == 0 && mazeGrid[x, y].wallSouth)
                {
                    Vector3 wallPos = new Vector3(x * 10, wallHeight / 2, y * 10 - 5 + wallThickness / 2);
                    GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.identity, mazeParent);
                    wall.transform.GetChild(0).GetComponent<Renderer>().material = southMaterial;
                    wall.transform.GetChild(1).GetComponent<Renderer>().material = northMaterial;
                }

                // If we are on the leftmost column, draw the West boundary wall
                if (x == 0 && mazeGrid[x, y].wallWest)
                {
                    Vector3 wallPos = new Vector3(x * 10 - 5 + wallThickness / 2, wallHeight / 2, y * 10);
                    GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.Euler(0, 90, 0), mazeParent);
                    wall.transform.GetChild(0).GetComponent<Renderer>().material = westMaterial;
                    wall.transform.GetChild(1).GetComponent<Renderer>().material = eastMaterial;
                }
            }
        }
    }

    private void PositionPlayer()
    {
        if (playerObject == null)
        {
            Debug.LogWarning("Player Object not assigned in the MazeGenerator script!");
            return;
        }

        // Pick a random grid cell for the player
        int randomX = Random.Range(0, width);
        int randomY = Random.Range(0, height);

        // Store these coordinates so the enemy doesn't spawn here
        playerSpawnCoords = new Vector2Int(randomX, randomY);

        // Calculate the world position
        float xPos = randomX * 10;
        float zPos = randomY * 10;
        Vector3 spawnPos = new Vector3(xPos, 1f, zPos);

        // Temporarily disable the CharacterController to teleport the player
        CharacterController controller = playerObject.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            playerObject.transform.position = spawnPos;
            controller.enabled = true;
        }
        else
        {
            // Fallback for objects without a CharacterController
            playerObject.transform.position = spawnPos;
        }

        Debug.Log("Player placed at random cell: (" + randomX + ", " + randomY + ")");
    }

    private void PositionWinZone()
    {
        // Check if the winZone has been assigned in the Inspector
        if (winZone != null)
        {
            // Calculate the position of the last cell (top-right corner)
            float xPos = (width - 1) * 10;
            float zPos = (height - 1) * 10;

            // Set the win zone's position (Y is slightly above the floor)
            winZone.transform.position = new Vector3(xPos, 1f, zPos);

            Debug.Log("Win Zone placed at: " + winZone.transform.position);
        }
        else
        {
            Debug.LogWarning("Win Zone object not assigned in the MazeGenerator script!");
        }
    }
    private void PositionEnemy()
    {
        if (enemyObject == null)
        {
            Debug.LogWarning("Enemy Object not assigned in the MazeGenerator script!");
            return;
        }

        int randomX, randomY;

        // Keep picking a random spot until it's different from the player's spawn
        do
        {
            randomX = Random.Range(0, width);
            randomY = Random.Range(0, height);
        } while (randomX == playerSpawnCoords.x && randomY == playerSpawnCoords.y);


        float xPos = randomX * 10;
        float zPos = randomY * 10;

        // Use agent.Warp to place the enemy correctly on the NavMesh
        UnityEngine.AI.NavMeshAgent agent = enemyObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.Warp(new Vector3(xPos, 1f, zPos));
        }
        else
        {
            enemyObject.transform.position = new Vector3(xPos, 1f, zPos);
        }

        Debug.Log("Enemy placed at random cell: (" + randomX + ", " + randomY + ")");
    }
}