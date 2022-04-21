using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_object_Concept
{
	class Student
	{
		public string name;
		public int age;
		int E_id;
		string Course;
		public void Insert()
		{
			Console.WriteLine("Enter name of the Student");
			name = Console.ReadLine();
			Console.WriteLine("Entrer age of the Student");
			age = int.Parse(Console.ReadLine());
			Console.WriteLine("Entrer Employee id of the Student");
			E_id = int.Parse(Console.ReadLine());
			Console.WriteLine("Entrer Course of the Student");
			Course = Console.ReadLine();
			Console.ReadLine();
		}


	}
}
