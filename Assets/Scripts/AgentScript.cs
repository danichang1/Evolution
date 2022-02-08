using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentScript : MonoBehaviour {

    public NavMeshAgent agent;
    private int foodCount;
    private Vector3 newPos;
    private Vector3 homePos;
    private float radius;
    private bool posCheck;
    private bool reproduce;
    private bool atHome;
    private bool counterCheck;
    public float energy;
    public float sense;
    public float speed;
    public static float size;
    private float energyTimer;
    private bool childSet;
    private bool meSet;
    private bool active;
    private float speedChange;
    private float senseChange;
    private float sizeChange;
    Transform child;
    Transform me;

    [SerializeField]
    Transform agentPrefab;


    void Start(){
        active = true;
        childSet = false;
        meSet = false;
        counterCheck = false;
        ReproduceCounter.counter++;
        reproduce = true;
        atHome = false;
        energy = 100;
        energyTimer = 0;
        posCheck = false;
        radius = 10;
        sense = 7;
        speed = 7;
        size = 4;
        agent = this.GetComponent<NavMeshAgent>();
        agent.speed = speed;
        newDestination();
        ReproduceCounter.divideBy++;
        
    }

    void Update(){
        this.transform.localScale = new Vector3(size, size, size);
        energyTimer = energyTimer + Time.deltaTime;
        if (energyTimer >= 1 && atHome == false && active == true){
            //energy going down
            energy = energy - (agent.speed * agent.speed + sense) / 10;
            energyTimer = 0;
        }

        if (atHome == false && energy <= 0 && active == true){
            //die
            ReproduceCounter.counter--;
            ReproduceCounter.divideBy--;
            ReproduceCounter.totalSpeed = ReproduceCounter.totalSpeed - agent.speed;
            ReproduceCounter.totalSense = ReproduceCounter.totalSense - sense;
            Destroy(this.gameObject);
            
            
        } else if ((foodCount >= 2 && active == true) || ((foodCount == 1 && energy <= 7) && active == true)){
            //go home
            GameObject[] homeList;
            homeList = GameObject.FindGameObjectsWithTag("Home");
            float distance = Mathf.Infinity;
            GameObject closest = null;
            foreach(GameObject currentHome in homeList){
                float curDistance = Vector3.Distance(currentHome.transform.position, this.transform.position);
                if (curDistance < distance){
                    closest = currentHome.transform.parent.gameObject;
                    distance = curDistance;
                }
            }
            if (closest.tag == "North"){
                homePos.Set(this.transform.position.x, this.transform.position.y, 40f);
            } else if (closest.tag == "South"){
                homePos.Set(this.transform.position.x, this.transform.position.y, -40f);
            } else if (closest.tag == "East"){
                homePos.Set(40f, this.transform.position.y, this.transform.position.z);
            } else{ 
                homePos.Set(-40f, this.transform.position.y, this.transform.position.z);
            }
            agent.SetDestination(homePos);
            
        } else{
            if (Vector3.Distance (this.transform.position, newPos) <= 2.9 && active == true){
                //look for food
                GameObject[] foodList;
                foodList = GameObject.FindGameObjectsWithTag("Food");
                float distance = Mathf.Infinity;
                GameObject closest = null;
                
                foreach(GameObject currentFood in foodList){
                    float curDistance = Vector3.Distance(currentFood.transform.position, this.transform.position);
                    if (curDistance < distance){
                        closest = currentFood;
                        distance = curDistance;
                    }
                }
                if (Vector3.Distance(closest.transform.position, this.transform.position) > sense){
                    //no food - wander
                    newDestination();
                } else {
                    //food - go to food
                    newPos = closest.transform.position;
                    agent.SetDestination(newPos);
                }
            }
        }
        

        if (Vector3.Distance (this.transform.position, homePos) <= 2.1){
            atHome = true;
            
        }

        if (counterCheck == false && Vector3.Distance (this.transform.position, homePos) <= 2.1){
            counterCheck = true;
            ReproduceCounter.counter--;
        }

        if (atHome == true && foodCount >= 2 && reproduce == true && ReproduceCounter.reproduceGo == true){
            //reproduce
            me = Instantiate(agentPrefab, this.transform.position, Quaternion.identity);
            meSet = true;
            child = Instantiate(agentPrefab, this.transform.position, Quaternion.identity);
            childSet = true;
            speedChange = Random.Range(.7f, 1.3f);
            senseChange = Random.Range(.7f, 1.3f);
            reproduce = false;
            ReproduceCounter.divideBy--;
            ReproduceCounter.totalSpeed = ReproduceCounter.totalSpeed + agent.speed * speedChange;
            ReproduceCounter.totalSense = ReproduceCounter.totalSense + sense * senseChange;
        } else if (atHome == true && foodCount == 1 && reproduce == true && ReproduceCounter.reproduceGo == true){
            //go back out
            me = Instantiate(agentPrefab, this.transform.position, Quaternion.identity);
            meSet = true;
            reproduce = false;
            ReproduceCounter.divideBy--;
            
        }

        //set child stats
        if (childSet == true && child != null){
            child.gameObject.GetComponent<AgentScript>().agent.speed = agent.speed * speedChange;
            child.gameObject.GetComponent<AgentScript>().sense = sense * senseChange;
            Renderer mesh = this.GetComponent<MeshRenderer>();
            agent.enabled = false;
            mesh.enabled = false;
            active = false;
        }

        //set clone stats
        if (meSet == true && me != null){
            me.gameObject.GetComponent<AgentScript>().agent.speed = agent.speed;
            me.gameObject.GetComponent<AgentScript>().sense = sense;
            Renderer mesh = this.GetComponent<MeshRenderer>();
            agent.enabled = false;
            mesh.enabled = false;
            active = false;
        }

        if (active == false && child == null && me == null){
            Destroy(this.gameObject);
        }
    }


    void OnTriggerEnter(Collider other){
        //eat
        if(other.gameObject.CompareTag("Food")){
            Destroy(other.gameObject);
            foodCount++;
        }
    }

    void newDestination(){
        //new random position
        while (posCheck == false){
            var vector2 = Random.insideUnitCircle.normalized * radius;
            if (this.transform.position.x + vector2.x > -35 && this.transform.position.x + vector2.x < 35 && this.transform.position.z + vector2.y > -35 && this.transform.position.z + vector2.y < 35){
                posCheck = true;
                newPos = new Vector3(this.transform.position.x + vector2.x, 0, this.transform.position.z + vector2.y);
            }
        }
        agent.SetDestination(newPos);
        posCheck = false;
    }
}
