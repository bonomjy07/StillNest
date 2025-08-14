using UnityEngine;

public class BossMoving : MonsterMoving
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        MoveAround();
    }
}
