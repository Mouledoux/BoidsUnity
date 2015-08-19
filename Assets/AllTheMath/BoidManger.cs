using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoidManger : MonoBehaviour
{
    /*
        Boid's Algorithm is an algorithm used to simulate a flocking behavior
     * commonly observed in nature by various animals such as birds.
     */
    [Header("GameObjects")]
    GameObject target;
    public GameObject pred;
    public GameObject boid; //used to hold a refrence to are prefab
    public List<GameObject> BOIDS; //List that hold all of the boids 


    [Header("Modifiers")]
    [Space(10)]
    public float cMod; //cohession modifier
    public float sMod; //seperation modifier
    public float aMod; //alignment modifier

    public float vLimit;

    [Header("Number of Boids")]
    [Space(10)]
    public int numOfBoid = 10; //max size of the list can be adjusted in the inspector window 
    // Update is called once per frame
    void Start()
    {
        drawBoid(); // loads all the boids into the scene
    }

    void Update ()
    {
        moveBoid(); // moves the boids around the screen
	}

    void drawBoid()
    {
        for (int i = 0; i < numOfBoid; i++)
        {

            GameObject bwird = Instantiate(boid) as GameObject; //creates the boids to the screen as gameobjects to be added to the lsit
            bwird.transform.parent = gameObject.transform; // sets the new gameobject to the position of the parent object
            BOIDS.Add(bwird);



            foreach (GameObject b in BOIDS)
            {
                b.GetComponent<Bird>().pos = new Vector3(Random.Range(10, 20),
                                             Random.Range(10, 20),
                                             Random.Range(10, 20));


                b.GetComponent<Bird>().velo = new Vector3(Random.Range(-10, 10),
                                                          Random.Range(-10, 10),
                                                          Random.Range(-10, 10));
            }


            /*
                Sets a random starting point and a velocity for each of the boids in the list between -10 and 10     
            */
        }


    }

    void moveBoid()
    {
        Vector3 v1, v2, v3, v4; //created a varible for each of the rules in Boid's Algorithm

        foreach(GameObject b in BOIDS)
        {
            v1 = rule1(b) * cMod; //cohesion
            v2 = rule2(b) * sMod; //seperation
            v3 = rule3(b) * aMod; //allignment
            v4 = borderPos(b);
           
            b.GetComponent<Bird>().velo = b.GetComponent<Bird>().velo.normalized + v1 + v2 + v3 + v4; //calculates a new velocity for the boid by add a normlaized velocity plus each of the returned values from the rules
            veloLimit(b); //makes sure the velocity does not exceed a certain speed
            b.GetComponent<Bird>().pos += b.GetComponent<Bird>().velo.normalized; //calculates the boid's posistion by by add its posistion plus its normalized velocity
        }
    }

    void veloLimit(GameObject a)
    {
        /*
            checks to see if the magvitude of the boids velocity is greater than the velocity limit
         *  it sets the boid's velocity to the normalized velocity
         */
        if(a.GetComponent<Bird>().velo.magnitude > vLimit) 
        {
            a.GetComponent<Bird>().velo = a.GetComponent<Bird>().velo.normalized;
        }
    }

    float calcDis(Vector3 a, Vector3 b)
    {
        /*
            calcualtes the distance 
         * the distance is = the square root of the sum of each of the posistion values in each gameobject
         * formula
         * // d = squareroot((pos1.x - pos2.x)^2 * (pos1.y - pos2.y)^2 * (pos1.z - pos2.z)^2)
         */

        boid.GetComponent<Bird>().dis = Mathf.Sqrt(((a.x - b.x) * (a.x - b.x)) +
                                                ((a.y - b.y) * (a.y - b.y)) +
                                                ((a.z - b.z) * (a.z - b.z)));
        return boid.GetComponent<Bird>().dis;
    }

    Vector3 rule1(GameObject boidJ) //cohession 
    {
        Vector3 pc = Vector3.zero; //zeros out the varibale for the percived center

        /*
          Loops through each of the boids in the list
         * if the boid is not equal to the boid in the boid passed in to the function
         * we calculate the pecevied center to be it self + the current boids posistion
         * 
         * 
         * after we loop through each of the boids in the list we set the percived center equal to it self divided by the size of the list minus 1
         * so we dont count the boid we passed into the function
         * 
         * we return the percivied center minus the posistion of the boid divied by a 100 
         * to move it only 1% of the way to the center
         */

        foreach(GameObject b in BOIDS)
        {
            if(b != boidJ)
            {
                pc += b.GetComponent<Bird>().pos;
            }
        }
        pc = pc / (BOIDS.Count - 1);
        return (pc - boidJ.GetComponent<Bird>().pos) / 100;
    }

    Vector3 rule2(GameObject boidJ) //seperation
    {
        Vector3 c = Vector3.zero; //displacment of the boids
        foreach(GameObject b in BOIDS)
        {
            if (b != boidJ)
            {
                /*
                  checks to see if the total distance between two points is less than 100 to make sure they do not collide with each other
                 * 
                 * if the is less that a certain number of units from another boid we set the displacment euqal to itself minus the boid in the loops posistion minus the passed in boids posistion
                 * to move it away from the other one
                 * we then return the displacment
                 */

                if (calcDis(b.GetComponent<Bird>().pos, boidJ.GetComponent<Bird>().pos) < 5)
                {
                    c -= (b.GetComponent<Bird>().pos - boidJ.GetComponent<Bird>().pos);
                }
            }
        }
        return c;
    }

    Vector3 rule3(GameObject boidJ) //allignment 
    {
        Vector3 pvJ = Vector3.zero; //percived velocity
        foreach (GameObject b in BOIDS)
        {
            if (b != boidJ)
            {
                /*
                    set the percived velocity equal to it self plus the velocity of the boid
                */
                pvJ += b.GetComponent<Bird>().velo.normalized;
            }
        }
        /*
            after we loop through each of the boids 
            we get a new percived velocity by dividing it self by the number of boids in the array - the boid
            we pass into the function;
        */

        pvJ /= (BOIDS.Count - 1);
        return (pvJ - boidJ.GetComponent<Bird>().velo.normalized) / 8;
    }

    Vector3 borderPos(GameObject boidJ)  //keeps the boids within a confined space in the scene
    {
        Vector3 v = Vector3.zero;

        if (boidJ.GetComponent<Bird>().pos.x < (gameObject.transform.position.x - 50))
        {
            v.x = 50;
        }
        else if (boidJ.GetComponent<Bird>().pos.x > (gameObject.transform.position.x + 50))
        {
            v.x = -50;
        }


        if (boidJ.GetComponent<Bird>().pos.y < (gameObject.transform.position.y - 50))
        {
            v.y = 50;
        }
        else if (boidJ.GetComponent<Bird>().pos.y > (gameObject.transform.position.y + 50))
        {
            v.y = -50;
        }


        if (boidJ.GetComponent<Bird>().pos.z < (gameObject.transform.position.z - 50))
        {
            v.z = 50;
        }
        else if (boidJ.GetComponent<Bird>().pos.z > (gameObject.transform.position.x + 50))
        {
            v.z = -50;
        }

        return v;
    }

    //functions for sliders to change the values of each modifer
    public void setC_Mod(float a)
    {
        cMod = a;
    }

    public void setS_Mod(float a)
    {
        sMod = a;
    }

    public void setA_Mod(float a)
    {
        aMod = a;
    }
}
