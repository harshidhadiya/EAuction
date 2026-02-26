namespace MACUTION.Data
{
    public class Request_admin
    {
        public int Id { get; set; }
        public int request_person_id { get; set; }
        public int ?give_access_person_id { get; set; }
        public bool verified_admin { get; set; }
        public User requet_person{ get; set; }
        public User ?permission_person { get; set; }
    }
}