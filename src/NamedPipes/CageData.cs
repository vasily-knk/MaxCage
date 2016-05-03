using System;
using System.Collections.Generic;
using NamedPipes;
using UnityEngine;

using EntityId = System.UInt32;
using ModelId = System.UInt32;

    class CageData
    {
        public Dictionary<EntityId, EntityData> ents {get; private set;}
        public Dictionary<ModelId, Model> modelsPending { get; private set; }
        private List<EntityId> creationPending;
        private List<EntityId> transformPending;
        
        public CageData()
        {
            ents = new Dictionary<EntityId, EntityData>();
            modelsPending = new Dictionary<ModelId, Model>();
            creationPending = new List<EntityId>();
            transformPending = new List<EntityId>();
        }

        public void AddEntity(EntityId id, EntityData data)
        {
            ents.Add(id, data);
            creationPending.Add(id);
        }

        public void AddModel(ModelId id, Model model)
        {
            modelsPending.Add(id, model);
        }

        public void UpdateEntityTransform(EntityId id, NamedPipes.Transform transform)
        {
            ents[id].transform = transform;
            transformPending.Add(id);
        }

        public List<EntityId> FlushCreation()
        {
            var result = creationPending;
            creationPending = new List<EntityId>();
            return result;
        }

        public List<EntityId> FlushTransform()
        {
            var result = transformPending;
            transformPending = new List<EntityId>();
            return result;
        }

        public Dictionary<ModelId, Model> FlushModels()
        {
            var result = modelsPending;
            modelsPending = new Dictionary<ModelId, Model>();
            return result;
        }

    }
