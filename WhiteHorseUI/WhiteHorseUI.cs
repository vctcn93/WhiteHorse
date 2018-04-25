using System;
using System.Collections.Generic;

using OfficeOpenXml.Drawing.Chart;

using DSRevitNodesUI;
using Dynamo.Graph.Nodes;
using ProtoCore.AST.AssociativeAST;
using Dynamo.Utilities;
using RevitServices.Persistence;
using Autodesk.Revit.DB;
using CoreNodeModels;
using System.Linq;
using Autodesk.DesignScript.Runtime;

namespace RevitUI
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CustomGenericEnumerationDropDown : RevitDropDownBase
    {
        public Type EnumerationType;

        public CustomGenericEnumerationDropDown(string name, Type enumerationType) : base(name)
        {
            this.EnumerationType = enumerationType;
            this.PopulateItems();
        }

        protected override DSDropDownBase.SelectionState PopulateItemsCore(string currentSelection)
        {
            this.PopulateItems();
            return 0;
        }

        public new void PopulateItems()
        {
            if (this.EnumerationType != null)
            {
                base.Items.Clear();
                string[] names = Enum.GetNames(this.EnumerationType);
                for (int i = 0; i < names.Length; i++)
                {
                    string text = names[i];
                    string name = null;
                    switch (text)
                    {
                        case "Left":
                            name = "左";
                            break;
                        case "Right":
                            name = "右";
                            break;
                        case "Standard":
                            name = "标准";
                            break;
                        case "StirrupTie":
                            name = "箍筋";
                            break;
                        default:
                            name = text;
                            break;
                    }
                    base.Items.Add(new DynamoDropDownItem(name, text));
                }
                base.Items = ExtensionMethods.ToObservableCollection<DynamoDropDownItem>(from x in base.Items
                                                                                         orderby x.Name
                                                                                         select x);
            }
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (base.Items.Count == 0 || base.Items.Count == -1)
            {
                this.PopulateItems();
            }
            StringNode stringNode = AstFactory.BuildStringNode(base.Items[base.SelectedIndex].Item.ToString());
            BinaryExpressionNode item = AstFactory.BuildAssignment(this.GetAstIdentifierForOutputIndex(0), stringNode);
            return new List<AssociativeNode>
            {
                item
            };
        }
    }

    public abstract class CustomRevitElementDropDown : RevitDropDownBase
    {
        private const string noTypes = "当前没有可用类型.";

        public Type ElementType;

        public CustomRevitElementDropDown(string name, Type elementType) : base(name)
        {
            this.ElementType = elementType;
            this.PopulateItems();
        }

        protected override SelectionState PopulateItemsCore(string currentSelection)
        {
            this.PopulateItems();
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public new void PopulateItems()
        {
            if (this.ElementType != null)
            {
                base.Items.Clear();
                FilteredElementCollector filteredElementCollector = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument).OfClass(this.ElementType);
                if (filteredElementCollector.ToElements().Count == 0)
                {
                    base.Items.Add(new DynamoDropDownItem("当前没有可用类型.", null));
                    base.SelectedIndex = 0;
                }
                else
                {
                    if (this.ElementType.FullName == "Autodesk.Revit.DB.Structure.RebarHookType")
                    {
                        base.Items.Add(new DynamoDropDownItem("None", null));
                    }
                    foreach (Autodesk.Revit.DB.Element current in filteredElementCollector.ToElements())
                    {
                        base.Items.Add(new DynamoDropDownItem(current.Name, current));
                    }
                    base.Items = ExtensionMethods.ToObservableCollection(from x in base.Items orderby x.Name select x);
                }
            }
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            IEnumerable<AssociativeNode> result;
            if (base.Items.Count == 0 || base.Items[0].Name == "当前没有可用类型." || base.SelectedIndex == -1 || base.Items[base.SelectedIndex].Name == "None")
            {
                result = new BinaryExpressionNode[]
                {
                    AstFactory.BuildAssignment(this.GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode())
                };
            }
            else
            {
                Type typeFromHandle = typeof(Autodesk.Revit.DB.Element);
                ElementId id = ((Autodesk.Revit.DB.Element)base.Items[base.SelectedIndex].Item).Id;

                AssociativeNode associativeNode = AstFactory.BuildFunctionCall("Revit.Elements.ElementSelector", "ByElementId", new List<AssociativeNode>
                {
                    AstFactory.BuildIntNode(id.IntegerValue)
                }, null);
                result = new BinaryExpressionNode[]
                {
                    AstFactory.BuildAssignment(this.GetAstIdentifierForOutputIndex(0), associativeNode)
                };
            }
            return result;
        }
    }

    [IsDesignScriptCompatible, NodeCategory("WhiteHorse.Revit.WH_Rebar.Query"), NodeDescription("钢筋类型"), NodeName("Rebar Bar Type")]
    public class RebarBarType : CustomRevitElementDropDown
    {
        public RebarBarType() : base("Rebar Bar Type", typeof(Autodesk.Revit.DB.Structure.RebarBarType))
        {
        }
    }

    [IsDesignScriptCompatible, NodeCategory("WhiteHorse.Revit.WH_Rebar.Query"), NodeDescription("钢筋样式"), NodeName("Rebar Style")]
    public class RebarStyle : CustomGenericEnumerationDropDown
    {
        public RebarStyle() : base("Rebar Style", typeof(Autodesk.Revit.DB.Structure.RebarStyle))
        {
        }
    }

    [IsDesignScriptCompatible, NodeCategory("WhiteHorse.Revit.WH_Rebar.Query"), NodeDescription("弯钩类型"), NodeName("Rebar Hook Type")]
    public class RevitRebarHookType : CustomRevitElementDropDown
    {
        public RevitRebarHookType() : base("Rebar Hook Type", typeof(Autodesk.Revit.DB.Structure.RebarHookType))
        {
        }
    }

    [IsDesignScriptCompatible, NodeCategory("WhiteHorse.Revit.WH_Rebar.Query"), NodeDescription("弯钩方向"), NodeName("Rebar Hook Orientation")]
    public class RebarHookOrientation : CustomGenericEnumerationDropDown
    {
        public RebarHookOrientation() : base("Rebar Hook Orientation", typeof(Autodesk.Revit.DB.Structure.RebarHookOrientation))
        {
        }
    }

    #region chartType
    /// <summary>
    /// ChartType
    /// </summary>
    [NodeName("ChartType")]
    [NodeCategory("WhiteHorse.Excel.ChartType")]
    [NodeDescription("图表样式")]
    [IsDesignScriptCompatible]
    public class ChartTypes : CustomGenericEnumerationDropDown
    {
        /// <summary>
        /// 
        /// </summary>
        public ChartTypes() : base("ChartType", typeof(eChartType)) { }
    }
    #endregion

    #region All Elements of Category
    /// <summary>
    /// All Element of Category
    /// </summary>
    [NodeName("All Elements of Category")]
    [NodeCategory("WhiteHorse.Revit.WH_Collector")]
    [NodeDescription("获取指定Category的所有元素")]
    [IsDesignScriptCompatible]
    public class OfCategoryDropDown : RevitDropDownBase
    {
        private const string noElements = "项目中没有当前类型的元素。";

        [IsVisibleInDynamoLibrary(false)]
        public OfCategoryDropDown() : base("Elements")
        {

        }

        [IsVisibleInDynamoLibrary(false)]
        private static string getFullName(Autodesk.Revit.DB.Category category)
        {
            string result = string.Empty;
            if (category != null)
            {
                Autodesk.Revit.DB.Category parent = category.Parent;
                if (parent == null)
                {
                    result = category.Name.ToString();
                }
                else
                {
                    result = parent.Name.ToString() + " - " + category.Name.ToString();
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override DSDropDownBase.SelectionState PopulateItemsCore(string currentSelection)
        {
            base.Items.Clear();
            Document currentDBDocument = DocumentManager.Instance.CurrentDBDocument;
            foreach (BuiltInCategory builtInCategory in Enum.GetValues(typeof(BuiltInCategory)))
            {
                Autodesk.Revit.DB.Category category;
                try
                {
                    category = Autodesk.Revit.DB.Category.GetCategory(currentDBDocument, builtInCategory);
                }
                catch
                {
                    continue;
                }
                if (category != null)
                {
                    string fullName = getFullName(category);
                    base.Items.Add(new DynamoDropDownItem(fullName, category.Id.IntegerValue));
                }
            }
            base.Items = ExtensionMethods.ToObservableCollection<DynamoDropDownItem>(from x in base.Items
                                                                                     orderby x.Name
                                                                                     select x);
            return SelectionState.Done;
        }


        /// <summary>
        /// 
        /// </summary>
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {


            int builtInCategoryId = (int)Items[SelectedIndex].Item;

            Func<int, IList<Revit.Elements.Element>> func = new Func<int, IList<Revit.Elements.Element>>(Functions.RevitUtils.ElementsOfCategoryTS);
            List<AssociativeNode> node = new List<AssociativeNode> { AstFactory.BuildIntNode(builtInCategoryId) };
            AssociativeNode associativeNode = AstFactory.BuildFunctionCall(func, node, null);
            return new BinaryExpressionNode[]
            {
                AstFactory.BuildAssignment(this.GetAstIdentifierForOutputIndex(0), associativeNode)
            };
        }
    }
    #endregion

    #region All Elements At Level
    /// <summary>
    /// All Elements At Level
    /// </summary>
    [NodeName("All Elements At Level")]
    [NodeCategory("WhiteHorse.Revit.WH_Collector")]
    [NodeDescription("获取指定标高上的所有元素")]
    [IsDesignScriptCompatible]
    public class AtLevelDropDown : RevitDropDownBase
    {
        private const string noElements = "项目中没有当前类型的元素。";

        [IsVisibleInDynamoLibrary(false)]
        public AtLevelDropDown() : base("Elements")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected override DSDropDownBase.SelectionState PopulateItemsCore(string currentSelection)
        {
            base.Items.Clear();
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            filteredElementCollector.OfClass(typeof(Level));
            IList<Element> source = filteredElementCollector.ToElements();
            if (!source.Any<Element>())
            {
                base.Items.Add(new DynamoDropDownItem("当前项目无Level可用", null));
                base.SelectedIndex = 0;
                return 0;
            }
            base.Items = ExtensionMethods.ToObservableCollection<DynamoDropDownItem>(from x in source
                                                                                     select new DynamoDropDownItem(x.Name, x.Id.IntegerValue) into x
                                                                                     orderby x.Name
                                                                                     select x);
            return SelectionState.Done;
        }


        /// <summary>
        /// 
        /// </summary>
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {


            int levelId = (int)Items[SelectedIndex].Item;

            Func<int?, IList<Revit.Elements.Element>> func = new Func<int?, IList<Revit.Elements.Element>>(Functions.RevitUtils.ElementsAtLevelTS);
            List<AssociativeNode> node = new List<AssociativeNode> { AstFactory.BuildIntNode(levelId) };
            AssociativeNode associativeNode = AstFactory.BuildFunctionCall(func, node, null);
            return new BinaryExpressionNode[]
            {
                AstFactory.BuildAssignment(this.GetAstIdentifierForOutputIndex(0), associativeNode)
            };
        }
    }
    #endregion

    #region All Elements Of FamilyType
    /// <summary>
    /// All Elements Of FamilyType
    /// </summary>
    [NodeName("All Elements Of FamilyType")]
    [NodeCategory("WhiteHorse.Revit.WH_Collector")]
    [NodeDescription("获取指定FamilyType的所有元素")]
    [IsDesignScriptCompatible]
    public class OfFamilyTypeDropDown : RevitDropDownBase
    {
        [IsVisibleInDynamoLibrary(false)]
        public OfFamilyTypeDropDown() : base("Elements")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected override DSDropDownBase.SelectionState PopulateItemsCore(string currentSelection)
        {
            base.Items.Clear();
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            filteredElementCollector.OfClass(typeof(Family));
            IList<Element> list = filteredElementCollector.ToElements();
            if (!list.Any<Element>())
            {
                base.Items.Add(new DynamoDropDownItem("没有可用的 family type.", null));
                base.SelectedIndex = 0;
                return 0;
            }
            using (IEnumerator<Element> enumerator = list.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Family family = (Family)enumerator.Current;
                    foreach (ElementId current in family.GetFamilySymbolIds())
                    {
                        Element element = family.Document.GetElement(current);
                        base.Items.Add(new DynamoDropDownItem(string.Format("{0}:{1}", family.Name, element.Name), current.IntegerValue));
                    }
                }
            }
            base.Items = ExtensionMethods.ToObservableCollection<DynamoDropDownItem>(from x in base.Items
                                                                                     orderby x.Name
                                                                                     select x);

            return SelectionState.Done;
        }


        /// <summary>
        /// 
        /// </summary>
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {


            int familyTypeId = (int)Items[SelectedIndex].Item;

            Func<int?, IList<Revit.Elements.Element>> func = new Func<int?, IList<Revit.Elements.Element>>(Functions.RevitUtils.ElementsOfFamilyType);
            List<AssociativeNode> node = new List<AssociativeNode> { AstFactory.BuildIntNode(familyTypeId) };
            AssociativeNode associativeNode = AstFactory.BuildFunctionCall(func, node, null);
            return new BinaryExpressionNode[]
            {
                AstFactory.BuildAssignment(this.GetAstIdentifierForOutputIndex(0), associativeNode)
            };
        }
    }
    #endregion
}


