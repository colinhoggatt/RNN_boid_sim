using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector3 velocity;
    public float maxVelocity;
    public RNNRLAgent rnnAgent;

    // Start is called before the first frame update
    void Start()
    {
        // Randomize x,y,z components for initial direction of boids and normalize, initializing with maxVelocity
        velocity = new Vector3(Random.Range(-1f, 1f), 
                               Random.Range(-1f, 1f), 
                               Random.Range(-1f, 1f)).normalized * maxVelocity;
        // inputSize, hiddenSize, outputSize
        //rnnAgent = new RNNRLAgent(6, 10, 3);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 changeVelocity = new Vector3(Random.Range(-0.1f, 0.1f),
                                             Random.Range(-0.1f, 0.1f),
                                             Random.Range(-0.1f, 0.1f));
        velocity += changeVelocity;

        // float[] inputs = {transform.position.x, transform.position.y, transform.position.z, velocity.x, velocity.y, velocity.z};
        //  // Log inputs
        // Debug.Log("RNN Inputs: " + string.Join(", ", inputs));

        // Forward propagation to get RNN outputs
        // float[] outputs = rnnAgent.ForwardPropagation(inputs);

        // // Log outputs
        // Debug.Log("RNN Outputs: " + string.Join(", ", outputs));


        // Vector3 rnnOutputVelocity = new Vector3(outputs[0], outputs[1], outputs[2]);
        

        // velocity += rnnOutputVelocity;

        if (velocity.magnitude > maxVelocity){
            velocity = velocity.normalized * maxVelocity;
        }
        this.transform.position += velocity * Time.deltaTime;
        this.transform.rotation = Quaternion.LookRotation(velocity);
        
    }
}
