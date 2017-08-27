using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Parser.Test
{
    public partial class Main : Form
    {
        private Jhu.Graywulf.Jobs.Query.QueryFactory queryFactory;
        private Node rootNode;

        public Jhu.Graywulf.Jobs.Query.QueryFactory QueryFactory
        {
            get { return queryFactory; }
            set { queryFactory = value; }
        }

        public Main()
        {
            InitializeComponent();
        }

        private void toolbuttonParse_Click(object sender, EventArgs e)
        {
            DateTime start = DateTime.Now;

            try
            {
                //queryFactory = new Jhu.SkyQuery.Jobs.Query.XMatchQueryFactory();
                queryFactory = QueryFactory.Create(typeof(Jhu.Graywulf.Jobs.Query.SqlQueryFactory).AssemblyQualifiedName, null);

                Jhu.Graywulf.Parsing.Parser parser = queryFactory.CreateParser();

                rootNode = (Node)parser.Execute(sql.Text);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            RefreshNodeTree(null, rootNode);
            parsed.Text = Jhu.Graywulf.Sql.CodeGeneration.SqlServer.SqlServerCodeGenerator.GetCode(rootNode, false);
        }

        private void toolbuttonResolve_Click(object sender, EventArgs e)
        {
            if (rootNode != null)
            {
                using (RegistryContext context = ContextManager.Instance.CreateReadOnlyContext())
                {

                    //try
                    //{
                    var qs = rootNode.FindDescendant<QueryExpression>().FindDescendant<QuerySpecification>();

                    //Jhu.Graywulf.Schema.SqlServerSchemaManager sm = new Schema.SqlServerSchemaManager();


                    Jhu.Graywulf.Schema.SchemaManager sm =
                        GraywulfSchemaManager.Create(new FederationContext(context, null));

                    // *** Add test datasets here
                    sm.Datasets["MYDB"] = new Jhu.Graywulf.Schema.SqlServer.SqlServerDataset("MYDB", "Data Source=localhost;Integrated Security=true;Initial Catalog=GraywulfSchemaTest");
                    //sm.Datasets.Add(new Jhu.Graywulf.Schema.MySqlDataset("MYSQL", "..."));


                    var qf = QueryFactory.Create(typeof(Jhu.Graywulf.Jobs.Query.SqlQueryFactory).AssemblyQualifiedName, null);
                    var nr = qf.CreateNameResolver();
                    nr.SchemaManager = sm;

                    nr.DefaultTableDatasetName = "MYDB";
                    nr.Execute((StatementBlock)rootNode);

                    //List<SqlParser.TableReference> rt = new List<SqlParser.TableReference>(qs.e);
                    List<SearchConditionReference> pc = new List<SearchConditionReference>(EnumerateConditions(qs));

                    SelectList sl = qs.FindDescendant<SelectList>();
                    List<ColumnExpression> ce = new List<ColumnExpression>(sl.EnumerateDescendants<ColumnExpression>());

                    parsed.Text = Jhu.Graywulf.Sql.CodeGeneration.SqlServer.SqlServerCodeGenerator.GetCode(rootNode, true);
                    //}
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show(ex.Message);
                    //}

                }
            }
        }

        public IEnumerable<SearchConditionReference> EnumerateConditions(QuerySpecification qs)
        {
            WhereClause wh = qs.FindDescendant<WhereClause>();

            if (wh == null)
            {
                yield break;
            }
            else
            {
                BooleanExpression sc = wh.FindDescendant<BooleanExpression>();
                if (sc == null)
                {
                    yield break;
                }
                else
                {
                    foreach (object n in sc.Nodes)
                    {
                        SearchConditionReference wc;
                        if (n is Predicate)
                        {
                            wc = new SearchConditionReference((Predicate)n);
                            yield return wc;
                        }
                        else if (n is BooleanExpressionBrackets)
                        {
                            wc = new SearchConditionReference((BooleanExpressionBrackets)n);
                            yield return wc;
                        }
                    }
                }
            }
        }

        private void RefreshColumnList(Sql.NameResolution.ITableReference tr)
        {
            columns.Items.Clear();

            foreach (var cr in tr.TableReference.ColumnReferences)
            {
                ListViewItem ni;

                if (cr.TableReference != null)
                {
                    ni = new ListViewItem(cr.TableReference.UniqueName);
                }
                else
                {
                    ni = new ListViewItem("?");
                }

                ni.SubItems.Add(cr.ColumnName);
                ni.SubItems.Add(cr.ColumnAlias);
                ni.SubItems.Add(cr.ColumnContext.ToString());
                ni.SubItems.Add(cr.IsComplexExpression.ToString());

                columns.Items.Add(ni);
            }
        }


        /*
                void ParseSpatial(Node s)
                {
                    LinkedListNode<object> n = s.Stack.First;

                    while (n != null)
                    {
                        if (n.Value is Parser.vector)
                        {
                            Parser.vector v = n.Value as Parser.vector;

                            Parser.argument_list al = v.Stack.First.Next.Value as Parser.argument_list;
                            int args = CountArguments(al);
                            n.Value = CreateFunctionCall("spatial", "vector_" + args.ToString(), al);
                        }

                        if (n.Value is Node)
                        {
                            ParseSpatial(n.Value as Node);
                        }

                        n = n.Next;
                    }
                }
         * */

        /*
        Parser.FunctionCall CreateFunctionCall(string schemaName, string functionName, Parser.ArgumentList arguments)
        {
            Token t;

            Parser.FunctionCall fc = new Parser.FunctionCall();
            
            Parser.FunctionIdentifier fi = new Parser.FunctionIdentifier();
            fc.Stack.AddLast(fi);

            Parser.SchemaName sn = new Parser.SchemaName();
            fi.Stack.AddLast(sn);

            t = new Parser.Terminals.Identifier();
            t.Value = schemaName;
            sn.Stack.AddLast(t);

            t = new Parser.Terminals.Dot();
            t.Value = ".";
            fi.Stack.AddLast(t);

            Parser.UdfName un = new Parser.UdfName();
            fi.Stack.AddLast(un);

            t = new Parser.Terminals.Identifier();
            t.Value = functionName;
            un.Stack.AddLast(t);

            t = new Parser.Terminals.BracketOpen();
            t.Value = "(";
            fi.Stack.AddLast(t);

            fi.Stack.AddLast(arguments);

            t = new Parser.Terminals.BracketClose();
            t.Value = ")";
            fi.Stack.AddLast(t);

            return fc;
        }
         * */

        /*
        int CountArguments(Parser.ArgumentList al)
        {
            if (al.Stack.Last.Value is Parser.ArgumentList)
                return 1 + CountArguments(al.Stack.Last.Value as Parser.ArgumentList);
            else
                return 1;
        }
        */

        void GenerateCode(ref string code, Node l)
        {
            if (code == null) code = string.Empty;

            foreach (object o in l.Nodes)
                if (o is Node)
                    GenerateCode(ref code, (Node)o);
                else
                    GenerateCodeToken(ref code, (Token)o);
        }

        void GenerateCodeToken(ref string code, Token t)
        {
            code += t.Value + " ";
        }

        void RefreshNodeTree(TreeNode n, Node l)
        {
            TreeNode ni = GenerateNode(l);

            ni.Tag = l;
            if (n == null)
            {
                tree.Nodes.Clear();
                tree.Nodes.Add(ni);
            }
            else
            {
                n.Nodes.Add(ni);
            }

            //foreach (object o in l.Stack)   
            foreach (object o in l.Nodes)   // use l.Nodes to get rolled-up tree
            {
                if (o is Node)
                {
                    RefreshNodeTree(ni, (Node)o);
                }
                else
                {
                    ni.Nodes.Add(GenerateToken((Token)o));
                }
            }
        }

        TreeNode GenerateNode(Node l)
        {
            var ni = new TreeNode();

            var name = l.GetType().Name;

            if (l is Sql.NameResolution.ITableReference)
            {
                name += " : ITableReference";
            }

            ni.Text = name;
            ni.Tag = l;

            return ni;
        }

        TreeNode GenerateToken(Token t)
        {
            TreeNode ni = new TreeNode(t.GetType().Name + " " + t.Value);
            ni.ForeColor = Color.Gray;
            return ni;
        }

        private void wITHQueryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sql.Text =
@"WITH alma AS
(
 SELECT alma.*, beta.a, dbo.fv(x,z) AS b FROM alma
 INNER JOIN korte ON alma = korte
 WHERE (x + b + c + (-a * f - d) > y)
)
SELECT * FROM alma
";

        }

        private void vectorQueryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sql.Text =
@"SELECT { a, b, c } FROM alma";
        }

        private void tree_DoubleClick(object sender, EventArgs e)
        {
            Node nn = (Node)tree.SelectedNode.Tag;
            Console.Write(":");
        }

        private void toolLinearize_Click(object sender, EventArgs e)
        {
            /*if (rootNode != null)
            {
                var ss = (SqlParser.SelectStatement)rootNode;

                // this will collect the lines
                StringBuilder sb = new StringBuilder();

                foreach (var qs in ss.EnumerateQuerySpecifications())
                {
                    var cnr = new SqlParser.SearchConditionNormalizer();
                    cnr.Execute(qs);

                    foreach (var tr in ss.EnumerateSourceTableReferences(true))
                    {
                        var cg = new Jhu.Graywulf.SqlParser.SqlCodeGen.SqlServerCodeGenerator();
                        sb.AppendLine(cg.GenerateMostRestrictiveTableQuery(qs, tr, 0));
                    }
                }

                parsed.Text = sb.ToString();
            }*/
        }

        private void tree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag is Sql.NameResolution.ITableReference)
            {
                RefreshColumnList((Sql.NameResolution.ITableReference)e.Node.Tag);
            }
        }
    }
}