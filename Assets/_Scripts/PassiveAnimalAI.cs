using UnityEngine;

public class PassiveAnimalAI : MonoBehaviour
{
    public float speed = 2f;
    public float minMoveTime = 2f;
    public float maxMoveTime = 5f;

    private Vector3 targetDirection;
    private Animator anim;
    private SpriteRenderer sr;

    private float timer;
    private float wanderInterval = 3f;

    private bool isMoving = false;
    private float moveTimer = 0f;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isMoving)
        {
            timer += Time.deltaTime;
            anim.SetBool("wander", false);
            if (timer > wanderInterval)
            {
                targetDirection = GetRandomDirection();
                timer = 0;
                anim.SetBool("wander", true);
                isMoving = true;
                moveTimer = 0f;
            }
        }
        else
        {
            transform.Translate(targetDirection * speed * Time.deltaTime);
            moveTimer += Time.deltaTime;
            if (moveTimer > GetRandomMoveTime())
            {
                isMoving = false;
                wanderInterval = Random.Range(minMoveTime, maxMoveTime);
            }
        }
        if (targetDirection.magnitude > 0)
        {
            sr.flipX = targetDirection.x < 0;
        }
    }

    private Vector3 GetRandomDirection()
    {
        Vector3[] validDirections = { Vector3.left, Vector3.right, Vector3.up, Vector3.down, Vector3.up + Vector3.left, Vector3.up + Vector3.right, Vector3.down + Vector3.left, Vector3.down + Vector3.right };
        return validDirections[Random.Range(0, validDirections.Length)];
    }

    private float GetRandomMoveTime()
    {
        return Random.Range(minMoveTime, maxMoveTime);
    }
}
