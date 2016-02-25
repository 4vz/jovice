using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aphysoft.Common.Html;

namespace Aphysoft.Share
{
    public class GridView : IUIControls
    {
        #region Fields

        private string id;

        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        #endregion

        #region Constructor

        public GridView()
        {

        }

        #endregion

        #region Methods

        public void Process(HtmlNode node, string id)
        {
            HtmlDocument document = node.OwnerDocument;

            #region Table
            HtmlNode table = new HtmlNode(HtmlNodeType.Element, document);
            table.Name = "table";
            table.Attributes.Add("id", id);
            
            node.ParentNode.InsertAfter(table, node);

            HtmlNode tbody = new HtmlNode(HtmlNodeType.Element, document);
            tbody.Name = "tbody";

            table.AppendChild(tbody);

            #endregion

            #region Init Script
            // TODO harusnya gak gini
            HtmlNode script = new HtmlNode(HtmlNodeType.Element, document);
            script.Name = "script";
            script.Attributes.Add("type", "text/javascript");
            script.InnerHtml = "share.ui.gridview(\"" + id + "\");";

            node.ParentNode.InsertAfter(script, table);
            #endregion

            HtmlNodeCollection nChildren = node.ChildNodes;

            List<HtmlNode> nRows = new List<HtmlNode>();

            foreach (HtmlNode nChild in nChildren)
            {
                if (nChild.Name == "row") nRows.Add(nChild);
            }

            foreach (HtmlNode nRow in nRows)
            {
                #region Row

                HtmlNode row = new HtmlNode(HtmlNodeType.Element, document);
                row.Name = "tr";

                tbody.AppendChild(row);

                #endregion

                List<HtmlNode> nCells = new List<HtmlNode>();

                foreach (HtmlNode nRowChild in nRow.ChildNodes)
                {
                    if (nRowChild.Name == "cell") nCells.Add(nRowChild);
                }

                foreach (HtmlNode nRowCell in nCells)
                {
                    #region Cell

                    HtmlNode cell = new HtmlNode(HtmlNodeType.Element, document);
                    cell.Name = "td";
                    cell.InnerHtml = nRowCell.InnerHtml;

                    row.AppendChild(cell);

                    #endregion
                }
            }


        }

        #endregion
    }
}
