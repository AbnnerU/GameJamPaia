using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using UnityEngine.UI;

public class EnemysOnMap : MonoBehaviour
{
    [SerializeField] private bool active = true;

    [SerializeField] private bool drawGizmos;

    [SerializeField] private AgentConfig[] agents;

    [SerializeField] private MapPositionsConfig[] mapConfig;

   

    private void Update()
    {
        if (!active) return;

        int count = mapConfig.Length;
        int agentsCount = agents.Length;

        NativeArray<Vector3> mapCenterArray = new NativeArray<Vector3>(count,Allocator.TempJob);
        NativeArray<Vector3> mapAreaSizeArray = new NativeArray<Vector3>(count, Allocator.TempJob);
        NativeArray<int> enemyPositionId = new NativeArray<int>(agentsCount, Allocator.TempJob);
        NativeArray<Vector3> agentsCurrentPositionArray = new NativeArray<Vector3>(agentsCount, Allocator.TempJob);
        TransformAccessArray a = new TransformAccessArray(agentsCount);

        for(int i=0; i < count; i++)
        {
            mapCenterArray[i] = mapConfig[i].center.position;
            mapAreaSizeArray[i] = mapConfig[i].areaSize/2;
        }

        for(int i=0; i < agents.Length; i++)
        {
            agentsCurrentPositionArray[i] = agents[i].agentRealWordTransform.position;
        }

        MarkerParallelJob markerParallelJob = new MarkerParallelJob
        {
            mapCenterRef = mapCenterArray,
            mapAreaSizeRef = mapAreaSizeArray,
            resultsIndex = enemyPositionId,
            currentAgentPosition = agentsCurrentPositionArray
        };

        JobHandle jobHandle = markerParallelJob.Schedule(agentsCount, 1);

        jobHandle.Complete();


        for(int i=0; i < agentsCount; i++)
        {
            if (agents[i].agentRealWordTransform.gameObject.activeSelf == false)
            {
                agents[i].agentUIImage.enabled = false;
                continue;
            }
            else           
                agents[i].agentUIImage.enabled = true;
            

            if (markerParallelJob.resultsIndex[i] >= 0)
            {
                agents[i].agentRectTransform.anchoredPosition = mapConfig[markerParallelJob.resultsIndex[i]].mapRef.anchoredPosition;
            }
        }

        mapCenterArray.Dispose();
        mapAreaSizeArray.Dispose();
        enemyPositionId.Dispose();
        agentsCurrentPositionArray.Dispose();
       
    }



    [BurstCompile]
    public struct MarkerParallelJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Vector3> mapCenterRef;

        [ReadOnly]
        public NativeArray<Vector3> mapAreaSizeRef;

        
        public NativeArray<int> resultsIndex;

        [ReadOnly]
        public NativeArray<Vector3> currentAgentPosition;

        public void Execute(int index)
        {
            resultsIndex[index] = -1;

            for (int i = 0; i < mapAreaSizeRef.Length; i++)
            {
                //    //agent inside the area
                if (currentAgentPosition[index].x < mapCenterRef[i].x + mapAreaSizeRef[i].x &&
                    currentAgentPosition[index].x > mapCenterRef[i].x - mapAreaSizeRef[i].x &&
                    currentAgentPosition[index].y < mapCenterRef[i].y + mapAreaSizeRef[i].y &&
                    currentAgentPosition[index].y > mapCenterRef[i].y - mapAreaSizeRef[i].y)
                {
                    resultsIndex[index] = i;
                }
            }
        }

    }   

    [System.Serializable]
    public struct MapPositionsConfig
    {
        public Transform center;
        public Vector2 areaSize;
        public RectTransform mapRef;
    }

    [System.Serializable]    
    public struct AgentConfig
    {
        public Transform agentRealWordTransform;
        public Image agentUIImage;
        public RectTransform agentRectTransform;
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            for(int i=0;i<mapConfig.Length;i++)
            {
                Gizmos.DrawCube(mapConfig[i].center.position, mapConfig[i].areaSize);
            }
        }
    }


}
