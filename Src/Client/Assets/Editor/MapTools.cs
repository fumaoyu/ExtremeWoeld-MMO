using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MapTools
{
    [MenuItem("Map Tools/Export Teleporters(导出传送点坐标位置)")]
    public static void ExportTeleporters()
    {
        DataManager.Instance.Load();

        Scene current = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();//当前场景
        string currentScene = current.name;

        if (current.isDirty)
        {
            EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确定");//创建一个提示窗口
            return;
        }
        List<TeleporterObject> allTeleporter = new List<TeleporterObject>();//

        foreach (var map in DataManager.Instance.Maps)
        {
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            if (!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("Scene {0} not existed", sceneFile);
                continue;
            }
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);

            TeleporterObject[] teleporters = GameObject.FindObjectsOfType<TeleporterObject>();//找到地图中所有的传送点；
            foreach (var teleporter in teleporters)
            {
                if (!DataManager.Instance.Teleporters.ContainsKey(teleporter.TelePorID))
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图 {0} 中配置的 Teleporter:[{1}] 中不存在", map.Value.Resource, teleporter.TelePorID), "确定");
                    return;
                }

                TeleporterDefine def = DataManager.Instance.Teleporters[teleporter.TelePorID];
                if (def.MapID != map.Value.ID)
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图 {0} 中配置的 Teleportter:[{1}] 中不1存在", map.Value.Resource, def.ID), "确定");
                    return;
                }

                def.Position = GameObjectTool.WorldToLogicN(teleporter.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(teleporter.transform.forward);
            }

        }
        DataManager.Instance.SaveTeleporters();//保存传送点坐标数据

        EditorSceneManager.OpenScene("Assets/Levels/" + currentScene + ".unity");
        EditorUtility.DisplayDialog("提示", "传送点导出完成", "确定");
    }


    [MenuItem("Map Tools/Export SpawnPoints")]
    public static void ExportSpawnPoints()
    {
        DataManager.Instance.Load();

        Scene scene = EditorSceneManager.GetActiveScene();
        string currentScene = scene.name;
        if (scene.isDirty)
        {
            EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确定");
            return;
        }
        if (DataManager.Instance.SpawnPoints == null)
            DataManager.Instance.SpawnPoints = new Dictionary<int, Dictionary<int, SpawnPointDefine>>();

        foreach (var map in DataManager.Instance.Maps)
        {
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";

            if (!System.IO.File.Exists(sceneFile))
            {
                Debug.LogFormat("Scene {0} not existed!", sceneFile);
                continue;
            }
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);

            SpawnPoint[] spawnpoints = GameObject.FindObjectsOfType<SpawnPoint>();
            if (!DataManager.Instance.SpawnPoints.ContainsKey(map.Value.ID))
            {
                DataManager.Instance.SpawnPoints[map.Value.ID] = new Dictionary<int, SpawnPointDefine>();
            }
            foreach (var sp in spawnpoints)
            {
                if (!DataManager.Instance.SpawnPoints[map.Value.ID].ContainsKey(sp.ID))
                {
                    DataManager.Instance.SpawnPoints[map.Value.ID][sp.ID] = new SpawnPointDefine();
                }
                SpawnPointDefine def = DataManager.Instance.SpawnPoints[map.Value.ID][sp.ID];
                def.ID = sp.ID;
                def.MapID = map.Value.ID;
                def.Position = GameObjectTool.WorldToLogicN(sp.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(sp.transform.forward);
            }
        }
        DataManager.Instance.SaveSpawnPoints();
        EditorSceneManager.OpenScene("Assets/Levels/" + currentScene + ".unity");
        EditorUtility.DisplayDialog("提示", "刷怪点导出完成", "确定");
    }

    [MenuItem("Map Tools/寻路网格")]
    public static void GetNavMeshGrid()
    {
        Scene scene=EditorSceneManager.GetActiveScene();
        //Material red = new Material(Shader.Find("Particles/Alpha Blended"));///红色材质
        //red.color = Color.red;
        //red.SetColor("_TinColor", Color.red);
        //red.enableInstancing = true;

        GameObject go = GameObject.Find("MiniMapBoxBorder");


       if (go != null)
        {
            GameObject root = new GameObject("Root");

            BoxCollider  bound=go.GetComponent<BoxCollider>();
            float step = 1f;

            for (float x = bound.bounds.min.x; x < bound.bounds.max.x; x+=step)
            {
                for (float z = bound.bounds.min.z; z < bound.bounds.max.z; z+=step)

                {
                    for (float y = bound.bounds.max.y; y< bound.bounds.max.y+2f; y += step)//地面网上加五米
                    {
                        //Debug.Log("x；" + x + "y:" + y + "z:" + z);
                       
                        var pos=new Vector3(x,y,z);
                        NavMeshHit hit;

                        ////采样点半径0.5米内有没有可行走的
                        if (NavMesh.SamplePosition(pos, out hit, 0.3f, NavMesh.AllAreas))
                        {
                            if (hit.hit)///有没有命中。，
                            {
                                var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                box.name="Hit"+hit.mask;
                                box.GetComponent<MeshRenderer>().sharedMaterial.color=Color.red;
                                //box.GetComponent<MeshRenderer>().sharedMaterial = red;
                                box.transform.SetParent(root.transform,true);
                                box.transform.position = pos;
                                box.transform.localScale = Vector3.one * 0.9f;
                                
                            }
                        }
                    }
                }
            }

        }
       

    }
}
