using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEstrella
{
    internal class PlaceWithChilds
    {
        string name = "";
        List<Place> childs = new List<Place>();

        public PlaceWithChilds(String name)
        {
            this.name = name;
        }

        public PlaceWithChilds(String name, List<Place> childs)
        {
            this.name = name;
            this.childs = childs;
        }

        public string getName()
        {
            return this.name;
        }

        public void setName(String name)
        {
            this.name = name;
        }

        public List<Place> getChilds()
        {
            return this.childs;
        }

        public void setChilds(List<Place> childs)
        {
            this.childs = childs;
        }

        public void addChild(Place child)
        {
            childs.Add(child);
        }
    }
}
