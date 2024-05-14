using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    [Serializable]
    public class Car
    {
        public Car()
        {
        }
        public string Model { get; set; }
        public Engine Engine { get; set; }
        public int Year { get; set; }

        public Car(string model, Engine engine, int year)
        {
            Model = model;
            Engine = engine;
            Year = year;
        }
    }
}
