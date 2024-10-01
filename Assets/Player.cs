using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed;
    public float jump;

    // Update is called once per frame
    void Update()
    {
       // Move
        transform.Translate(Vector2.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * jump, ForceMode2D.Impulse);
        }
        
    }
}
