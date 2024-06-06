using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AEstrella
{
    internal class Program
    {
        // Data
        private string[] dataGps =
        {
            "LUGO:LUGO(0);SANTANDER(390);BILBAO(-1);PAMPLONA(-1);LEON(221);LOGRONO(-1);SARAGOSSA(-1);BARCELONA(-1);SORIA(-1);MADRID(-1);VALENCIA(-1)",
            "SANTANDER:LUGO(390);SANTANDER(0);BILBAO(105);PAMPLONA(-1);LEON(269);LOGRONO(-1);SARAGOSSA(-1);BARCELONA(-1);SORIA(353);MADRID(-1);VALENCIA(-1)",
            "BILBAO:LUGO(-1);SANTANDER(105);BILBAO(0);PAMPLONA(157);LEON(-1);LOGRONO(153);SARAGOSSA(-1);BARCELONA(-1);SORIA(-1);MADRID(-1);VALENCIA(-1)",
            "PAMPLONA:LUGO(-1);SANTANDER(-1);BILBAO(157);PAMPLONA(0);LEON(-1);LOGRONO(86);SARAGOSSA(172);BARCELONA(-1);SORIA(-1);MADRID(-1);VALENCIA(-1)",
            "LEON:LUGO(221);SANTANDER(264);BILBAO(-1);PAMPLONA(-1);LEON(0);LOGRONO(290);SARAGOSSA(-1);BARCELONA(-1);SORIA(311);MADRID(-1);VALENCIA(-1)",
            "LOGRONO:LUGO(-1);SANTANDER(-1);BILBAO(153);PAMPLONA(86);LEON(290);LOGRONO(0);SARAGOSSA(171);BARCELONA(-1);SORIA(100);MADRID(327);VALENCIA(-1)",
            "SARAGOSSA:LUGO(-1);SANTANDER(-1);BILBAO(-1);PAMPLONA(172);LEON(-1);LOGRONO(171);SARAGOSSA(0);BARCELONA(311);SORIA(-1);MADRID(314);VALENCIA(-1)",
            "BARCELONA:LUGO(-1);SANTANDER(-1);BILBAO(-1);PAMPLONA(-1);LEON(-1);LOGRONO(-1);SARAGOSSA(311);BARCELONA(0);SORIA(-1);MADRID(-1);VALENCIA(374)",
            "SORIA:LUGO(-1);SANTANDER(353);BILBAO(-1);PAMPLONA(-1);LEON(311);LOGRONO(100);SARAGOSSA(-1);BARCELONA(-1);SORIA(0);MADRID(229);VALENCIA(-1)",
            "MADRID:LUGO(-1);SANTANDER(-1);BILBAO(-1);PAMPLONA(-1);LEON(-1);LOGRONO(327);SARAGOSSA(314);BARCELONA(-1);SORIA(229);MADRID(0);VALENCIA(355)",
            "VALENCIA:LUGO(-1);SANTANDER(-1);BILBAO(-1);PAMPLONA(-1);LEON(-1);LOGRONO(-1);SARAGOSSA(-1);BARCELONA(374);SORIA(-1);MADRID(355);VALENCIA(0)",
        };
        static void Main(string[] args)
        {
            // Eliminate static statement
            var exe = new AEstrella.Program();
            exe.start();
        }

        public void start()
        {
            var option = "";
            var nodes = dataGps.Length;
            var matrixPlaces = new PlaceWithChilds[nodes];
            var matrixNames = new string[nodes];

            // Loads the data into the arrays
            setMatrix(nodes, matrixNames, matrixPlaces);

            do
            {
                Console.WriteLine("Avalible places:");

                // Show all places on the GPS
                showArray(matrixNames);

                // Ask user the place to start and the place to end
                var indexStart = getUserPlace("Write your starting place", matrixNames);
                var indexEnd = getUserPlace("Write your ending place", matrixNames);

                // Find and show the path and cost to go from the starting place, to the ending place
                executePathFinding(indexStart, indexEnd, matrixNames, matrixPlaces);

                // End program?
                Console.WriteLine("End program? ('YES' to end program)");
                option = Console.ReadLine().ToUpper();
            } while (option != "YES");
        }

        private void setMatrix(int nodes, string[] matrixNames, PlaceWithChilds[] matrixPlaces)
        {
            var place = 0;

            while (place < nodes)
            {
                var nodeInfo = dataGps[place];

                // Get the name of the actual node
                var indexEndNodeName = nodeInfo.IndexOf(':');
                var nodeName = nodeInfo.Substring(0, indexEndNodeName);

                // Get all childs from the actual node
                var allChildrensInfo = nodeInfo.Substring(indexEndNodeName + 1);
                var childrens = allChildrensInfo.Split(';');

                // Sets the name to the names array and adds the new place with childs in the places array
                matrixNames[place] = nodeName;
                matrixPlaces[place] = new PlaceWithChilds(nodeName, extractChilds(childrens));

                place++;
            }
        }

        private void showArray(string[] array)
        {
            foreach (var item in array)
            {
                Console.WriteLine(" - " + item);
            }
        }

        private List<Place> extractChilds(string[] childrens)
        {
            var listChilds = new List<Place>();

            foreach (var child in childrens)
            {
                // Extracts the name of the child
                var indexEndChildName = child.IndexOf("(");
                var childName = child.Substring(0, indexEndChildName);
                
                // Extracts the value of the child
                var indexEndChildValue = child.IndexOf(")");
                var lengthValueString = indexEndChildValue - indexEndChildName;
                var childValue = double.Parse(child.Substring(indexEndChildName + 1, lengthValueString - 1));

                // If the child is a known brother of the main child, add it to the list of childs
                if (childValue > 0)
                {
                    listChilds.Add(new Place(childName, childValue));
                }
            }

            return listChilds;
        }

        // Gets the input of the user, making sure that the user writes a place that is on the matrix
        private int getUserPlace(string message, string[] matrixNames)
        {
            var index = -1;

            Console.WriteLine("-----------------------------");
            Console.WriteLine(message);

            while (index < 0)
            {
                var place = Console.ReadLine().ToUpper();

                index = Array.IndexOf(matrixNames, place);

                if (index < 0)
                {
                    Console.WriteLine("We don't have this place");
                }
            }

            return index;
        }

        // Set up the first node, executes the A* algoritm and shows the ending place
        private void executePathFinding(int indexStart, int indexEnd, string[] matrixNames, PlaceWithChilds[] matrixPlaces)
        {
            var path = new List<Place>();
            var namePlaceStart = matrixNames[indexStart];

            path.Add(new Place(namePlaceStart));

            findPlace(matrixNames[indexEnd], path, matrixNames, matrixPlaces);

            showPath(path[0]);

            Console.ReadLine();
        }

        // Searches the optimal path to take to find the place sended as parameter
        private void findPlace(string endPlace, List<Place> path, string[] matrixNames, PlaceWithChilds[] matrixPlaces)
        {
            var actualPlace = path[0].getName();

            while (!actualPlace.Equals(endPlace))
            {
                var indexPlace = Array.IndexOf(matrixNames, actualPlace);
                var childs = matrixPlaces[indexPlace].getChilds();

                foreach (var child in childs)
                {
                    // Search that the actual child is not in the path, and if is not in the path, it gets added into the path
                    var found = isItemReplicated(child.getName(), path[0]);

                    if (!found)
                    {
                        path.Add(createChild(child, path[0]));
                    }
                }

                // Remove actual place and replicated items
                path.RemoveAt(0);
                removeReplicatedItems(path);

                // Order path array by cost
                path.Sort((a, b) => (a.getCost() + a.getHeuristic()).CompareTo((b.getCost() + b.getHeuristic())));

                // Get the first place name
                actualPlace = path[0].getName();
            }
        }

        // Search for replicated name places on the path
        private bool isItemReplicated(string childName, Place place)
        {
            var found = false;

            foreach (var parentPath in place.getPath())
            {
                if (parentPath.Equals(childName))
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        public Place createChild(Place child, Place parentNode)
        {
            // Creates a new place with the name of the child and with the cost of the actual child added to the parentNode cost
            var newChild = new Place(child.getName(), child.getCost() + parentNode.getCost());

            foreach (var item in parentNode.getPath())
            {
                newChild.addToPath(item);
            }

            // adds the parent path to the array of path
            newChild.addToPath(parentNode.getName());

            return newChild;
        }

        private void removeReplicatedItems(List<Place> path)
        {
            var objectsToRemove = new List<Place>();

            // Search for repeted names on the path array and saves the places on the objectsToRemove array
            foreach (var pathTaked in path)
            {
                foreach (var findPath in path)
                {
                    if (findPath.getName().Equals(pathTaked.getName()))
                    {
                        if (findPath.getCost() < pathTaked.getCost())
                        {
                            objectsToRemove.Add(pathTaked);
                        }
                    }
                }
            }

            // Eliminates from the path array all items that are on the objectsToRemove array
            foreach (var removeItem in objectsToRemove)
            {
                path.Remove(removeItem);
            }
        }

        // Shows the information of the place and the path taked
        private void showPath(Place place)
        {
            Console.WriteLine("-----------------------------");
            Console.WriteLine(place.getName() + " " + place.getCost() + " " + place.getHeuristic());
            Console.WriteLine("Path taked:");

            foreach (var item1 in place.getPath())
            {
                Console.WriteLine(" - " + item1);
            }
        }
    }
}
