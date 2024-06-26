using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]CharacterMovement cm;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTextFinish(string dest)
    {
        int x = int.Parse(dest)%10;
        int y = int.Parse(dest)/10;
        print(x);
        print(y);
        x = x*3-15; y= y*3 - 15;
        print(x + " " + y);
        cm.Move(new Vector3(x, 0, y));
    }
}
