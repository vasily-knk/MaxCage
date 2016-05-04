using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NamedPipes;


public class Cage : MonoBehaviour
{
    private TcpServer server_;
    private CageData gameData_;
    private Dictionary<EntityId, GameObject> objects_;
    private Dictionary<ModelId, UnityEngine.Mesh> meshes_;
    private Material material_;

    private GameObject VMCamera_, unityCamera_;


    public Cage()
    {
        gameData_ = new CageData();
        objects_ = new Dictionary<EntityId, GameObject>(); 
        meshes_ = new Dictionary<ModelId, UnityEngine.Mesh>();
    }

    private void OnMsgAddEntity(MsgAddEntity msg)
    {
        Debug.LogFormat("Add entity: {0}", msg.data.getText());
        gameData_.AddEntity(msg.id, msg.data);
    }

    private void OnMsgDeleteEntity(MsgDeleteEntity msg)
    {
        
    }

    private void OnMsgUpdateEntityTransform(MsgUpdateEntityTransform msg)
    {
        gameData_.AddTransform(msg.id, msg.transform);
    }

    private void OnMsgAddModel(MsgAddModel msg)
    {
        Debug.LogFormat("Add model: {0}", msg.model.name);
        gameData_.AddModel(msg.id, msg.model);
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

        unityCamera_ = GameObject.Find("Main Camera");

        material_ = Resources.Load<Material>("MyMaterial");
        
        Debug.Log("Starting listener...");
        server_ = new TcpServer(dispatcher);
        server_.Start();
	}

    private static void UpdateTransformImpl(GameObject obj, NamedPipes.Transform tr)
    {
        const float ratio = 0.1f;
        obj.transform.localPosition = tr.pos * ratio;
        obj.transform.localRotation = tr.rot;
        obj.transform.localScale = tr.scale * ratio;
    }
    private void UpdateTransform(GameObject obj, NamedPipes.Transform tr)
    {
        UpdateTransformImpl(obj, tr);
        if (obj == VMCamera_)
            UpdateTransformImpl(unityCamera_, tr);

    }

    private void ProcessModels()
    {
        var newModels = gameData_.FlushModels();

        foreach (var r in newModels)
        {
            var id = r.first;
            var model = r.second;

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

            //Debug.LogFormat("Created mesh {0}", model.name);
        }
    }

    private void ProcessEntities()
    {
        var newEntities = gameData_.FlushEntities();

        foreach(var r in newEntities)
        {
            var id = r.first;
            var data = r.second;

            var newObj = new GameObject();
            newObj.name = "ent_" + data.classname + "_" + data.name;
            newObj.transform.parent = this.transform;

            if (data.modelId.initialized())
            {
                if (meshes_.ContainsKey(data.modelId))
                {
                    var meshFilter = newObj.AddComponent<MeshFilter>();
                    meshFilter.mesh = meshes_[data.modelId];

                    var meshRenderer = newObj.AddComponent<MeshRenderer>();
                    meshRenderer.materials = new Material[] { material_ };
                }
                else
                {
                    Debug.LogErrorFormat("Non-Existing mesh: {0}", data.modelId);
                }
            }

            if (data.classname == "Camera" && data.name == "Tower View0")
                VMCamera_ = newObj;

            objects_.Add(id, newObj);
            UpdateTransform(newObj, data.transform);
        }
    }

    private void ProcessTransforms()
    {
        var newTransforms = gameData_.FlushTransforms();

        foreach (var r in newTransforms)
        {
            UpdateTransform(objects_[r.first], r.second);
        }
    }

    void Update() 
    {
        ProcessModels();
        ProcessEntities();
        ProcessTransforms();
	}
}
