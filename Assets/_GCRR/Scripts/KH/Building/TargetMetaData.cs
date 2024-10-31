using System;

namespace GCRR.VirtualMap
{
    [Serializable]
    public class TargetMetaData
    {
        public string id;/* { get; set; }*/
        public string filename;/* { get; set; }*/
        public string name;/* { get; set; }*/
        public string address;
        public string centercoord;
        public string complexname;
        public string buildingtype;
        public string structure;
        public string groundfloor;
        public string area;
        public string height;
        public string createdate;
        public string picturedate;
    }
}
