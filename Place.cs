using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AEstrella
{
    internal class Place
    {
        string name = "";
        double cost = 0;
        double heuristic = 0;
        List<string> pathTaked = new List<string>();

        public Place(string name)
        {
            this.name = name;
        }

        public Place(string name, double cost)
        {
            this.name = name;
            this.cost = cost;
        }

        public Place(string name, double cost, double heuristic)
        {
            this.name = name;
            this.cost = cost;
            this.heuristic = heuristic;
        }

        public Place(string name, double cost, double heuristic, List<string> pathTaked)
        {
            this.name = name;
            this.cost = cost;
            this.heuristic = heuristic;
            this.pathTaked = pathTaked;
        }

        public string getName()
        {
            return this.name;
        }

        public void setName(String name)
        {
            this.name = name;
        }

        public double getCost()
        {
            return this.cost;
        }

        public void setCost(double cost)
        {
            this.cost = cost;
        }

        public double getHeuristic()
        {
            return this.heuristic;
        }

        public void setHeuristic(double heuristic)
        {
            this.heuristic = heuristic;
        }

        public List<string> getPath()
        {
            return this.pathTaked;
        }

        public void setPath(List<string> pathTaked)
        {
            this.pathTaked = pathTaked;
        }

        public void addToPath(string path)
        {
            pathTaked.Add(path);
        }
    }
}
