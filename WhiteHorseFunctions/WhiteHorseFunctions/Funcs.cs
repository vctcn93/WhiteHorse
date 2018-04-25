using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using RevitServices.Persistence;
using Autodesk.Revit.DB;
using System.Linq;
using Revit.Elements;
using WhiteHorseLib.Extension;
using Element = Revit.Elements.Element;
using FamilyInstance = Autodesk.Revit.DB.FamilyInstance;

namespace Functions
{
    [IsVisibleInDynamoLibrary(false)]
    public static class RevitUtils
    {
        //public static IList<Revit.Elements.Element> ElementsOfCategory(int categoryId)
        //{
        //    ElementId id = new ElementId(categoryId);
        //    FilteredElementCollector filteredElementCollector = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
        //    IList<Autodesk.Revit.DB.Element> elements = filteredElementCollector.OfCategoryId(id).WhereElementIsNotElementType().ToElements();
        //    List<Revit.Elements.Element> outElements = new List<Revit.Elements.Element>();

        //    foreach (var item in elements)
        //    {
        //        outElements.Add(Revit.Elements.ElementWrapper.ToDSType(item, true));
        //    }
        //    return outElements;
        //}




        //public static IList<Revit.Elements.Element> ElementsAtLevel(int? levelId)
        //{

        //    if (levelId == null)
        //    {
        //        return null;
        //    }

        //    ElementId id = new ElementId((int)levelId);

        //    ElementLevelFilter elementLevelFilter = new ElementLevelFilter(id);
        //    FilteredElementCollector filteredElementCollector = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);

        //    List<Revit.Elements.Element> outElements = new List<Revit.Elements.Element>();
        //    foreach (var elementId in filteredElementCollector.WherePasses(elementLevelFilter).WhereElementIsNotElementType().ToElementIds())
        //    {
        //        outElements.Add(ElementSelector.ByElementId(elementId.IntegerValue));
        //    }
        //    return outElements;
        //}

        [IsObsolete]
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

        public static List<Revit.Elements.Element> ElementsOfFamilyTypeTS(int? familyTypeId)
        {
            if (familyTypeId == null)
                return new List<Element>();
            return DocumentManager.Instance.CurrentDBDocument.GetCollector()
                .WherePasses(new ElementClassFilter(typeof(FamilyInstance)))
                .WhereElementIsNotElementType()
                .Where(n => n.Id.IntegerValue == familyTypeId)
                .Select(m => m.Id.ToPyElement()).ToList();
        }
        /// <summary>
        /// 获取某一楼层上的元素（Pyelement）
        /// </summary>
        /// <param name="levelId"></param>
        /// <returns></returns>
        public static IList<Element> ElementsAtLevelTS(int? levelId)
        {
            if (levelId == null)
                return new List<Element>();
            return DocumentManager.Instance.CurrentDBDocument.GetCollector()
                .WherePasses(new ElementLevelFilter((levelId == null ? -1 : (int)levelId).ToElementId()))
                .WhereElementIsNotElementType()
                .Select(m => m.Id.ToPyElement()).ToList();
        }
        public static IList<Element> ElementsOfCategoryTS(int categoryId)
        {
            return DocumentManager.Instance.CurrentDBDocument.GetCollector()
                .OfCategoryId(categoryId.ToElementId())
                .WhereElementIsNotElementType()
                .Select(m => m.Id.ToPyElement()).ToList();
        }
        //根据元素familytypeId 获取元素的类型
        public static Element ToElementType(this int familyTypeId)
        {
            return familyTypeId.ToPyElement();
        }
        public static Element ToElementsType(this ElementId familyTypeId)
        {
            return familyTypeId.ToPyElement();
        }





    }
}
