namespace Building_Permits.Core.Entities
{
    public class Stage
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int OrderNo { get; set; }

        public int DefaultDaysLimit { get; set; }


        public virtual ICollection<PermitStage> PermitStages { get; set; } = new List<PermitStage>();
    }

}
