using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary
{
	[Serializable]
	 public class User
	{
		private DateTime date;
		public DateTime Date { get; set; }
		private string name;
		public string Name { get; set; }


		public User(string name)
		{
			Name = name;
			Date = DateTime.Now;
		}

		public void GetInfo(int i)
        {
			Console.WriteLine($"{i}. Имя пользователя {Name}, дата создания {Date}");
        }

	}
}
