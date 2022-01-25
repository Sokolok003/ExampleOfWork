using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary
{
	[Serializable]
	public class Project
	{
		private string name;
		private int max;
		public int Max { get; }
		public string Name { get; set; }
		public List<Excersice> Excersices = new List<Excersice>();

		
		public Project(string name)
		{
			Name = name;
			this.max = 30;
		}

		public void GetInfo()
		{
			Console.Write($"Имя проекта {Name}, количество задач в проекте {Excersices.Count}.");
		}
	}
}
