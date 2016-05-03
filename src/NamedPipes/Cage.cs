﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NamedPipes;

using EntityId = System.UInt32;
using ModelId = System.UInt32;

public class Cage : MonoBehaviour
{
    private TcpServer server_;
    private CageData gameData_;
    private Dictionary<EntityId, GameObject> objects_;
    private Dictionary<ModelId, UnityEngine.Mesh> meshes_;
    private Material material_;

    public Cage()
    {
        gameData_ = new CageData();
        objects_ = new Dictionary<EntityId, GameObject>(); 
        meshes_ = new Dictionary<ModelId, UnityEngine.Mesh>();
    }

    private void OnMsgAddEntity(MsgAddEntity msg)
    {
        lock (gameData_)
        {
            gameData_.AddEntity(msg.id, msg.data);
        }

        Debug.LogFormat("Adding entity: {0}", msg.data.getText());
    }

    private void OnMsgDeleteEntity(MsgDeleteEntity msg)
    {
        
    }

    private void OnMsgUpdateEntityTransform(MsgUpdateEntityTransform msg)
    {
        lock(gameData_)
        {
            gameData_.UpdateEntityTransform(msg.id, msg.transform);
        }
    }

    private void OnMsgAddModel(MsgAddModel msg)
    {
        lock (gameData_)
        {
            gameData_.AddModel(msg.id, msg.model);
        }
    }

    private void OnNextFrame(MsgNextFrame msg)
    {
    }

	// Use this for initialization
	void Start () {
        var dispatcher = new Dispatcher();
        dispatcher.Add<MsgAddEntity>(0, OnMsgAddEntity);
        dispatcher.Add<MsgDeleteEntity>(1, OnMsgDeleteEntity);
        dispatcher.Add<MsgUpdateEntityTransform>(2, OnMsgUpdateEntityTransform);
        dispatcher.Add<MsgAddModel>(3, OnMsgAddModel);
        dispatcher.Add<MsgNextFrame>(100, OnNextFrame);


        material_ = Resources.Load<Material>("MyMaterial");
        
        Debug.Log("Starting listener...");
        server_ = new TcpServer(dispatcher);
        server_.Start();
	}

    private static void UpdateTransform(GameObject obj, NamedPipes.Transform tr)
    {
        const float ratio = 0.1f;
        obj.transform.localPosition = tr.pos * ratio;
        obj.transform.localRotation = tr.rot;
        obj.transform.localScale = tr.scale  * ratio;
    }
	
	void Update() 
    {
        IList<EntityId> newEnts, newTransforms;

        lock(gameData_)
        {
            var models = gameData_.FlushModels();

            foreach (ModelId id in models.Keys)
            {
                var model = models[id];
                
                var mesh = new UnityEngine.Mesh();

                var verts = new Vector3[model.verts.Length];
                var normals = new Vector3[model.verts.Length];
                for (int i = 0; i < model.verts.Length; ++i)
                {
                    var vert = model.verts[i];
                    verts[i] = vert.pos;
                    normals[i] = vert.normal;
                }

                mesh.vertices = verts;
                mesh.normals = normals;

                var allIndices = new List<int>();
                for (int sm = 0; sm < model.meshes.Length; ++sm)
                {
                    allIndices.AddRange(model.meshes[sm].indices);
                }

                mesh.triangles = allIndices.ToArray();
                mesh.UploadMeshData(true);
                
                meshes_.Add(id, mesh);

                Debug.LogFormat("Created mesh {0}", model.name);
            }
            
            newEnts = gameData_.FlushCreation();

            foreach (EntityId id in newEnts)
            {
                EntityData data = gameData_.ents[id];
                
                var newObj = new GameObject();
                newObj.name = "ent_" + data.name;
                newObj.transform.parent = this.transform;

                if (data.modelId != 0)
                {
                    if (meshes_.ContainsKey(data.modelId))
                    {
                        var meshFilter = newObj.AddComponent<MeshFilter>();
                        meshFilter.mesh = meshes_[data.modelId];

                        
                        var meshRenderer = newObj.AddComponent<MeshRenderer>();
                        meshRenderer.materials = new Material[]{material_};
                    }
                    else
                    {
                        Debug.LogErrorFormat("Non-Existing mesh: {0}", data.modelId);
                    }
                }

                objects_.Add(id, newObj);
                UpdateTransform(newObj, data.transform);
            }

            newTransforms = gameData_.FlushTransform();

            foreach (EntityId id in newTransforms)
            {
                UpdateTransform(objects_[id], gameData_.ents[id].transform);
            }

        }
	}
}
