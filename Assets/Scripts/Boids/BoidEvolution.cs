using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidEvolution : MonoBehaviour
{   
    // How long and how close boids must be to evolve
    public float reqProximity = 1.0f;
    public float cohesionTime = 2.0f;
    private List<Boid> boids;
    private Dictionary<Boid, float> closeBoidsTime;
    public GameObject boidPrefab;


    // Start is called before the first frame update
    void Start()
    {   
        // initialize boids list with all boid game objects in the scene
        boids = new List<Boid>(FindObjectsOfType<Boid>());
        // initialize dictionary that keeps track of how long two boids are close to each other
        closeBoidsTime = new Dictionary<Boid, float>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var boid in boids)
        {
            foreach (var otherboid in boids)
            {
                // if boids are close and not the same
                if (boid != otherboid && Vector3.Distance(boid.transform.position, otherboid.transform.position) < reqProximity)
                {
                    // update the cohesion time in the dictionary
                    if (!closeBoidsTime.ContainsKey(boid))
                    {
                        closeBoidsTime[boid] = 0f;
                    }

                    closeBoidsTime[boid] += Time.deltaTime;

                    if (closeBoidsTime[boid] > cohesionTime)
                    {
                        EvolveBoids(boid, otherboid);
                        closeBoidsTime[boid] = 0f;
                        break;
                    }
                }
            }
        }
        
    }

    private void EvolveBoids(Boid parent1, Boid parent2)
    {
        // Retrieve weights from the parent RNN agents
        // Combine and mutate them, storing them for child RNN agents
        // Instantiate 2 offspring boids with new mutated weights
        // Destroy the parents

        var rnn1 = parent1.GetComponent<RNNRLAgent>();
        var rnn2 = parent2.GetComponent<RNNRLAgent>();

        var childRNN1 = CombineAndMutate(rnn1, rnn2);
        var childRNN2 = CombineAndMutate(rnn2, rnn1);

        // Initialize offspring
        CreateChild(childRNN1, parent1.transform.position);
        CreateChild(childRNN2, parent2.transform.position);

        // Destroy parents
        Destroy(parent1.gameObject);
        Destroy(parent2.gameObject);

    }

    private RNNRLAgent CombineAndMutate(RNNRLAgent rnn1, RNNRLAgent rnn2)
    {
        RNNRLAgent childRNN = new RNNRLAgent(rnn1.inputSize, rnn1.hiddenSize, rnn1.outputSize);

        for (int i=0; i < rnn1.inputHiddenWeights.GetLength(0); i++)
        {
            for (int j=0; j < rnn1.inputHiddenWeights.GetLength(1); j++)
            {
                // 50% chance of selecting input weights from rnn1 or rnn2
                childRNN.inputHiddenWeights[i,j] = UnityEngine.Random.value < 0.5f? rnn1.inputHiddenWeights[i,j] : rnn2.inputHiddenWeights[i,j];
            }
        }
        for (int i=0; i < rnn1.hiddenHiddenWeights.GetLength(0); i++)
        {
            for (int j=0; j < rnn1.hiddenHiddenWeights.GetLength(1); j++)
            {
                // 50% chance of selecting input weights from rnn1 or rnn2
                childRNN.hiddenHiddenWeights[i,j] = UnityEngine.Random.value < 0.5f? rnn1.hiddenHiddenWeights[i,j] : rnn2.hiddenHiddenWeights[i,j];
            }
        }
        for (int i=0; i < rnn1.hiddenOutputWeights.GetLength(0); i++)
        {
            for (int j=0; j < rnn1.hiddenOutputWeights.GetLength(1); j++)
            {
                // 50% chance of selecting input weights from rnn1 or rnn2
                childRNN.hiddenOutputWeights[i,j] = UnityEngine.Random.value < 0.5f? rnn1.hiddenOutputWeights[i,j] : rnn2.hiddenOutputWeights[i,j];
            }
        }

        return childRNN;
    }

    private void CreateChild(RNNRLAgent childRNN, Vector3 parentPosition)
    {
        GameObject newBoid = Instantiate(boidPrefab, parentPosition, Quaternion.identity);
        Boid boidComponent = newBoid.GetComponent<Boid>();
        boidComponent.rnnAgent = childRNN;

        
    }

}
