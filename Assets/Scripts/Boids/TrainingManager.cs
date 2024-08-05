using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingManager : MonoBehaviour
{
    public RNNRLAgent rnnAgent;
    public float trainingInterval = 5f; // Interval between training steps in seconds
    public int numberOfIterations = 100; // Number of training iterations
    public float maxReward = 10f; // Maximum reward value

    private void Start()
    {
        StartCoroutine(TrainingLoop());
    }

    private IEnumerator TrainingLoop()
    {
        for (int iteration = 0; iteration < numberOfIterations; iteration++)
        {
            // Collect data from boids
            var boids = FindObjectsOfType<Boid>();
            foreach (var boid in boids)
            {
                // Get inputs (e.g., position, velocity) from the boid
                float[] inputs = GetInputsFromBoid(boid);

                // Forward propagation to get the RNN outputs
                float[] outputs = rnnAgent.ForwardPropagation(inputs);

                // Apply RNN outputs to the boid
                ApplyOutputsToBoid(boid, outputs);

                // Calculate reward for the boid
                float reward = CalculateReward(boid);

                // Collect target outputs based on reward
                float[] targets = GetTargetsFromReward(reward);

                // Update RNN weights
                rnnAgent.BackwardPropagation(inputs, outputs, targets);
            }

            // Wait for the next training interval
            yield return new WaitForSeconds(trainingInterval);
        }

        // Save weights after training
        rnnAgent.SaveWeights("Assets/TrainedRNNWeights.txt");
    }

    private float[] GetInputsFromBoid(Boid boid)
    {
        // Collect input data from the boid (e.g., position, velocity)
        Vector3 position = boid.transform.position;
        Vector3 velocity = boid.velocity;

        // Example of simple inputs: [x, y, z, vx, vy, vz]
        return new float[]
        {
            position.x, position.y, position.z,
            velocity.x, velocity.y, velocity.z
        };
    }

    private void ApplyOutputsToBoid(Boid boid, float[] outputs)
    {
        // Apply RNN outputs to boid (e.g., update velocity)
        Vector3 rnnOutputVelocity = new Vector3(outputs[0], outputs[1], outputs[2]);
        boid.velocity += rnnOutputVelocity;

        // Clip velocity to max velocity
        if (boid.velocity.magnitude > boid.maxVelocity)
        {
            boid.velocity = boid.velocity.normalized * boid.maxVelocity;
        }
    }

    private float CalculateReward(Boid boid)
    {
        // Define your reward calculation logic here
        // Example: Return a reward based on boid's behavior
        return Random.Range(0f, maxReward); // Placeholder reward
    }

    private float[] GetTargetsFromReward(float reward)
    {
        // Define target outputs based on reward
        // Example: Set target outputs equal to the reward
        return new float[] { reward, reward, reward };
    }
}

