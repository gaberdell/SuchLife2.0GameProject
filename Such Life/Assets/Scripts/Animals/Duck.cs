using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck : AnimalBase
{
    // Start is called before the first frame update
    void Start()
    {
        //status initialization
        HPCap = 45f;
        currHP = HPCap;
        currMaxSpeed = 0;
        walkspeed = 1f;
        runspeed = 1.4f;
        awareness = 5;
        currState = State.Idling;
        hungerCap = 100f;
        hunger = 100f;
        hungerDrain = 0.1f;
        hungerCap = 100f;
        //position and time initialization
        position = new Vector2(transform.position.x, transform.position.y);
        newposition = position;
        time = 0f;
        timeDelay = 1f;
        //object initialization
        player = GameObject.Find("MC");
        aniSprite = GetComponent<SpriteRenderer>();
        navi = GetComponent<UnityEngine.AI.NavMeshAgent>();
        navi.updateRotation = false;
        navi.updateUpAxis = false;
        food = null;
        foodtypes = new List<string>();
        foodtypes.Add("Grass");
    }

    // Update is called once per frame
    void Update()
    {
            //If the Duck has 0HP, it dies
            if (currHP <= 0)
            {
                currHP = 0;
                currState = State.Dying;
                aniSprite.flipY = true; //Temporary death effect. It flips upside-down
            }

            //This part of the Update doesn't work by frame.
            //It works on a timer defined by timeDelay
            time = time + 1f * Time.deltaTime;
            if (time >= timeDelay)
            {
                time = 0f;


                //If the duck is idling, it has a 50% chance to start wandering
                if (currState == State.Idling)
                {
                    int gen = Random.Range(0, 100);
                    if (gen > 50)
                    {
                        Walk();
                    }
                }

                //If the duck is wandering, it has a 10% chance of stopping.
                if (currState == State.Walking)
                {
                    PositionChange();
                    flipSprite();
                    int gen = Random.Range(0, 100);
                    if (gen > 90)
                    {
                        Idle();
                    }
                }
                //If the hunger is greater than or equal to  80, the duck can heal.
                if (hunger >= 80)
            {
                heal(1);
            }
                //If the hunger is less than or equal to 30, it starts looking for grass
                if (hunger <= 30)
                {
                    navi.speed = walkspeed / 2;
                    LookForFood(foodtypes);
                }
                //
                //If the hunger is 0, it starts dying
                if (hunger <= 0)
                {
                    if (hunger < 0)
                    {
                        hunger = 0;
                    }
                    takeDamage(1);
                }
                //Hunger drains if its not 0
                else
                {
                    hunger = hunger - hungerCap * hungerDrain;
                }
            }

            //While the timeDelay isn't met
            else
            {
                //If the duck is being pushed, it flies away. It doesn't fly as far away if the player has food. 
                if (currState == State.Pushed)
                {
                    //to be implemented.
                }
                //How the duck follows the player
                else if (currState == State.Following)
                {
                    newposition.x = player.transform.position.x;
                    newposition.y = player.transform.position.y;
                    navi.speed = walkspeed;
                    navi.SetDestination(newposition);
                    flipSprite();
                }
            }
            position = transform.position;

        }
    }
