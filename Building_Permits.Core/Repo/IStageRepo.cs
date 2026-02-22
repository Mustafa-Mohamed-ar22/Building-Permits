using Building_Permits.Core.Entities;

namespace Building_Permits.Core.Repo
{
    public interface IStageRepo
    {
        public List<Stage> GetAllStages();
        public Stage GetStageById(int id);
        Task<List<Stage>> GetFirst_6_StagesAsync();
        public List<Stage> GetFirst_6_Stages();
        public List<Stage> Get_Second_Stages();
        public List<Stage> Get_Third_Stages();
        public List<Stage> Get_Fourth_Stages();

    }
}
