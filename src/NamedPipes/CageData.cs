using System;
using System.Collections.Generic;
using NamedPipes;
using UnityEngine;

class CageData
{
    private List<Pair<EntityId, EntityData>> entities_;
    private List<Pair<ModelId, Model>> models_;
    private List<Pair<EntityId, NamedPipes.Transform>> transforms_;

    public CageData()
    {
        entities_ = new List<Pair<EntityId, EntityData>>();
        models_ = new List<Pair<ModelId, Model>>();
        transforms_ = new List<Pair<EntityId, NamedPipes.Transform>>();
    }

    public void AddEntity(EntityId id, EntityData data)
    {
        lock(entities_)
        {
            entities_.Add(Pair.New(id, data));
        }
    }

    public void AddModel(ModelId id, Model model)
    {
        lock (models_)
        {
            models_.Add(Pair.New(id, model));
        }
    }

    public void AddTransform(EntityId id, NamedPipes.Transform transform)
    {
        lock (transforms_)
        {
            transforms_.Add(Pair.New(id, transform));
        }
    }

    public List<Pair<EntityId, EntityData>> FlushEntities()
    {
        lock (entities_)
        {
            var result = entities_;
            entities_ = new List<Pair<EntityId, EntityData>>();
            return result;
        }
    }

    public List<Pair<ModelId, Model>> FlushModels()
    {
        lock (models_)
        {
            var result = models_;
            models_ = new List<Pair<ModelId, Model>>();
            return result;
        }
    }

    public List<Pair<EntityId, NamedPipes.Transform>> FlushTransforms()
    {
        lock (transforms_)
        {
            var result = transforms_;
            transforms_ = new List<Pair<EntityId, NamedPipes.Transform>>();
            return result;
        }

    }
}
