using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisGuy : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float speed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), 
                                       Input.GetAxisRaw("Vertical"));
        rb.velocity = movement.normalized * speed * Time.deltaTime;
    }
}
