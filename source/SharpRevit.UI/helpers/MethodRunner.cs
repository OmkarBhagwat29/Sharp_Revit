using DocumentFormat.OpenXml.Spreadsheet;
using Nice3point.Revit.Toolkit.External.Handlers;
using RevitCore.Extensions;
using System;
using System.Reflection;

namespace SharpRevit.UI.helpers
{
    public static class MethodRunner
    {

        public static object CallRevitApiMethod(Type targetType, string methodName, Type[] parameterTypes, object[] parameters)
        {
            MethodInfo methodInfo = targetType.GetMethod(
                methodName,
                BindingFlags.Static | BindingFlags.Public,
                null, // Use default binder
                parameterTypes, // Specify the exact parameter types
                null // No parameter modifiers
            );

            if (methodInfo == null)
            {
                Console.WriteLine($"Method {methodName} not found on {targetType.Name}");
                return null;
            }

            return methodInfo.Invoke(null, parameters); // null for static method invocation
        }


        public static object CallRevitApiConstructor(Type targetType, object[] arguments) {

            // Get the constructor that matches the parameter types
            ConstructorInfo constructor = targetType.GetConstructor(arguments.Select(p => p.GetType()).ToArray());

            if (constructor == null)
            {
                Console.WriteLine($"Constructor not found for type {targetType.Name} with given parameters.");
                return null;
            }

            // Invoke the constructor
            var obj = constructor.Invoke(arguments);

            return obj;

        }


        public static void Test(ActionEventHandler _externalHandler)
        {
            //doc.AutoJoinElements
            try
            {
                Type xyzType = typeof(XYZ);

                #region ai output
                var startX = 0;
                var startY = 0;
                var startZ = 0;

                var endX = 10;
                var endY = 0;
                var endZ = 0;

                #endregion


                _externalHandler.Raise((uiApp) =>
                {
                    var doc = uiApp.ActiveUIDocument.Document;

                    doc.UseTransaction(() =>
                    {
                        var start = (XYZ)CallRevitApiConstructor(xyzType, [startX, startY, startZ]);
                        var end = (XYZ)CallRevitApiConstructor(xyzType, [endX, endY, endZ]);


                        Type[] curveParamTypes = [typeof(XYZ), typeof(XYZ)];
                        object[] curveParams = [
                            start,
                            end
                        ];


                        object curveResult = CallRevitApiMethod(typeof(Line), "CreateBound", curveParamTypes, curveParams);
                        if (curveResult == null)
                            return;

                        Level level = new FilteredElementCollector(doc)
                                        .OfClass(typeof(Level))
                                        .Cast<Level>()
                                        .FirstOrDefault();


                        // Specify parameter types for the Create method
                        Type[] parameterTypes = new Type[]
                            {
                                typeof(Document),
                                typeof(Curve),
                                typeof(ElementId),
                                typeof(bool)
                            };

                        // Provide parameters matching the method signature
                        object[] parameters = new object[]
                        {
                                doc,
                                curveResult,
                                level.Id,
                                false
                        };

                        object result = MethodRunner.CallRevitApiMethod(
                            typeof(Wall),
                            "Create",
                            parameterTypes,
                            parameters
                        );

                        Console.WriteLine($"Wall Created: {result}");
                    }, "Create Entity");
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }


        }
    }
}
