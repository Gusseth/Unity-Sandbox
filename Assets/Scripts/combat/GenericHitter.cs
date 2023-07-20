using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Thank you to HamJoy Games for this system. You can find the tutorial here:
 * https://www.youtube.com/watch?v=TM9tADSh9nE
 * 
 * This is a heavily modified version of the script in the video.
 */
public class GenericHitter : MonoBehaviour, IHitter
{
    [SerializeField] bool isAttacking;
    [SerializeField] int damage_100;
    [SerializeField] HitterBox hitterBox;
    public int damage { get => damage_100; }

    public bool CheckHit(Hit data)
    {
        return true;
    }

    public void Response(Hit data)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        hitterBox.hitter = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking)
        {
            hitterBox.CheckHit(null);
        }
    }
}
