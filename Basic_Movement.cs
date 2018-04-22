using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic_Movement : MonoBehaviour
{


    public float speed = 50.0F;
    public float gravity = 0.0F; //potentially build in antigravity movement?
    public int clampMouseAngleMin = 20; //for camera use later?
    public int clampMouseAngleMax = 80; //for camera
    public float rotateSpeed = 30.0f;


    private Vector3 moveDirection = Vector3.zero;





    void Update()
    {
        Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);

        //always point to mouse X

        //have speed as an incremental value, i.e. speed up not flat
        //if (Input.GetAxis("Mouse X") < 0)
        //{
        //    //Code for action on mouse moving left
        //    transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        //}
        //if (Input.GetAxis("Mouse X") > 0)
        //{
        //    //Code for action on mouse moving right
        //    transform.Rotate(-Vector3.up * rotateSpeed * Time.deltaTime);
        //}

        float movey = mouseOnScreen.x * 900;
        transform.rotation = Quaternion.Euler(new Vector3(0f, movey, 0f));
        Vector3 m = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            m += transform.forward * speed * 4;
            // transform.Translate(Vector3.forward * speed * 4 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            m += -transform.forward * speed;
            //transform.Translate(Vector3.back * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            m += -transform.right * speed * 2;
            //transform.Translate(Vector3.left * speed * 2 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            m += transform.right * speed * 2;
            //transform.Translate(Vector3.right * speed * 2 * Time.deltaTime);
        }

        gameObject.GetComponent<CharacterController>().SimpleMove(m * Time.deltaTime);
    }
    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

}
