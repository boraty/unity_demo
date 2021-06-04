using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Mover : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpSpeed = 4f;
    Vector3 offSet = new Vector3(1.5f, 4f, -13);
    List<Transform> points = new List<Transform>();
    void Start()
    {
        DOTween.Init();
        points.Add(transform);
    }

    void Update()
    {
        MoveForward();
        MoveSideways();
        // Fixing the camera to the latest element in list with offset
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, points[points.Count - 1].position + offSet, Time.deltaTime);
    }

    void MoveSideways()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = Vector3.Lerp(transform.position, (Vector3.left * moveSpeed) + transform.position, Time.deltaTime);
            if (transform.position.x < -4)
            {
                transform.position = new Vector3(-4, transform.position.y, transform.position.z);
            }
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = Vector3.Lerp(transform.position, (Vector3.right * moveSpeed) + transform.position, Time.deltaTime);
            if (transform.position.x > 4)
            {
                transform.position = new Vector3(4, transform.position.y, transform.position.z);
            }
        }
    }

    void MoveForward()
    {
        transform.position = Vector3.Lerp(transform.position, (Vector3.forward * moveSpeed) + transform.position, Time.deltaTime);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Point")
        {
            //move to the point where stack will drop, then add to list and join as child element
            collision.transform.DOMove(points[points.Count - 1].position + (Vector3.forward * moveSpeed / jumpSpeed), 1f / jumpSpeed).OnComplete(() =>
                {
                    collision.transform.parent = transform;
                    collision.transform.position = new Vector3(transform.position.x, 1, transform.position.z);
                });
            //make every element jump
            foreach (Transform currentTransform in points)
            {
                Vector3 landingPosition = currentTransform.position + new Vector3(0, 1.1f, moveSpeed / jumpSpeed);
                currentTransform.DOJump(landingPosition, 2.5f, 1, 1f / jumpSpeed);
            }
            points.Add(collision.transform);
        }
        else if (collision.gameObject.tag == "Obstacle")
        {
            if (points.Count == 1) return;
            //remove last element then drop the stack
            points[points.Count - 1].parent = null;
            points.RemoveAt(points.Count - 1);
            StartCoroutine(DropAfterObstacle());

        }
    }

    IEnumerator DropAfterObstacle()
    {
        //waits until passing the obstacle, then drops with ease
        yield return new WaitForSeconds((1 / moveSpeed) * 2);
        Vector3 landingPosition = transform.position + new Vector3(0, -1.1f, moveSpeed / jumpSpeed);
        transform.DOJump(landingPosition, 0.5f, 1, 1f / jumpSpeed);
    }
}
