using System;
using System.Collections.Generic;
using Dynamo.Graph.Nodes;
using ProtoCore.AST.AssociativeAST;


namespace RevitUI
{
    
    [NodeName("All Elements Of Category")]
    [NodeDescription("返回曲线上指定Parameter处的点")]
    [NodeCategory("WhiteHorse.Revit.WH_Collector")]
    [InPortDescriptions("需要取点的Curve")]
    [OutPortNames("Elements")]
    [OutPortTypes("Element")]
    [OutPortDescriptions("返回的点")]
    [IsDesignScriptCompatible]
    public class ElementsOfCategory : NodeModel
    {
        
    }
}