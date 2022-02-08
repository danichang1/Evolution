using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    Transform foodPrefab;

    [SerializeField]
    Transform agentPrefab;

    public int foodCount = 0;
    public int agentCount = 0;

    public TextMeshProUGUI dayText;

    public TextMeshProUGUI avgSpeed;
    public TextMeshProUGUI avgSense;

    public int day = 1;

    void Start(){
        ReproduceCounter.totalSpeed = 7 * agentCount;
        ReproduceCounter.totalSense = 7 * agentCount;
        for (int i = 0; i < foodCount; i++){
            Transform point = Instantiate(foodPrefab, new Vector3(Random.Range(-38.0f, 38.0f), 0.5f, Random.Range(-38.0f, 38.0f)), Quaternion.identity);  
        }

        for (int i = 0; i < agentCount; i++){
            var whichEdge = Random.Range(1,5);
            if (whichEdge == 1){
                Transform point = Instantiate(agentPrefab, new Vector3(40f, 0.5f, Random.Range(-40.0f, 40.0f)), Quaternion.identity);
            } else if (whichEdge == 2){
                Transform point = Instantiate(agentPrefab, new Vector3(-40f, 0.5f, Random.Range(-40.0f, 40.0f)), Quaternion.identity);
            } else if (whichEdge == 3){
                Transform point = Instantiate(agentPrefab, new Vector3(Random.Range(-40.0f, 40.0f), 0.5f, 40f), Quaternion.identity);
            } else {
                Transform point = Instantiate(agentPrefab, new Vector3(Random.Range(-40.0f, 40.0f), 0.5f, -40f), Quaternion.identity);
            }
        }
    }

    void Update(){
        avgSpeed.text = "Average Speed: " + Mathf.Round((ReproduceCounter.totalSpeed / ReproduceCounter.divideBy) * 1000f) / 1000f;
        avgSense.text = "Average Sense: " + Mathf.Round((ReproduceCounter.totalSense / ReproduceCounter.divideBy) * 1000f) / 1000f;
        dayText.text = "Day " + day;
        if (ReproduceCounter.counter == 0){
            ReproduceCounter.reproduceGo = true;
            var allFood = GameObject.FindGameObjectsWithTag("Food");
            foreach(GameObject food in allFood){
                Destroy(food);
            }
            for (int i = 0; i < foodCount; i++){
            Transform point = Instantiate(foodPrefab, new Vector3(Random.Range(-38.0f, 38.0f), 0.5f, Random.Range(-38.0f, 38.0f)), Quaternion.identity);  
            }
            day++;
        } else {
            ReproduceCounter.reproduceGo = false;
        }
    }
}
