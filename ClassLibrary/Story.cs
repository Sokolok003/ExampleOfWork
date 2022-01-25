using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
	[Serializable]
	class Story : Excersice
	{
		public List<User> users = new List<User>();
		public User userFirst = new User("исполнитель отстуствует");

		public Story(string name) : base(name)
		{
			type = "Story";	
		}

        public override void AddSubtask(Excersice excersice)
        {
            throw new NotImplementedException();
        }

        public override void GiveUser(User user)
        {
			bool check = true;
			for(int i = 0; i<users.Count; ++i)
            {
				if (user.Name == users[i].Name)
					check = false;
            }
			if(check == true)
				users.Add(user);
        }

		public void GiveUser(List<User> useres)
        {	
			for(int i = 0; i<useres.Count; ++i)
            {
				bool check = true;
				for (int j = 0; j<users.Count; ++j)
                {
                    if (useres[i].Name == users[j].Name)
                    {
						check = false;
                    }
                }
				if (check == true)
					users.Add(useres[i]);
            }
        }

		public override void GetInfo(int ind)
		{
			string names = "";
			if(users.Count == 0)
            {
				names = userFirst.Name;
            }
            else
            {
				for (int i = 0; i < users.Count; ++i)
				{
					names += users[i].Name + ", ";
				}
			}			
			Console.WriteLine($"{ind}. Имя задачи { Name}, пользователи задачи {names}, дата создания { Date}, статус задачи { Status}.");
		}


        public override void DelUser(int index)
        {
			users.Remove(users[index]);
        }

        public override List<string> AllUsers()
        {
			List<string> names = new List<string>();
			if (users.Count == 0)
			{
				names.Add(userFirst.Name);
			}
			else
			{
				for (int i = 0; i < users.Count; ++i)
				{
					names.Add(users[i].Name);
				}
			}
			return names;
		}

		public override void ShowAllSubtasts()
        {
            throw new NotImplementedException();
        }
		public override string UserName()
		{
			string names = "";
			if (users.Count == 0)
			{
				names = userFirst.Name;
			}
			else
			{
				for (int i = 0; i < users.Count; ++i)
				{
					names += users[i].Name + ", ";
				}
			}
			return names;
		}

		public override List<Excersice> Subs()
		{
			throw new NotImplementedException();
		}
		public override void DelSubtask(int index)
		{
			throw new NotImplementedException();
		}
	}
}
