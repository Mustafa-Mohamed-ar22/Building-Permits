using Building_Permits.Core.Entities;

namespace Building_Permits.Core.Repo
{
    public interface IPermitStageRepo
    {
        public List<PermitStage> GetAllPermitStages();
        public PermitStage GetPermitStageById(int id);
        public bool AddPermitStage(PermitStage permitStage);
        public void Save();
        Task SaveAsync();
        
    }
}
