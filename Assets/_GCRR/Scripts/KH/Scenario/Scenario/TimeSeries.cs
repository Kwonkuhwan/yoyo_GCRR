using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable]
public class TimeSeriesData
{
    public ServiceIdentification ServiceIdentification;
    public ServiceProvider ServiceProvider; 
    public Timeseries Timeseries;
}

[Serializable]
public class ServiceIdentification
{
    public string Title;
    public string Abstract;
    public string ServiceType;
    public string ServiceTypeVersion;   
}

[Serializable]
public class ServiceProvider
{
    public string ProviderName;
    public string ProviderSite;
    public string ServiceContact;
}

[Serializable]
public class Timeseries
{
    public List<ImageLayer> imageLayer;
    public List<TerrainLayer> terrainLayer;
    public List<Tile3DLayer> tile3dLayer;
}

[Serializable]
public class ImageLayer
{
    public string Title;
    public string Abstract;
    public string Identifier;
    public string Format;
}

[Serializable]
public class TerrainLayer
{
    public string Title;
    public string Abstract;
    public string Identifier;
    public string Format;
}

[Serializable]
public class Tile3DLayer
{
    public string Title;
    public string Abstract;
    public string Identifier;
    public string Format;
}