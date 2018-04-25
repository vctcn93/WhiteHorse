using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using RevitServices.Persistence;
using Autodesk.Revit.DB;
using System.Linq;
using Revit.Elements;

namespace Functions
{
    [IsVisibleInDynamoLibrary(false)]
    public static class RevitUtils
    {
        public static IList<Revit.Elements.Element> ElementsOfCategory(int categoryId)
        {
            ElementId id = new ElementId(categoryId);
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            IList<Autodesk.Revit.DB.Element> elements = filteredElementCollector.OfCategoryId(id).WhereElementIsNotElementType().ToElements();
            List<Revit.Elements.Element> outElements = new List<Revit.Elements.Element>();

            foreach (var item in elements)
            {
                outElements.Add(Revit.Elements.ElementWrapper.ToDSType(item, true));
            }
            return outElements;
        }

        public static IList<Revit.Elements.Element> ElementsAtLevel(int? levelId)
        {

            if (levelId == null)
            {
                return null;
            }

            ElementId id = new ElementId((int)levelId);

            ElementLevelFilter elementLevelFilter = new ElementLevelFilter(id);
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);

            List<Revit.Elements.Element> outElements = new List<Revit.Elements.Element>();
            foreach (var elementId in filteredElementCollector.WherePasses(elementLevelFilter).WhereElementIsNotElementType().ToElementIds())
            {
                outElements.Add(ElementSelector.ByElementId(elementId.IntegerValue));
            }
            return outElements;
        }

        public static List<Revit.Elements.Element> ElementsOfFamilyType(int? familyTypeId)
        {
            if (familyTypeId == null)
            {
                return null;
            }

            ElementId id = new ElementId((int)familyTypeId);

            ElementClassFilter elementClassFilter = new ElementClassFilter(typeof(Autodesk.Revit.DB.FamilyInstance));
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            IEnumerable<Autodesk.Revit.DB.FamilyInstance> source = filteredElementCollector.WherePasses(elementClassFilter).WhereElementIsNotElementType().ToElements().Cast<Autodesk.Revit.DB.FamilyInstance>();
            IEnumerable<Autodesk.Revit.DB.FamilyInstance> source2 = from x in source
                                                                    where x.Symbol.Id.IntegerValue == familyTypeId
                                                                    select x;
            return (from x in source2
                    select ElementSelector.ByElementId(x.Id.IntegerValue)).ToList<Revit.Elements.Element>();
        }
       
    }
}
