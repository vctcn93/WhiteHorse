using Dynamo.Controls;
using Dynamo.Wpf;

namespace RevitUI
{

    public class ElementsOfCategoryView : INodeViewCustomization<ElementsOfCategory>
    {
        public void CustomizeView(ElementsOfCategory model, NodeView nodeView)
        {
            var ui = new ElementsOfCategoryUI();
            nodeView.inputGrid.Children.Add(ui);
            ui.DataContext = model;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
        }
    }

}
