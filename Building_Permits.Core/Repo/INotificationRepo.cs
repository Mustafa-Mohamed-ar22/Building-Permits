using Building_Permits.Core.Entities;

namespace Building_Permits.Core.Repo
{
	public interface INotificationRepo	
	{
		public void SendStageOverdue(PermitStage permitStage);
		public void sendArhcievedPermit(Permit permit);
		public void Sendneglected(Permit permit);
		public void SendReminder(Permit permit);

	}
}
