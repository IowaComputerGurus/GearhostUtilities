using System.Collections.Generic;

namespace IowaComputerGurus.Utility.GearHost.Models
{
    public class GearHostDatabase
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Plan { get; set; }
        public string Type { get; set; }
        public long Size { get; set; }
        public bool Locked { get; set; }
        public string DateCreated { get; set; }
    }

    public class GearHostGetDatabasesResponse
    {
        public List<GearHostDatabase> Databases { get; set; }
    }
}