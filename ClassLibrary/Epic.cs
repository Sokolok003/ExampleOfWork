using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
	[Serializable]
	class Epic : Excersice
	{
		public List<Excersice> subtasks = new List<Excersice>();
		private int max;
		public int Max { get; }
		public Epic (string name) : base(name)
		{
			this.max = 30;
			type = "Epic";
		}

        public override void GiveUser(User user)
        {
            throw new NotImplementedException();
        }

        public override void DelUser(int index)
        {
            throw new NotImplementedException();
        }

        public override void AddSubtask(Excersice excersice)
        {
            switch (excersice.type)
            {
				case "Epic":
					throw new Exception("Задачи типа Epic не может быть подзадачей!");

				case "Bug":
					throw new Exception("Задачи типа Bug не может быть подзадачей!");

				case "Story":
					bool check = true;
					for (int i = 0; i < subtasks.Count; ++i)
					{
						if (excersice.Name == subtasks[i].Name)
							check = false;
					}
					if (check != false)
					{
						subtasks.Add(excersice);
					}
					if (subtasks.Count > 30)
					{
						subtasks.Remove(excersice);
						throw new Exception("Добавление новой подзадачи не возможно! Максимум превышен.");
					}
					break;

				case "Task":
					check = true;
					for (int i = 0; i < subtasks.Count; ++i)
					{
						if (excersice.Name == subtasks[i].Name)
							check = false;
					}
					if (check != false)
					{
						subtasks.Add(excersice);
					}
					if (subtasks.Count > 30)
					{
						subtasks.Remove(excersice);
						throw new Exception("Добавление новой подзадачи не возможно! Максимум превышен.");
					}
					break;
			}
        }

        public override void ShowAllSubtasts()
        {
			if(subtasks.Count == 0)
            {
				throw new Exception("Список подзадач на данный момент пуст!");
            }
            else
            {
				for (int i = 0; i < subtasks.Count; ++i)
				{
					subtasks[i].GetInfo(i);
				}
			}
            
        }

        public override List<Excersice> Subs()
        {
			return subtasks;
        }

        public override void DelSubtask(int index)
        {
			subtasks.Remove(subtasks[index]);
        }

        public override void GetInfo(int ind)
		{
			List<string> users = new List<string>();

			for(int i = 0; i< subtasks.Count; ++i)
			{
				bool check = true;
				for(int j = 0; j<users.Count; ++j)
                {
					if (subtasks[i].UserName() == users[j])
						check = false;
                }
				if(check==true)
					users.Add(subtasks[i].UserName());
			}
			
			string names = "";
			for(int i = 0; i<users.Count; ++i)
			{
				names += users[i] + ", ";
			}

			Console.WriteLine($"{ind}. Имя задачи {Name}, пользователи задачи {names}, дата создания {Date}, статус задачи {Status}.");
		}

		public override List<string> AllUsers()
        {
			List<string> users = new List<string>();

			for (int i = 0; i < subtasks.Count; ++i)
			{
				bool check = true;
				for (int j = 0; j < users.Count; ++j)
				{
					if (subtasks[i].UserName() == users[j])
						check = false;
				}
				if (check == true)
					users.Add(subtasks[i].UserName());
			}
			return users;
		}

		public override string UserName()
		{
			List<string> users = new List<string>();

			for (int i = 0; i < subtasks.Count; ++i)
			{
				bool check = true;
				for (int j = 0; j < users.Count; ++j)
				{
					if (subtasks[i].Name == users[j])
						check = false;
				}
				if (check == true)
					users.Add(subtasks[i].Name);
			}

			string names = "";
			for (int i = 0; i < users.Count; ++i)
			{
				names += users[i] + ", ";
			}
			return names;
		}
	}
}
