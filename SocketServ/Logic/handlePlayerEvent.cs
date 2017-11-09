using System;

public class HandlePlayerEvent
{
	//上线
	public void OnLogin(Player player)
	{

	}
	//下线
	public void OnLogout(Player player)
	{
		if (player.tempData.status != PlayerTempData.Status.None) 
		{
			Room room = player.tempData.room;
			RoomMgr.instance.LeaveRoom (player);
			if(room != null)
				room.Broadcast(room.GetRoomInfo());
		}
	}
}