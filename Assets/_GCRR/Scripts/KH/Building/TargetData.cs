using System;
using System.Security.Policy;

[Serializable]
public class TargetData
{
    public Asset asset { get; set; }
    public float geometricError { get; set; }
    public TileSetRoot root { get; set; }

    public TargetData()
    {
        asset = new Asset();
        root = new TileSetRoot();
    }
}

[Serializable]
public class Asset
{
    public string version { get; set; }
    public string gltfUpAxis { get; set; }
}

[Serializable]
public class Extras
{
    public Ion ion { get; set; }

    public Extras()
    {
        ion = new Ion();
    }
}

[Serializable]
public class Ion
{
    public bool georeferenced { get; set; }
    public bool movable { get; set; }
    public float terrainId { get; set; }
}

[Serializable]
public class BoundingVolume
{
    public double[] region { get; set; }

    public BoundingVolume() { region = new double[0]; }
}

[Serializable]
public class Content
{
    public string uri { get; set; }

    public Content() { uri = string.Empty; }
}

[Serializable]
public class TileSetRoot
{
    public BoundingVolume boundingVolume { get; set; }
    public float geometricError { get; set; }
    public string refine { get; set; }
    public TileSet[] children { get; set; }

    public TileSetRoot()
    {
        boundingVolume = new BoundingVolume();
        children = new TileSet[0];
    }
}

[Serializable]
public class TileSet
{
    public BoundingVolume boundingVolume { get; set; }
    public float geometricError { get; set; }
    public Content content { get; set; }
    public double[] transform { get; set; }

    public TileSet()
    {
        boundingVolume = new BoundingVolume();
        content = new Content();
    }
}