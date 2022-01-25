using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    [Serializable]
    class Task : Excersice
    {
        private User user;
        public User User { get { if (user.Name == "исполнитель отстуствует") return new User("исполнитель отстуствует"); else return user; } set { user = value; } }

        public Task(string name) : base(name)
        {
            type = "Task";
            user = new User("исполнитель отстуствует");
        }

        public override void GiveUser(User user)
        {
            User = user;
        }

        public override string UserName()
        {
            return User.Name;
        }

        public override void DelUser(int index)
        {
            user.Name = "исполнитель отстуствует";
        }

        public override void GetInfo(int i)
        {
            Console.WriteLine($"{i}. Имя задачи {Name}, пользователь задачи {User.Name}, дата создания {Date}, статус задачи {Status}.");
        }

        public override List<string> AllUsers()
        {
            throw new Exception();
        }

        public override void AddSubtask(Excersice excersice)
        {
            throw new NotImplementedException();
        }
        public override void ShowAllSubtasts()
        {
            throw new NotImplementedException();
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
