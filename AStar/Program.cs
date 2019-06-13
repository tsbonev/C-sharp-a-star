using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AStar
{
    class Program
    {
        static void Main(string[] args)
        {

            var nodeA = new Node(0, 0);
            var nodeB = new Node(1, 2);
            var nodeC = new Node(4, 1);
            var nodeD = new Node(0, 5);
            var nodeE = new Node(5, 2);
            var nodeF = new Node(5, 6);
            var nodeG = new Node(7, 7);

            nodeA.Connect(nodeC);
            nodeA.Connect(nodeD);

            nodeB.Connect(nodeC);
            nodeB.Connect(nodeE);
            nodeB.Connect(nodeD);

            nodeC.Connect(nodeE);

            nodeD.Connect(nodeF);

            nodeE.Connect(nodeG);

            nodeF.Connect(nodeG);

            var path = FindPath(nodeA, nodeG);

            if (path != null)
            {
                Console.WriteLine("Path Found!");
                path.Print();
            }
            else
            {
                Console.WriteLine("No path found!");
            }
        }

        public static Path FindPath(Node start, Node end)
        {
            var closed = new HashSet<Node>();

            var queue = new PriorityQueue<double, Path>();

            queue.Enqueue(0, new Path(start));

            while (!queue.IsEmpty)
            {
                var path = queue.Dequeue();

                if(closed.Contains(path.CurrentStep)) continue;

                Console.WriteLine("Stepping on node {0}, {1} with weight to end of {2}", path.CurrentStep.X,
                    path.CurrentStep.Y, path.TotalWeight);

                if (path.CurrentStep.Equals(end)) return path;

                closed.Add(path.CurrentStep);

                foreach (Node lastNeighbor in path.CurrentStep.Neighbors)
                {
                    var weight = path.CurrentStep.DistanceTo(lastNeighbor);
                    var newPath = path.AddStep(lastNeighbor, weight);
                    queue.Enqueue(newPath.TotalWeight + lastNeighbor.DistanceTo(end), newPath);
                }
            }

            return null;
        }

        public class Path
        {
            public Node CurrentStep { get; }
            public Path PreviousPath { get; }
            
            public double TotalWeight { get; }

            private Path(Node currentStep, Path previousPath, double totalWeight)
            {
                CurrentStep = currentStep;
                PreviousPath = previousPath;
                TotalWeight = totalWeight;
            }

            public Path(Node start) : this(start, null, 0) { }

            public Path AddStep(Node step, double weight)
            {
                return new Path(step, this, weight + TotalWeight);
            }

            public void Print()
            {
                var nodes = new Stack<Node>();
                var path = this;
                while (path.PreviousPath != null)
                {
                    nodes.Push(path.CurrentStep);
                    path = path.PreviousPath;
                }

                nodes.Push(path.CurrentStep);

                foreach (var node in nodes)
                {
                    Console.WriteLine("{0}, {1}", node.X, node.Y);
                }
            }
        }

        public struct Node
        {
            public int X { get; }
            public int Y { get; }

            public List<Node> Neighbors;

            public Node(int x, int y)
            {
                X = x;
                Y = y;
                Neighbors = new List<Node>();
            }

            public void Connect(Node node)
            {
                node.Neighbors.Add(this);
                this.Neighbors.Add(node);
            }

            public double DistanceTo(Node node)
            {
                return Math.Sqrt(Math.Pow(this.X - node.X, 2) + Math.Pow(this.Y - node.Y, 2));
            }
        }

        class PriorityQueue<P, V>
        {
            private SortedDictionary<P, Queue<V>> list = new SortedDictionary<P, Queue<V>>();
            public void Enqueue(P priority, V value)
            {
                Queue<V> q;
                if (!list.TryGetValue(priority, out q))
                {
                    q = new Queue<V>();
                    list.Add(priority, q);
                }
                q.Enqueue(value);
            }
            public V Dequeue()
            {
                var pair = list.First();
                var v = pair.Value.Dequeue();
                if (pair.Value.Count == 0)
                    list.Remove(pair.Key);
                return v;
            }
            public bool IsEmpty => !list.Any();
        }
    }
}
