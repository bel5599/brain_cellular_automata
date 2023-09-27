using System;
using System.Collections.Generic;
using System.Text;

namespace AutomataCelular
{
    class Node
    {
        public int name;
        public int distance;
        public Node pi;
        public string color;
        public Cell cell;
        public Pos pos;
        public string type;
        public Node(int name, Pos pos, string type)
        {
            this.name = name;
            this.pos = pos;
            this.type = type;
        }
    }
    class Edge
    {
        public Node left_node;
        public Node right_node;
        public Edge(Node left_node, Node right_node)
        {
            this.left_node = left_node;
            this.right_node = right_node;
        }
    }

    class Graph
    {
        public List<Node> nodes;
        public List<Edge> edges;
        public List<List<Node>> ady_list;
        public Graph()
        {
            nodes = new List<Node>();
            edges = new List<Edge>();
            ady_list = new List<List<Node>>();
            
        }
        public void AddNode(Node node)
        {
            //verfificar que no exista ya este nodo
            nodes.Add(node);
        }
        public void AddEdge(Edge edge)
        {
            //verificar que no exista ya esta arista
            edges.Add(edge);
        }
        public Node FindNode(int x, int y)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].pos.X == x && nodes[i].pos.Y == y) return nodes[i];
            }
            return null;
        }
    }
}
