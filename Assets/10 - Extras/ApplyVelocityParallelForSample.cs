using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

class ApplyVelocityParallelForSample : MonoBehaviour
{
    struct VelocityJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Vector3> velocity;
        public NativeArray<Vector3> position;
        public float deltaTime;

        public void Execute(int i)
        {
            position[i] = position[i] + velocity[i] * deltaTime;
        }
    }

    void Update()
    {
        var position = new NativeArray<Vector3>(500, Allocator.Persistent);
        var velocity = new NativeArray<Vector3>(500, Allocator.Persistent);
        
        //1. We create some test data
        for (var i = 0; i < velocity.Length; i++)
            velocity[i] = new Vector3(0, 10, 0);

        //2. We create the job and configure it with the data
        var job = new VelocityJob()
        {
            deltaTime = Time.deltaTime,
            position = position,
            velocity = velocity
        };

        //3. We schedule a job indicating the amount of times it needs to execute and the batch count.
        var jobHandle = job.Schedule(position.Length, 64);

        //4. We can use this blocking function to stop the update until the job is finished
        jobHandle.Complete();
        
        Debug.Log(position[0]);

        //5. Remember to manually dispose the arrays because they live in native memory
        position.Dispose();
        velocity.Dispose();
    }
}