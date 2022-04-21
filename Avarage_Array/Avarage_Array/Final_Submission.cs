using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avarage_Array
{
	 class Final_Submission
	{
        static void Main(string[] args)
        {
            Console.WriteLine("Enter number of values:");
            int n = Convert.ToInt32(Console.ReadLine());
            if (n == 0)
            {
                Console.WriteLine("Empty Array");
            }
            else
            {
                int[] arr = new int[n];
                Console.WriteLine("Enter the values:");
                for (int i = 0; i < n; i++)
                {
                    arr[i] = Convert.ToInt32(Console.ReadLine());
                }

                string result = FindAverage(arr);

                Console.Write("The Average is: " + result);

                Console.ReadLine();
            }
            Console.ReadLine();
        }

        //write here logic to calculate the average an array
        public static String FindAverage(int[] a)
        {
            int sum = 0, avg;
            if (a.Length == 0)
                return "Array is Empty";
            else
            {
                foreach (int e in a)
                {
                    if (e < 0)
                    {
                        return "Give proper positive integer values";
                    }
                    else
                        sum = sum + e;
                }

                avg = (sum / a.Length);

                string res = Convert.ToString(avg);

                return  res;
            }
        }
    }
}
