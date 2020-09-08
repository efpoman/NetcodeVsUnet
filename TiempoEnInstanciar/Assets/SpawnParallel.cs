using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using System.Diagnostics;

public class SpawnParallel : MonoBehaviour
{
    public GameObject sheepPrefab;
    Transform[] allSheepTransform;

    struct MoveJob : IJobParallelForTransform
    {
        public void Execute(int index, TransformAccess transform)
        {
            transform.position += 0.1f * (transform.rotation * new Vector3(0, 0, 1));
            if (transform.position.z > 50)
                transform.position = new Vector3(transform.position.x, 0, -50);
        }
    }

    MoveJob moveJob;
    JobHandle moveHandle;
    TransformAccessArray transforms;

    const int numSheep = 15000;

    // Start is called before the first frame update
    void Start()
    {
        allSheepTransform = new Transform[numSheep];
        Stopwatch timeMeasure = new Stopwatch();
        timeMeasure.Start();
        for (int i = 0; i < numSheep; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
            GameObject sheep = Instantiate(sheepPrefab, pos, Quaternion.identity);
            allSheepTransform[i] = sheep.transform;
        }
        transforms = new TransformAccessArray(allSheepTransform);
        timeMeasure.Stop();
        UnityEngine.Debug.Log($"Tiempo: {timeMeasure.Elapsed.TotalMilliseconds} ms");
    }

    void Update()
    {
        moveJob = new MoveJob { };
        moveHandle = moveJob.Schedule(transforms);
    }

    void LateUpdate()
    {
        moveHandle.Complete();
    }

    void OnDestroy()
    {
        transforms.Dispose();
    }

}
