using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using System.Xml.Linq;
using BoTech.DesignerForAvalonia.Models.Project.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace BoTech.DesignerForAvalonia.Services.CSharp;

public class CSharpParser
{
    /// <summary>
    /// Determines if a class described by an ExtractedClassInfo object is completely implemented in a given assembly.
    /// This Method does not check if the documentation is correct.
    /// </summary>
    /// <param name="assembly">The Assembly to search for the class implementation.</param>
    /// <param name="extractedClassInfo">Represents the extracted class information, including its properties, methods, and namespace.</param>
    /// <returns>True if the class and its related members are fully implemented in the given assembly; otherwise, false.</returns>
    public static bool IsClassInfoCompletelyImplementedInAssembly(Assembly assembly, ExtractedClassInfo extractedClassInfo)
    {
        // Try to find the main class in assembly
        Type? classInAssembly = assembly.GetType(extractedClassInfo.Namespace + "." + extractedClassInfo.ClassName);
        if (classInAssembly == null)
        {
            classInAssembly = assembly.GetTypes().FirstOrDefault(t => t.Name == extractedClassInfo.ClassName);
            if (classInAssembly == null) return false;
        }
    
        // Verify all properties exist and match
        foreach (ExtractedPropertyInfo property in extractedClassInfo.Properties)
        {
            var propertyInAssembly = classInAssembly.GetProperty(property.Name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            
            if (propertyInAssembly == null) return false;
            
            // Check property details
            if (property.Type != propertyInAssembly.PropertyType.ToString()) return false;
            if (property.HasGetter != (propertyInAssembly.GetGetMethod() != null)) return false;
            if (property.HasSetter != (propertyInAssembly.GetSetMethod() != null)) return false;
            
            // Check access modifier
            bool isPublic = propertyInAssembly.GetAccessors()[0].IsPublic;
            bool isPrivate = propertyInAssembly.GetAccessors()[0].IsPrivate;
            bool isProtected = propertyInAssembly.GetAccessors()[0].IsFamily;
            bool isInternal = propertyInAssembly.GetAccessors()[0].IsAssembly;
            bool isStatic = propertyInAssembly.GetAccessors()[0].IsStatic;
            
            if ((isPublic && property.AccessModifier != Modifier.Public) ||
                (isPrivate && property.AccessModifier != Modifier.Private) ||
                (isProtected && property.AccessModifier != Modifier.Protected) ||
                (isInternal && property.AccessModifier != Modifier.Internal) || 
                isStatic != property.IsStatic) return false;
        }
    
        // Verify all methods exist and match
        foreach (ExtractedMethodInfo method in extractedClassInfo.Methods)
        {
            var methodInAssembly = classInAssembly.GetMethod(method.Name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            
            if (methodInAssembly == null) return false;
            
            // Check return type
            if (method.ReturnType != methodInAssembly.ReturnType.ToString()) return false;
            
            // Check parameters
            var parameters = methodInAssembly.GetParameters();
            if (parameters.Length != method.Parameters.Count) return false;
            // Important => The params must be in the same order too.
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType.ToString() != method.Parameters[i].Type) return false;
                if (parameters[i].Name != method.Parameters[i].Name) return false;
                if (parameters[i].DefaultValue?.ToString() != method.Parameters[i].DefaultValue) return false;
            }
        }
    
        // Verify all subclasses exist recursively
        foreach (ExtractedClassInfo subclass in extractedClassInfo.SubClasses)
        {
            if (!IsClassInfoCompletelyImplementedInAssembly(assembly, subclass))
                return false;
        }
        return true;
    }
    /// <summary>
    /// Parses the complete given .cs File and extract all necessary information to fill the Models ExtractedClassInfo, ExtractedPropertyInfo and ExtractedMethodInfo.
    /// </summary>
    /// <param name="filePath">The given .cs File path.</param>
    /// <returns>A ExtractedClassInfo ord null when there was an error => Path or File not found</returns>
    /// <exception cref="ArgumentException">When the File is not available</exception>
    public static ExtractedClassInfo? GetClassesInfoFromFile(string filePath)
    {
        if (filePath.EndsWith(".cs") && File.Exists(filePath))
        {
            List<ExtractedClassInfo> result = new List<ExtractedClassInfo>();
            string mainClassName = Path.GetFileNameWithoutExtension(filePath);
            ExtractedClassInfo mainExtractedClass = new ExtractedClassInfo();
            
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(filePath), new CSharpParseOptions(LanguageVersion.Latest));
            
            SyntaxNode root = syntaxTree.GetRoot();
        

            // Extract all class declarations
            IEnumerable<ClassDeclarationSyntax> classDeclarations = root.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax>();
            foreach (ClassDeclarationSyntax classDeclaration in classDeclarations)
            {
                ExtractedClassInfo currentExtractedClass = new ExtractedClassInfo();
                
                // Extracting the Documentation
                DocumentationCommentTriviaSyntax? documentation =
                classDeclaration.GetLeadingTrivia().Select(trivia => trivia.GetStructure())
                    .OfType<DocumentationCommentTriviaSyntax>().FirstOrDefault();
                if(documentation != null) currentExtractedClass.Documentation = documentation.Content.ToString();
                
                // Extracting the Class Name
                currentExtractedClass.ClassName = classDeclaration.Identifier.ToString();
                
                // Extracting the Namespace
                NamespaceDeclarationSyntax? namespaceDeclaration =
                    classDeclaration.GetLeadingTrivia().Select(trivia => trivia.GetStructure())
                        .OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
                if(namespaceDeclaration != null) currentExtractedClass.Namespace = namespaceDeclaration.Name.ToString();
                
                // Extracting all Functions
                AddAllFunctionsToClassInfo(currentExtractedClass, classDeclaration);
                // Extracting all Properties
                AddAllPropertiesToClassInfo(currentExtractedClass, classDeclaration);
                
                // Is the parsed Class the Main Class or a subclass
                if (classDeclaration.Identifier.ToString().Equals(mainClassName))
                {
                    // if it's the main class only update all infos.
                    // Do not update the subclass list because there could be subclasses stored already.
                    mainExtractedClass.Methods = currentExtractedClass.Methods;
                    mainExtractedClass.Properties = currentExtractedClass.Properties;
                    mainExtractedClass.Documentation = currentExtractedClass.Documentation;
                    mainExtractedClass.Namespace = currentExtractedClass.Namespace;
                    mainExtractedClass.ClassName = currentExtractedClass.ClassName;
                }
                else
                {
                    mainExtractedClass.SubClasses.Add(currentExtractedClass);
                }
            }
            return mainExtractedClass;
        }
        else
        {
            throw new ArgumentException("CSharpParser: GetClassInfoFromFile: The file path does not end with a .cs or does not exist.");
        }

        return null;
    }
    /// <summary>
    /// This Method is a helper Method to load all declared Methods from the .cs File 
    /// </summary>
    /// <param name="extractedClassInfo">The Class Model where the new methods will be injected.</param>
    /// <param name="classDeclaration">The model which was generated from the source Code and contains info about the class.</param>
    private static void AddAllFunctionsToClassInfo(ExtractedClassInfo extractedClassInfo, ClassDeclarationSyntax classDeclaration)
    {
        IEnumerable<MethodDeclarationSyntax> declaredMethods = classDeclaration.Members.OfType<MethodDeclarationSyntax>();
        foreach (MethodDeclarationSyntax declaredMethod in declaredMethods)
        {
            ExtractedMethodInfo newExtractedMethod = new ExtractedMethodInfo()
            {
                Name = declaredMethod.Identifier.ToString(),
                ReturnType = declaredMethod.ReturnType.ToString(),
            };
            
            // Adding the Documentation
            
            Dictionary<string, string> parameterDocumentation = new Dictionary<string, string>();
            
            DocumentationCommentTriviaSyntax? documentation =
                declaredMethod.GetLeadingTrivia().Select(trivia => trivia.GetStructure())
                    .OfType<DocumentationCommentTriviaSyntax>().FirstOrDefault();
            if (documentation != null)
            {
                // first let's remove all linebreaks and '///'
                string fullDocumentation = documentation.ToFullString();
                fullDocumentation = fullDocumentation.Replace("\n", string.Empty);
                fullDocumentation = fullDocumentation.Replace("///", string.Empty);
                try
                {
                    XElement xmlDocument = XElement.Parse(fullDocumentation);
                    newExtractedMethod.Documentation = xmlDocument.Element("summary")?.Value ?? string.Empty;
                    foreach (XElement param in xmlDocument.Elements("param"))
                    {
                        parameterDocumentation.Add(param.Attribute("name")?.Value ?? "???", param.Value);
                    }
                }
                catch (Exception e)
                {
                    newExtractedMethod.Documentation = "Error by parsing the XML based documentation: " 
                                                       + e.Message + " \n\n" +
                                                       "The documentation will be displayed as plain text: \n\n" 
                                                       + fullDocumentation;
                }
            }

            // Adding the Params
            foreach (ParameterSyntax parameter in declaredMethod.ParameterList.Parameters)
            {
                string paramDoc = "";
                try
                {
                    paramDoc = parameterDocumentation[parameter.Identifier.ToString()];
                }
                catch (Exception e)
                {
                    paramDoc = "Error by parsing the XML based documentation: " + e.Message;
                }
                ExtractedParamInfo extractedParamInfo = new ExtractedParamInfo()
                {
                    Name = parameter.Identifier.ToString(),
                    Type = parameter.Type?.ToString() ?? "???",
                    DefaultValue = parameter.Default?.Value.ToString() ?? string.Empty,
                    Documentation = paramDoc,
                };
                newExtractedMethod.Parameters.Add(extractedParamInfo);
            }
            extractedClassInfo.Methods.Add(newExtractedMethod);
        }
    }

    /// <summary>
    /// This Method is a helper Method to load all declared <c>Properties</c> from the .cs File 
    /// </summary>
    /// <param name="extractedClassInfo">The Class Model where the new properties will be injected.</param>
    /// <param name="classDeclaration">The model which was generated from the source Code and contains info about the class.</param>
    private static void AddAllPropertiesToClassInfo(ExtractedClassInfo extractedClassInfo, ClassDeclarationSyntax classDeclaration)
    {
        IEnumerable<PropertyDeclarationSyntax> declaredProperties = classDeclaration.Members.OfType<PropertyDeclarationSyntax>();
        foreach (PropertyDeclarationSyntax property in declaredProperties)
        {
            ExtractedPropertyInfo newExtractedProperty = new ExtractedPropertyInfo();
            newExtractedProperty.Name = property.Identifier.ToString();
            
            // Getting the Documentation
            
            DocumentationCommentTriviaSyntax? documentation =
                property.GetLeadingTrivia().Select(trivia => trivia.GetStructure())
                    .OfType<DocumentationCommentTriviaSyntax>().FirstOrDefault();
            newExtractedProperty.Documentation = documentation?.Content.ToString() ?? string.Empty;
            
            newExtractedProperty.Type = property.Type?.ToString() ?? "???";
            newExtractedProperty.DefaultValue = property.Initializer?.Value.ToString() ?? string.Empty;
            
            // Setting the AccessModifiers and all flags.
            foreach (SyntaxToken modifier in property.Modifiers)
            {
                if (modifier.IsKind(SyntaxKind.PrivateKeyword)) newExtractedProperty.AccessModifier = Modifier.Private;
                if (modifier.IsKind(SyntaxKind.ProtectedKeyword)) newExtractedProperty.AccessModifier = Modifier.Protected;
                if (modifier.IsKind(SyntaxKind.InternalKeyword)) newExtractedProperty.AccessModifier = Modifier.Internal;
                if (modifier.IsKind(SyntaxKind.PublicKeyword)) newExtractedProperty.AccessModifier = Modifier.Public;
                if (modifier.IsKind(SyntaxKind.StaticKeyword)) newExtractedProperty.IsStatic = true;
                if (modifier.IsKind(SyntaxKind.ConstKeyword)) newExtractedProperty.IsConstant = true;
                if (modifier.IsKind(SyntaxKind.ReadOnlyKeyword)) newExtractedProperty.IsReadOnly = true;
            }
            // Checking for {get; set;} definitions
            if (property.AccessorList != null)
            {
                foreach (AccessorDeclarationSyntax accessor in property.AccessorList.Accessors)
                {
                    if(accessor.IsKind(SyntaxKind.GetAccessorDeclaration)) newExtractedProperty.HasGetter = true;
                    if(accessor.IsKind(SyntaxKind.SetAccessorDeclaration)) newExtractedProperty.HasSetter = true;
                }
            }
            extractedClassInfo.Properties.Add(newExtractedProperty);
        }
    }
}