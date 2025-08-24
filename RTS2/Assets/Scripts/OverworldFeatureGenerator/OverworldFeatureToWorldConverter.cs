using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldFeatureToWorldConverter
{
    public virtual void GenerateFeature(WorldChunkBatch toGenerateIn)
    {

    }

    public virtual OverworldFeature GetFeatureIGenerate()
    {
        return OverworldFeature.Backroad;
    }
}
