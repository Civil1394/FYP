using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Vector3 _input;
    [SerializeField] private Rigidbody rb;
    Plane plane = new Plane(Vector3.up, 0);
    [SerializeField] private float _speed = 5;
    [SerializeField] private Transform firePivot;
    private bool canFire;
    private Vector3 rotatedInput;
    private Vector3 mousePos;
    Camera mainCamera;
    [SerializeField] private float _turnSpeed;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = FindFirstObjectByType<Camera>();
    }
     void FixedUpdate()
    {
        Move();
       
    }
    // Update is called once per frame
    void Update()
    {

    
        GatherInput();
        Look();
    }

    void GatherInput()
    {   //movement
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        rotatedInput = Quaternion.Euler(0, 45, 0) * _input;
        plane = new Plane(new Vector3(0,this.transform.position.y,0), 0);
        //rotation
         float distance;
         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            mousePos = ray.GetPoint(distance);
        }

    }

    void Move()
    {
        //rb.MovePosition(transform.position + (transform.forward * _input.magnitude) * _speed * Time.deltaTime);
        rb.velocity = new Vector3(rotatedInput.x* _speed ,rb.velocity.y,rotatedInput.z* _speed );
        //rb.AddForce(rotatedInput* _speed);
    }

    void Look()
    {
         if(_input != Vector3.zero)
         {        
            
             var relative = (transform.position + _input.ToIso()) - transform.position;
             var rot = Quaternion.LookRotation(relative, Vector3.up);
                //transform.rotation =rot;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed) ;
         }
        //Vector3 mousePosition = new Vector3( mousePos.x, this.transform.position.y, mousePos.z ) ;
        //Debug.Log(mousePosition);
        //this.transform.LookAt( mousePosition ) ;

    }
}
