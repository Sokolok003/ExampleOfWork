using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary
{
	[Serializable]
	public abstract class Excersice
	{
		public string type;
		private string name;
		private DateTime date;
		private string status;
		public string Status { get; set; }
		public string Name { get; set; }
		public DateTime Date { get; set; }

		public Excersice(string name)
		{
			Name = name;
			Date = DateTime.Now;
			Status = "Открытая задача";
		}

		abstract public string UserName();
		abstract public void GetInfo(int i);
		abstract public void GiveUser(User user);
		abstract public void DelUser(int index);
		abstract public List<string> AllUsers();
		abstract public void AddSubtask(Excersice excersice);
		abstract public void ShowAllSubtasts();
		abstract public List<Excersice> Subs();
		abstract public void DelSubtask(int index);
	}
}
