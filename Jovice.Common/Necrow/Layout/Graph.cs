using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;
using Aphysoft.Common;

namespace Jovice
{
    public class JoviceVertex
    {
        #region Fields

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        #endregion

        #region Constructors

        public JoviceVertex(string name)
        {
            this.name = name;
        }

        #endregion
    }

    public class JoviceEdge : Edge<JoviceVertex>
    {
        public JoviceEdge(JoviceVertex source, JoviceVertex target) : base(source, target)
        {
        }
    }
    
    public class JoviceGraph : BidirectionalGraph<JoviceVertex, JoviceEdge>
    {

        #region Constructors

        public JoviceGraph() : base(true)
        {
        }

        #endregion

        #region Static

        public static void Update()
        {
            Necrow.Event("Updating Graph...");

            JoviceGraph graph = new JoviceGraph();

            JoviceVertex a = new JoviceVertex("afis");
            JoviceVertex b = new JoviceVertex("aimee");
            JoviceVertex c = new JoviceVertex("anisa");

            Dictionary<JoviceVertex, System.Windows.Size> sizes = new Dictionary<JoviceVertex, System.Windows.Size>();
            sizes.Add(a, new System.Windows.Size(100, 50));
            sizes.Add(b, new System.Windows.Size(100, 50));
            sizes.Add(c, new System.Windows.Size(100, 50));

            Dictionary<JoviceVertex, System.Windows.Thickness> thickness = new Dictionary<JoviceVertex, System.Windows.Thickness>();
            thickness.Add(a, new System.Windows.Thickness(0));
            thickness.Add(b, new System.Windows.Thickness(0));
            thickness.Add(c, new System.Windows.Thickness(0));

            Dictionary<JoviceVertex, GraphSharp.Algorithms.Layout.Compound.CompoundVertexInnerLayoutType> types = new Dictionary<JoviceVertex, GraphSharp.Algorithms.Layout.Compound.CompoundVertexInnerLayoutType>();
            types.Add(a, GraphSharp.Algorithms.Layout.Compound.CompoundVertexInnerLayoutType.Automatic);
            types.Add(b, GraphSharp.Algorithms.Layout.Compound.CompoundVertexInnerLayoutType.Automatic);
            types.Add(c, GraphSharp.Algorithms.Layout.Compound.CompoundVertexInnerLayoutType.Automatic);

            graph.AddVertex(a);
            graph.AddVertex(b);
            graph.AddVertex(c);

            graph.AddEdge(new JoviceEdge(a, b));
            graph.AddEdge(new JoviceEdge(b, c));
                        
            GraphSharp.Algorithms.Layout.Compound.FDP.CompoundFDPLayoutAlgorithm<JoviceVertex, JoviceEdge, JoviceGraph> alg =
                new GraphSharp.Algorithms.Layout.Compound.FDP.CompoundFDPLayoutAlgorithm<JoviceVertex, JoviceEdge, JoviceGraph>(graph, sizes, thickness, types);

            alg.Finished += delegate (object sender, EventArgs e)
            {
                Necrow.Event("compute finished");
                foreach (KeyValuePair<JoviceVertex, System.Windows.Point> pair in alg.VertexPositions)
                {
                    Necrow.Event(pair.Key.Name + " " + pair.Value.X + " " + pair.Value.Y);
                }
            };

            alg.Compute();

            
        }

        #endregion
    }

    
}
