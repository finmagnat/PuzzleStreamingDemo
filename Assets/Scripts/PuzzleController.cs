using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    [Header("Grid Settings")]
    public float cellSize = 1f;
    public float moveSpeed = 5f;
    public bool isPawn = true;

    private Vector3 targetPosition;
    private bool isMoving;

    private void Start()
    {
        targetPosition = SnapToGrid(transform.position);
        transform.position = targetPosition;
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
                isMoving = false;
            return;
        }

        Vector3 input = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) input = Vector3.forward;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) input = Vector3.back;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) input = Vector3.left;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) input = Vector3.right;

        if (input != Vector3.zero)
        {
            Vector3 newPos = targetPosition + input * cellSize;
            if (isPawn)
            {
                targetPosition = newPos;
                isMoving = true;
            }
            else
            {
                targetPosition = newPos;
                transform.position = targetPosition;
            }
        }
    }

    private Vector3 SnapToGrid(Vector3 pos)
    {
        pos.x = Mathf.Round(pos.x / cellSize) * cellSize;
        pos.z = Mathf.Round(pos.z / cellSize) * cellSize;
        return new Vector3(pos.x, pos.y, pos.z);
    }
}
